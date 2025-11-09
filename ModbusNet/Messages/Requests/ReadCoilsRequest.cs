using ModbusNet.Messages;

namespace ModbusNet.Messages.Requests
{
    public class ReadCoilsRequest 
    {
        public byte SlaveId { get; }
        public byte FunctionCode => 0x01;
        public ushort StartAddress { get; }
        public ushort NumberOfPoints { get; }

        public ReadCoilsRequest(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > 2000)
                throw new ArgumentException("Number of points must be between 1 and 2000");

            SlaveId = slaveId;
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        public byte[] Serialize()
        {
            var data = new byte[6];
            data[0] = SlaveId;
            data[1] = FunctionCode;
            data[2] = (byte)(StartAddress >> 8);
            data[3] = (byte)StartAddress;
            data[4] = (byte)(NumberOfPoints >> 8);
            data[5] = (byte)NumberOfPoints;

            return data;
        }
    }
}