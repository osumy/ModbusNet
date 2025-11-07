namespace ModbusNet.Messages.Responses
{
    public class WriteSingleRegisterResponse
    {
        public byte SlaveId { get; }
        public ushort Address { get; }
        public ushort Value { get; }

        private WriteSingleRegisterResponse(byte slaveId, ushort address, ushort value)
        {
            SlaveId = slaveId;
            Address = address;
            Value = value;
        }

        public static WriteSingleRegisterResponse Deserialize(byte slaveId, byte[] data)
        {
            if (data.Length != 6 || data[0] != slaveId || data[1] != 0x06)
                throw new ArgumentException("Invalid response data");

            var address = (ushort)((data[2] << 8) | data[3]);
            var value = (ushort)((data[4] << 8) | data[5]);

            return new WriteSingleRegisterResponse(slaveId, address, value);
        }
    }
}
