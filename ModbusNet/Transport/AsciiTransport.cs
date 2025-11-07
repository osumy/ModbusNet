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
        private bool _disposed = false;


        public AsciiTransport(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public void Connect()
        {
            ThrowIfDisposed();

            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        public void Disconnect()
        {
            ThrowIfDisposed();

            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        public byte[] SendRequest(byte[] request)
        {
            Connect();

            var asciiFrame = ConvertToAscii(request);

            _serialPort.Write(asciiFrame);

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
            //var receivedLrc = bytes.Last();
            //var calculatedLrc = CalculateLrc(data);

            //if (receivedLrc != calculatedLrc)
            //    throw new InvalidOperationException("LRC check failed");

            return data;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _serialPort?.Dispose();
                _disposed = true;
            }
        }
    }
}