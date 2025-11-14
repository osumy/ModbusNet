using Modbus.Models.Enums;
using System.IO.Ports;

namespace Modbus.Models
{
    public class ConnectionSettings
    {
        // === Modbus connection configurations ===

        public ConnectionType connectionType { get; set; } = ConnectionType.SerialPort;
        public PortName portName { get; set; } = PortName.COM3;
        public int baudRate { get; set; } = 9600;
        public Parity parity { get; set; } = Parity.Even;
        public DataBits dataBits { get; set; } = DataBits.db7;
        public StopBits stopBits { get; set; } = StopBits.One;
        public bool isRTU { get; set; } = false;
        public int responseTimeout = 2000;
        public int delay = 20;
        public int retryCount { get; set; } = 3;
        public int retryDelay { get; set; } = 50;
    }
}
