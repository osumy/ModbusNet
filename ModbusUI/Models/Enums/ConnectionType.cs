using System.ComponentModel;

namespace Modbus.Models.Enums
{
    public enum ConnectionType
    {
        [Description("Serial Port")]
        SerialPort,

        [Description("TCP/IP")]
        TcpIp
    }
}
