namespace ModbusNet.Messages.Responses
{
    namespace ModbusNet.Messages.Responses
    {
        public class ReadHoldingRegistersResponse
        {
            public byte SlaveId { get; }
            public ushort[] Registers { get; }

            private ReadHoldingRegistersResponse(byte slaveId, ushort[] registers)
            {
                SlaveId = slaveId;
                Registers = registers;
            }

            public static ReadHoldingRegistersResponse Deserialize(byte slaveId, byte[] data)
            {
                if (data.Length < 2 || data[0] != slaveId || data[1] != 0x03)
                    throw new ArgumentException("Invalid response data");

                var byteCount = data[2];
                if (data.Length != 3 + byteCount)
                    throw new ArgumentException("Invalid response length");

                var registerCount = byteCount / 2;
                var registers = new ushort[registerCount];

                for (int i = 0; i < registerCount; i++)
                {
                    var highByte = data[3 + (i * 2)];
                    var lowByte = data[3 + (i * 2) + 1];
                    registers[i] = (ushort)((highByte << 8) | lowByte);
                }

                return new ReadHoldingRegistersResponse(slaveId, registers);
            }
        }
    }
}
