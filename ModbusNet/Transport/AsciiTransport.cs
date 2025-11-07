using ModbusNet.Core;
using System.IO.Ports;
using System.Text;

namespace ModbusNet.Transport
{
    public class AsciiTransport : IModbusTransport
    {
        private readonly SerialPort _serialPort;
        private readonly ASCIIEncoding _encoding = new ASCIIEncoding();

        public bool IsConnected => _serialPort.IsOpen;

        public AsciiTransport(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public void Connect()
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        public void Disconnect()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        public byte[] SendRequest(byte[] request)
        {
            Connect();

            // تبدیل به ASCII
            var asciiFrame = ConvertToAscii(request);

            // ارسال فریم
            _serialPort.Write(asciiFrame);

            // دریافت پاسخ
            return ReceiveResponse();
        }

        private string ConvertToAscii(byte[] data)
        {
            //var lrc = CalculateLrc(data);
            //var dataWithLrc = new byte[data.Length + 1];
            //Array.Copy(data, 0, dataWithLrc, 0, data.Length);
            //dataWithLrc[data.Length] = lrc;

            //var hexString = BitConverter.ToString(dataWithLrc).Replace("-", "");
            //return _settings.AsciiStartDelimiter + hexString + _settings.AsciiEndDelimiter;
            return "";
        }

        private byte[] ReceiveResponse()
        {
            //// خواندن تا Start Delimiter
            //while (_serialPort.ReadByte() != _settings.AsciiStartDelimiter[0]) { }

            //// خواندن داده تا End Delimiter
            //var buffer = new System.Collections.Generic.List<byte>();
            //var endDelimiter = _encoding.GetBytes(_settings.AsciiEndDelimiter);
            //var endIndex = 0;

            //while (true)
            //{
            //    var b = (byte)_serialPort.ReadByte();

            //    if (b == endDelimiter[endIndex])
            //    {
            //        endIndex++;
            //        if (endIndex == endDelimiter.Length)
            //            break;
            //    }
            //    else
            //    {
            //        endIndex = 0;
            //        buffer.Add(b);
            //    }
            //}

            //var hexString = _encoding.GetString(buffer.ToArray());
            //return ConvertFromAscii(hexString);
            return Array.Empty<byte>();
        }

        private byte[] ConvertFromAscii(string asciiFrame)
        {
            var byteCount = asciiFrame.Length / 2;
            var bytes = new byte[byteCount];

            for (int i = 0; i < byteCount; i++)
            {
                bytes[i] = Convert.ToByte(asciiFrame.Substring(i * 2, 2), 16);
            }

            // بررسی LRC
            var data = bytes.Take(bytes.Length - 1).ToArray();
            var receivedLrc = bytes.Last();
            var calculatedLrc = CalculateLrc(data);

            if (receivedLrc != calculatedLrc)
                throw new InvalidOperationException("LRC check failed");

            return data;
        }

        private byte CalculateLrc(byte[] data)
        {
            byte lrc = 0;
            foreach (byte b in data)
            {
                lrc += b;
            }
            return (byte)(-(sbyte)lrc);
        }

        public void Dispose()
        {
            _serialPort?.Dispose();
        }
    }
}