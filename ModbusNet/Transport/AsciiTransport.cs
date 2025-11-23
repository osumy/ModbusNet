using ModbusNet.Messages;
using ModbusNet.Utils;
using System.IO.Ports;
using System.Text;

namespace ModbusNet.Transport
{
    /// <summary>
    /// Implements the Modbus ASCII transport layer over a serial port.
    /// This class handles framing, checksum validation (LRC), and communication
    /// using the ASCII encoding scheme as defined in the Modbus specification.
    /// </summary>
    public class AsciiTransport : ModbusTransportBase
    {
        #region Fields and Properties

        private readonly SerialPort _serialPort;
        private readonly ModbusSettings _settings;

        /// <summary>
        /// Gets a value indicating whether the underlying serial port is open and connected.
        /// </summary>
        public bool IsConnected => _serialPort.IsOpen;

        private readonly SemaphoreSlim _writeLock = new(1, 1);
        private bool _disposed = false;

        /// <summary>
        /// Byte array representation of the ASCII start delimiter (typically <c>':'</c>).
        /// </summary>
        public byte[] StartDelimiterAsciiArray { get; private set; }

        /// <summary>
        /// Byte array representation of the ASCII end delimiter (typically <c>CRLF</c> i.e., <c>"\r\n"</c>).
        /// </summary>
        public byte[] EndDelimiterAsciiArray { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsciiTransport"/> class.
        /// </summary>
        /// <param name="serialPort">The serial port to use for communication. Must not be null.</param>
        /// <param name="settings">The Modbus configuration settings. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serialPort"/> or <paramref name="settings"/> is null.</exception>
        public AsciiTransport(SerialPort serialPort, ModbusSettings settings)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (!_serialPort.IsOpen)
                _serialPort.Open();

            StartDelimiterAsciiArray = Encoding.ASCII.GetBytes(settings.AsciiStartDelimiter);
            EndDelimiterAsciiArray = Encoding.ASCII.GetBytes(settings.AsciiEndDelimiter);
        }

        #endregion

        #region Synchronous Public Methods

        /// <summary>
        /// Sends a Modbus request and waits for the corresponding response.
        /// Retries on timeout up to <see cref="ModbusSettings.RetryCount"/> times.
        /// </summary>
        /// <param name="request">The raw ASCII-encoded request frame to send.</param>
        /// <returns>A <see cref="ModbusResponse"/> containing the decoded PDU.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the serial port is not connected.</exception>
        /// <exception cref="TimeoutException">Thrown if all retry attempts fail.</exception>
        /// <exception cref="ModbusExceptionLRC">Thrown if the LRC checksum is invalid.</exception>
        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            ThrowIfDisposed();

            if (!IsConnected)
            {
                throw new InvalidOperationException("Serial port is not connected.");
            }

            for (int attempt = 0; attempt < _settings.RetryCount; attempt++)
            {
                try
                {
                    _serialPort.Write(request, 0, request.Length);
                    var responsePDU = ReceiveResponse();

                    var fcAscii = new byte[2];
                    Array.Copy(request, 3, fcAscii, 0, fcAscii.Length);

                    ValidateFC(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);
                    ValidateByteCount(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);

                    return BuildResponse(responsePDU);
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    Thread.Sleep(_settings.RetryDelayMs);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.RetryCount + 1} attempts");
        }

        /// <summary>
        /// Sends a Modbus request without waiting for a response (fire-and-forget).
        /// Still validates the echoed response function code for error checking.
        /// Retries on timeout up to <see cref="ModbusSettings.RetryCount"/> times.
        /// </summary>
        /// <param name="request">The raw ASCII-encoded request frame to send.</param>
        /// <exception cref="InvalidOperationException">Thrown if the serial port is not connected.</exception>
        /// <exception cref="TimeoutException">Thrown if all retry attempts fail.</exception>
        /// <exception cref="ModbusExceptionLRC">Thrown if the LRC checksum is invalid.</exception>
        public override void SendRequestIgnoreResponse(byte[] request)
        {
            ThrowIfDisposed();

            if (!IsConnected)
            {
                throw new InvalidOperationException("Serial port is not connected.");
            }

            for (int attempt = 0; attempt < _settings.RetryCount; attempt++)
            {
                try
                {
                    _serialPort.Write(request, 0, request.Length);
                    var responsePDU = ReceiveResponse();

                    var fcAscii = new byte[2];
                    Array.Copy(request, 3, fcAscii, 0, fcAscii.Length);

                    ValidateFC(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);
                    return;
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    Thread.Sleep(_settings.RetryDelayMs);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.RetryCount + 1} attempts");
        }

        #endregion

        #region Synchronous Private Methods

        /// <summary>
        /// Receives and decodes a Modbus ASCII response from the serial port.
        /// Expects a frame starting with ':' and ending with CRLF.
        /// Validates LRC checksum and returns the PDU (function code + data).
        /// </summary>
        /// <returns>The decoded PDU as a byte array.</returns>
        /// <exception cref="IOException">Thrown if the serial port is closed or no data is available.</exception>
        /// <exception cref="FormatException">Thrown if the frame is malformed or too short.</exception>
        /// <exception cref="ModbusExceptionLRC">Thrown if the LRC checksum does not match.</exception>
        private byte[] ReceiveResponse()
        {
            // Wait for start char ':' (blocks until found or ReadTimeout triggers)
            int bInt;
            do
            {
                bInt = _serialPort.ReadByte();
                if (bInt < 0) throw new IOException("Serial port closed or no data available.");
            } while ((byte)bInt != (byte)':');

            // Read until CRLF ("\r\n")
            var payload = new List<byte>();
            bool sawCr = false;
            while (true)
            {
                bInt = _serialPort.ReadByte();
                if (bInt < 0) throw new IOException("Serial port closed or no data available while reading payload.");

                byte b = (byte)bInt;

                if (!sawCr)
                {
                    if (b == (byte)'\r')
                    {
                        sawCr = true;
                        continue;
                    }
                    else
                    {
                        payload.Add(b);
                    }
                }
                else
                {
                    if (b == (byte)'\n')
                    {
                        break;
                    }
                    else
                    {
                        payload.Add((byte)'\r');
                        if (b == (byte)'\r')
                        {
                            sawCr = true;
                        }
                        else
                        {
                            payload.Add(b);
                            sawCr = false;
                        }
                    }
                }
            }

            if (payload.Count == 0)
                throw new FormatException("Empty Modbus ASCII payload received.");

            if ((payload.Count & 1) != 0)
                throw new FormatException("Invalid ASCII payload length (must be even number of hex chars).");

            var decoded = AsciiUtility.FromAsciiBytes(payload.ToArray());

            if (decoded.Length < 1)
                throw new FormatException("Decoded Modbus frame too short.");

            var lrc = decoded[decoded.Length - 1];
            var message = new byte[decoded.Length - 1];
            Array.Copy(decoded, 0, message, 0, message.Length);

            ChecksumsMatch(message, [lrc]);

            var pdu = new byte[message.Length - 1];
            Array.Copy(message, 1, pdu, 0, pdu.Length);

            return pdu;
        }

        #endregion

        #region Asynchronous Public Methods

        /// <summary>
        /// Asynchronously sends a Modbus request without waiting for a response.
        /// Retries on failure up to <see cref="ModbusSettings.RetryCount"/> times.
        /// </summary>
        /// <param name="request">The raw ASCII-encoded request frame to send.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that completes when the write succeeds or fails after retries.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the serial port is not connected.</exception>
        /// <exception cref="TimeoutException">Thrown if all retry attempts fail.</exception>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
        public override async Task SendRequestIgnoreResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (!IsConnected) throw new InvalidOperationException("Serial port is not connected.");

            for (int attempt = 0; attempt <= _settings.RetryCount; attempt++)
            {
                try
                {
                    await WriteAsync(request, cancellationToken).ConfigureAwait(false);
                    return;
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
                catch (IOException) when (attempt < _settings.RetryCount)
                {
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
            }

            throw new TimeoutException($"Write failed after {_settings.RetryCount + 1} attempts");
        }

        /// <summary>
        /// Asynchronously sends a Modbus request and waits for the response.
        /// Retries on timeout or I/O error up to <see cref="ModbusSettings.RetryCount"/> times.
        /// </summary>
        /// <param name="request">The raw ASCII-encoded request frame to send.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that returns the <see cref="ModbusResponse"/> upon success.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the serial port is not connected.</exception>
        /// <exception cref="TimeoutException">Thrown if all retry attempts fail.</exception>
        /// <exception cref="ModbusExceptionLRC">Thrown if the LRC checksum is invalid.</exception>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
        public override async Task<ModbusResponse> SendRequestReceiveResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (!IsConnected) throw new InvalidOperationException("Serial port is not connected.");

            for (int attempt = 0; attempt <= _settings.RetryCount; attempt++)
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                linkedCts.CancelAfter(_settings.WriteTimeout);

                try
                {
                    await WriteAsync(request, linkedCts.Token).ConfigureAwait(false);
                    var responsePDU = await ReceiveResponseAsync(linkedCts.Token).ConfigureAwait(false);

                    var fcAscii = new byte[2];
                    Array.Copy(request, 3, fcAscii, 0, fcAscii.Length);
                    ValidateFC(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);
                    ValidateByteCount(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);

                    return BuildResponse(responsePDU);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (OperationCanceledException) when (attempt < _settings.RetryCount)
                {
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
                catch (IOException) when (attempt < _settings.RetryCount)
                {
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.RetryCount + 1} attempts");
        }

        #endregion

        #region Asynchronous Private Methods

        /// <summary>
        /// Asynchronously writes a byte array to the serial port with thread-safe locking.
        /// </summary>
        /// <param name="buffer">The data to write.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task representing the asynchronous write operation.</returns>
        private async Task WriteAsync(byte[] buffer, CancellationToken ct)
        {
            await _writeLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (_serialPort.BaseStream.CanWrite)
                {
                    await _serialPort.BaseStream.WriteAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false);
                }
                else
                {
                    await Task.Run(() => _serialPort.Write(buffer, 0, buffer.Length), ct).ConfigureAwait(false);
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>
        /// Asynchronously receives and decodes a Modbus ASCII response.
        /// Waits for ':' start delimiter, reads until CRLF, validates LRC, and returns PDU.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A task that returns the decoded PDU byte array.</returns>
        /// <exception cref="IOException">Thrown if the serial port is closed or no data is available.</exception>
        /// <exception cref="FormatException">Thrown if the frame is malformed.</exception>
        /// <exception cref="ModbusExceptionLRC">Thrown if LRC validation fails.</exception>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
        private async Task<byte[]> ReceiveResponseAsync(CancellationToken ct)
        {
            var asciiPayload = new List<byte>();
            bool foundStart = false;
            bool sawCr = false;

            Stream baseStream = null;
            try { baseStream = _serialPort?.BaseStream; } catch { baseStream = null; }

            async Task<int> ReadOneAsync()
            {
                ct.ThrowIfCancellationRequested();

                if (baseStream != null && baseStream.CanRead)
                {
                    var one = new byte[1];
                    int rn = await baseStream.ReadAsync(one, 0, 1, ct).ConfigureAwait(false);
                    return rn == 0 ? -1 : one[0];
                }
                else
                {
                    return await Task.Run(() =>
                    {
                        try
                        {
                            return _serialPort.ReadByte();
                        }
                        catch (TimeoutException) { throw; }
                        catch (IOException) { throw; }
                    }, ct).ConfigureAwait(false);
                }
            }

            while (!foundStart)
            {
                int r = await ReadOneAsync().ConfigureAwait(false);
                if (r < 0) throw new IOException("Serial port closed or no data available.");
                if ((byte)r == (byte)':')
                {
                    foundStart = true;
                    break;
                }
            }

            while (true)
            {
                int r = await ReadOneAsync().ConfigureAwait(false);
                if (r < 0) throw new IOException("Serial port closed or no data available while reading payload.");
                byte b = (byte)r;

                if (!sawCr)
                {
                    if (b == (byte)'\r')
                    {
                        sawCr = true;
                        continue;
                    }
                    else
                    {
                        asciiPayload.Add(b);
                    }
                }
                else
                {
                    if (b == (byte)'\n')
                    {
                        break;
                    }
                    else
                    {
                        asciiPayload.Add((byte)'\r');
                        if (b == (byte)'\r')
                        {
                            sawCr = true;
                        }
                        else
                        {
                            asciiPayload.Add(b);
                            sawCr = false;
                        }
                    }
                }
            }

            if (asciiPayload.Count == 0)
                throw new FormatException("Empty Modbus ASCII payload received.");

            if ((asciiPayload.Count & 1) != 0)
                throw new FormatException("Invalid ASCII payload length (must be even number of hex chars).");

            var decoded = AsciiUtility.FromAsciiBytes(asciiPayload.ToArray());

            if (decoded.Length < 1)
                throw new FormatException("Decoded Modbus frame too short.");

            var lrc = decoded[decoded.Length - 1];
            var message = new byte[decoded.Length - 1];
            Array.Copy(decoded, 0, message, 0, message.Length);

            ChecksumsMatch(message, new byte[] { lrc });

            if (message.Length < 2)
                throw new FormatException("Decoded Modbus frame is too short for address + function.");

            var pdu = new byte[message.Length - 1];
            Array.Copy(message, 1, pdu, 0, pdu.Length);
            return pdu;
        }

        #endregion

        #region Request Building and Validation

        /// <summary>
        /// Builds a complete Modbus ASCII request frame from a slave address and PDU.
        /// Includes start delimiter, LRC checksum, and end delimiter (CRLF).
        /// </summary>
        /// <param name="slaveAddress">The Modbus slave address (1–247).</param>
        /// <param name="pdu">The Protocol Data Unit (function code + data).</param>
        /// <returns>A byte array representing the full ASCII-encoded request frame.</returns>
        public override byte[] BuildRequest(byte slaveAddress, byte[] pdu)
        {
            return AsciiUtility.BuildAsciiFrame(
                slaveAddress,
                pdu,
                StartDelimiterAsciiArray,
                EndDelimiterAsciiArray
            );
        }

        /// <summary>
        /// Validates that the LRC checksum of the message matches the provided checksum byte.
        /// </summary>
        /// <param name="rawMessage">The message bytes (excluding LRC).</param>
        /// <param name="errorCheckBytes">Array containing the expected LRC byte.</param>
        /// <exception cref="ModbusExceptionLRC">Thrown if checksum does not match.</exception>
        private void ChecksumsMatch(byte[] rawMessage, byte[] errorCheckBytes)
        {
            if (!ErrorCheckUtility.ValidateLrc(rawMessage, errorCheckBytes[0]))
            {
                throw new ModbusExceptionLRC();
            }
        }

        #endregion

        #region Disposal and Utility

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if this instance has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object is disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Releases unmanaged resources and disposes the underlying serial port.
        /// </summary>
        public override void Dispose()
        {
            if (!_disposed)
            {
                _serialPort?.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}