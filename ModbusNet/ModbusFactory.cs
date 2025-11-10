using ModbusNet.Device;
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
            return new ModbusMaster(transport);
        }

        public static ModbusMaster CreateAsciiMaster(ModbusSettings modbusSettings)
        {
            var serialPort = CreateSerialPort(modbusSettings);
            var transport = new AsciiTransport(serialPort, modbusSettings);
            return new ModbusMaster(transport);
        }

        private static SerialPort CreateSerialPort(ModbusSettings modbusSettings)
        {
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