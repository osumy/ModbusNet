namespace ModbusNet.Frames
{
    public record ModbusAsciiFrame(
        byte UnitId,
        byte FunctionCode,
        ReadOnlyMemory<byte> Data,
        byte Lrc
    );
}