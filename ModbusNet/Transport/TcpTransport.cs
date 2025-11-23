using ModbusNet.Messages;
using System.Net.Sockets;

namespace ModbusNet.Transport
{
    /// <summary>
    /// Implements the Modbus TCP transport layer over a TCP/IP socket.
    /// Encapsulates Modbus requests in MBAP (Modbus Application Protocol) headers
    /// and manages connection lifecycle, transaction IDs, and thread-safe communication.
    /// </summary>
    public class TcpTransport : ModbusTransportBase
    {
        #region Fields and Properties

        private TcpClient _tcpClient;
        private NetworkStream _stream;

        private readonly string _ipAddress;
        private readonly int _port;
        private int _transactionId;
        private readonly ModbusSettings _settings;

        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private bool _disposed = false;

        /// <summary>
        /// Gets a value indicating whether the underlying TCP connection is active.
        /// </summary>
        public bool IsConnected => _tcpClient?.Connected == true;

        #endregion

        #region Constructors and Factory Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpTransport"/> class and establishes a synchronous TCP connection.
        /// </summary>
        /// <param name="settings">Modbus TCP configuration settings containing IP address and port.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> is null.</exception>
        /// <exception cref="Exception">Thrown if connection to the remote endpoint fails.</exception>
        private TcpTransport(ModbusSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _ipAddress = settings.IpAddress;
            _port = settings.Port;

            _transactionId = Random.Shared.Next(1, ushort.MaxValue);

            _tcpClient = new TcpClient
            {
                SendTimeout = _settings.WriteTimeout,
                ReceiveTimeout = _settings.ReadTimeout,
                NoDelay = true // Disable Nagle’s algorithm for real-time Modbus traffic
            };

            try
            {
                _tcpClient.Connect(_ipAddress, _port);
                _stream = _tcpClient.GetStream();
            }
            catch (Exception ex)
            {
                throw new Exception($"Connection failed to {_ipAddress}:{_port}", ex);
            }
        }

        /// <summary>
        /// Factory method to synchronously create and connect a <see cref="TcpTransport"/> instance.
        /// </summary>
        /// <param name="settings">Modbus TCP configuration settings.</param>
        /// <returns>A connected <see cref="TcpTransport"/> instance.</returns>
        public static TcpTransport Create(ModbusSettings settings)
        {
            return new TcpTransport(settings);
        }

        /// <summary>
        /// Asynchronously establishes a TCP connection using the configured IP address and port.
        /// </summary>
        /// <exception cref="TimeoutException">Thrown if connection times out.</exception>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
        public async Task ConnectAsync()
        {
            _tcpClient?.Dispose(); // Clean up any previous connection

            _tcpClient = new TcpClient
            {
                SendTimeout = _settings.WriteTimeout,
                ReceiveTimeout = _settings.ReadTimeout,
                NoDelay = true
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_settings.Timeout));

            try
            {
                await _tcpClient.ConnectAsync(_ipAddress, _port, cts.Token).ConfigureAwait(false);
                _stream = _tcpClient.GetStream();
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Connection to {_ipAddress}:{_port} timed out after {_settings.Timeout}ms");
            }
        }

        #endregion

        #region Request Building

        /// <summary>
        /// Builds a complete Modbus TCP request frame (MBAP header + PDU).
        /// Assigns a unique transaction ID and includes the unit (slave) address.
        /// </summary>
        /// <param name="slaveAddress">The unit identifier (typically 1–247, though often ignored in TCP).</param>
        /// <param name="pdu">The Protocol Data Unit (function code + data).</param>
        /// <returns>A byte array representing the full Modbus TCP frame.</returns>
        public override byte[] BuildRequest(byte slaveAddress, byte[] pdu)
        {
            int newId = Interlocked.Increment(ref _transactionId);
            ushort transactionId = (ushort)(newId & 0xFFFF);
            return BuildMbapFrame(slaveAddress, pdu, transactionId);
        }

        /// <summary>
        /// Constructs the MBAP (Modbus Application Protocol) header and appends the PDU.
        /// </summary>
        /// <param name="slaveAddress">Unit ID (byte 6 of MBAP).</param>
        /// <param name="pdu">The PDU to embed.</param>
        /// <param name="transactionId">Unique request identifier.</param>
        /// <returns>Full Modbus TCP frame as byte array.</returns>
        private byte[] BuildMbapFrame(byte slaveAddress, byte[] pdu, ushort transactionId)
        {
            var frame = new byte[7 + pdu.Length];
            frame[0] = (byte)(transactionId >> 8);
            frame[1] = (byte)transactionId;
            frame[2] = 0x00; // Protocol ID high
            frame[3] = 0x00; // Protocol ID low
            frame[4] = (byte)((pdu.Length + 1) >> 8); // Length high (includes Unit ID)
            frame[5] = (byte)((pdu.Length + 1) & 0xFF); // Length low
            frame[6] = slaveAddress; // Unit ID
            Array.Copy(pdu, 0, frame, 7, pdu.Length);
            return frame;
        }

        #endregion

        #region Asynchronous Public Methods

        /// <summary>
        /// Asynchronously sends a Modbus TCP request and awaits the response.
        /// Thread-safe and validates MBAP header fields.
        /// </summary>
        /// <param name="request">The full Modbus TCP frame to send.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A <see cref="ModbusResponse"/> containing the decoded PDU.</returns>
        /// <exception cref="InvalidOperationException">Thrown if not connected.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the transport is disposed.</exception>
        /// <exception cref="Exception">Thrown on protocol error or connection loss.</exception>
        public override async Task<ModbusResponse> SendRequestReceiveResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            await _sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                ThrowIfDisposed();
                EnsureConnected();

                await _stream.WriteAsync(request, 0, request.Length, cancellationToken).ConfigureAwait(false);

                var header = await ReadExactAsync(7, cancellationToken).ConfigureAwait(false);

                var transactionId = (ushort)((header[0] << 8) | header[1]);
                var protocolId = (ushort)((header[2] << 8) | header[3]);
                var length = (ushort)((header[4] << 8) | header[5]);
                var unitId = header[6];

                if (protocolId != 0)
                    throw new Exception("Invalid protocol ID in Modbus TCP response (expected 0).");

                var pduLength = length - 1; // Length includes Unit ID
                var pdu = await ReadExactAsync(pduLength, cancellationToken).ConfigureAwait(false);

                return BuildResponse(pdu);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        /// <summary>
        /// Asynchronously sends a Modbus TCP request without waiting for a response (fire-and-forget).
        /// </summary>
        /// <param name="request">The full Modbus TCP frame to send.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the write operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if not connected.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the transport is disposed.</exception>
        public override async Task SendRequestIgnoreResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            await _sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                ThrowIfDisposed();
                EnsureConnected();

                await _stream.WriteAsync(request, 0, request.Length, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        #endregion

        #region Synchronous Methods (Not Implemented)

        /// <inheritdoc />
        /// <remarks>
        /// Synchronous operations are not implemented for TCP transport to encourage async usage
        /// and avoid blocking threads in network I/O.
        /// </remarks>
        public override void SendRequestIgnoreResponse(byte[] request)
        {
            throw new NotImplementedException("Synchronous TCP operations are not supported. Use async methods instead.");
        }

        /// <inheritdoc />
        /// <remarks>
        /// Synchronous operations are not implemented for TCP transport to encourage async usage
        /// and avoid blocking threads in network I/O.
        /// </remarks>
        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            throw new NotImplementedException("Synchronous TCP operations are not supported. Use async methods instead.");
        }

        #endregion

        #region Private Utilities

        /// <summary>
        /// Reads an exact number of bytes from the network stream.
        /// </summary>
        /// <param name="length">Number of bytes to read.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Byte array of exactly <paramref name="length"/> bytes.</returns>
        /// <exception cref="Exception">Thrown if the connection closes prematurely.</exception>
        private async Task<byte[]> ReadExactAsync(int length, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[length];
            int totalRead = 0;

            while (totalRead < length)
            {
                int read = await _stream.ReadAsync(buffer, totalRead, length - totalRead, cancellationToken).ConfigureAwait(false);
                if (read == 0)
                    throw new Exception("Remote host closed the connection during read.");
                totalRead += read;
            }

            return buffer;
        }

        /// <summary>
        /// Throws if the transport is not connected.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when not connected.</exception>
        private void EnsureConnected()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to Modbus TCP device.");
        }

        /// <summary>
        /// Throws if the object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Releases unmanaged resources and closes the TCP connection.
        /// </summary>
        public override void Dispose()
        {
            if (!_disposed)
            {
                _stream?.Dispose();
                _tcpClient?.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}