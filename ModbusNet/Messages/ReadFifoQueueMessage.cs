using ModbusNet.Enum;

namespace ModbusNet.Messages
{
    internal class ReadFifoQueueMessage
    {
        internal static byte[] BuildRequestPDU(ushort fifoAddress)
        {
            var pdu = new byte[3];

            pdu[0] = (byte)ModbusPublicFunctionCode.Read_FIFO_Queue;
            pdu[1] = (byte)(fifoAddress >> 8); // High byte of FIFO address
            pdu[2] = (byte)(fifoAddress & 0xFF); // Low byte of FIFO address

            return pdu;
        }
    }
}
