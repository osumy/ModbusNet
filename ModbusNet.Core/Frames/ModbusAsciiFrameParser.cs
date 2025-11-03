using System.Globalization;

namespace ModbusNet.Core.Frames
{
    public static class ModbusAsciiFrameParser
    {
        public static ModbusAsciiFrame Parse(string frame)
        {
            if (string.IsNullOrWhiteSpace(frame))
                throw new ArgumentException("Frame cannot be empty.");

            if (frame[0] != ':')
                throw new FormatException("Invalid start delimiter.");
            if (!frame.EndsWith("\r\n"))
                throw new FormatException("Frame must end with CRLF.");

            // Remove delimiters
            string content = frame.Substring(1, frame.Length - 3); // remove ':' and CRLF

            // Convert ASCII hex to bytes
            byte[] bytes = new byte[content.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(content.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            if (bytes.Length < 3)
                throw new FormatException("Frame too short.");

            byte unitId = bytes[0];
            byte functionCode = bytes[1];
            byte lrc = bytes[^1];
            var data = new ReadOnlyMemory<byte>(bytes, 2, bytes.Length - 3);

            // Validate LRC
            if (!LrcCalculator.Validate(bytes.AsSpan(0, bytes.Length - 1), lrc))
                throw new FormatException("Invalid LRC checksum.");

            return new ModbusAsciiFrame(unitId, functionCode, data, lrc);
        }
    }
}