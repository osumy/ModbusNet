using ModbusNet.Messages;
using ModbusNet.Utils;
using System.Text;

namespace ModbusNet.Frames
{
    public static class ModbusAsciiFrameBuilder
    {
        public static string Build(ModbusMessage message)
        {
            // Combine raw bytes for LRC
            var buffer = new List<byte>
            {
                message.UnitId,
                message.FunctionCode
            };
            buffer.AddRange(message.Data.ToArray());

            byte lrc = ErrorCheckUtility.ComputeLrc(buffer.ToArray());

            // Convert all to ASCII HEX
            var sb = new StringBuilder();
            sb.Append(':');
            foreach (byte b in buffer)
                sb.AppendFormat("{0:X2}", b);
            sb.AppendFormat("{0:X2}", lrc);
            sb.Append("\r\n");

            return sb.ToString();
        }
    }
}