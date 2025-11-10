namespace ModbusNet
{
    public class ModbusSettings
    {
        public static ModbusSettings Default => new ModbusSettings();

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
        public byte Retries { get; set; } = 3;
        public int DelayBetweenRetriesMs { get; set; } = 100;

        public string AsciiStartDelimiter { get; set; } = ":";
        public string AsciiEndDelimiter { get; set; } = "\r\n";

        public int RtuInterCharTimeoutMs { get; set; } = 2;
        public int RtuInterFrameTimeoutMs { get; set; } = 5;
    }
}