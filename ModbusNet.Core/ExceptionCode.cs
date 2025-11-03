namespace ModbusNet.Core
{
    /// <summary>
    /// Modbus protocol-defined exception codes returned by slaves when an error occurs.
    /// See: Modbus Application Protocol Specification v1.1b3, Section 7.
    /// </summary>
    public enum ExceptionCode : byte
    {
        /// <summary>
        /// The function code received in the query is not an allowable action
        /// for the server (or slave). 
        /// </summary>
        IllegalFunction = 0x01,

        /// <summary>
        /// The data address received in the query is not an allowable address
        /// for the server (or slave).
        /// </summary>
        IllegalDataAddress = 0x02,

        /// <summary>
        /// A value contained in the query data field is not an allowable value
        /// for the server (or slave).
        /// </summary>
        IllegalDataValue = 0x03,

        /// <summary>
        /// An unrecoverable error occurred while the server (or slave) was attempting
        /// to perform the requested action.
        /// </summary>
        SlaveDeviceFailure = 0x04,

        /// <summary>
        /// The server (or slave) has accepted the request and is processing it,
        /// but a long duration of time is required.
        /// </summary>
        Acknowledge = 0x05,

        /// <summary>
        /// The server (or slave) is busy processing a long-duration program command.
        /// </summary>
        SlaveDeviceBusy = 0x06,

        /// <summary>
        /// The server (or slave) cannot perform the program function received in the query.
        /// </summary>
        NegativeAcknowledge = 0x07,

        /// <summary>
        /// The server (or slave) attempted to read extended memory or record file, but 
        /// detected a parity error in memory.
        /// </summary>
        MemoryParityError = 0x08,

        /// <summary>
        /// The gateway was unable to allocate an internal communication path
        /// to process the request. Usually means the gateway is misconfigured or overloaded.
        /// </summary>
        GatewayPathUnavailable = 0x0A,

        /// <summary>
        /// The gateway targeted device failed to respond. Usually means that the device
        /// is not present on the network.
        /// </summary>
        GatewayTargetDeviceFailedToRespond = 0x0B
    }
}
