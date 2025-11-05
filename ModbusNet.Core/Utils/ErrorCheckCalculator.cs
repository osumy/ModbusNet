namespace ModbusNet.Core.Utils
{
    /// <summary>
    /// Provides utility methods for error checking using LRC (Longitudinal Redundancy Check) 
    /// and CRC (Cyclic Redundancy Check) algorithms commonly used in Modbus communication.
    /// </summary>
    public static class ErrorCheckCalculator
    {
        //// Longitudinal Redundancy Check (LRC) ////

        /// <summary>
        /// Computes the Longitudinal Redundancy Check (LRC) for the given data.
        /// </summary>
        /// <param name="data">The data to compute the LRC for.</param>
        /// <returns>The computed LRC value as a byte.</returns>
        public static byte ComputeLrc(ReadOnlySpan<byte> data)
        {
            byte lrc = 0;
            foreach (byte b in data) // Sum all bytes
                lrc += b;

            return (byte)(-lrc);     // Two's complement
        }

        /// <summary>
        /// Validates the LRC against the expected value.
        /// </summary>
        /// <param name="data">The data to validate.</param>
        /// <param name="expectedLrc">The expected LRC value.</param>
        /// <returns>True if the computed LRC matches the expected value; otherwise, false.</returns>
        public static bool ValidateLrc(ReadOnlySpan<byte> data, byte expectedLrc)
        {
            return ComputeLrc(data) == expectedLrc;
        }


        ////  Cyclic Redundancy Check (CRC) ////

        private static ushort[]? _crcTable;

        /// <summary>
        /// Ensures the CRC-16 lookup table is initialized. Uses the Modbus polynomial 0xA001.
        /// </summary>
        private static void EnsureCrcTable()
        {
            if (_crcTable != null) return;

            const ushort poly = 0xA001;
            _crcTable = new ushort[256];
            for (int i = 0; i < 256; i++)
            {
                ushort crc = (ushort)i;
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ poly);
                    else
                        crc >>= 1;
                }
                _crcTable[i] = crc;
            }
        }

        /// <summary>
        /// Computes the Cyclic Redundancy Check (CRC-16) for the given data using a lookup table.
        /// </summary>
        /// <param name="data">The data to compute the CRC for.</param>
        /// <returns>The computed CRC-16 value as a ushort.</returns>
        public static ushort ComputeCrc(ReadOnlySpan<byte> data)
        {
            EnsureCrcTable();

            ushort crc = 0xFFFF;
            foreach (byte b in data)
                crc = (ushort)((crc >> 8) ^ _crcTable![(crc ^ b) & 0xFF]);

            // Swap bytes (Modbus sends low byte first)
            return (ushort)(((crc & 0xFF) << 8) | ((crc >> 8) & 0xFF));
        }

        /// <summary>
        /// Validates the CRC against the expected value.
        /// </summary>
        /// <param name="data">The data to validate.</param>
        /// <param name="expectedCrc">The expected CRC value.</param>
        /// <returns>True if the computed CRC matches the expected value; otherwise, false.</returns>
        public static bool ValidateCrc(ReadOnlySpan<byte> data, ushort expectedCrc)
        {
            return ComputeCrc(data) == expectedCrc;
        }
    }
}
