using ModbusNet;
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
            _serialPort = serialPort;
            _settings = settings;

            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }


        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            throw new NotImplementedException();
        }
        public override void SendRequestIgnoreResponse(byte[] request)
        {
            throw new NotImplementedException();
        }

        public override void ChecksumsMatch(byte[] rawMessage, byte[] ErrorCheckBytes)
        {
            throw new NotImplementedException();
        }
        public override byte[] BuildRequest(byte slaveAddress, byte[] pdu)
        {
            throw new NotImplementedException();
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