using ModbusNet;

var master = ModbusFactory.CreateAsciiMaster(ModbusSettings.Default);


var resp = master.ReadMultipleHoldingRegisters(slaveAddress: 1, startAddress: 6500, numberOfPoints: 5);

foreach (var reg in resp)
{
    Console.WriteLine(reg);
}