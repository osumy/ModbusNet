using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class GetCommunicationEventCounterMessage
    {
        internal static byte[] BuildRequestPDU()
        {
            var pdu = new byte[1];

            pdu[0] = (byte)ModbusFunctionCode.Get_Com_Event_Counter;

            return pdu;
        }
    }
}
