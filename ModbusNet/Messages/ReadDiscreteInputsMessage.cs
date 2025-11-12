using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class ReadDiscreteInputsMessage
    {
        internal static byte[] BuildRequestPDU(ushort startAddress, ushort numberOfPoints)
        {
            var pdu = new byte[5];

            pdu[0] = (byte)ModbusPublicFunctionCode.Read_Discrete_Inputs;
            pdu[1] = (byte)(startAddress >> 8); // High byte of start address
            pdu[2] = (byte)(startAddress & 0xFF); // Low byte of start address
            pdu[3] = (byte)(numberOfPoints >> 8); // High byte of count
            pdu[4] = (byte)(numberOfPoints & 0xFF); // Low byte of count

            return pdu;
        }

        internal static ModbusResponse ParseResponsePDU(byte[] pdu)
        {
            byte byteCount = pdu[1];

            // Calculate number of discrete inputs from byte count
            int discreteInputCount = byteCount * 8; // 8 bits per byte
            bool[] discreteInputs = new bool[discreteInputCount];

            int pos = 2; // start of data
            for (int i = 0; i < byteCount; i++)
            {
                byte b = pdu[pos];
                for (int bit = 0; bit < 8; bit++)
                {
                    int index = i * 8 + bit;
                    if (index < discreteInputCount)
                    {
                        discreteInputs[index] = (b & (1 << bit)) != 0;
                    }
                }
                pos++;
            }

            return new ModbusResponse(discreteInputs);
        }
    }
}
