using ModbusNet.Enum;
using System.IO.Ports;

namespace ModbusNet
{
    public class ModbusSettings
    {
        public static ModbusSettings Default => new ModbusSettings();

        #region General Settings

        public ConnectionType ConnectionType { get; set; } = ConnectionType.ASCII;
        public byte RetryCount { get; set; } = 3;
        public int RetryDelayMs { get; set; } = 50;

        #endregion

        #region Serial Port Settings

        public int Timeout
        {
            get => ReadTimeout;
            set
            {
                ReadTimeout = value;
                WriteTimeout = value;
            }
        }
        public int ReadTimeout { get; set; } = 2000;
        public int WriteTimeout { get; set; } = 2000;

        public PortName PortName { get; set; } = PortName.COM4;
        public int BaudRate { get; set; } = 9600;
        public Parity Parity { get; set; } = Parity.Even;
        public DataBits DataBits { get; set; } = DataBits.db7;
        public StopBits StopBits { get; set; } = StopBits.One;

        #endregion

        #region ASCII/RTU Specific Settings

        public string AsciiStartDelimiter { get; set; } = ":";
        public string AsciiEndDelimiter { get; set; } = "\r\n";


        public int RtuInterCharTimeoutMs { get; set; } = 2;
        public int RtuInterFrameTimeoutMs { get; set; } = 2;

        #endregion
    }
}