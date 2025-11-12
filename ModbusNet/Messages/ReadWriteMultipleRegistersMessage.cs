using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class ReadWriteMultipleRegistersMessage
    {
        internal static byte[] BuildRequestPDU(ushort readStartAddress, ushort numberOfPointsToRead,
                                                     ushort writeStartAddress, ushort[] writeData)
        {
            // Calculate number of bytes needed to store the write register data
            int writeDataByteCount = writeData.Length * 2;
            int pduLength = 10 + writeDataByteCount; // Function code (1) + read start (2) + read count (2) + write start (2) + write count (2) + byte count (1) + write data bytes
            var pdu = new byte[pduLength];

            pdu[0] = (byte)ModbusPublicFunctionCode.ReadWrite_Multiple_Registers;
            pdu[1] = (byte)(readStartAddress >> 8); // High byte of read starting address
            pdu[2] = (byte)(readStartAddress & 0xFF); // Low byte of read starting address
            pdu[3] = (byte)(numberOfPointsToRead >> 8); // High byte of number of registers to read
            pdu[4] = (byte)(numberOfPointsToRead & 0xFF); // Low byte of number of registers to read
            pdu[5] = (byte)(writeStartAddress >> 8); // High byte of write starting address
            pdu[6] = (byte)(writeStartAddress & 0xFF); // Low byte of write starting address
            pdu[7] = (byte)(writeData.Length >> 8); // High byte of number of registers to write
            pdu[8] = (byte)(writeData.Length & 0xFF); // Low byte of number of registers to write
            pdu[9] = (byte)writeDataByteCount; // Byte count of write data

            // Pack the write register data into bytes (each register is 2 bytes, MSB first)
            for (int i = 0; i < writeData.Length; i++)
            {
                int byteIndex = 10 + (i * 2); // Write data starts at index 10, each register is 2 bytes

                // Store high byte first (MSB), then low byte (LSB) - big-endian
                pdu[byteIndex] = (byte)(writeData[i] >> 8);     // High byte of register
                pdu[byteIndex + 1] = (byte)(writeData[i] & 0xFF); // Low byte of register
            }

            return pdu;
        }

        internal static ModbusResponse ParseResponsePDU(byte[] pdu)
        {
            byte byteCount = pdu[1];

            int registerCount = byteCount / 2;
            ushort[] registers = new ushort[registerCount];

            int pos = 2; // start of data
            for (int i = 0; i < registerCount; i++)
            {
                registers[i] = (ushort)((pdu[pos] << 8) | pdu[pos + 1]);
                pos += 2;
            }

            return new ModbusResponse(registers);
        }
    }
}
