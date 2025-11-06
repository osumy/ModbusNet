namespace ModbusNet.Core
{
    public class ModbusSettings
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
        public byte Retries { get; set; } = 3;
        public int DelayBetweenRetriesMs { get; set; } = 100;

        // تنظیمات خاص ASCII
        public string AsciiStartDelimiter { get; set; } = ":";
        public string AsciiEndDelimiter { get; set; } = "\r\n";

        // تنظیمات خاص RTU
        public int RtuInterCharTimeoutMs { get; set; } = 2;
        public int RtuInterFrameTimeoutMs { get; set; } = 5;

        public static ModbusSettings Default => new ModbusSettings();
    }
}