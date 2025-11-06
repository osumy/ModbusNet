namespace ModbusNet.Core.Messages
{
    /// <summary>
    ///     A message built by the master (client) that initiates a Modbus transaction.
    /// </summary>
    public interface IModbusMessage
    {
        /// <summary>
        ///     Address of the slave (server).
        /// </summary>
        byte SlaveId { get; set; }

        /// <summary>
        ///     The function code tells the server what kind of action to perform.
        /// </summary>
        byte FunctionCode { get; set; }



    }
}
