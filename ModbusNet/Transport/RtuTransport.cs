using ModbusNet.Messages;
using ModbusNet.Utils;
using System.IO.Ports;

namespace ModbusNet.Transport
{
    public class RtuTransport : ModbusTransportBase
    {
        private readonly SerialPort _serialPort;
        private readonly ModbusSettings _settings;

        public bool IsConnected => _serialPort.IsOpen;
        private bool _disposed = false;


        public RtuTransport(SerialPort serialPort, ModbusSettings settings)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }


        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            ThrowIfDisposed();

            throw new NotImplementedException();
        }
        public override void SendRequestIgnoreResponse(byte[] request)
        {
            ThrowIfDisposed();

            throw new NotImplementedException();
        }

        public override byte[] BuildRequest(byte slaveAddress, byte[] pdu)
        {
            // Create the message frame: slave address + PDU + CRC
            var frame = new byte[1 + pdu.Length + 2];

            frame[0] = slaveAddress;
            pdu.AsSpan().CopyTo(frame.AsSpan(1, pdu.Length));

            var crcBytes = ErrorCheckUtility.ComputeCrc(frame.AsSpan(0, 1 + pdu.Length));

            // Copy CRC bytes 
            frame[^2] = crcBytes[0];
            frame[^1] = crcBytes[1];

            return frame;
        }

        public override void ChecksumsMatch(byte[] rawMessage, byte[] ErrorCheckBytes)
        {
            if (!ErrorCheckUtility.ValidateCrc(rawMessage, ErrorCheckBytes))
            {
                throw new ModbusExceptionLRC();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _serialPort?.Dispose();
                _disposed = true;
            }
        }

    }
}