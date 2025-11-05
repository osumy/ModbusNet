namespace ModbusNet.Core.Utils
{
    public static class ErrorCheckCalculator
    {
        // Computes the Longitudinal Redundancy Check (LRC) for the given data.
        public static byte ComputeLrc(ReadOnlySpan<byte> data)
        {
            byte lrc = 0;
            foreach (byte b in data) // Sum all bytes
                lrc += b;
            return (byte)(-lrc);     // Two's complement
        }

        // Validates the LRC against the expected value.
        public static bool ValidateLrc(ReadOnlySpan<byte> data, byte expectedLrc)
        {
            return ComputeLrc(data) == expectedLrc;
        }

        // Computes the Cyclic Redundancy Check (CRC) for the given data.

        public static ushort ComputeCrc(ReadOnlySpan<byte> data)
        {
            ushort crc = 0xFFFF;
            foreach (byte b in data)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return crc;
        }

        // Validates the CRC against the expected value.
        public static bool ValidateCrc(ReadOnlySpan<byte> data, ushort expectedCrc)
        {
            return ComputeCrc(data) == expectedCrc;
        }
    }
}
