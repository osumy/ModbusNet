namespace ModbusNet.Device.Validation
{
    public static class ValidationLimits
    {
        public const int MaxReadCoils = 2000;
        public const int MaxReadDiscreteInputs = 2000;
        public const int MaxReadMultipleHoldingRegisters = 125;
        public const int MaxReadInputRegisters = 125;
        public const int MaxWriteMultipleCoils = 1968;
        public const int MaxWriteMultipleRegisters = 123;
        public const int MaxReadWriteMultipleRegistersRead = 125;
        public const int MaxReadWriteMultipleRegistersWrite = 121;
        public const int MaxWriteFileRecord = 244;
    }
}
