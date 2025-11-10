using ModbusNet.Messages;

namespace ModbusNet.Transport
{
    public interface IModbusSerialTransport : IModbusTransport
    {
        void DiscardInBuffer();

        bool CheckFrame { get; set; }

        //bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame);

        void IgnoreResponse();
    }
}
