using ModbusNet.Core.Messages;
using System.Buffers;

namespace ModbusNet.Messages.Interfaces
{
    public interface IMessageSerializer
    {
        // Serializes ModbusMessage to a full ASCII frame (including ':' and CRLF) into the writer.
        void WriteAsciiFrame(ModbusMessage message, IBufferWriter<byte> writer);

        // Try parse a full ASCII frame from a ReadOnlySequence<byte>. If success, returns true and sets message and consumed position.
        // consumed will be a SequencePosition that marks how many bytes were consumed from the sequence (so caller can slice).
        bool TryParseAsciiFrame(ReadOnlySequence<byte> buffer, out ModbusMessage? message, out SequencePosition consumed);
    }
}
