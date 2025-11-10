using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class ReadMultipleHoldingRegistersMessage
    {
        public static byte[] BuildRequestPDU(ushort StartAddress, ushort NumberOfPoints)
        {
            var pdu = new byte[5];

            pdu[0] = (byte)ModbusPublicFunctionCode.Read_Multiple_Holding_Registers;
            pdu[1] = (byte)(StartAddress >> 8); // High byte of start address
            pdu[2] = (byte)(StartAddress & 0xFF); // Low byte of start address
            pdu[3] = (byte)(NumberOfPoints >> 8); // High byte of count
            pdu[4] = (byte)(NumberOfPoints & 0xFF); // Low byte of count

            return pdu;
        }
    }
}
