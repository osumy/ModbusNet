using ModbusNet.Messages;
using ModbusNet.Utils;
using System.IO.Ports;
using System.Text;

namespace ModbusNet.Transport
{
    public class AsciiTransport : IModbusTransport
    {
        private readonly SerialPort _serialPort;
        private readonly ModbusSettings _settings;

        public bool IsConnected => _serialPort.IsOpen;
        private bool _disposed = false;


        public AsciiTransport(SerialPort serialPort, ModbusSettings settings)
        {
            _serialPort = serialPort;
            _settings = settings;

            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }


        public ushort[] SendRequestWithRetry16A(byte[] request)
        {
            var retries = 3;
            var delayBetweenRetriesMs = 100;

            for (int attempt = 0; attempt <= retries; attempt++)
            {
                try
                {
                    return SendRequest(request);
                }
                catch (TimeoutException) when (attempt < retries)
                {
                    Thread.Sleep(delayBetweenRetriesMs);
                }
            }

            throw new TimeoutException($"Request failed after {retries + 1} attempts");
        }


        //public override byte[] BuildMessageFrame(IModbusMessage message)
        //{
        //    var msgFrame = message.MessageFrame;

        //    var msgFrameAscii = AsciiUtility.GetAsciiBytes(msgFrame);
        //    var lrcAscii = AsciiUtility.GetAsciiBytes(ErrorCheckCalculator.ComputeLrc(msgFrame));
        //    var nlAscii = Encoding.UTF8.GetBytes(NewLine.ToCharArray());

        //    var frame = new MemoryStream(1 + msgFrameAscii.Length + lrcAscii.Length + nlAscii.Length);
        //    frame.WriteByte((byte)':');
        //    frame.Write(msgFrameAscii, 0, msgFrameAscii.Length);
        //    frame.Write(lrcAscii, 0, lrcAscii.Length);
        //    frame.Write(nlAscii, 0, nlAscii.Length);

        //    return frame.ToArray();
        //}
        public byte[] BuildFrame(byte[] msgFrame)
        {
            var msgFrameAscii = AsciiUtility.ToAsciiBytes(msgFrame);
            var lrcAscii = AsciiUtility.ToAsciiBytes(ErrorCheckUtility.ComputeLrc(msgFrame));
            var edAscii = Encoding.UTF8.GetBytes(_settings.AsciiEndDelimiter.ToCharArray());
            var stAscii = Encoding.UTF8.GetBytes(_settings.AsciiStartDelimiter.ToCharArray());

            var frame = new MemoryStream(stAscii.Length + msgFrameAscii.Length + lrcAscii.Length + edAscii.Length);
            frame.Write(stAscii, 0, stAscii.Length);
            frame.Write(msgFrameAscii, 0, msgFrameAscii.Length);
            frame.Write(lrcAscii, 0, lrcAscii.Length);
            frame.Write(edAscii, 0, edAscii.Length);

            return frame.ToArray();
        }

        //public bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        //{
        //    return ErrorCheckUtility.ComputeLrc(message.MessageFrame) == messageFrame[messageFrame.Length - 1];
        //}

        //public byte[] ReadRequest()
        //{
        //    return ReadRequestResponse();
        //}
        //public void IgnoreResponse()
        //{
        //    ReadRequestResponse();
        //}

        //public override IModbusMessage ReadResponse<T>()
        //{
        //    return CreateResponse<T>(ReadRequestResponse());
        //}

        //internal byte[] ReadRequestResponse()
        //{
        //    //// read message frame, removing frame start ':'
        //    //string frameHex = StreamResourceUtility.ReadLine(StreamResource).Substring(1);

        //    //// convert hex to bytes
        //    //byte[] frame = ModbusUtility.HexToBytes(frameHex);
        //    //Logger.Trace($"RX: {string.Join(", ", frame)}");

        //    //if (frame.Length < 3)
        //    //{
        //    //    throw new IOException("Premature end of stream, message truncated.");
        //    //}

        //    //return frame;
        //    return null;
        //}




        public ushort[] SendRequest(byte[] request)
        {
            ThrowIfDisposed();

            if(IsConnected == false)
            {
                throw new InvalidOperationException("Serial port is not connected.");
            }

            _serialPort.Write(request, 0, request.Length);
            var response = ReceiveResponse();

            return ParseReadHoldingRegisters(response);
        }

        //private string ConvertToAscii(byte[] data)
        //{
        //    var lrc = CalculateLrc(data);
        //    var dataWithLrc = new byte[data.Length + 1];
        //    Array.Copy(data, 0, dataWithLrc, 0, data.Length);
        //    dataWithLrc[data.Length] = lrc;

        //    var hexString = BitConverter.ToString(dataWithLrc).Replace("-", "");
        //    return _settings.AsciiStartDelimiter + hexString + _settings.AsciiEndDelimiter;
        //}

        private byte[] ReceiveResponse()
        {
            // Wait for start char ':' (blocks until found or ReadTimeout triggers)
            int bInt;
            do
            {
                bInt = _serialPort.ReadByte();
                if (bInt < 0) throw new IOException("Serial port closed or no data available.");
            } while ((byte)bInt != (byte)':');

            // Read until CRLF ("\r\n")
            var payload = new List<byte>(); // will hold ASCII hex bytes (e.g. '0','1','A','F', ...)
            bool sawCr = false;
            while (true)
            {
                bInt = _serialPort.ReadByte();
                if (bInt < 0) throw new IOException("Serial port closed or no data available while reading payload.");

                byte b = (byte)bInt;

                if (!sawCr)
                {
                    if (b == (byte)'\r')
                    {
                        // possible start of CRLF - don't add to payload, set flag and continue to check next byte
                        sawCr = true;
                        continue;
                    }
                    else
                    {
                        payload.Add(b);
                    }
                }
                else
                {
                    // previously saw '\r', now expect '\n'
                    if (b == (byte)'\n')
                    {
                        break; // finished reading payload
                    }
                    else
                    {
                        // false alarm: previous '\r' was actually data (rare), so add the '\r' and this byte as normal
                        payload.Add((byte)'\r');
                        // if current byte is '\r' again, keep sawCr true, otherwise reset and add current byte
                        if (b == (byte)'\r')
                        {
                            sawCr = true; // consecutive CRs; stay in CR-check mode
                        }
                        else
                        {
                            payload.Add(b);
                            sawCr = false;
                        }
                    }
                }
            }

            if (payload.Count == 0)
                throw new FormatException("Empty Modbus ASCII payload received.");

            // payload now contains ASCII characters representing hex bytes, e.g. "010300100002FB" as bytes
            // Validate even length
            if ((payload.Count & 1) != 0)
                throw new FormatException("Invalid ASCII payload length (must be even number of hex chars).");

            // Decode ASCII hex -> raw bytes using your AsciiUtility.DecodeHex
            var decoded = AsciiUtility.FromAsciiBytes(payload.ToArray());
            //if (decoded < 0)
            //    throw new FormatException("Invalid hex characters in ASCII payload.");

            //if (decoded != raw.Length)
            //    throw new InvalidOperationException("Decoded length mismatch.");

            if (decoded.Length < 1)
                throw new FormatException("Decoded Modbus frame too short.");

            // Last byte is LRC
            var lrc = decoded[decoded.Length - 1];

            // Extract message without LRC
            var message = new byte[decoded.Length - 1];
            Array.Copy(decoded, 0, message, 0, message.Length);

            // Validate LRC
            var calc = ErrorCheckUtility.ComputeLrc(message);
            if (calc != lrc)
                throw new InvalidDataException($"LRC mismatch. Calculated 0x{calc:X2}, Received 0x{lrc:X2}.");

            // Return the message (address + function + data), without LRC
            return message;
        }

        public ushort[] ParseReadHoldingRegisters(byte[] response)
        {
            if (response == null || response.Length < 3)
                throw new ArgumentException("Invalid response.");

            byte slave = response[0];
            byte function = response[1];
            byte byteCount = response[2];

            if (function != 0x03)
                throw new InvalidOperationException($"Unexpected function code: {function}");

            if (byteCount != response.Length - 3)
                throw new FormatException("Byte count mismatch.");

            int registerCount = byteCount / 2;
            ushort[] registers = new ushort[registerCount];

            int pos = 3; // start of data
            for (int i = 0; i < registerCount; i++)
            {
                registers[i] = (ushort)((response[pos] << 8) | response[pos + 1]);
                pos += 2;
            }

            return registers;
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

        //public override void IgnoreResponse()
        //{
        //    throw new NotImplementedException();
        //}

        //public override bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        //{
        //    throw new NotImplementedException();
        //}

        //public override byte[] ReadRequest()
        //{
        //    throw new NotImplementedException();
        //}
    }
}