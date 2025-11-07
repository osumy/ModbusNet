namespace ModbusNet.Enum
{
    /// <summary>
    /// Modbus protocol-defined exception codes returned by slaves when an error occurs.
    /// See: Modbus Application Protocol Specification v1.1b3, Section 7.
    /// </summary>
    public enum ModbusExceptionCode : byte
    {
        /// <summary>
        /// The function code received in the query is not an allowable action for the slave.
        /// This may be because the function code is only applicable to newer devices, and 
        /// was not implemented in the unit selected.  It could also indicate that the slave
        /// is in the wrong state to process a request of this type, for example because it 
        /// is unconfigured and is being asked to return register values. If a Poll Program 
        /// Complete command was issued, this code indicates that no program function preceded it.
        /// </summary>
        IllegalFunction = 0x01,

        /// <summary>
        /// The data address received in the query is not an allowable address for the slave.
        /// More specifically, the combination of reference number and transfer length is invalid.
        /// For a controller with 100 registers, a request with offset 96 and length 4 would succeed,
        /// a request with offset 96 and length 5 will generate exception 02.
        /// </summary>
        IllegalDataAddress = 0x02,

        /// <summary>
        /// A value contained in the query data field is not an allowable value for the slave. 
        /// This indicates a fault in the structure of remainder of a complex request, such as
        /// that the implied length is incorrect. It specifically does NOT mean that a data item
        /// submitted for storage in a register has a value outside the expectation of the 
        /// application program, since the MODBUS protocol is unaware of the significance of
        /// any particular value of any particular register.
        /// </summary>
        IllegalDataValue = 0x03,

        /// <summary>
        /// An unrecoverable error occurred while the server (or slave) was attempting
        /// to perform the requested action.
        /// </summary>
        SlaveDeviceFailure = 0x04,

        /// <summary>
        /// Specialized use in conjunction with programming commands.
        /// The slave has accepted the request and is processing it, but a long duration of time
        /// will be required to do so.  This response is returned to prevent a timeout error from 
        /// occurring in the master. The master can next issue a Poll Program Complete message to 
        /// determine if processing is completed.
        /// </summary>
        Acknowledge = 0x05,

        /// <summary>
        /// Specialized use in conjunction with programming commands.
        /// The slave is engaged in processing a long-duration program command. The master should
        /// retransmit the message later when the slave is free.
        /// </summary>
        SlaveDeviceBusy = 0x06,

        /// <summary>
        /// The slave cannot perform the program function received in the query. This code is returned 
        /// for an unsuccessful programming request using function code 13 or 14 decimal. The master 
        /// should request diagnostic or error information from the slave.
        /// </summary>
        NegativeAcknowledge = 0x07,

        /// <summary>
        /// Specialized use in conjunction with function codes 20 and 21 and reference type 6, to
        /// indicate that the extended file area failed to pass a consistency check.
        /// The slave attempted to read extended memory or record file, but detected a parity error in memory. 
        /// The master can retry the request, but service may be required on the slave device.
        /// </summary>
        MemoryParityError = 0x08,

        /// <summary>
        /// Specialized use in conjunction with gateways, indicates that the gateway was unable to allocate an 
        /// internal communication path from the input port to the output port for processing the request. 
        /// Usually means the gateway is misconfigured or overloaded.
        /// </summary>
        GatewayPathUnavailable = 0x0A,

        /// <summary>
        /// Specialized use in conjunction with gateways, indicates that no response was obtained from the target
        /// device. Usually means that the device is not present on the network.
        /// </summary>
        GatewayTargetDeviceFailedToRespond = 0x0B
    }
}
