using System;
namespace ModbusNet.Core
{
    public class ModbusProtocolException : Exception
    {
        public ModbusProtocolException(string message) : base(message) { }
    }

    public class ModbusCrcException : ModbusProtocolException
    {
        public ModbusCrcException(string message) : base(message) { }
    }
}
