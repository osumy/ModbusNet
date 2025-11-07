using ModbusNet.Core;
using System.IO.Ports;

var factory = new ModbusFactory();

SerialPort _serialPort = new SerialPort(
                    portName: "COM3",
                    baudRate: 9600,
                    parity: Parity.Even,
                    dataBits: 7,
                    stopBits: StopBits.One)
{
    ReadTimeout = 2000,
    WriteTimeout = 2000,
    Handshake = Handshake.None
};

var master = factory.CreateAsciiMaster(_serialPort);

Console.WriteLine("Hello, World!");
