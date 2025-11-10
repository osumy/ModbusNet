namespace ModbusNet.Enum
{
    /// <summary>
    /// Supported public function codes
    /// </summary>
    public enum ModbusPublicFunctionCode : byte
    {
        //// Bit Access ////
        // Discrete inputs
        Read_Discrete_Inputs = 0x02,

        // Coils
        Read_Coils = 0x01,
        Write_Single_Coil = 0x05,
        Write_Multiple_Coils = 0x0F,

        //// Word (16-bit) Access //// 
        // Input Registers
        Read_Input_Registers = 0x04,

        // Holding Registers
        Read_Multiple_Holding_Registers = 0x03,
        Write_Single_Holding_Register = 0x06,
        Write_Multiple_Holding_Registers = 0x10,
        ReadWrite_Multiple_Registers = 0x17,
        Mask_Write_Register = 0x16,
        Read_FIFO_Queue = 0x18,

        //// File Record Access //// 
        Read_File_Record = 0x14,
        Write_File_Record = 0x15,

        //// Diagnostics (Serial Only) ////
        Read_Exception_Status = 0x07,       
        Diagnostics = 0x08,                 
        Get_Com_Event_Counter = 0x0B,      
        Get_Com_Event_Log = 0x0C,           
        Report_Server_ID = 0x11,            

        //// Other Function Codes //// 
        Read_Device_Identification = 0x2B
    }

    public static class ModbusPublicFunctionCodeExtensions
    {
        /// Mask used to identify error codes in Modbus function codes.
        private const byte ErrorMask = 0b1000_0000;

        /// <summary>
        /// Determines if the given function code represents an error.
        /// </summary>
        /// <param name="code">The function code to check.</param>
        /// <returns>True if the function code represents an error; otherwise, false.</returns>
        public static bool IsError(this ModbusPublicFunctionCode code) => ((byte)code & ErrorMask) != 0;

        /// <summary>
        /// Converts the given function code to its error representation.
        /// </summary>
        /// <param name="code">The function code to convert.</param>
        /// <returns>The error representation of the function code.</returns>
        public static ModbusPublicFunctionCode ToError(this ModbusPublicFunctionCode code) => (ModbusPublicFunctionCode)((byte)code | ErrorMask);

        /// <summary>
        /// Removes the error representation from the given function code.
        /// </summary>
        /// <param name="code">The function code to modify.</param>
        /// <returns>The function code without the error representation.</returns>
        public static ModbusPublicFunctionCode WithoutError(this ModbusPublicFunctionCode code) => (ModbusPublicFunctionCode)((byte)code & 0b0111_1111);
    }
}
