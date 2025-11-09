namespace ModbusNet.Messages
{
    public record ModbusMessage(byte UnitId, byte FunctionCode, ReadOnlyMemory<byte> Data);
}
