
using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class GetCommunicationEventLogMessage
    {
        internal static byte[] BuildRequestPDU()
        {
            var pdu = new byte[1];

            pdu[0] = (byte)ModbusPublicFunctionCode.Get_Com_Event_Log;

            return pdu;
        }
    }
}
