namespace ModbusNet.Messages.Requests
{
    public class ReadHoldingRegistersRequest
    {
        public byte SlaveId { get; }
        public ushort StartAddress { get; }
        public ushort NumberOfPoints { get; }

        public ReadHoldingRegistersRequest(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > 125)
                throw new ArgumentOutOfRangeException(nameof(numberOfPoints), "Must be 1-125 for Read Holding Registers");

            SlaveId = slaveId;
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        public byte[] Serialize()
        {
            var data = new byte[6];
            data[0] = SlaveId;
            data[1] = 0x03; // Function code
            data[2] = (byte)(StartAddress >> 8); // High byte of start address
            data[3] = (byte)(StartAddress & 0xFF); // Low byte of start address
            data[4] = (byte)(NumberOfPoints >> 8); // High byte of count
            data[5] = (byte)(NumberOfPoints & 0xFF); // Low byte of count

            return data;
        }
    }
}
