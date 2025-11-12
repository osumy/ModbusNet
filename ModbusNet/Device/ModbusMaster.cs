using ModbusNet.Device.Validation;
using ModbusNet.Messages;
using ModbusNet.Transport;
using ModbusNet.Utils;

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


        #region Bit Access - Discrete Inputs

        public bool[] ReadDiscreteInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            //Validate(ValidationType.ReadDiscreteInputs, numberOfPoints);

            var pdu = ReadDiscreteInputsMessage.BuildRequestPDU(startAddress, numberOfPoints);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            return null;
            //return SendRequestWithRetry(request);
        }

        #endregion

        #region Bit Access - Coils

        public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            //Validate(ValidationType.ReadCoils, numberOfPoints);

            var pdu = ReadCoilsMessage.BuildRequestPDU(startAddress, numberOfPoints);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            return null;
            //return SendRequestWithRetry(request);
        }

        public void WriteSingleCoil(byte slaveAddress, ushort address, ushort value)
        {
            //Validate(ValidationType.WriteSingleCoil, value);

            var pdu = WriteSingleCoilMessage.BuildRequestPDU(address, value);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
        }

        public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data)
        {
            //Validate(ValidationType.WriteMultipleCoils, numberOfPoints);

            var pdu = WriteMultipleCoilsMessage.BuildRequestPDU(startAddress, data);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
        }

        #endregion

        #region Word Access - Input Registers

        public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            //Validate(ValidationType.ReadInputRegisters, numberOfPoints);

            var pdu = ReadInputRegistersMessage.BuildRequestPDU(startAddress, numberOfPoints);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            return _transport
                .SendRequestReceiveResponse(request)
                .Registers;
        }

        #endregion

        #region Word Access - Holding Registers

        public ushort[] ReadMultipleHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
        {
            //Validate(ValidationType.ReadMultipleHoldingRegisters, numberOfPoints);

            var pdu = ReadMultipleHoldingRegistersMessage.BuildRequestPDU(startAddress, numberOfPoints);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            return _transport
                .SendRequestReceiveResponse(request)
                .Registers;
        }

        public void WriteSingleHoldingRegister(byte slaveAddress, ushort address, ushort value)
        {
            //Validate(ValidationType., numberOfPoints);

            var pdu = WriteSingleHoldingRegisterMessage.BuildRequestPDU(address, value);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //SendRequestWithRetry(request);
        }

        public void WriteMultipleHoldingRegisters(byte slaveAddress, ushort startAddress, ushort[] data)
        {
            //Validate(ValidationType., numberOfPoints);

            var pdu = WriteMultipleHoldingRegistersMessage.BuildRequestPDU(startAddress, data);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //SendRequestWithRetry(request);
        }

        public ushort[] ReadWriteMultipleRegisters(byte slaveAddress, ushort readStartAddress, ushort numberOfPointsToRead,
                                                   ushort writeStartAddress, ushort[] writeData)
        {
            //Validate(ValidationType.ReadWriteMultipleRegisters, numberOfPoints);

            var pdu = ReadWriteMultipleRegistersMessage.BuildRequestPDU(readStartAddress, numberOfPointsToRead, writeStartAddress, writeData);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
            return null;
        }

        public void MaskWriteRegister(byte slaveAddress, ushort address, ushort andMask, ushort orMask)
        {
            //Validate(ValidationType., numberOfPoints);

            var pdu = MaskWriteRegisterMessage.BuildRequestPDU(address, andMask, orMask);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //SendRequestWithRetry(request);
        }

        public ushort[] ReadFifoQueue(byte slaveAddress, ushort fifoAddress)
        {
            //Validate(ValidationType., numberOfPoints);

            var pdu = ReadFifoQueueMessage.BuildRequestPDU(fifoAddress);

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
            return null;

        }

        #endregion

        #region File Record Access

        public byte[] ReadFileRecord(byte slaveAddress, ushort fileNumber, ushort startingAddress, ushort numberOfRegisters)
        {
            //Validate(ValidationType., numberOfPoints);

            //var pdu = ReadFileRecordMessage.BuildRequestPDU(startAddress, numberOfPoints);

            //var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
            return null;
        }

        public void WriteFileRecord(byte slaveAddress, ushort fileNumber, ushort startingAddress, byte[] data)
        {
            //Validate(ValidationType.WriteFileRecord, numberOfPoints);

            //var pdu = WriteFileRecordMessage.BuildRequestPDU(startAddress, numberOfPoints);

            //var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
        }

        #endregion

        #region Diagnostics (Serial Only)

        public byte ReadExceptionStatus(byte slaveAddress)
        {
            //Validate(ValidationType., numberOfPoints);

            var pdu = ReadExceptionStatusMessage.BuildRequestPDU();

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
            return 0;
        }

        public ushort[] Diagnostics(byte slaveAddress, ushort subFunctionCode, ushort[] data)
        {
            //Validate(ValidationType., numberOfPoints);

            //var pdu = DiagnosticsMessage.BuildRequestPDU(startAddress, numberOfPoints);

            //var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
            return null;
        }

        public (ushort Status, ushort EventCount) GetCommunicationEventCounter(byte slaveAddress)
        {
            //Validate(ValidationType., numberOfPoints);

            var pdu = GetCommunicationEventCounterMessage.BuildRequestPDU();

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
            return (0, 0);
        }

        public (ushort Status, ushort EventCount, ushort MessageCount, byte[] Events) GetCommunicationEventLog(byte slaveAddress)
        {
            //Validate(ValidationType., numberOfPoints);

            var pdu = GetCommunicationEventLogMessage.BuildRequestPDU();

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
            return (0, 0, 0, null);
        }

        public byte[] ReportServerId(byte slaveAddress)
        {
            //Validate(ValidationType., numberOfPoints);

            var pdu = ReportServerIdMessage.BuildRequestPDU();

            var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
            return null;
        }

        #endregion

        #region Other Function Codes

        public byte[] ReadDeviceIdentification(byte slaveAddress, byte objectId, byte meiType)
        {
            //Validate(ValidationType., numberOfPoints);

            //var pdu = ReadDeviceIdentificationMessage.BuildRequestPDU(startAddress, numberOfPoints);

            //var request = _transport.BuildRequest(slaveAddress, pdu);

            //return SendRequestWithRetry(request);
            return null;
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
        protected void Validate(ValidationType validationType, ushort data16 = 0, Array data = null)
        {
            ThrowIfDisposed();

            switch (validationType)
            {
                case ValidationType.ReadCoils:
                case ValidationType.ReadDiscreteInputs:
                    ValidateNumberOfPoints(data16, ValidationLimits.MaxReadCoils);
                    break;

                case ValidationType.ReadMultipleHoldingRegisters:
                case ValidationType.ReadInputRegisters:
                    ValidateNumberOfPoints(data16, ValidationLimits.MaxReadMultipleHoldingRegisters);
                    break;

                case ValidationType.WriteMultipleCoils:
                    ValidateData(data, ValidationLimits.MaxWriteMultipleCoils);
                    break;

                case ValidationType.WriteMultipleRegisters:
                    ValidateData(data, ValidationLimits.MaxWriteMultipleRegisters);
                    break;

                case ValidationType.ReadWriteMultipleRegisters:
                    ValidateNumberOfPoints(data16, ValidationLimits.MaxReadWriteMultipleRegistersRead);
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