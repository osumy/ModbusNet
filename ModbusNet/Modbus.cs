using ModbusNet.Device;
using ModbusNet.Transport;
using System.IO.Ports;

namespace ModbusNet
{
    public static class Modbus
    {
        public static ModbusMaster CreateRtuMaster(SerialPort serialPort, ModbusSettings modbusSettings)
        {
            var transport = new RtuTransport(serialPort, modbusSettings);
            return new ModbusMaster(transport);
        }

        public static ModbusMaster CreateAsciiMaster(SerialPort serialPort, ModbusSettings modbusSettings)
        {
            var transport = new AsciiTransport(serialPort, modbusSettings);
            return new ModbusMaster(transport);
        }
    }
}