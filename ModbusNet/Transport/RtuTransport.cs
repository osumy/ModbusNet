using ModbusNet;
using ModbusNet.Utils;
using System.IO.Ports;

namespace ModbusNet.Transport
{
    public class RtuTransport : IModbusTransport
    {
        private readonly SerialPort _serialPort;
        private readonly ModbusSettings _settings;

        public bool IsConnected => _serialPort.IsOpen;
        private bool _disposed = false;


        public RtuTransport(SerialPort serialPort, ModbusSettings settings)
        {
            _serialPort = serialPort;
            _settings = settings;

            if (!_serialPort.IsOpen)
                _serialPort.Open();
        }


        public byte[] SendRequest(byte[] request)
        {
            ThrowIfDisposed();
            //// اضافه کردن CRC
            //var crc = ErrorCheckCalculator.ComputeCrc(request);
            //var frame = new byte[request.Length + 2];
            //Array.Copy(request, 0, frame, 0, request.Length);
            //frame[request.Length] = (byte)(crc & 0xFF);
            //frame[request.Length + 1] = (byte)((crc >> 8) & 0xFF);

            //// ارسال
            //_serialPort.Write(frame, 0, frame.Length);

            //// دریافت پاسخ
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
            var calculatedCrc = ErrorCheckUtility.ComputeCrc(dataWithoutCrc);

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

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _serialPort?.Dispose();
                _disposed = true;
            }
        }

        ushort[] IModbusTransport.SendRequest(byte[] request)
        {
            throw new NotImplementedException();
        }

    }
}