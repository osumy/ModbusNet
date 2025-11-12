
using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class WriteMultipleHoldingRegistersMessage
    {
        internal static byte[] BuildRequestPDU(ushort startAddress, ushort[] data)
        {
            // Calculate number of bytes needed to store the register data
            // Each register is 2 bytes, so total data bytes = data.Length * 2
            int dataByteCount = data.Length * 2;
            int pduLength = 6 + dataByteCount; // Function code + start address (2) + quantity (2) + byte count (1) + data bytes
            var pdu = new byte[pduLength];

            pdu[0] = (byte)ModbusPublicFunctionCode.Write_Multiple_Holding_Registers;
            pdu[1] = (byte)(startAddress >> 8); // High byte of start address
            pdu[2] = (byte)(startAddress & 0xFF); // Low byte of start address
            pdu[3] = (byte)(data.Length >> 8); // High byte of quantity of registers
            pdu[4] = (byte)(data.Length & 0xFF); // Low byte of quantity of registers
            pdu[5] = (byte)dataByteCount; // Byte count

            // Pack the register data into bytes (each register is 2 bytes, MSB first)
            for (int i = 0; i < data.Length; i++)
            {
                int byteIndex = 6 + (i * 2); // Data starts at index 6, each register is 2 bytes

                // Store high byte first (MSB), then low byte (LSB) - big-endian
                pdu[byteIndex] = (byte)(data[i] >> 8);     // High byte of register
                pdu[byteIndex + 1] = (byte)(data[i] & 0xFF); // Low byte of register
            }

            return pdu;
        }
    }
}
