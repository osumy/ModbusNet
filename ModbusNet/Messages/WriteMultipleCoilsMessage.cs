using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class WriteMultipleCoilsMessage
    {
        internal static byte[] BuildRequestPDU(ushort startAddress, bool[] data)
        {
            // Calculate number of bytes needed to store the coil data
            int byteCount = (data.Length + 7) / 8;
            int pduLength = 6 + byteCount; // Function code + start address (2) + quantity (2) + byte count (1) + data bytes
            var pdu = new byte[pduLength];

            pdu[0] = (byte)ModbusPublicFunctionCode.Write_Multiple_Coils;
            pdu[1] = (byte)(startAddress >> 8); // High byte of start address
            pdu[2] = (byte)(startAddress & 0xFF); // Low byte of start address
            pdu[3] = (byte)(data.Length >> 8); // High byte of quantity of outputs
            pdu[4] = (byte)(data.Length & 0xFF); // Low byte of quantity of outputs
            pdu[5] = (byte)byteCount; // Byte count

            // Pack the coil data into bytes
            for (int i = 0; i < data.Length; i++)
            {
                int byteIndex = 6 + (i / 8); // Data starts at index 6
                int bitIndex = i % 8;

                if (data[i])
                {
                    pdu[byteIndex] |= (byte)(1 << bitIndex);
                }
                else
                {
                    pdu[byteIndex] &= (byte)~(1 << bitIndex);
                }
            }

            return pdu;
        }
    }
}