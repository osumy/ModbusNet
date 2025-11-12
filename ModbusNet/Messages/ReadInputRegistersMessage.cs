
using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class ReadInputRegistersMessage
    {
        internal static byte[] BuildRequestPDU(ushort startAddress, ushort numberOfPoints)
        {
            var pdu = new byte[5];

            pdu[0] = (byte)ModbusPublicFunctionCode.Read_Input_Registers;
            pdu[1] = (byte)(startAddress >> 8); // High byte of start address
            pdu[2] = (byte)(startAddress & 0xFF); // Low byte of start address
            pdu[3] = (byte)(numberOfPoints >> 8); // High byte of count
            pdu[4] = (byte)(numberOfPoints & 0xFF); // Low byte of count

            return pdu;
        }
    }
}
