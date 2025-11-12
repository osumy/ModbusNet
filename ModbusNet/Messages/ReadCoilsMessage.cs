
using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class ReadCoilsMessage
    {
        internal static byte[] BuildRequestPDU(ushort startAddress, ushort numberOfPoints)
        {
            var pdu = new byte[5];

            pdu[0] = (byte)ModbusPublicFunctionCode.Read_Coils;
            pdu[1] = (byte)(startAddress >> 8); // High byte of start address
            pdu[2] = (byte)(startAddress & 0xFF); // Low byte of start address
            pdu[3] = (byte)(numberOfPoints >> 8); // High byte of count
            pdu[4] = (byte)(numberOfPoints & 0xFF); // Low byte of count

            return pdu;
        }

        internal static ModbusResponse ParseResponsePDU(byte[] pdu)
        {
            byte byteCount = pdu[1];

            // Calculate number of coils from byte count
            int coilCount = byteCount * 8; // 8 bits per byte
            bool[] coils = new bool[coilCount];

            int pos = 2; // start of data
            for (int i = 0; i < byteCount; i++)
            {
                byte b = pdu[pos];
                for (int bit = 0; bit < 8; bit++)
                {
                    int index = i * 8 + bit;
                    if (index < coilCount)
                    {
                        coils[index] = (b & (1 << bit)) != 0;
                    }
                }
                pos++;
            }

            return new ModbusResponse(coils);
        }
    }
}
