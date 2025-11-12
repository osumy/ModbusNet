using System;
namespace ModbusNet
{
    public class ModbusException : Exception
    {
        public byte ExceptionCode { get; }
        public byte OriginalFunctionCode { get; }

        public ModbusException(byte originalFC, byte exceptionCode)
            : base($"Modbus exception: FC={originalFC:X2}, Code={exceptionCode}")
        {
            OriginalFunctionCode = originalFC;
            ExceptionCode = exceptionCode;
        }


    }

    public class ModbusExceptionLRC : Exception
    {
        public ModbusExceptionLRC()
            : base($"Modbus LRC exception.")
        { }
    }

    public class ModbusExceptionCRC : Exception
    {
        public ModbusExceptionCRC()
            : base($"Modbus CRC exception.")
        { }
    }
}
