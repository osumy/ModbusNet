using ModbusNet.Messages.Requests;

using ModbusNet.Transport;

namespace ModbusNet.Device
{
    /// <summary>
    ///     Modbus master device
    /// </summary>
    public class ModbusMaster : IDisposable
    {
        private readonly IModbusTransport _transport;
        private bool _disposed = false;

        public ModbusMaster(IModbusTransport transport)
        {
            _transport = transport;
        }

        ///// <summary>
        /////    Reads from 1 to 2000 contiguous coils status.
        ///// </summary>
        ///// <param name="slaveAddress">Address of device to read values from.</param>
        ///// <param name="startAddress">Address to begin reading.</param>
        ///// <param name="numberOfPoints">Number of coils to read.</param>
        ///// <returns>Coils status.</returns>
        //public bool[] ReadCoils(byte slaveId, ushort startAddress, ushort numberOfPoints)
        //{
        //    ThrowIfDisposed();

        //    //ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);

        //    var request = new ReadCoilsRequest(slaveId, startAddress, numberOfPoints);
        //    //var responseData = SendRequestWithRetry(request.Serialize());
        //    //var response = ReadCoilsResponse.Deserialize(slaveId, responseData);
        //    //return response.Coils.Take(numberOfPoints).ToArray();
        //    return Array.Empty<bool>();
        //}

        //public bool ReadCoil(byte slaveId, ushort address)
        //{
        //    var coils = ReadCoils(slaveId, address, 1);
        //    return coils[0];
        //}

        //public void WriteCoil(byte slaveId, ushort address, bool value)
        //{
        //    var request = new WriteCoilRequest(slaveId, address, value);
        //    var responseData = SendRequestWithRetry(request.Serialize());
        //    var response = WriteCoilResponse.Deserialize(slaveId, responseData);

        //    if (response.Address != address || response.Value != value)
        //        throw new InvalidOperationException("Write coil verification failed");
        //}

        /// <summary>
        ///    Reads contiguous block of holding registers.
        /// </summary>
        /// <param name="slaveId">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Holding registers status.</returns>
        public ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            ThrowIfDisposed();

            if (numberOfPoints < 1 || numberOfPoints > 125)
                throw new ArgumentOutOfRangeException(nameof(numberOfPoints), "Must be 1-125 for Read Holding Registers");

            var request = new ReadHoldingRegistersRequest(0x03, slaveId, startAddress, numberOfPoints);

            //return PerformReadRegisters(request);
            request.Serialize();
            var responseData = SendRequestWithRetry(request.MessageFrame);
            //var response = ReadHoldingRegistersResponse.Deserialize(slaveId, responseData);
            return responseData;
        }

        public ushort ReadHoldingRegister(byte slaveId, ushort address)
        {
            ThrowIfDisposed();

            var registers = ReadHoldingRegisters(slaveId, address, 1);
            return registers[0];
        }

        /// <summary>
        ///    Writes a single holding register.
        /// </summary>
        /// <param name="slaveId">Address of the device to write to.</param>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        public void WriteSingleRegister(byte slaveId, ushort registerAddress, ushort value)
        {
            //ThrowIfDisposed();

            //var request = new WriteSingleRegisterRequest(slaveId, registerAddress, value);
            //var responseData = SendRequestWithRetry(request.Serialize());
            //var response = WriteSingleRegisterResponse.Deserialize(slaveId, responseData);

            //if (response.Address != registerAddress || response.Value != value)
            //    throw new InvalidOperationException("Write register verification failed");
        }

        //private ushort[] PerformReadRegisters(ReadHoldingRegistersRequest request)
        //{
        //    ReadHoldingRegistersResponse response =
        //            Transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);

        //    var response = ReadHoldingRegistersResponse.Deserialize(slaveId, responseData);
        //    return response.Registers;

        //    return response.Data.Take(request.NumberOfPoints).ToArray();
        //}

        private ushort[] SendRequestWithRetry(byte[] request)
        {
            ThrowIfDisposed();

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

        private void ThrowIfDisposed()
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
    }
}
