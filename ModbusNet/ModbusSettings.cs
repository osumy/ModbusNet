namespace ModbusNet
{
    public class ModbusSettings
    {
        public static ModbusSettings Default => new ModbusSettings();


        public TimeSpan Timeout
        {
            get => responseTimeout;
            set
            {
                responseTimeout = value;
                requestTimeout = value;
            }
        }
        public TimeSpan responseTimeout { get; set; } = TimeSpan.FromSeconds(2);
        public TimeSpan requestTimeout { get; set; } = TimeSpan.FromSeconds(2);


        public byte retryCount { get; set; } = 3;
        public int retryDelayMs { get; set; } = 50;


        public string AsciiStartDelimiter { get; set; } = ":";
        public string AsciiEndDelimiter { get; set; } = "\r\n";


        public int RtuInterCharTimeoutMs { get; set; } = 2;
        public int RtuInterFrameTimeoutMs { get; set; } = 5;
    }
}