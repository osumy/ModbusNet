using ModbusNet.Messages;
using ModbusNet.Utils;
using System.IO.Ports;
using System.Text;

namespace ModbusNet.Transport
{
    public class AsciiTransport : ModbusTransportBase
    {
        private readonly SerialPort _serialPort;
        private readonly ModbusSettings _settings;

        public bool IsConnected => _serialPort.IsOpen;
        private readonly SemaphoreSlim _writeLock = new(1, 1);
        private bool _disposed = false;

        public byte[] StartDelimiterAsciiArray { get; private set; }
        public byte[] EndDelimiterAsciiArray { get; private set; }


        public AsciiTransport(SerialPort serialPort, ModbusSettings settings)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (!_serialPort.IsOpen)
                _serialPort.Open();

            StartDelimiterAsciiArray = Encoding.ASCII.GetBytes(settings.AsciiStartDelimiter);
            EndDelimiterAsciiArray = Encoding.ASCII.GetBytes(settings.AsciiEndDelimiter);
        }


        public override ModbusResponse SendRequestReceiveResponse(byte[] request)
        {
            ThrowIfDisposed();

            if(IsConnected == false)
            {
                throw new InvalidOperationException("Serial port is not connected.");
            }


            for (int attempt = 0; attempt < _settings.RetryCount; attempt++)
            {
                try
                { 
                    _serialPort.Write(request, 0, request.Length);
                    var responsePDU = ReceiveResponse();

                    var fcAscii = new byte[2];
                    Array.Copy(request, 3, fcAscii, 0, fcAscii.Length);

                    ValidateFC(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);
                    ValidateByteCount(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);

                    return BuildResponse(responsePDU);
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    Thread.Sleep(_settings.RetryDelayMs);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.RetryCount + 1} attempts");
        }

        public override void SendRequestIgnoreResponse(byte[] request)
        {
            ThrowIfDisposed();

            if (IsConnected == false)
            {
                throw new InvalidOperationException("Serial port is not connected.");
            }


            for (int attempt = 0; attempt < _settings.RetryCount; attempt++)
            {
                try
                {
                    _serialPort.Write(request, 0, request.Length);

                    var responsePDU = ReceiveResponse();

                    var fcAscii = new byte[2];
                    Array.Copy(request, 3, fcAscii, 0, fcAscii.Length);

                    ValidateFC(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);
                    return;
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    Thread.Sleep(_settings.RetryDelayMs);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.RetryCount + 1} attempts");
        }

        private byte[] ReceiveResponse()
        {
            // Wait for start char ':' (blocks until found or ReadTimeout triggers)
            int bInt;
            do
            {
                bInt = _serialPort.ReadByte();
                if (bInt < 0) throw new IOException("Serial port closed or no data available.");
            } while ((byte)bInt != (byte)':');

            // Read until CRLF ("\r\n")
            var payload = new List<byte>(); // will hold ASCII hex bytes (e.g. '0','1','A','F', ...)
            bool sawCr = false;
            while (true)
            {
                bInt = _serialPort.ReadByte();
                if (bInt < 0) throw new IOException("Serial port closed or no data available while reading payload.");

                byte b = (byte)bInt;

                if (!sawCr)
                {
                    if (b == (byte)'\r')
                    {
                        // possible start of CRLF - don't add to payload, set flag and continue to check next byte
                        sawCr = true;
                        continue;
                    }
                    else
                    {
                        payload.Add(b);
                    }
                }
                else
                {
                    // previously saw '\r', now expect '\n'
                    if (b == (byte)'\n')
                    {
                        break; // finished reading payload
                    }
                    else
                    {
                        // false alarm: previous '\r' was actually data (rare), so add the '\r' and this byte as normal
                        payload.Add((byte)'\r');
                        // if current byte is '\r' again, keep sawCr true, otherwise reset and add current byte
                        if (b == (byte)'\r')
                        {
                            sawCr = true; // consecutive CRs; stay in CR-check mode
                        }
                        else
                        {
                            payload.Add(b);
                            sawCr = false;
                        }
                    }
                }
            }

            if (payload.Count == 0)
                throw new FormatException("Empty Modbus ASCII payload received.");

            // payload now contains ASCII characters representing hex bytes, e.g. "010300100002FB" as bytes
            // Validate even length
            if ((payload.Count & 1) != 0)
                throw new FormatException("Invalid ASCII payload length (must be even number of hex chars).");

            // Decode ASCII hex -> raw bytes using your AsciiUtility.DecodeHex
            var decoded = AsciiUtility.FromAsciiBytes(payload.ToArray());

            if (decoded.Length < 1)
                throw new FormatException("Decoded Modbus frame too short.");

            // Last byte is LRC
            var lrc = decoded[decoded.Length - 1];

            // Extract message without LRC
            var message = new byte[decoded.Length - 1];
            Array.Copy(decoded, 0, message, 0, message.Length);

            ChecksumsMatch(message, [lrc]);

            var pdu = new byte[message.Length - 1];
            Array.Copy(message, 1, pdu, 0, pdu.Length);

            // Return the PDU (function code + data), without LRC and address
            return pdu;
        }

        #region Async IO
        // ---------- Async public methods ----------

        /// <summary>
        /// Send request, do not wait for response (only waits for the write to complete).
        /// </summary>
        public override async Task SendRequestIgnoreResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (!IsConnected) throw new InvalidOperationException("Serial port is not connected.");

            for (int attempt = 0; attempt <= _settings.RetryCount; attempt++)
            {
                try
                {
                    await WriteAsync(request, cancellationToken).ConfigureAwait(false);
                    return; // we intentionally don't wait for a response
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
                catch (IOException) when (attempt < _settings.RetryCount)
                {
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
            }

            throw new TimeoutException($"Write failed after {_settings.RetryCount + 1} attempts");
        }

        /// <summary>
        /// Send request and await response PDU (async, with timeout).
        /// </summary>
        public override async Task<ModbusResponse> SendRequestReceiveResponseAsync(byte[] request, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (!IsConnected) throw new InvalidOperationException("Serial port is not connected.");

            for (int attempt = 0; attempt <= _settings.RetryCount; attempt++)
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                linkedCts.CancelAfter(_settings.WriteTimeout);

                try
                {
                    await WriteAsync(request, linkedCts.Token).ConfigureAwait(false);
                    var responsePDU = await ReceiveResponseAsync(linkedCts.Token).ConfigureAwait(false);

                    // extract function code from request (you used bytes 3..4 previously)
                    var fcAscii = new byte[2];
                    Array.Copy(request, 3, fcAscii, 0, fcAscii.Length);
                    ValidateFC(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);
                    ValidateByteCount(responsePDU, AsciiUtility.FromAsciiBytes(fcAscii)[0]);

                    return BuildResponse(responsePDU);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // user cancellation - rethrow as is
                    throw;
                }
                catch (OperationCanceledException) when (attempt < _settings.RetryCount)
                {
                    // timeout for this attempt - retry after delay
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
                catch (TimeoutException) when (attempt < _settings.RetryCount)
                {
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
                catch (IOException) when (attempt < _settings.RetryCount)
                {
                    await Task.Delay(_settings.RetryDelayMs, cancellationToken).ConfigureAwait(false);
                }
            }

            throw new TimeoutException($"Request failed after {_settings.RetryCount + 1} attempts");
        }


        // ---------- Async private methods ----------

        private async Task WriteAsync(byte[] buffer, CancellationToken ct)
        {
            // ensure only one writer at a time
            await _writeLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                // Prefer BaseStream async if available
                if (_serialPort.BaseStream.CanWrite)
                {
                    await _serialPort.BaseStream.WriteAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false);
                    // Do not rely on FlushAsync for SerialPort.BaseStream
                }
                else
                {
                    // Fallback: blocking write on threadpool
                    await Task.Run(() => _serialPort.Write(buffer, 0, buffer.Length), ct).ConfigureAwait(false);
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>
        /// Async version of your ReceiveResponse: reads an ASCII Modbus frame:
        /// waits for ':' then reads payload until CRLF, decodes, validates, and returns PDU (function + data).
        /// </summary>
        private async Task<byte[]> ReceiveResponseAsync(CancellationToken ct)
        {
            // Use a small buffer for reads
            var readBuffer = new byte[256];
            var asciiPayload = new List<byte>(); // ASCII hex chars between ':' and CRLF
            bool foundStart = false;
            bool sawCr = false;

            Stream baseStream = null;
            try { baseStream = _serialPort?.BaseStream; } catch { baseStream = null; }

            // Helper to read one byte (async if possible, else via Task.Run)
            async Task<int> ReadOneAsync()
            {
                ct.ThrowIfCancellationRequested();

                if (baseStream != null && baseStream.CanRead)
                {
                    var one = new byte[1];
                    int rn = await baseStream.ReadAsync(one, 0, 1, ct).ConfigureAwait(false);
                    if (rn == 0) return -1;
                    return one[0];
                }
                else
                {
                    // Fallback to blocking ReadByte on threadpool
                    return await Task.Run(() =>
                    {
                        try
                        {
                            return _serialPort.ReadByte();
                        }
                        catch (TimeoutException) { throw; }
                        catch (IOException) { throw; }
                    }, ct).ConfigureAwait(false);
                }
            }

            // First find ':' start delimiter
            while (!foundStart)
            {
                int r = await ReadOneAsync().ConfigureAwait(false);
                if (r < 0) throw new IOException("Serial port closed or no data available.");
                if ((byte)r == (byte)':')
                {
                    foundStart = true;
                    break;
                }
            }

            // Read payload until CRLF
            while (true)
            {
                int r = await ReadOneAsync().ConfigureAwait(false);
                if (r < 0) throw new IOException("Serial port closed or no data available while reading payload.");
                byte b = (byte)r;

                if (!sawCr)
                {
                    if (b == (byte)'\r')
                    {
                        sawCr = true;
                        continue;
                    }
                    else
                    {
                        asciiPayload.Add(b);
                    }
                }
                else
                {
                    if (b == (byte)'\n')
                    {
                        break; // done
                    }
                    else
                    {
                        // false alarm - previous '\r' was data
                        asciiPayload.Add((byte)'\r');
                        if (b == (byte)'\r')
                        {
                            // consecutive CRs - keep sawCr true (treat as possible start of CRLF)
                            sawCr = true;
                        }
                        else
                        {
                            asciiPayload.Add(b);
                            sawCr = false;
                        }
                    }
                }
            }

            if (asciiPayload.Count == 0)
                throw new FormatException("Empty Modbus ASCII payload received.");

            if ((asciiPayload.Count & 1) != 0)
                throw new FormatException("Invalid ASCII payload length (must be even number of hex chars).");

            // decode ASCII hex to bytes (e.g. "010300100002FB" => raw bytes)
            var decoded = AsciiUtility.FromAsciiBytes(asciiPayload.ToArray());

            if (decoded.Length < 1)
                throw new FormatException("Decoded Modbus frame too short.");

            // last decoded byte is LRC
            var lrc = decoded[decoded.Length - 1];
            var message = new byte[decoded.Length - 1];
            Array.Copy(decoded, 0, message, 0, message.Length);

            // Validate LRC
            ChecksumsMatch(message, new byte[] { lrc });

            // message layout: [address][function][...data...]
            if (message.Length < 2)
                throw new FormatException("Decoded Modbus frame is too short for address + function.");

            var pdu = new byte[message.Length - 1];
            Array.Copy(message, 1, pdu, 0, pdu.Length);
            return pdu;
        }

        #endregion


        public override byte[] BuildRequest(byte slaveAddress, byte[] pdu)
        {
            return AsciiUtility.BuildAsciiFrame(
                slaveAddress,
                pdu,
                StartDelimiterAsciiArray,
                EndDelimiterAsciiArray
                );
        }

        private void ChecksumsMatch(byte[] rawMessage, byte[] ErrorCheckBytes)
        {
            if (!ErrorCheckUtility.ValidateLrc(rawMessage, ErrorCheckBytes[0]))
            {
                throw new ModbusExceptionLRC();
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _serialPort?.Dispose();
                _disposed = true;
            }
        }
    }
}