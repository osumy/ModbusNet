namespace ModbusNet.Utils
{
    /// <summary>
    /// Provides utility methods for converting between binary bytes and ASCII hexadecimal representations.
    /// </summary>
    /// <remarks>
    /// This utility class handles conversion of binary data to and from ASCII-encoded hexadecimal strings,
    /// supporting only uppercase hexadecimal characters (0-9, A-F) as per Modbus ASCII protocol requirements.
    /// </remarks>
    public static class AsciiUtility
    {

        /// <summary>
        /// Converts an array of binary bytes to their ASCII hexadecimal representation.
        /// </summary>
        /// <param name="bytes">The binary byte array to convert.</param>
        /// <returns>An array of ASCII bytes representing the hexadecimal values.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="bytes"/> is null.</exception>
        /// <remarks>
        /// Each input byte is converted to two ASCII bytes representing its hexadecimal value.
        /// Example: { 0xFF, 0x10, 0x01 } converts to [0x46, 0x46, 0x31, 0x30, 0x30, 0x31] which represents "FF1001".
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
        /// Converts an ASCII hexadecimal byte array back to its binary representation.
        /// </summary>
        /// <param name="asciiBytes">The ASCII byte array containing hexadecimal characters.</param>
        /// <returns>The original binary byte array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="asciiBytes"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="asciiBytes"/> has odd length or contains invalid hexadecimal characters.
        /// </exception>
        /// <remarks>
        /// The input must contain only valid uppercase hexadecimal characters (0-9, A-F) and have even length.
        /// Each pair of ASCII bytes is converted to one binary byte.
        /// Example: [0x46, 0x46, 0x31, 0x30] ("FF10") converts to { 0xFF, 0x10 }.
        /// </remarks>
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

        /// <summary>
        /// Converts a single ASCII hexadecimal character to its numeric value.
        /// </summary>
        /// <param name="hexChar">The ASCII character representing a hexadecimal digit.</param>
        /// <returns>The numeric value of the hexadecimal character (0-15).</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="hexChar"/> is not a valid hexadecimal character.</exception>
        /// <remarks>
        /// Only supports uppercase hexadecimal characters: '0'-'9' (values 0-9) and 'A'-'F' (values 10-15).
        /// Lowercase characters and other values are considered invalid.
        /// </remarks>
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
