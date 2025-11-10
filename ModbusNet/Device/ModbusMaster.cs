using ModbusNet.Device.Validation;
using ModbusNet.Messages;
using ModbusNet.Messages.Requests;
using ModbusNet.Transport;

namespace ModbusNet.Device
{
    public class ModbusMaster : IModbusMaster
    {
        private readonly IModbusTransport _transport;
        private readonly ModbusSettings _settings;

        private bool _disposed = false;

        public ModbusMaster(IModbusTransport transport, ModbusSettings settings)
        {
            _transport = transport;
            _settings = settings;
        }


        private ushort[] SendRequestWithRetry(byte[] request)
        {
            var retries = 3;
            var delayBetweenRetriesMs = 100;

            for (int attempt = 0; attempt <= retries; attempt++)
            {
                try
                {
                    return _transport.SendRequest(request);
                }
                catch (TimeoutException) when (attempt < retries)
                {
                    Thread.Sleep(delayBetweenRetriesMs);
                }
            }

            throw new TimeoutException($"Request failed after {retries + 1} attempts");
        }


        #region Bit Access - Discrete Inputs

        public bool[] ReadDiscreteInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Bit Access - Coils

        public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            throw new NotImplementedException();
        }

        public void WriteSingleCoil(byte slaveAddress, ushort coilAddress, bool value)
        {
            throw new NotImplementedException();
        }

        public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Word Access - Input Registers

        public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Word Access - Holding Registers

        public ushort[] ReadMultipleHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            Validate(ValidationType.ReadMultipleHoldingRegisters, numberOfPoints);

            var pdu = ReadMultipleHoldingRegistersMessage.BuildRequestPDU(startAddress, numberOfPoints);

            //var request = new ReadHoldingRegistersRequest(0x03, slaveAddress, startAddress, numberOfPoints);
            //request.Serialize();
            var request = AsciiMessageSerializer.BuildAsciiFrame(
                slaveAddress,
                pdu,
                System.Text.Encoding.ASCII.GetBytes(_settings.AsciiStartDelimiter),
                System.Text.Encoding.ASCII.GetBytes(_settings.AsciiEndDelimiter)
                );

            return SendRequestWithRetry(request);
        }

        public void WriteSingleHoldingRegister(byte slaveAddress, ushort registerAddress, ushort value)
        {
            throw new NotImplementedException();
        }

        public void WriteMultipleHoldingRegisters(byte slaveAddress, ushort startAddress, ushort[] data)
        {
            throw new NotImplementedException();
        }

        public ushort[] ReadWriteMultipleRegisters(byte slaveAddress, ushort startReadAddress, ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
        {
            throw new NotImplementedException();
        }

        public void MaskWriteRegister(byte slaveAddress, ushort registerAddress, ushort andMask, ushort orMask)
        {
            throw new NotImplementedException();
        }

        public ushort[] ReadFifoQueue(byte slaveAddress, ushort fifoPointerAddress)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region File Record Access

        public byte[] ReadFileRecord(byte slaveAddress, ushort fileNumber, ushort startingAddress, ushort numberOfRegisters)
        {
            throw new NotImplementedException();
        }

        public void WriteFileRecord(byte slaveAddress, ushort fileNumber, ushort startingAddress, byte[] data)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Diagnostics (Serial Only)

        public byte ReadExceptionStatus(byte slaveAddress)
        {
            throw new NotImplementedException();
        }

        public ushort[] Diagnostics(byte slaveAddress, ushort subFunctionCode, ushort[] data)
        {
            throw new NotImplementedException();
        }

        public (ushort Status, ushort EventCount) GetCommunicationEventCounter(byte slaveAddress)
        {
            throw new NotImplementedException();
        }

        public (ushort Status, ushort EventCount, ushort MessageCount, byte[] Events) GetCommunicationEventLog(byte slaveAddress)
        {
            throw new NotImplementedException();
        }

        public byte[] ReportServerId(byte slaveAddress)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Other Function Codes

        public byte[] ReadDeviceIdentification(byte slaveAddress, byte objectId, byte meiType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Validation and Disposal

        /// <summary>
        /// Validates the operation parameters based on the validation type and checks if the instance is disposed.
        /// </summary>
        /// <param name="validationType">The type of Modbus operation to validate.</param>
        /// <param name="numberOfPoints">The number of points to read/write.</param>
        /// <param name="data">The data array to validate (optional).</param>
        /// <param name="argumentName">The name of the argument being validated.</param>
        protected void Validate(ValidationType validationType, ushort numberOfPoints = 0, Array data = null)
        {
            ThrowIfDisposed();

            switch (validationType)
            {
                case ValidationType.ReadCoils:
                case ValidationType.ReadDiscreteInputs:
                    ValidateNumberOfPoints(numberOfPoints, ValidationLimits.MaxReadCoils);
                    break;

                case ValidationType.ReadMultipleHoldingRegisters:
                case ValidationType.ReadInputRegisters:
                    ValidateNumberOfPoints(numberOfPoints, ValidationLimits.MaxReadMultipleHoldingRegisters);
                    break;

                case ValidationType.WriteMultipleCoils:
                    ValidateData(data, ValidationLimits.MaxWriteMultipleCoils);
                    break;

                case ValidationType.WriteMultipleRegisters:
                    ValidateData(data, ValidationLimits.MaxWriteMultipleRegisters);
                    break;

                case ValidationType.ReadWriteMultipleRegisters:
                    ValidateNumberOfPoints(numberOfPoints, ValidationLimits.MaxReadWriteMultipleRegistersRead);
                    if (data != null)
                    {
                        ValidateData(data, ValidationLimits.MaxReadWriteMultipleRegistersWrite);
                    }
                    break;

                case ValidationType.WriteFileRecord:
                    ValidateMaxData(data, ValidationLimits.MaxWriteFileRecord);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(validationType), $"Unknown validation type: {validationType}");
            }
        }

        private static void ValidateData(Array data, int maxDataLength)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0 || data.Length > maxDataLength)
            {
                string msg = $"The length of argument data must be between 1 and {maxDataLength} inclusive.";
                throw new ArgumentException(msg);
            }
        }

        private static void ValidateMaxData(Array data, int maxDataLength)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length > maxDataLength)
            {
                string msg = $"The length of argument data must not be greater than {maxDataLength}.";
                throw new ArgumentException(msg);
            }
        }

        private static void ValidateNumberOfPoints(ushort numberOfPoints, ushort maxNumberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > maxNumberOfPoints)
            {
                string msg = $"Argument numberOfPoints must be between 1 and {maxNumberOfPoints} inclusive.";
                throw new ArgumentException(msg);
            }
        }

        /// <summary>
        /// Validates that the instance has not been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _transport?.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}