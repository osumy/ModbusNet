using System.ComponentModel;

namespace ModbusNet.Enum
{
    public enum ConnectionType
    {
        [Description("Serial Port")]
        SerialPort,

        [Description("TCP/IP")]
        TcpIp
    }
}
