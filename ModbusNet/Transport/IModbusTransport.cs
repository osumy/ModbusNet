using ModbusNet.Messages;

namespace ModbusNet.Transport
{
    public interface IModbusTransport : IDisposable
    {
        ushort[] SendRequest(byte[] request);
        bool IsConnected { get; }

        //int Retries { get; set; }

        //uint RetryOnOldResponseThreshold { get; set; }

        //bool SlaveBusyUsesRetryCount { get; set; }

        //int WaitToRetryMilliseconds { get; set; }

        //int ReadTimeout { get; set; }

        //int WriteTimeout { get; set; }

        //T UnicastMessage<T>(IModbusMessage message) where T : IModbusMessage, new();

        //byte[] ReadRequest();

        //byte[] BuildMessageFrame(IModbusMessage message);

        //void Write(IModbusMessage message);

        //IStreamResource StreamResource { get; }
    }
}