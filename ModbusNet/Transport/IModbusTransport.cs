namespace ModbusNet.Transport
{
    public interface IModbusTransport : IDisposable
    {
        byte[] SendRequest(byte[] request);
        bool IsConnected { get; }
        void Connect();
        void Disconnect();
    }
}