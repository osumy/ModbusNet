using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class ReportServerIdMessage
    {
        internal static byte[] BuildRequestPDU()
        {
            var pdu = new byte[1];

            pdu[0] = (byte)ModbusFunctionCode.Report_Server_ID;

            return pdu;
        }
    }
}
