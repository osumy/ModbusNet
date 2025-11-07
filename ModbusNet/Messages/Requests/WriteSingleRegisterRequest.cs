namespace ModbusNet.Messages.Requests
{
    public class WriteSingleRegisterRequest
    {
        public byte SlaveId { get; }
        public ushort Address { get; }
        public ushort Value { get; }

        public WriteSingleRegisterRequest(byte slaveId, ushort address, ushort value)
        {
            SlaveId = slaveId;
            Address = address;
            Value = value;
        }

        public byte[] Serialize()
        {
            var data = new byte[6];
            data[0] = SlaveId;
            data[1] = 0x06; // Function code
            data[2] = (byte)(Address >> 8); // High byte of address
            data[3] = (byte)(Address & 0xFF); // Low byte of address
            data[4] = (byte)(Value >> 8); // High byte of value
            data[5] = (byte)(Value & 0xFF); // Low byte of value

            return data;
        }
    }
}
