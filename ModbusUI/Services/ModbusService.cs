using ModbusNet;
using ModbusNet.Device;

namespace ModbusUI.Services
{
    /// <summary>
    /// Comprehensive Modbus service for serial communication
    /// Supports both RTU and ASCII protocols
    /// </summary>
    public class ModbusService : IDisposable
    {
        private IModbusMaster _master;
        private bool _isConnected = false;

        #region Connection Management

        /// <summary>
        /// Connects to Modbus device
        /// </summary>
        /// <param name="settings">Connection parameters</param>
        /// <returns>True if connection successful</returns>
        public void Connect(ModbusSettings settings)
        {
            try
            {
                _master = ModbusFactory.CreateAsciiMaster(settings);

                _isConnected = true;
            }
            catch (Exception ex)
            {
                _isConnected = false;
            }
        }


        /// <summary>
        /// Checks if connected to Modbus device
        /// </summary>
        public bool IsConnected => _isConnected;

        #endregion

        #region Holding Register Operations (Function 03/06/16)

        /// <summary>
        /// Reads holding registers (Function Code 03)
        /// </summary>
        /// <param name="slaveId">Slave device ID</param>
        /// <param name="startAddress">Starting register address</param>
        /// <param name="numberOfPoints">Number of registers to read</param>
        /// <returns>Array of register values</returns>
        public ushort[] ReadHoldingRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            ValidateConnection();
            return _master.ReadMultipleHoldingRegisters(slaveId, startAddress, numberOfPoints);
        }

        /// <summary>
        /// Writes single holding register (Function Code 06)
        /// </summary>
        /// <param name="slaveId">Slave device ID</param>
        /// <param name="registerAddress">Register address to write</param>
        /// <param name="value">Value to write</param>
        public void WriteSingleRegister(byte slaveId, ushort registerAddress, ushort value)
        {
            ValidateConnection();
            _master.WriteSingleHoldingRegister(slaveId, registerAddress, value);
        }

        /// <summary>
        /// Writes multiple holding registers (Function Code 16)
        /// </summary>
        /// <param name="slaveId">Slave device ID</param>
        /// <param name="startAddress">Starting register address</param>
        /// <param name="data">Array of values to write</param>
        public void WriteMultipleRegisters(byte slaveId, ushort startAddress, ushort[] data)
        {
            ValidateConnection();
            _master.WriteMultipleHoldingRegisters(slaveId, startAddress, data);
        }

        #endregion

        #region Coil Operations (Function 01/05/15)

        /// <summary>
        /// Reads coils (Function Code 01)
        /// </summary>
        /// <param name="slaveId">Slave device ID</param>
        /// <param name="startAddress">Starting coil address</param>
        /// <param name="numberOfPoints">Number of coils to read</param>
        /// <returns>Array of coil states</returns>
        public bool[] ReadCoils(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            ValidateConnection();
            return _master.ReadCoils(slaveId, startAddress, numberOfPoints);
        }

        /// <summary>
        /// Writes single coil (Function Code 05)
        /// </summary>
        /// <param name="slaveId">Slave device ID</param>
        /// <param name="coilAddress">Coil address to write</param>
        /// <param name="value">Value to write (true=ON, false=OFF)</param>
        public void WriteSingleCoil(byte slaveId, ushort coilAddress, bool value)
        {
            ValidateConnection();
            _master.WriteSingleCoil(slaveId, coilAddress, value);
        }

        /// <summary>
        /// Writes multiple coils (Function Code 15)
        /// </summary>
        /// <param name="slaveId">Slave device ID</param>
        /// <param name="startAddress">Starting coil address</param>
        /// <param name="data">Array of coil states to write</param>
        public void WriteMultipleCoils(byte slaveId, ushort startAddress, bool[] data)
        {
            ValidateConnection();
            _master.WriteMultipleCoils(slaveId, startAddress, data);
        }

        #endregion

        #region Input Register Operations (Function 04)

        /// <summary>
        /// Reads input registers (Function Code 04)
        /// </summary>
        /// <param name="slaveId">Slave device ID</param>
        /// <param name="startAddress">Starting register address</param>
        /// <param name="numberOfPoints">Number of registers to read</param>
        /// <returns>Array of register values</returns>
        public ushort[] ReadInputRegisters(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            ValidateConnection();
            return _master.ReadInputRegisters(slaveId, startAddress, numberOfPoints);
        }

        #endregion

        #region Discrete Input Operations (Function 02)

        /// <summary>
        /// Reads discrete inputs (Function Code 02)
        /// </summary>
        /// <param name="slaveId">Slave device ID</param>
        /// <param name="startAddress">Starting input address</param>
        /// <param name="numberOfPoints">Number of inputs to read</param>
        /// <returns>Array of input states</returns>
        public bool[] ReadDiscreteInputs(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            ValidateConnection();
            return _master.ReadDiscreteInputs(slaveId, startAddress, numberOfPoints);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Tests communication with slave device by reading a single register
        /// </summary>
        /// <param name="slaveId">Slave device ID to test</param>
        /// <param name="testAddress">Register address to read for test</param>
        /// <returns>True if communication successful</returns>
        public bool TestConnection(byte slaveId, ushort testAddress = 0)
        {
            try
            {
                ValidateConnection();
                var result = _master.ReadMultipleHoldingRegisters(slaveId, testAddress, 1);
                return result != null && result.Length == 1;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates that connection is established and ready
        /// </summary>
        private void ValidateConnection()
        {
            if (!IsConnected || _master == null)
                throw new InvalidOperationException("Not connected to Modbus device");
        }

        #endregion

        #region Dispose

        private bool _disposed = false;

 
        public void Dispose()
        {
            if (!_disposed)
            {
                _master.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}