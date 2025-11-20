using ModbusNet.Messages;
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
            frame[2] = 0x00;
            frame[3] = 0x00;
            frame[4] = (byte)((pdu.Length + 1) >> 8);
            frame[5] = (byte)((pdu.Length + 1) & 0xFF);
            frame[6] = slaveAddress;
            Array.Copy(pdu, 0, frame, 7, pdu.Length);
            return frame;
        }

        public override void ChecksumsMatch(byte[] rawMessage, byte[] ErrorCheckBytes)
        {
            throw new NotImplementedException();
        }

        public override void SendRequestIgnoreResponse(byte[] request)
        {
            throw new NotImplementedException();
        }

        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            throw new NotImplementedException();
        }
    }
}
