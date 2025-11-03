using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace ModbusNet.Core
{
    internal static class AsciiHelpers
    {
        // Compute LRC: LRC = (-sum) & 0xFF where sum is over address, function, data bytes.
        public static byte ComputeLrc(ReadOnlySpan<byte> data)
        {
            int sum = 0;
            foreach (var b in data) sum += b;
            return (byte)((-sum) & 0xFF);
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
