namespace ModbusNet.Samples
{
    internal class TcpIp
    {
        public async static void Test()
        {
            var settings = new ModbusSettings
            {
                IpAddress = "192.168.1.10",
                Port = 502,
                WriteTimeout = 5000,
                ReadTimeout = 5000
            };

            var master = ModbusFactory.CreateTcpMaster(settings);

            var resp =  master.ReadMultipleHoldingRegisters(slaveAddress: 1, startAddress: 6500, numberOfPoints: 5);

            foreach (var reg in resp)
            {
                Console.WriteLine(reg);
            }
        }
    }
}
