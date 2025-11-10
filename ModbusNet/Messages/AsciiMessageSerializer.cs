using ModbusNet.Utils;

namespace ModbusNet.Messages
{
    public sealed class AsciiMessageSerializer
    {
        public static byte[] BuildAsciiFrame(byte slaveAddress, byte[] pdu, byte[] startDelimiter, byte[] endDelimiter)
        {
            // Create the message frame: slave address + PDU
            var messageFrame = new byte[1 + pdu.Length];
            messageFrame[0] = slaveAddress;
            Array.Copy(pdu, 0, messageFrame, 1, pdu.Length);

            var msgFrameAscii = AsciiUtility.ToAsciiBytes(messageFrame);
            var lrcAscii = AsciiUtility.ToAsciiBytes(ErrorCheckUtility.ComputeLrc(messageFrame));

            // Convert to ASCII: ':' + hex chars of messageFrame + hex chars of LRC + CRLF
            var frame = new MemoryStream(startDelimiter.Length + msgFrameAscii.Length + lrcAscii.Length + endDelimiter.Length);
            frame.Write(startDelimiter, 0, startDelimiter.Length);
            frame.Write(msgFrameAscii, 0, msgFrameAscii.Length);
            frame.Write(lrcAscii, 0, lrcAscii.Length);
            frame.Write(endDelimiter, 0, endDelimiter.Length);

            return frame.ToArray();
        }
    }
}
