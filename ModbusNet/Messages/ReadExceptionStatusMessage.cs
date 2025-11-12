
using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class ReadExceptionStatusMessage
    {
        internal static byte[] BuildRequestPDU()
        {
            var pdu = new byte[1];

            pdu[0] = (byte)ModbusPublicFunctionCode.Read_Exception_Status;

            return pdu;
        }
    }
}
