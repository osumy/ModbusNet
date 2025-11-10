using ModbusNet.Utils;
using System.Buffers;
using System.Runtime;
using System.Text;

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

        public static byte[] BuildAsciiFrame2(byte slaveAddress, byte[] pdu, byte[] startDelimiter, byte[] endDelimiter)
        {
            // Create the message frame: slave address + PDU
            var messageFrame = new byte[1 + pdu.Length];
            messageFrame[0] = slaveAddress;
            Array.Copy(pdu, 0, messageFrame, 1, pdu.Length);

            // Convert entire message frame to ASCII hex string first
            var hexString = BitConverter.ToString(messageFrame).Replace("-", "");

            // Compute LRC on the raw message frame bytes
            byte lrc = ErrorCheckUtility.ComputeLrc(messageFrame);
            var lrcHex = lrc.ToString("X2");

            // Build the complete ASCII frame
            var frameBuilder = new System.Text.StringBuilder();
            frameBuilder.Append(Encoding.ASCII.GetString(startDelimiter)); // ":"
            frameBuilder.Append(hexString);                                // Message data as hex
            frameBuilder.Append(lrcHex);                                   // LRC as hex
            frameBuilder.Append(Encoding.ASCII.GetString(endDelimiter));   // "\r\n"

            return Encoding.ASCII.GetBytes(frameBuilder.ToString());
        }

        //public bool TryParseAsciiFrame(ReadOnlySequence<byte> buffer, out ModbusMessage? message, out SequencePosition consumed)
        //{
        //    // We need to find a ':' start and CRLF end (':' .. CR LF)
        //    // For streaming support, we will support single-segment fast path and fallback to pooled copy.

        //    message = null;
        //    consumed = buffer.Start;

        //    if (buffer.Length == 0) return false;

        //    // Find ':' position
        //    SequencePosition startPos = FindByte(buffer, Colon);
        //    if (startPos.Equals(buffer.End)) return false; // no start in buffer yet

        //    // Slice from start
        //    var afterStart = buffer.Slice(startPos);
        //    // Try to find CRLF in afterStart
        //    var crPos = FindByte(afterStart, Cr);
        //    if (crPos.Equals(afterStart.End)) return false; // no CR yet -> incomplete
        //    // ensure next byte is LF and that both are in sequence bounds
        //    var next = afterStart.Slice(crPos).Slice(0, 2); // should be CR LF
        //    if (next.Length < 2) return false; // incomplete
        //    Span<byte> crlfCheck = stackalloc byte[2];
        //    afterStart.Slice(crPos, 2).CopyTo(crlfCheck);
        //    if (crlfCheck[0] != Cr || crlfCheck[1] != Lf)
        //    {
        //        // malformed (no CRLF) — consume up to CR (to avoid infinite loop) and throw protocol exception
        //        // We'll treat it as malformed frame
        //        throw new ModbusProtocolException("ASCII frame missing CRLF terminator.");
        //    }

        //    // Now compute frame length in bytes (ASCII hex content length)
        //    // ASCII payload is between ':' and CR (exclusive). Convert to contiguous bytes of hex (2 chars per data byte)
        //    // We'll copy content into pooled buffer for simplicity
        //    var sliceAfterStart = afterStart.Slice(1);
        //    var crPosInSlice = sliceAfterStart.PositionOf(Cr);
        //    if (crPosInSlice == null) return false; // no CR found
        //    int asciiContentLength = (int)sliceAfterStart.Slice(0, crPosInSlice.Value).Length;
        //    // Simpler: copy whole afterStart until CR into buffer
        //    // For correctness and to keep code concise: copy using SequenceToArray
        //    int copyLength = (int)afterStart.Slice(0, (SequencePosition)afterStart.Slice(0).PositionOf(Cr)).Length;
        //    byte[] asciiBytes = ArrayPool<byte>.Shared.Rent(copyLength);
        //    try
        //    {
        //        afterStart.Slice(0, copyLength).CopyTo(asciiBytes);

        //        // asciiBytes[0] == ':', ascii hex content is asciiBytes[1..(copyLength-?)]
        //        // Extract hex span (exclude ':')
        //        var hexSpan = new ReadOnlySpan<byte>(asciiBytes, 1, copyLength - 1);
        //        // hexSpan length must be even and >= (unit+func + LRC)*2 => at least (2+1)*2 = 6 chars
        //        if ((hexSpan.Length & 1) != 0) throw new ModbusProtocolException("ASCII hex payload length must be even.");

        //        int rawLen = hexSpan.Length / 2;
        //        if (rawLen < 3) throw new ModbusProtocolException("ASCII payload too short.");

        //        byte[] raw = ArrayPool<byte>.Shared.Rent(rawLen);
        //        try
        //        {
        //            int written = 0;//AsciiUtility.DecodeHex(hexSpan, raw);
        //            if (written != rawLen) throw new ModbusProtocolException("Invalid hex characters in ASCII frame.");

        //            // raw layout: [Unit][Function][Data...][LRC]
        //            int dataLen = rawLen - 3; // unit + function + lrc
        //            byte unit = raw[0];
        //            byte function = raw[1];
        //            var data = new byte[dataLen];
        //            if (dataLen > 0) Array.Copy(raw, 2, data, 0, dataLen);
        //            byte lrc = raw[rawLen - 1];

        //            // Verify LRC
        //            Span<byte> lrcSpan = stackalloc byte[2 + (dataLen)];
        //            lrcSpan[0] = unit;
        //            lrcSpan[1] = function;
        //            if (dataLen > 0) new ReadOnlySpan<byte>(raw, 2, dataLen).CopyTo(lrcSpan.Slice(2));

        //            byte computed = ErrorCheckUtility.ComputeLrc(lrcSpan);
        //            if (computed != lrc) throw new ModbusCrcException($"LRC mismatch: computed=0x{computed:X2} received=0x{lrc:X2}");

        //            message = new ModbusMessage(unit, function, data);
        //            // consumed is start + (copyLength) + 2 (for CRLF)
        //            var afterCrLf = buffer.GetPosition(copyLength + 2, startPos); // startPos at ':'
        //            consumed = afterCrLf;
        //            return true;
        //        }
        //        finally
        //        {
        //            ArrayPool<byte>.Shared.Return(raw);
        //        }
        //    }
        //    finally
        //    {
        //        ArrayPool<byte>.Shared.Return(asciiBytes);
        //    }
        //}

        //// helper: find index of a byte in sequence. Returns SequencePosition.End if not found.
        //private static SequencePosition FindByte(ReadOnlySequence<byte> seq, byte value)
        //{
        //    var reader = new SequenceReader<byte>(seq);
        //    while (!reader.End)
        //    {
        //        if (reader.TryReadTo(out ReadOnlySpan<byte> segment, value, advancePastDelimiter: false))
        //        {
        //            // We found delimiter at reader.Position + segment.Length
        //            return seq.GetPosition(reader.Consumed, seq.Start);
        //        }
        //        // If TryReadTo can't be used (older API), we can fallback — but SequenceReader is ok in net8
        //        break;
        //    }

        //    // fallback simple scan (makes sequence contiguous first if multi segment)
        //    if (seq.IsSingleSegment)
        //    {
        //        var span = seq.First.Span;
        //        for (int i = 0; i < span.Length; i++)
        //            if (span[i] == value) return seq.GetPosition(i, seq.Start);
        //        return seq.End;
        //    }

        //    // multi-segment:
        //    long offset = 0;
        //    foreach (var memory in seq)
        //    {
        //        var span = memory.Span;
        //        for (int i = 0; i < span.Length; i++)
        //        {
        //            if (span[i] == value) return seq.GetPosition(offset + i);
        //        }
        //        offset += span.Length;
        //    }
        //    return seq.End;
        //}
    }
}
