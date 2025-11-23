using ModbusNet.Messages;
using ModbusNet.Utils;
using System.IO.Ports;

namespace ModbusNet.Transport
{
    /// <summary>
    /// Implements the Modbus RTU (Remote Terminal Unit) transport layer over a serial port.
    /// Uses binary encoding with CRC-16 error checking and enforces inter-character and inter-frame timing
    /// as defined in the Modbus over Serial Line specification.
    /// </summary>
    public class RtuTransport : ModbusTransportBase
    {
        #region Fields and Properties

        private readonly SerialPort _serialPort;
        private readonly ModbusSettings _settings;
        private bool _disposed = false;

        /// <summary>
        /// Gets a value indicating whether the underlying serial port is open and connected.
        /// </summary>
        public bool IsConnected => _serialPort.IsOpen;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtuTransport"/> class.
        /// </summary>
        /// <param name="serialPort">The serial port used for communication. Must not be null.</param>
        /// <param name="settings">The Modbus configuration settings. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serialPort"/> or <paramref name="settings"/> is null.</exception>
        public RtuTransport(SerialPort serialPort, ModbusSettings settings)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        #endregion

        #region Synchronous Public Methods

        /// <summary>
        /// Sends a Modbus RTU request and waits for the response.
        /// Retries on timeout up to <see cref="ModbusSettings.RetryCount"/> times.
        /// </summary>
        /// <param name="request">The raw RTU-encoded request frame to send.</param>
        /// <returns>A <see cref="ModbusResponse"/> containing the decoded PDU.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the serial port is not connected.</exception>
        /// <exception cref="TimeoutException">Thrown if all retry attempts fail.</exception>
        /// <exception cref="ModbusExceptionCRC">Thrown if the CRC checksum is invalid.</exception>
        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            ThrowIfDisposed();

            if (!IsConnected)
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

        /// <summary>
        /// Sends a Modbus RTU request without waiting for a response (fire-and-forget with echo validation).
        /// Still validates the echoed function code for error checking.
        /// Retries on timeout up to <see cref="ModbusSettings.RetryCount"/> times.
        /// </summary>
        /// <param name="request">The raw RTU-encoded request frame to send.</param>
        /// <exception cref="InvalidOperationException">Thrown if the serial port is not connected.</exception>
        /// <exception cref="TimeoutException">Thrown if all retry attempts fail.</exception>
        /// <exception cref="ModbusExceptionCRC">Thrown if the CRC checksum is invalid.</exception>
        public override void SendRequestIgnoreResponse(byte[] request)
        {
            ThrowIfDisposed();

            if (!IsConnected)
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

        #endregion

        #region Frame Reception and Parsing

        /// <summary>
        /// Receives a complete Modbus RTU response frame from the serial port,
        /// respecting inter-character and inter-frame timing, validates CRC,
        /// and returns the PDU (function code + data).
        /// </summary>
        /// <returns>The decoded PDU as a byte array.</returns>
        /// <exception cref="IOException">Thrown if the serial port is closed or no data is available.</exception>
        /// <exception cref="FormatException">Thrown if the frame is too short or malformed.</exception>
        /// <exception cref="ModbusExceptionCRC">Thrown if the CRC checksum does not match.</exception>
        private byte[] ReceiveResponse()
        {
            var frame = new List<byte>();

            int firstByte = _serialPort.ReadByte();
            if (firstByte < 0) throw new IOException("Serial port closed or no data available.");
            frame.Add((byte)firstByte);

            int interCharTimeout = Math.Max(1, _settings.RtuInterCharTimeMs);
            int interFrameTimeout = Math.Max(1, _settings.RtuInterFrameTimeMs);
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
                            _serialPort.ReadTimeout = interCharTimeout;

                            if (frame.Count >= 4)
                            {
                                int expectedLength = GetExpectedFrameLength(frame);
                                if (frame.Count >= expectedLength)
                                {
                                    Thread.Sleep(interFrameTimeout);
                                    if (_serialPort.BytesToRead == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (TimeoutException)
                    {
                        break;
                    }
                }
            }
            finally
            {
                _serialPort.ReadTimeout = originalTimeout;
            }

            if (frame.Count < 4)
                throw new FormatException($"RTU frame too short. Expected at least 4 bytes, got {frame.Count}.");

            var crcBytes = new byte[2]
            {
                frame[frame.Count - 2],
                frame[frame.Count - 1]
            };

            var message = new byte[frame.Count - 2];
            Array.Copy(frame.ToArray(), message, message.Length);

            ChecksumsMatch(message, crcBytes);

            var pdu = new byte[message.Length - 1];
            Array.Copy(message, 1, pdu, 0, pdu.Length);

            return pdu;
        }

        /// <summary>
        /// Determines the expected total frame length (including address and CRC) based on the function code
        /// and available bytes in the response.
        /// </summary>
        /// <param name="frame">The partially received RTU frame.</param>
        /// <returns>The expected total frame length in bytes.</returns>
        private int GetExpectedFrameLength(List<byte> frame)
        {
            if (frame.Count < 2) return 4;

            byte functionCode = frame[1];

            switch (functionCode)
            {
                case 0x01: // Read Coils
                case 0x02: // Read Discrete Inputs
                case 0x03: // Read Holding Registers  
                case 0x04: // Read Input Registers
                    if (frame.Count >= 3)
                    {
                        byte byteCount = frame[2];
                        return 3 + byteCount + 2; // addr + func + byteCount + data + CRC
                    }
                    break;

                case 0x05: // Write Single Coil
                case 0x06: // Write Single Register
                case 0x0F: // Write Multiple Coils (response)
                case 0x10: // Write Multiple Registers (response)
                    return 6;

                case 0x16: // Mask Write Register
                    return 8;

                case 0x17: // Read/Write Multiple Registers
                    if (frame.Count >= 3)
                    {
                        byte byteCount = frame[2];
                        return 3 + byteCount + 2;
                    }
                    break;
            }

            return 4;
        }

        #endregion

        #region Request Building and Validation

        /// <summary>
        /// Builds a complete Modbus RTU request frame from a slave address and PDU.
        /// Appends a CRC-16 checksum to ensure data integrity.
        /// </summary>
        /// <param name="slaveAddress">The Modbus slave address (1–247).</param>
        /// <param name="pdu">The Protocol Data Unit (function code + data).</param>
        /// <returns>A byte array representing the full RTU frame (address + PDU + CRC).</returns>
        public override byte[] BuildRequest(byte slaveAddress, byte[] pdu)
        {
            var frame = new byte[1 + pdu.Length + 2];
            frame[0] = slaveAddress;
            pdu.AsSpan().CopyTo(frame.AsSpan(1, pdu.Length));

            var messagePart = frame.AsSpan(0, 1 + pdu.Length);
            var crcBytes = ErrorCheckUtility.ComputeCrc(messagePart);

            frame[^2] = crcBytes[0];
            frame[^1] = crcBytes[1];

            return frame;
        }

        /// <summary>
        /// Validates that the CRC-16 checksum of the message matches the provided checksum bytes.
        /// </summary>
        /// <param name="rawMessage">The message bytes (excluding CRC).</param>
        /// <param name="errorCheckBytes">The two-byte CRC checksum.</param>
        /// <exception cref="ModbusExceptionCRC">Thrown if the CRC does not match.</exception>
        private void ChecksumsMatch(byte[] rawMessage, byte[] errorCheckBytes)
        {
            if (!ErrorCheckUtility.ValidateCrc(rawMessage, errorCheckBytes))
            {
                throw new ModbusExceptionCRC();
            }
        }

        #endregion

        #region Asynchronous Methods (Not Implemented)

        /// <inheritdoc />
        /// <remarks>
        /// Asynchronous RTU transport is not currently supported due to timing constraints
        /// inherent in the RTU protocol (inter-character delays, etc.).
        /// </remarks>
        public override Task<ModbusResponse> SendRequestReceiveResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Async RTU transport is not supported.");
        }

        /// <inheritdoc />
        /// <remarks>
        /// Asynchronous RTU transport is not currently supported due to timing constraints
        /// inherent in the RTU protocol (inter-character delays, etc.).
        /// </remarks>
        public override Task SendRequestIgnoreResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Async RTU transport is not supported.");
        }

        #endregion

        #region Disposal and Utility

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if this instance has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object is disposed.</exception>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Releases unmanaged resources and disposes the underlying serial port.
        /// </summary>
        public override void Dispose()
        {
            if (!_disposed)
            {
                _serialPort?.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}