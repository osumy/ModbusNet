# 📡 ModbusNet: A .NET Library for Modbus Communication

![.NET](https://img.shields.io/badge/.NET-8.0%2B-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey.svg)

A modern, lightweight, and production-ready .NET library for communicating with Modbus devices using **Modbus ASCII (Serial)**, **Modbus RTU (Serial)**, and **Modbus TCP/IP** protocols.

Built for industrial automation, IoT gateways, SCADA systems, and embedded .NET applications.

---

## ✅ Features

- **Three transport modes**:
  - ✅ **Modbus ASCII** over serial ports (with LRC error checking)
  - ✅ **Modbus RTU** over serial ports (with CRC-16 and inter-frame timing)
  - ✅ **Modbus TCP** over Ethernet (with MBAP framing and async I/O)
- **Full function code support**:
  - Read/Write Coils & Discrete Inputs
  - Read/Write Holding & Input Registers
  - Mask Write, Read-Write Multiple, FIFO Queue
  - File Record Access, Diagnostics (serial-only), Device Identification
- **Robust error handling**:
  - Automatic exception decoding (Modbus exception codes)
  - LRC/CRC validation with custom exceptions (`ModbusExceptionLRC`, `ModbusExceptionCRC`)
- **Thread-safe & async-ready**:
  - `SendRequestReceiveResponseAsync()` for TCP and ASCII
  - Semaphore-locked writes to prevent frame corruption
- **Clean architecture**:
  - `IModbusMaster` interface for testability
  - Factory-based creation (`ModbusFactory`)
  - Settings-driven configuration (`ModbusSettings`)
- **Extensible design**:
  - Easy to add new function codes or transports

---

## 🚀 Quick Start

### 1. Configure Settings

```csharp
var settings = new ModbusSettings
{
    ConnectionType = ConnectionType.TCP,
    IpAddress = "192.168.1.10",
    Port = 502,
    RetryCount = 3,
    RetryDelayMs = 100
};
```

For serial (RTU or ASCII):
```csharp
var settings = new ModbusSettings
{
    ConnectionType = ConnectionType.RTU, // or ASCII
    PortName = PortName.COM4,
    BaudRate = 9600,
    Parity = Parity.Even,
    DataBits = DataBits.db8,
    StopBits = StopBits.One
};
```
### 2. Create a Master
```csharp
// TCP
var master = ModbusFactory.CreateTcpMaster(settings);

// RTU
var master = ModbusFactory.CreateRtuMaster(settings);

// ASCII
var master = ModbusFactory.CreateAsciiMaster(settings);
```
### 3. Use It!
```csharp
// Read 10 holding registers starting at address 0
ushort[] registers = master.ReadMultipleHoldingRegisters(slaveAddress: 1, startAddress: 0, numberOfPoints: 10);

// Write a single coil
master.WriteSingleCoil(slaveAddress: 1, address: 5, value: true);

// Read-Write atomically
ushort[] readData = master.ReadWriteMultipleRegisters(
    slaveAddress: 1,
    startReadAddress: 0,
    numberOfPointsToRead: 3,
    startWriteAddress: 10,
    writeData: new ushort[] { 100, 200, 300 }
);
```

> 💡 All methods validate inputs, handle retries, and throw meaningful exceptions on error.

 ## 🧩 Supported Function Codes
 The ModbusNet library provides full support for standard Modbus function codes across all three transport modes: ASCII, RTU, and TCP. Operations are organised by data type and use case, enabling robust communication with industrial devices.
| Category              | Function Code | Method Name                     |
|-----------------------|---------------|---------------------------------|
| Discrete Inputs       | `0x02`          | `ReadDiscreteInputs()`              |
| Coils                 | `0x01`          | `ReadCoils()`                       |
|                       | `0x05`          | `WriteSingleCoil()`                 |
|                       | `0x0F`          | `WriteMultipleCoils()`                 |
| Input Registers                 | `0x04`          | `ReadInputRegisters()`                       |
| Holding Registers     | `0x03`          | `ReadMultipleHoldingRegisters()`    |
|                       | `0x06`          | `WriteSingleHoldingRegister()`   |
|                       | `0x10`          | `WriteMultipleHoldingRegisters()`   |
|                       | `0x17`          | `ReadWriteMultipleRegisters()`   |
|                       | `0x16`          | `MaskWriteRegister()`   |
|                       | `0x18`          | `ReadFifoQueue()`   |
| File Record           | `0x14/0x1`          | `ReadFileRecord()`, `WriteFileRecord()`   |
| Diagnostics (Serial)  | `0x07–0x11`     | Serial-only diagnostics |
| Device ID  |`0x2B`     | `ReadDeviceIdentification()` |

> 📌 Diagnostics (0x07, 0x08, etc.) are only available in serial modes (RTU/ASCII). 

## ⚙️ Configuration (`ModbusSettings`)
| Property              | Description |
|-----------------------|---------------|
| `ConnectionType`       | `TCP`, `RTU` or `ASCII`      |
| `IpAddress`, `Port`       | For TCP mode     |
| `PortName`, `BaudRate`, etc       | For serial modes     |
| `AsciiStartDelimiter`       | Usually `":"`      |
| `AsciiEndDelimiter`       | Usually `"\r\n"`      |
| `RtuInterCharTimeMs`       | Inter-character timeout (auto-calculated if baud < 19200)      |
| `RtuInterFrameTimeMs`       | Inter-frame delay    |
| `RetryCount`       | Max retries on timeout (default: 3)    |
| `RetryDelayMs`       | Delay between retries (default: 50 ms)     |

> 🔄 RTU timing is automatically calculated for baud rates below 19200 per Modbus spec. 

## 📦 Project Structure
```
ModbusNet/
├── ModbusNet/               # Core library (IModbusMaster, transports, messages, utils)
├── ModbusNet.Samples/       # Console examples
├── ModbusUI/                # WinForms test UI 
└── ModbusNet.Tests/         # Unit tests (not required for usage)
```

## Testing & Sample Codes
The solution includes:
- A WinForms GUI (`ModbusUI`) for visual testing of all three protocols.
- A sample console project (`ModbusNet.Samples`) showing basic usage patterns.
  
> 🔒 The core library has no UI dependencies — the GUI is purely for testing/demo. 

## License
Distributed under the MIT License. See [LICENSE](LICENSE) for details.

## 🙌 Contributing
Contributions are welcome! Feel free to:
- Report bugs
- Suggest enhancements
- Implement other function codes
- Improve documentation

## 📬 Need Help?
Open an issue or contact the maintainers.

> ModbusNet brings industrial-grade Modbus communication to modern .NET developers with simplicity, correctness, and performance.





