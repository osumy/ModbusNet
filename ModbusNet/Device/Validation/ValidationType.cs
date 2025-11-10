namespace ModbusNet.Device.Validation
{
    public enum ValidationType
    {
        ReadCoils,
        ReadDiscreteInputs,
        ReadHoldingRegisters,
        ReadInputRegisters,
        WriteMultipleCoils,
        WriteMultipleRegisters,
        ReadWriteMultipleRegisters,
        WriteFileRecord
    }
}
