using ModbusNet.Messages;
using System.IO.Ports;
using System.Net.Sockets;

namespace ModbusNet.Transport
{
    public class TcpTransport : ModbusTransportBase
    {
        private TcpClient _tcpClient;    
        private NetworkStream _stream;

        private readonly string _ipAddress;
        private readonly int _port;
        private int _transactionId;
        private ModbusSettings _settings;

        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1); // For thread safety

        public bool IsConnected => _tcpClient.Connected;
        private bool _disposed = false;

        private TcpTransport(ModbusSettings settings)
        {
            _ipAddress = settings.IpAddress;
            _port = settings.Port;
            _settings = settings;

            // Initialize with random value
            _transactionId = Random.Shared.Next(1, ushort.MaxValue);

            _tcpClient = new TcpClient();

            // Configure before connecting
            _tcpClient.SendTimeout = _settings.WriteTimeout;
            _tcpClient.ReceiveTimeout = _settings.ReadTimeout;
            _tcpClient.NoDelay = true; // Disable Nagle's algorithm for immediate sending

            try
            {
                _tcpClient.Connect(_ipAddress, _port);
                _stream = _tcpClient.GetStream();
            }
             catch (Exception)
            {
                throw new Exception($"Connection Failed ({_ipAddress}:{_port})");
            }
        }

        // Factory method
        //public static async Task<TcpTransport> CreateAsync(ModbusSettings settings)
        //{
        //    var transport = new TcpTransport(settings);
        //    await transport.ConnectAsync();
        //    return transport;
        //}

        public static TcpTransport Create(ModbusSettings settings)
        {
            var transport = new TcpTransport(settings);
            return transport;
        }

        public async Task ConnectAsync()
        {
            _tcpClient = new TcpClient();

            // Configure before connecting
            _tcpClient.SendTimeout = _settings.WriteTimeout;
            _tcpClient.ReceiveTimeout = _settings.ReadTimeout;
            _tcpClient.NoDelay = true; // Disable Nagle's algorithm for immediate sending

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_settings.Timeout));

            try
            {
                await _tcpClient.ConnectAsync(_ipAddress, _port, cts.Token);
                _stream = _tcpClient.GetStream();
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"Connection to {_ipAddress}:{_port} timed out after {_settings.Timeout}ms");
            }
        }


        public override byte[] BuildRequest(byte slaveAddress, byte[] pdu)
        {
            // Thread-safe increment with wrap-around
            int newId = Interlocked.Increment(ref _transactionId);
            ushort transactionId = (ushort)(newId & 0xFFFF); // Ensure it stays within ushort range

            return BuildMbapFrame(slaveAddress, pdu, transactionId);
        }

        private byte[] BuildMbapFrame(byte slaveAddress, byte[] pdu, ushort transactionId)
        {
            var frame = new byte[7 + pdu.Length];
            frame[0] = (byte)(transactionId >> 8);
            frame[1] = (byte)(transactionId & 0xFF);
            frame[2] = 0x00;                            // Protocol ID High
            frame[3] = 0x00;                            // Protocol ID Low
            frame[4] = (byte)((pdu.Length + 1) >> 8);   // Length High (+1 for Unit ID)
            frame[5] = (byte)((pdu.Length + 1) & 0xFF); // Length Low
            frame[6] = slaveAddress;                    // Unit ID
            Array.Copy(pdu, 0, frame, 7, pdu.Length);
            return frame;
        }

        public override async Task<ModbusResponse> SendRequestReceiveResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            await _sendLock.WaitAsync(cancellationToken);

            try
            {
                ThrowIfDisposed();
                EnsureConnected();

                // Send request
                await _stream.WriteAsync(request, 0, request.Length, cancellationToken);

                // Read MBAP header
                var header = await ReadExactAsync(7, cancellationToken);

                // Validate MBAP header
                var transactionId = (ushort)((header[0] << 8) | header[1]);
                var protocolId = (ushort)((header[2] << 8) | header[3]);
                var length = (ushort)((header[4] << 8) | header[5]);
                var unitId = header[6];

                if (protocolId != 0)
                    throw new Exception("Invalid protocol ID in response");

                // Read PDU (length includes Unit ID, so subtract 1)
                var pduLength = length - 1;
                var pdu = await ReadExactAsync(pduLength, cancellationToken);

                // Create response - you'll need to adapt this to your ModbusResponse structure
                return BuildResponse(pdu);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public override async Task SendRequestIgnoreResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            await _sendLock.WaitAsync(cancellationToken);

            try
            {
                ThrowIfDisposed();
                EnsureConnected();

                await _stream.WriteAsync(request, 0, request.Length, cancellationToken);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private async Task<byte[]> ReadExactAsync(int length, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[length];
            int totalRead = 0;

            while (totalRead < length)
            {
                int read = await _stream.ReadAsync(buffer, totalRead, length - totalRead, cancellationToken);
                if (read == 0)
                    throw new Exception("Connection closed while reading response");
                totalRead += read;
            }

            return buffer;
        }

        public override void SendRequestIgnoreResponse(byte[] request)
        {
            throw new NotImplementedException();
        }

        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            throw new NotImplementedException();
        }


        private void EnsureConnected()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to Modbus TCP device");
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _tcpClient?.Dispose();
                _stream?.Dispose();
                _disposed = true;
            }
        }
    }
}
