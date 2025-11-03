namespace ModbusNet.Core.Frames
{
    public static class LrcCalculator
    {
        public static byte Compute(ReadOnlySpan<byte> data)
        {
            byte lrc = 0;
            foreach (byte b in data)
                lrc += b;

            lrc = (byte)(-lrc);
            return lrc;
        }

        public static bool Validate(ReadOnlySpan<byte> data, byte expectedLrc)
        {
            return Compute(data) == expectedLrc;
        }
    }
}