using System.ComponentModel;

namespace Modbus.Models.Enums
{
    public enum PortName
    {
        [Description("Communications Port (COM1)")]
        COM1,

        [Description("USB Serial Device (COM3)")]
        COM3
    }
}
