using ModbusNet.Device;
using ModbusNet.Enum;
using ModbusNet.Transport;
using System.IO.Ports;

namespace ModbusNet
{
    public static class ModbusFactory
    {
        public static ModbusMaster CreateRtuMaster(ModbusSettings modbusSettings)
        {
            var serialPort = CreateSerialPort(modbusSettings);
            var transport = new RtuTransport(serialPort, modbusSettings);
            return new ModbusMaster(transport, modbusSettings);
        }

        public static ModbusMaster CreateAsciiMaster(ModbusSettings modbusSettings)
        {
            var serialPort = CreateSerialPort(modbusSettings);
            var transport = new AsciiTransport(serialPort, modbusSettings);
            return new ModbusMaster(transport, modbusSettings);
        }

        private static SerialPort CreateSerialPort(ModbusSettings modbusSettings)
        {
            if (modbusSettings.ConnectionType == ConnectionType.RTU
                && modbusSettings.BaudRate < 19200)
            {
                modbusSettings.RtuInterFrameTimeMs = (int)Math.Ceiling(42.0 / modbusSettings.BaudRate);
                modbusSettings.RtuInterCharTimeMs = (int)Math.Ceiling(18.0 / modbusSettings.BaudRate);
            }

            return new SerialPort(
                portName: modbusSettings.PortName.ToString(),
                baudRate: modbusSettings.BaudRate,
                parity: modbusSettings.Parity,
                dataBits: (int)modbusSettings.DataBits,
                stopBits: modbusSettings.StopBits)
            {
                ReadTimeout = modbusSettings.ReadTimeout,
                WriteTimeout = modbusSettings.WriteTimeout,
                Handshake = Handshake.None
            };
        }
    }
}