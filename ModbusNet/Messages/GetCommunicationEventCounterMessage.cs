using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class GetCommunicationEventCounterMessage
    {
        internal static object BuildRequestPDU()
        {
            var pdu = new byte[1];

            pdu[0] = (byte)ModbusPublicFunctionCode.Get_Com_Event_Counter;

            return pdu;
        }
    }
}
