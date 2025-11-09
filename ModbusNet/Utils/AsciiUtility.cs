namespace ModbusNet.Utils
{
    public static class AsciiUtility
    {
        /// <summary>
        ///     Converts an array of bytes to an ASCII byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>An array of ASCII byte values.</returns>
        /// <remarks>
        /// Example:
        /// { 255, 16, 1 } -> [70, 70, 49, 48, 48, 49]
        /// </remarks>

        public static byte[] ToAsciiBytes(params byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var asciiBytes = new byte[bytes.Length * 2];
            var hexChars = "0123456789ABCDEF".AsSpan();

            for (int i = 0; i < bytes.Length; i++)
            {
                var b = bytes[i];
                asciiBytes[i * 2] = (byte)hexChars[b >> 4];
                asciiBytes[i * 2 + 1] = (byte)hexChars[b & 0x0F];
            }

            return asciiBytes;
        }


        /// <summary>
        ///     Converts an ASCII array to a byte array.
        /// </summary>
        /// <param name="asciiBytes">The ASCII array</param>
        /// <returns>Array of bytes.</returns>
        public static byte[] FromAsciiBytes(byte[] asciiBytes)
        {
            if (asciiBytes == null)
                throw new ArgumentNullException(nameof(asciiBytes));

            if (asciiBytes.Length % 2 != 0)
                throw new ArgumentException("Input must have even length");

            var result = new byte[asciiBytes.Length / 2];

            for (int i = 0; i < result.Length; i++)
            {
                byte high = asciiBytes[i * 2];
                byte low = asciiBytes[i * 2 + 1];

                result[i] = (byte)((HexCharToValue(high) << 4) | HexCharToValue(low));
            }

            return result;
        }

        private static byte HexCharToValue(byte hexChar)
        {
            if (hexChar >= '0' && hexChar <= '9')
                return (byte)(hexChar - '0');
            if (hexChar >= 'A' && hexChar <= 'F')
                return (byte)(hexChar - 'A' + 10);

            throw new ArgumentException($"Invalid hex character: {(char)hexChar}");
        }
    }
}
