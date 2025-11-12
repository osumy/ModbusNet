using ModbusNet.Messages;

namespace ModbusNet.Transport
{
    public interface IModbusTransport : IDisposable
    {
        public byte[] BuildRequest(byte slaveAddress, byte[] pdu);
        public void ValidatePDU(byte[] responsePdu, byte expectedFunctionCode);
        public ModbusResponse SendRequestReceiveResponse(byte[] request);
        public void SendRequestIgnoreResponse(byte[] request);
    }
}