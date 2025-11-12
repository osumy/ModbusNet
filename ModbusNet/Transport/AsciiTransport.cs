using ModbusNet.Messages;
using ModbusNet.Utils;
using System.IO.Ports;
using System.Text;

namespace ModbusNet.Transport
{
    public class AsciiTransport : ModbusTransportBase
    {
        private readonly SerialPort _serialPort;
        private readonly ModbusSettings _settings;

        public bool IsConnected => _serialPort.IsOpen;
        private bool _disposed = false;

        public byte[] StartDelimiterAsciiArray { get; private set; }
        public byte[] EndDelimiterAsciiArray { get; private set; }


        public AsciiTransport(SerialPort serialPort, ModbusSettings settings)
        {
            _serialPort = serialPort;
            _settings = settings;

            if (!_serialPort.IsOpen)
                _serialPort.Open();

            StartDelimiterAsciiArray = Encoding.ASCII.GetBytes(settings.AsciiStartDelimiter);
            EndDelimiterAsciiArray = Encoding.ASCII.GetBytes(settings.AsciiEndDelimiter);
        }


        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            ThrowIfDisposed();

            if(IsConnected == false)
            {
                throw new InvalidOperationException("Serial port is not connected.");
            }


            for (int attempt = 0; attempt <= _settings.retryCount; attempt++)
            {
                try
                { 
                    _serialPort.Write(request, 0, request.Length);
                    var responsePDU = ReceiveResponse();

                    var fcAscii = new byte[2];
                    Array.Copy(request, 3, fcAscii, 0, fcAscii.Length);

                    ValidatePDU(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);

                    return BuildResponse(responsePDU);
                }
                catch (TimeoutException) when (attempt < _settings.retryCount)
                {
                    Thread.Sleep(_settings.retryDelayMs);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.retryCount + 1} attempts");
        }

        public override void SendRequestIgnoreResponse(byte[] request)
        {
            ThrowIfDisposed();

            if (IsConnected == false)
            {
                throw new InvalidOperationException("Serial port is not connected.");
            }


            for (int attempt = 0; attempt <= _settings.retryCount; attempt++)
            {
                try
                {
                    _serialPort.Write(request, 0, request.Length);

                    var responsePDU = ReceiveResponse();

                    var fcAscii = new byte[2];
                    Array.Copy(request, 3, fcAscii, 0, fcAscii.Length);

                    ValidatePDU(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);
                }
                catch (TimeoutException) when (attempt < _settings.retryCount)
                {
                    Thread.Sleep(_settings.retryDelayMs);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.retryCount + 1} attempts");
        }

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

            if (decoded.Length < 1)
                throw new FormatException("Decoded Modbus frame too short.");

            // Last byte is LRC
            var lrc = decoded[decoded.Length - 1];

            // Extract message without LRC
            var message = new byte[decoded.Length - 1];
            Array.Copy(decoded, 0, message, 0, message.Length);

            ChecksumsMatch(message, [lrc]);

            var pdu = new byte[message.Length - 1];
            Array.Copy(message, 1, pdu, 0, pdu.Length);

            // Return the PDU (function code + data), without LRC and address
            return pdu;
        }

        public override byte[] BuildRequest(byte slaveAddress, byte[] pdu)
        {
            return AsciiUtility.BuildAsciiFrame(
                slaveAddress,
                pdu,
                StartDelimiterAsciiArray,
                EndDelimiterAsciiArray
                );
        }

        public override void ChecksumsMatch(byte[] rawMessage, byte[] ErrorCheckBytes)
        {
            if (!ErrorCheckUtility.ValidateLrc(rawMessage, ErrorCheckBytes[0]))
            {
                throw new ModbusExceptionLRC();
            }
        }

        protected void ThrowIfDisposed()
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