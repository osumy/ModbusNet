namespace ModbusNet.Device.Validation
{
    public enum ValidationType
    {
        ReadCoils,
        ReadDiscreteInputs,
        ReadMultipleHoldingRegisters,
        ReadInputRegisters,
        WriteMultipleCoils,
        WriteMultipleRegisters,
        ReadWriteMultipleRegisters,
        WriteFileRecord,
        WriteSingleCoil
    }
}
