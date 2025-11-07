using ModbusNet.Core;
using ModbusNet.Core.Messages;
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

            Connect();
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

        public override byte[] BuildMessageFrame(IModbusMessage message)
        {
            var msgFrame = message.MessageFrame;

            var msgFrameAscii = ModbusUtility.GetAsciiBytes(msgFrame);
            var lrcAscii = ModbusUtility.GetAsciiBytes(ModbusUtility.CalculateLrc(msgFrame));
            var nlAscii = Encoding.UTF8.GetBytes(Modbus.NewLine.ToCharArray());

            var frame = new MemoryStream(1 + msgFrameAscii.Length + lrcAscii.Length + nlAscii.Length);
            frame.WriteByte((byte)':');
            frame.Write(msgFrameAscii, 0, msgFrameAscii.Length);
            frame.Write(lrcAscii, 0, lrcAscii.Length);
            frame.Write(nlAscii, 0, nlAscii.Length);

            return frame.ToArray();
        }

        public bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        {
            return ModbusUtility.CalculateLrc(message.MessageFrame) == messageFrame[messageFrame.Length - 1];
        }

        public byte[] ReadRequest()
        {
            return ReadRequestResponse();
        }
        public void IgnoreResponse()
        {
            ReadRequestResponse();
        }

        public override IModbusMessage ReadResponse<T>()
        {
            return CreateResponse<T>(ReadRequestResponse());
        }

        internal byte[] ReadRequestResponse()
        {
            // read message frame, removing frame start ':'
            string frameHex = StreamResourceUtility.ReadLine(StreamResource).Substring(1);

            // convert hex to bytes
            byte[] frame = ModbusUtility.HexToBytes(frameHex);
            Logger.Trace($"RX: {string.Join(", ", frame)}");

            if (frame.Length < 3)
            {
                throw new IOException("Premature end of stream, message truncated.");
            }

            return frame;
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
            var lrc = CalculateLrc(data);
            var dataWithLrc = new byte[data.Length + 1];
            Array.Copy(data, 0, dataWithLrc, 0, data.Length);
            dataWithLrc[data.Length] = lrc;

            var hexString = BitConverter.ToString(dataWithLrc).Replace("-", "");
            return _settings.AsciiStartDelimiter + hexString + _settings.AsciiEndDelimiter;
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