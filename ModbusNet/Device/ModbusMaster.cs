using ModbusNet.Core;
using ModbusNet.Messages.Requests;
using ModbusNet.Messages.Responses;
using ModbusNet.Transport;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

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

        /// <summary>
        ///    Reads from 1 to 2000 contiguous coils status.
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <returns>Coils status</returns>
        public bool[] ReadCoils(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            //ValidateNumberOfPoints("numberOfPoints", numberOfPoints, 2000);

            var request = new ReadCoilsRequest(slaveId, startAddress, numberOfPoints);
            //var responseData = SendRequestWithRetry(request.Serialize());
            //var response = ReadCoilsResponse.Deserialize(slaveId, responseData);
            //return response.Coils.Take(numberOfPoints).ToArray();
            return Array.Empty<bool>();
        }

        //public bool ReadCoil(byte slaveId, ushort address)
        //{
        //    var coils = ReadCoils(slaveId, address, 1);
        //    return coils[0];
        //}

        //public ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        //{
        //    var request = new ReadHoldingRegistersRequest(slaveId, startAddress, numberOfPoints);
        //    var responseData = SendRequestWithRetry(request.Serialize());
        //    var response = ReadHoldingRegistersResponse.Deserialize(slaveId, responseData);
        //    return response.Registers.Take(numberOfPoints).ToArray();
        //}

        //public ushort ReadHoldingRegister(byte slaveId, ushort address)
        //{
        //    var registers = ReadHoldingRegisters(slaveId, address, 1);
        //    return registers[0];
        //}

        //public void WriteCoil(byte slaveId, ushort address, bool value)
        //{
        //    var request = new WriteCoilRequest(slaveId, address, value);
        //    var responseData = SendRequestWithRetry(request.Serialize());
        //    var response = WriteCoilResponse.Deserialize(slaveId, responseData);

        //    if (response.Address != address || response.Value != value)
        //        throw new InvalidOperationException("Write coil verification failed");
        //}

        //public void WriteRegister(byte slaveId, ushort address, ushort value)
        //{
        //    var request = new WriteRegisterRequest(slaveId, address, value);
        //    var responseData = SendRequestWithRetry(request.Serialize());
        //    var response = WriteRegisterResponse.Deserialize(slaveId, responseData);

        //    if (response.Address != address || response.Value != value)
        //        throw new InvalidOperationException("Write register verification failed");
        //}

        //private byte[] SendRequestWithRetry(byte[] request)
        //{
        //    for (int attempt = 0; attempt <= _settings.Retries; attempt++)
        //    {
        //        try
        //        {
        //            return _transport.SendRequest(request);
        //        }
        //        catch (TimeoutException) when (attempt < _settings.Retries)
        //        {
        //            System.Threading.Thread.Sleep(_settings.DelayBetweenRetriesMs);
        //        }
        //    }

        //    throw new TimeoutException($"Request failed after {_settings.Retries + 1} attempts");
        //}

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
