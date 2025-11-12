using ModbusNet.Enum;

namespace ModbusNet.Transport
{
    public abstract class ModbusTransportBase : IModbusTransport
    {
        public void ValidatePDU(byte[] responsePdu, byte expectedFunctionCode)
        {
            if (responsePdu == null || responsePdu.Length == 0)
                throw new ArgumentException("Response PDU is empty");

            byte responseFC = responsePdu[0];

            // Check for exception using the existing extension method
            if (((ModbusPublicFunctionCode)responseFC).IsError())
            {
                if (responsePdu.Length < 2)
                    throw new InvalidOperationException("Incomplete exception response");

                byte exceptionCode = responsePdu[1];
                throw new ModbusException(expectedFunctionCode, exceptionCode);
            }

            // Validate function code matches expected
            if (responseFC != expectedFunctionCode)
                throw new InvalidOperationException($"Unexpected function code: expected {expectedFunctionCode:X2}, got {responseFC:X2}");
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
