
using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class MaskWriteRegisterMessage
    {
        internal static byte[] BuildRequestPDU(ushort address, ushort andMask, ushort orMask)
        {
            var pdu = new byte[7];

            pdu[0] = (byte)ModbusFunctionCode.Mask_Write_Register;
            pdu[1] = (byte)(address >> 8); // High byte of reference address
            pdu[2] = (byte)(address & 0xFF); // Low byte of reference address
            pdu[3] = (byte)(andMask >> 8); // High byte of AND mask
            pdu[4] = (byte)(andMask & 0xFF); // Low byte of AND mask
            pdu[5] = (byte)(orMask >> 8); // High byte of OR mask
            pdu[6] = (byte)(orMask & 0xFF); // Low byte of OR mask

            return pdu;
        }
    }
}
