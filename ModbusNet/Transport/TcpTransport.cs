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

        public bool IsConnected => _tcpClient.Connected;
        private bool _disposed = false;

        private TcpTransport(ModbusSettings settings)
        {
            _ipAddress = settings.IpAddress;
            _port = settings.Port;
            _settings = settings;

            // Initialize with random value
            _transactionId = Random.Shared.Next(1, ushort.MaxValue);
        }

        // Factory method
        public static async Task<TcpTransport> CreateAsync(ModbusSettings settings)
        {
            var transport = new TcpTransport(settings);
            await transport.ConnectAsync();
            return transport;
        }

        public async Task ConnectAsync()
        {
            _tcpClient = new TcpClient();

            // Configure before connecting
            _tcpClient.SendTimeout = _settings.WriteTimeout;
            _tcpClient.ReceiveTimeout = _settings.ReadTimeout;
            _tcpClient.NoDelay = true; // Disable Nagle's algorithm for immediate sending

            await _tcpClient.ConnectAsync(_ipAddress, _port);
            _stream = _tcpClient.GetStream(); // Get the data stream
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

        public override void SendRequestIgnoreResponse(byte[] request)
        {
            throw new NotImplementedException();
        }

        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            throw new NotImplementedException();
        }

        public override Task<ModbusResponse> SendRequestReceiveResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            EnsureConnected();

            throw new NotImplementedException();
        }

        public override Task SendRequestIgnoreResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            EnsureConnected();

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
