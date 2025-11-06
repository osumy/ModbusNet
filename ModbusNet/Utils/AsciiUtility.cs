using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace ModbusNet.Core.Utils
{
    public static class AsciiUtility
    {
        /// <summary>
        ///     Converts an array of bytes to an ASCII byte array.
        /// </summary>
        /// <param name="numbers">The byte array.</param>
        /// <returns>An array of ASCII byte values.</returns>
        public static byte[] GetAsciiBytes(params byte[] numbers)
        {
            return Encoding.UTF8.GetBytes(numbers.SelectMany(n => n.ToString("X2")).ToArray());
        }


        /// <summary>
        ///     Converts a hex string to a byte array.
        /// </summary>
        /// <param name="hex">The hex string.</param>
        /// <returns>Array of bytes.</returns>
        public static byte[] HexToBytes(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException(nameof(hex));
            }

            if (hex.Length % 2 != 0)
            {
                throw new FormatException("Hex string must have even number of characters."); // TODO: Add resource
            }

            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        // Encode bytes to ASCII HEX into destination span. Returns number of bytes written (2 * src.Length).
        public static int EncodeHex(ReadOnlySpan<byte> src, Span<byte> dest)
        {
            if (dest.Length < src.Length * 2) throw new ArgumentException("dest too small");
            int pos = 0;
            foreach (var b in src)
            {
                dest[pos++] = (byte)GetHexChar((byte)(b >> 4));
                dest[pos++] = (byte)GetHexChar((byte)(b & 0x0F));
            }
            return pos;
        }

        // Decode ASCII hex bytes (not chars) -> raw bytes. Returns bytes written or -1 on invalid hex.
        // Accepts upper or lower-case hex. Input length must be even.
        public static int DecodeHex(ReadOnlySpan<byte> src, Span<byte> dest)
        {
            if ((src.Length & 1) != 0) return -1;
            if (dest.Length < src.Length / 2) throw new ArgumentException("dest too small");
            int j = 0;
            for (int i = 0; i < src.Length; i += 2)
            {
                int hi = FromHexChar((char)src[i]);
                int lo = FromHexChar((char)src[i + 1]);
                if (hi < 0 || lo < 0) return -1;
                dest[j++] = (byte)((hi << 4) | lo);
            }
            return j;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static char GetHexChar(byte v) => (char)(v < 10 ? '0' + v : 'A' + (v - 10));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int FromHexChar(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            return -1;
        }
    }
}
