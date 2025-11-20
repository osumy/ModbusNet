using ModbusNet.Enum;
using ModbusNet.Messages;

namespace ModbusNet.Transport
{
    public abstract class ModbusTransportBase : IDisposable
    {
        public abstract byte[] BuildRequest(byte slaveAddress, byte[] pdu);

        public ModbusResponse BuildResponse(byte[] pdu)
        {
            byte functionCode = pdu[0];

            return functionCode switch
            {
                (byte)ModbusFunctionCode.Read_Discrete_Inputs =>
                    ReadDiscreteInputsMessage.ParseResponsePDU(pdu),
                (byte)ModbusFunctionCode.Read_Coils =>
                    ReadCoilsMessage.ParseResponsePDU(pdu),
                (byte)ModbusFunctionCode.Read_Input_Registers =>
                    ReadInputRegistersMessage.ParseResponsePDU(pdu),
                (byte)ModbusFunctionCode.Read_Multiple_Holding_Registers =>
                    ReadMultipleHoldingRegistersMessage.ParseResponsePDU(pdu),
                (byte)ModbusFunctionCode.ReadWrite_Multiple_Registers =>
                    ReadWriteMultipleRegistersMessage.ParseResponsePDU(pdu),
                (byte)ModbusFunctionCode.Read_FIFO_Queue =>
                    ReadFifoQueueMessage.ParseResponsePDU(pdu),
                _ => throw new NotSupportedException($"Function code {functionCode:X2} not supported")
            };
        }

        public abstract ModbusResponse SendRequestReceiveResponse(byte[] request);
        public abstract void SendRequestIgnoreResponse(byte[] request);
        public void ValidateFC(byte[] responsePdu, byte expectedFunctionCode)
        {
            if (responsePdu == null || responsePdu.Length == 0)
                throw new ArgumentException("Response PDU is empty");

            byte responseFC = responsePdu[0];

            // Check for exception using the existing extension method
            if (((ModbusFunctionCode)responseFC).IsError())
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
        public void ValidateByteCount(byte[] responsePdu, byte expectedFunctionCode)
        {
            byte byteCount = responsePdu[1];

            if (byteCount != responsePdu.Length - 2)
                throw new FormatException("Byte count mismatch.");
        }
        public abstract void ChecksumsMatch(byte[] rawMessage, byte[] ErrorCheckBytes);

        public abstract void Dispose();
    }
}
