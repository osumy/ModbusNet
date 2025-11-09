using ModbusNet.Core.Messages;

namespace ModbusNet.Messages.Requests
{
    public class ReadHoldingRegistersRequest : IModbusMessage
    {
        public ushort StartAddress { get; set; }
        public ushort NumberOfPoints { get; set; }
        public byte FunctionCode { get; set; }
        public byte SlaveAddress { get; set; }

        public byte[] MessageFrame { get; set; }

        public byte[] ProtocolDataUnit { get; set; }


        public ReadHoldingRegistersRequest(byte functionCode, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        { 
            if (numberOfPoints < 1 || numberOfPoints > 125)
                throw new ArgumentOutOfRangeException(nameof(numberOfPoints), "Must be 1-125 for Read Holding Registers");

            FunctionCode = functionCode;
            SlaveAddress = slaveAddress;
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        public void Serialize()
        {
            var data = new byte[6];
            data[0] = SlaveAddress;
            data[1] = 0x03; // Function code
            data[2] = (byte)(StartAddress >> 8); // High byte of start address
            data[3] = (byte)(StartAddress & 0xFF); // Low byte of start address
            data[4] = (byte)(NumberOfPoints >> 8); // High byte of count
            data[5] = (byte)(NumberOfPoints & 0xFF); // Low byte of count

            MessageFrame = data;
        }

        public void Initialize(byte[] frame)
        {
            throw new NotImplementedException();
        }
    }
}
