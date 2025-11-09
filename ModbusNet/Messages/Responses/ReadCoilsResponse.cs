using ModbusNet.Messages;

namespace ModbusNet.Messages.Responses
{
    public class ReadCoilsResponse 
    {
        public byte SlaveId { get; }
        public byte FunctionCode => 0x01;
        public bool IsError => false;
        public byte ExceptionCode => 0;

        public bool[] Coils { get; }

        public ReadCoilsResponse(byte slaveId, byte[] data)
        {
            SlaveId = slaveId;
            Coils = ParseCoils(data);
        }

        private bool[] ParseCoils(byte[] data)
        {
            if (data.Length < 2)
                return new bool[0];

            var byteCount = data[1];
            var coils = new bool[byteCount * 8];

            for (int i = 0; i < byteCount; i++)
            {
                var b = data[2 + i];
                for (int bit = 0; bit < 8; bit++)
                {
                    if (i * 8 + bit < coils.Length)
                    {
                        coils[i * 8 + bit] = (b & (1 << bit)) != 0;
                    }
                }
            }

            return coils;
        }

        public static ReadCoilsResponse Deserialize(byte slaveId, byte[] data)
        {
            if (data.Length < 1)
                throw new ArgumentException("Invalid response data");

            //if (data[1] == 0x81) // Error response // TODO
            //{
            //    throw new ModbusException(data[2]);
            //}

            return new ReadCoilsResponse(slaveId, data);
        }
    }
}