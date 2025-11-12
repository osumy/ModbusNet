namespace ModbusNet.Messages
{
    public class ModbusResponse
    {
        public ushort[] Registers { get; private set; }
        public bool[] StatusArray { get; private set; }


        public ModbusResponse(ushort[] rgisters)
        {
            Registers = rgisters;
        }

        public ModbusResponse(bool[] statusArray)
        {
            StatusArray = statusArray;
        }

    }
}
