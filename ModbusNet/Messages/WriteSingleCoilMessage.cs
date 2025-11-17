
using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class WriteSingleCoilMessage
    {
        internal static byte[] BuildRequestPDU(ushort address, ushort value)
        {
            var pdu = new byte[5];

            pdu[0] = (byte)ModbusFunctionCode.Write_Single_Coil;
            pdu[1] = (byte)(address >> 8); // High byte of address
            pdu[2] = (byte)(address & 0xFF); // Low byte of address
            pdu[3] = (byte)(value >> 8); // High byte of value
            pdu[4] = (byte)(value & 0xFF); // Low byte of value

            return pdu;
        }
    }
}
