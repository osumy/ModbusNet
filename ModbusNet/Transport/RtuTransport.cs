using ModbusNet.Core;
using ModbusNet.Core.Utils;
using System.IO.Ports;

namespace ModbusNet.Transport
{
    public class RtuTransport : IModbusTransport
    {
        private readonly SerialPort _serialPort;
        private readonly ModbusSettings _settings;

        public bool IsConnected => _serialPort.IsOpen;

        public RtuTransport(string portName, int baudRate, ModbusSettings settings = null)
        {
            _settings = settings ?? ModbusSettings.Default;
            _serialPort = new SerialPort(portName, baudRate)
            {
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadTimeout = (int)_settings.Timeout.TotalMilliseconds,
                WriteTimeout = (int)_settings.Timeout.TotalMilliseconds
            };
        }

        public void Connect()
        {
            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }

        public void Disconnect()
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }

        public byte[] SendRequest(byte[] request)
        {
            Connect();

            // اضافه کردن CRC
            var crc = Crc16.Calculate(request);
            var frame = new byte[request.Length + 2];
            Array.Copy(request, 0, frame, 0, request.Length);
            frame[request.Length] = (byte)(crc & 0xFF);
            frame[request.Length + 1] = (byte)((crc >> 8) & 0xFF);

            // ارسال
            _serialPort.Write(frame, 0, frame.Length);

            // دریافت پاسخ
            return ReceiveResponse(request[0]); // slaveId
        }

        private byte[] ReceiveResponse(byte expectedSlaveId)
        {
            // خواندن هدر (SlaveId + FunctionCode)
            var header = new byte[2];
            ReadBytes(header, 0, 2);

            if (header[0] != expectedSlaveId)
                throw new InvalidOperationException("Slave ID mismatch");

            // تشخیص طول داده بر اساس Function Code
            int dataLength = GetExpectedDataLength(header[1]);
            var response = new byte[2 + dataLength + 2]; // header + data + crc

            // کپی هدر
            response[0] = header[0];
            response[1] = header[1];

            // خواندن بقیه داده
            ReadBytes(response, 2, dataLength + 2); // data + crc

            // بررسی CRC
            var dataWithoutCrc = response.Take(response.Length - 2).ToArray();
            var receivedCrc = (ushort)(response[response.Length - 2] | (response[response.Length - 1] << 8));
            var calculatedCrc = ErrorCheckCalculator.ComputeCrc(dataWithoutCrc);

            //if (receivedCrc != calculatedCrc) // TODO
            //    throw new InvalidOperationException("CRC check failed");

            return dataWithoutCrc;
        }

        private int GetExpectedDataLength(byte functionCode)
        {
            return functionCode switch
            {
                0x01 or 0x02 => 1, // Read Coils/Inputs - byte count
                0x03 or 0x04 => 1, // Read Registers - byte count  
                0x05 or 0x06 => 4, // Write Single
                0x0F or 0x10 => 2, // Write Multiple
                _ => throw new NotSupportedException($"Function code 0x{functionCode:X2} not supported")
            };
        }

        private void ReadBytes(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int read = _serialPort.Read(buffer, offset + totalRead, count - totalRead);
                if (read == 0)
                    throw new TimeoutException("Read timeout");
                totalRead += read;
            }
        }

        public void Dispose()
        {
            _serialPort?.Dispose();
        }
    }
}