using ModbusNet.Messages;

namespace ModbusNet.Transport
{
    public interface IModbusTransport : IDisposable
    {
        ushort[] SendRequest(byte[] request);
        bool IsConnected { get; }


        //T UnicastMessage<T>(IModbusMessage message) where T : IModbusMessage, new();

        //byte[] ReadRequest();

        //byte[] BuildMessageFrame(IModbusMessage message);

        //void Write(IModbusMessage message);

        //IStreamResource StreamResource { get; }
    }
}