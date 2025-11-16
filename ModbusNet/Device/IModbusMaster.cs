namespace ModbusNet.Device
{
    public interface IModbusMaster : IDisposable
    {
        #region Bit Access - Discrete Inputs

        /// <summary>
        /// Reads from 1 to 2000 contiguous discrete inputs status.
        /// Function Code: 0x02 (Read_Discrete_Inputs)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of discrete inputs to read.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Discrete inputs status as boolean array.</returns>
        bool[] ReadDiscreteInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints);

        #endregion

        #region Bit Access - Coils

        /// <summary>
        /// Reads from 1 to 2000 contiguous coils status.
        /// Function Code: 0x01 (Read_Coils)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of coils to read.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Coils status as boolean array.</returns>
        bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        /// Writes a single coil value.
        /// Function Code: 0x05 (Write_Single_Coil)
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="coilAddress">Address to write value to.</param>
        /// <param name="value">Value to write.</param>
        void WriteSingleCoil(byte slaveAddress, ushort address, bool value);

        /// <summary>
        /// Writes a sequence of coils.
        /// Function Code: 0x0F (Write_Multiple_Coils)
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data);

        #endregion

        #region Word Access - Input Registers

        /// <summary>
        /// Reads contiguous block of input registers.
        /// Function Code: 0x04 (Read_Input_Registers)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of input registers to read.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Input registers values.</returns>
        ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints);

        #endregion

        #region Word Access - Holding Registers

        /// <summary>
        /// Reads contiguous block of holding registers.
        /// Function Code: 0x03 (Read_Multiple_Holding_Registers)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Holding registers values.</returns>
        ushort[] ReadMultipleHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints);

        /// <summary>
        /// Writes a single holding register.
        /// Function Code: 0x06 (Write_Single_Holding_Register)
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        void WriteSingleHoldingRegister(byte slaveAddress, ushort address, ushort value);

        /// <summary>
        /// Writes a block of 1 to 123 contiguous registers.
        /// Function Code: 0x10 (Write_Multiple_Holding_Registers)
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        void WriteMultipleHoldingRegisters(byte slaveAddress, ushort startAddress, ushort[] data);

        /// <summary>
        /// Performs a combination of one read operation and one write operation in a single Modbus transaction.
        /// The write operation is performed before the read.
        /// Function Code: 0x17 (ReadWrite_Multiple_Registers)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="startReadAddress">Address to begin reading (Holding registers are addressed starting at 0).</param>
        /// <param name="numberOfPointsToRead">Number of registers to read.</param>
        /// <param name="startWriteAddress">Address to begin writing (Holding registers are addressed starting at 0).</param>
        /// <param name="writeData">Register values to write.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Read holding registers values.</returns>
        ushort[] ReadWriteMultipleRegisters(
            byte slaveAddress,
            ushort startReadAddress,
            ushort numberOfPointsToRead,
            ushort startWriteAddress,
            ushort[] writeData);

        /// <summary>
        /// Modifies the contents of a specified holding register using a combination of an AND mask, an OR mask, and the register's current contents.
        /// Function Code: 0x16 (Mask_Write_Register)
        /// </summary>
        /// <param name="slaveAddress">Address of the device to write to.</param>
        /// <param name="address">Address of the register to mask write.</param>
        /// <param name="andMask">AND mask to apply to the current register value.</param>
        /// <param name="orMask">OR mask to apply to the current register value.</param>
        void MaskWriteRegister(byte slaveAddress, ushort address, ushort andMask, ushort orMask);

        /// <summary>
        /// Reads the contents of a First-In-First-Out (FIFO) queue of register words.
        /// Function Code: 0x18 (Read_FIFO_Queue)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="fifoPointerAddress">Address of the FIFO pointer register.</param>
        /// <returns>FIFO queue contents.</returns>
        ushort[] ReadFifoQueue(byte slaveAddress, ushort fifoPointerAddress);

        #endregion

        #region File Record Access

        /// <summary>
        /// Reads a file record from the device.
        /// Function Code: 0x14 (Read_File_Record)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read values from.</param>
        /// <param name="fileNumber">The Extended Memory file number.</param>
        /// <param name="startingAddress">The starting register address within the file.</param>
        /// <param name="numberOfRegisters">Number of registers to read from the file.</param>
        /// <returns>File record data as byte array.</returns>
        byte[] ReadFileRecord(byte slaveAddress, ushort fileNumber, ushort startingAddress, ushort numberOfRegisters);

        /// <summary>
        /// Writes a file record to the device.
        /// Function Code: 0x15 (Write_File_Record)
        /// </summary>
        /// <param name="slaveAddress">Address of device to write values to.</param>
        /// <param name="fileNumber">The Extended Memory file number.</param>
        /// <param name="startingAddress">The starting register address within the file.</param>
        /// <param name="data">The data to be written.</param>
        void WriteFileRecord(byte slaveAddress, ushort fileNumber, ushort startingAddress, byte[] data);

        #endregion

        #region Diagnostics (Serial Only)

        /// <summary>
        /// Reads the exceptional status of the device (8 bits of device data).
        /// Function Code: 0x07 (Read_Exception_Status) - Serial Only
        /// </summary>
        /// <param name="slaveAddress">Address of device to read from.</param>
        /// <returns>Exception status byte.</returns>
        byte ReadExceptionStatus(byte slaveAddress);

        /// <summary>
        /// Performs diagnostic functions on the device.
        /// Function Code: 0x08 (Diagnostics) - Serial Only
        /// </summary>
        /// <param name="slaveAddress">Address of device to perform diagnostics on.</param>
        /// <param name="subFunctionCode">Diagnostic sub-function code.</param>
        /// <param name="data">Data associated with the diagnostic function.</param>
        /// <returns>Diagnostic return data.</returns>
        ushort[] Diagnostics(byte slaveAddress, ushort subFunctionCode, ushort[] data);

        /// <summary>
        /// Gets the communication event counter (count of messages and events).
        /// Function Code: 0x0B (Get_Com_Event_Counter) - Serial Only
        /// </summary>
        /// <param name="slaveAddress">Address of device to read from.</param>
        /// <returns>Communication event counter status.</returns>
        (ushort Status, ushort EventCount) GetCommunicationEventCounter(byte slaveAddress);

        /// <summary>
        /// Gets the communication event log (events and messages).
        /// Function Code: 0x0C (Get_Com_Event_Log) - Serial Only
        /// </summary>
        /// <param name="slaveAddress">Address of device to read from.</param>
        /// <returns>Communication event log data.</returns>
        (ushort Status, ushort EventCount, ushort MessageCount, byte[] Events) GetCommunicationEventLog(byte slaveAddress);

        /// <summary>
        /// Reports the server ID information.
        /// Function Code: 0x11 (Report_Server_ID) - Serial Only
        /// </summary>
        /// <param name="slaveAddress">Address of device to query.</param>
        /// <returns>Server identification data.</returns>
        byte[] ReportServerId(byte slaveAddress);

        #endregion

        #region Other Function Codes

        /// <summary>
        /// Reads the device identification information.
        /// Function Code: 0x2B (Read_Device_Identification)
        /// </summary>
        /// <param name="slaveAddress">Address of device to read from.</param>
        /// <param name="objectId">The object ID to read.</param>
        /// <param name="meiType">The MEI type (usually 0x0E for device identification).</param>
        /// <returns>Device identification data.</returns>
        byte[] ReadDeviceIdentification(byte slaveAddress, byte objectId, byte meiType);

        #endregion
    }
}