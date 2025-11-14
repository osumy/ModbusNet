using System.ComponentModel;

namespace ModbusNet.Enum
{
    public enum PortName
    {
        [Description("Communications Port (COM1)")]
        COM1,

        [Description("USB Serial Device (COM3)")]
        COM3
    }
}
