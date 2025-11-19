using ModbusNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusNet.Transport
{
    internal class tmp
    {
    }
    public class ModbusRTUFrame
    {
        public static bool IsValidFrame(byte[] data, int expectedLength)
        {
            if (data.Length < 4) return false; // Minimum: 1 byte address + 1 byte function + 2 bytes CRC

            ushort calculatedCRC = 0;// ErrorCheckUtility.ComputeCrc(data, 0, data.Length - 2);
            ushort receivedCRC = (ushort)((data[data.Length - 1] << 8) | data[data.Length - 2]);

            return calculatedCRC == receivedCRC;
        }

        public static int CalculateFrameLength(byte[] data, int startIndex)
        {
            // RTU frame length depends on function code
            if (data.Length - startIndex < 2) return -1;

            byte functionCode = data[startIndex + 1];

            switch (functionCode)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    return 8; // Standard request frame length
                case 15:
                case 16:
                    // Variable length for multiple writes
                    if (data.Length - startIndex >= 6)
                        return 9 + data[startIndex + 6];
                    break;
            }

            return -1;
        }
    }

    public class ModbusRTUParser
    {
        public static bool TryParseResponse(byte[] data, out byte slaveAddress, out byte functionCode, out byte[] payload)
        {
            slaveAddress = 0;
            functionCode = 0;
            payload = null;

            if (data.Length < 4) return false;
            //if (!ModbusRTUFrame.IsValidFrame(data)) return false;

            slaveAddress = data[0];
            functionCode = data[1];

            // Extract payload (excluding address, function code, and CRC)
            payload = new byte[data.Length - 4];
            Array.Copy(data, 2, payload, 0, data.Length - 4);

            return true;
        }

        public static bool[] ParseReadCoilsResponse(byte[] data, int expectedCoils)
        {
            if (data.Length < 3) return null;

            int byteCount = data[2];
            bool[] coils = new bool[expectedCoils];

            for (int i = 0; i < byteCount && i * 8 < expectedCoils; i++)
            {
                byte coilByte = data[3 + i];
                for (int bit = 0; bit < 8 && (i * 8 + bit) < expectedCoils; bit++)
                {
                    coils[i * 8 + bit] = ((coilByte >> bit) & 0x01) == 0x01;
                }
            }

            return coils;
        }
    }
}
