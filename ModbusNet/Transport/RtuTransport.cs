using ModbusNet.Messages;
using ModbusNet.Utils;
using System.Diagnostics;
using System.IO.Ports;

namespace ModbusNet.Transport
{
    public class RtuTransport : ModbusTransportBase
    {
        private readonly SerialPort _serialPort;
        private readonly ModbusSettings _settings;

        public bool IsConnected => _serialPort.IsOpen;
        private bool _disposed = false;


        public RtuTransport(SerialPort serialPort, ModbusSettings settings)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }


        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            ThrowIfDisposed();

            if (IsConnected == false)
            {
                throw new InvalidOperationException("Serial port is not connected.");
            }

            for (int attempt = 0; attempt < _settings.RetryCount; attempt++)
            {
                try
                {
                    _serialPort.Write(request, 0, request.Length);
                    var responsePDU = ReceiveResponse();

                    byte fc = request[1];

                    ValidateFC(responsePDU, fc);
                    ValidateByteCount(responsePDU, fc);

                    return BuildResponse(responsePDU);
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    Thread.Sleep(_settings.RetryDelayMs);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.RetryCount + 1} attempts");
        }

        public override void SendRequestIgnoreResponse(byte[] request)
        {
            ThrowIfDisposed();

            if (IsConnected == false)
            {
                throw new InvalidOperationException("Serial port is not connected.");
            }


            for (int attempt = 0; attempt < _settings.RetryCount; attempt++)
            {
                try
                {
                    _serialPort.Write(request, 0, request.Length);

                    var responsePDU = ReceiveResponse();

                    byte fc = request[1];
                    ValidateFC(responsePDU, fc);

                    return;
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    Thread.Sleep(_settings.RetryDelayMs);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.RetryCount + 1} attempts");
        }

        private byte[] ReceiveResponse()
        {
            var frame = new List<byte>();

            // Read first byte to start the frame
            int firstByte = _serialPort.ReadByte();
            if (firstByte < 0) throw new IOException("Serial port closed or no data available.");
            frame.Add((byte)firstByte);

            // Calculate inter-character and inter-frame timeout values
            int interCharTimeout = Math.Max(1, _settings.RtuInterCharTimeMs);
            int interFrameTimeout = Math.Max(1, _settings.RtuInterFrameTimeMs);

            // Use a shorter timeout for inter-character gaps
            int originalTimeout = _serialPort.ReadTimeout;
            _serialPort.ReadTimeout = interCharTimeout;

            try
            {
                while (true)
                {
                    try
                    {
                        int nextByte = _serialPort.ReadByte();
                        if (nextByte >= 0)
                        {
                            frame.Add((byte)nextByte);

                            // Reset timeout for next character
                            _serialPort.ReadTimeout = interCharTimeout;

                            // Check if we have a complete frame (minimum RTU frame is 4 bytes: addr + func + crc1 + crc2)
                            if (frame.Count >= 4)
                            {
                                // For some function codes we can determine exact frame length
                                int expectedLength = GetExpectedFrameLength(frame);
                                if (frame.Count >= expectedLength)
                                {
                                    // Wait a bit more to see if more data arrives (handle inter-frame gap)
                                    Thread.Sleep(interFrameTimeout);
                                    if (_serialPort.BytesToRead == 0)
                                    {
                                        break; // Frame is complete
                                    }
                                }
                            }
                        }
                    }
                    catch (TimeoutException)
                    {
                        // No more data within inter-character timeout - frame is complete
                        break;
                    }
                }
            }
            finally
            {
                // Restore original timeout
                _serialPort.ReadTimeout = originalTimeout;
            }

            if (frame.Count < 4)
                throw new FormatException($"RTU frame too short. Expected at least 4 bytes, got {frame.Count}.");

            // Extract CRC (last 2 bytes)
            var crcBytes = new byte[2];
            crcBytes[0] = frame[frame.Count - 2];
            crcBytes[1] = frame[frame.Count - 1];

            // Extract message without CRC
            var message = new byte[frame.Count - 2];
            for (int i = 0; i < message.Length; i++)
            {
                message[i] = frame[i];
            }

            // Validate CRC
            ChecksumsMatch(message, crcBytes);

            // Extract PDU (without slave address and CRC)
            var pdu = new byte[message.Length - 1];
            Array.Copy(message, 1, pdu, 0, pdu.Length);

            return pdu;
        }

        private int GetExpectedFrameLength(List<byte> frame)
        {
            if (frame.Count < 2) return 4; // Minimum frame length

            byte functionCode = frame[1];

            // Determine expected length based on function code
            switch (functionCode)
            {
                case 0x01: // Read Coils
                case 0x02: // Read Discrete Inputs
                case 0x03: // Read Holding Registers  
                case 0x04: // Read Input Registers
                    if (frame.Count >= 3)
                    {
                        // Response format: [addr][func][byte count][data...][crc1][crc2]
                        // Byte count tells us how many data bytes follow
                        byte byteCount = frame[2];
                        return 3 + byteCount + 2; // addr+func+byteCount + dataBytes + CRC
                    }
                    break;

                case 0x05: // Write Single Coil
                case 0x06: // Write Single Register
                    return 6; // Fixed length: [addr][func][2byte addr][2byte value][crc1][crc2]

                case 0x0F: // Write Multiple Coils
                case 0x10: // Write Multiple Registers
                    return 6; // Fixed length response: [addr][func][2byte addr][2byte quantity][crc1][crc2]

                case 0x16: // Mask Write Register
                    return 8; // Fixed length: [addr][func][2byte addr][2byte and mask][2byte or mask][crc1][crc2]

                case 0x17: // Read/Write Multiple Registers
                    if (frame.Count >= 3)
                    {
                        byte byteCount = frame[2];
                        return 3 + byteCount + 2; // addr+func+byteCount + dataBytes + CRC
                    }
                    break;
            }

            // Default: minimum frame length
            return 4;
        }

        public override byte[] BuildRequest(byte slaveAddress, byte[] pdu)
        {
            // Create the message frame: slave address + PDU + CRC
            var frame = new byte[1 + pdu.Length + 2];
            frame[0] = slaveAddress;
            pdu.AsSpan().CopyTo(frame.AsSpan(1, pdu.Length));

            var messagePart = frame.AsSpan(0, 1 + pdu.Length);
            var crcBytes = ErrorCheckUtility.ComputeCrc(messagePart);

            // Append CRC to the frame
            frame[^2] = crcBytes[0];
            frame[^1] = crcBytes[1];

            return frame;
        }

        private void ChecksumsMatch(byte[] rawMessage, byte[] ErrorCheckBytes)
        {
            if (!ErrorCheckUtility.ValidateCrc(rawMessage, ErrorCheckBytes))
            {
                throw new ModbusExceptionCRC();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _serialPort?.Dispose();
                _disposed = true;
            }
        }

        public override Task<ModbusResponse> SendRequestReceiveResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task SendRequestIgnoreResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}