using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KR
{
    /// <summary>
    /// Variant 5 frame protocol for simultaneous bidirectional file transfer over RS232C.
    ///
    /// Frame structure:
    ///   [0xFF] [FrameType] [payload bytes...]
    ///
    /// Frame types:
    ///   UPLINK        – connection request (either peer)
    ///   ACK_UPLINK    – connection accepted
    ///   RET_UPLINK    – connection rejected
    ///   DOWNLINK      – disconnect request
    ///   ACK_DOWNLINK  – disconnect acknowledged
    ///
    ///   FILE_INFO     – file metadata: [name_len_2B LE][name UTF-8][total_chunks_4B LE]
    ///   ACK_FILE_INFO – receiver ready
    ///   RET_FILE_INFO – receiver not ready / error
    ///
    ///   CHUNK         – one encoded chunk: [chunk_index_4B LE][data_len_2B LE][hamming-encoded data]
    ///   ACK_CHUNK     – chunk received OK
    ///   RET_CHUNK     – chunk error, please resend
    ///
    ///   TRANSFER_DONE – all chunks sent
    ///   ACK_DONE      – transfer complete acknowledged
    ///
    /// Thread safety: all port writes are protected by _writeLock so that the
    /// DataReceived handler (sending ACK/RET) and the sender thread (sending CHUNKs)
    /// do not interleave bytes on the TX line during simultaneous bidirectional transfer.
    /// </summary>
    internal class Frames
    {
        // ── Frame type byte values ────────────────────────────────────────────
        public enum FrameType : byte
        {
            UPLINK        = 0x01,
            ACK_UPLINK    = 0x02,
            RET_UPLINK    = 0x03,
            DOWNLINK      = 0x04,
            ACK_DOWNLINK  = 0x05,

            FILE_INFO     = 0x10,
            ACK_FILE_INFO = 0x11,
            RET_FILE_INFO = 0x12,

            CHUNK         = 0x20,
            ACK_CHUNK     = 0x21,
            RET_CHUNK     = 0x22,

            TRANSFER_DONE = 0x40,
            ACK_DONE      = 0x41,
        }

        // ── Test error injection mode ─────────────────────────────────────────
        /// <summary>
        /// Controls artificial bit-error injection for demonstration purposes.
        /// None    – normal operation, no errors injected.
        /// OneBit  – one bit is flipped in the Hamming-encoded payload of every
        ///           CHUNK frame; the [7,4] decoder corrects it automatically.
        /// TwoBit  – two bits are flipped in the SAME codeword; the SECDED decoder
        ///           detects but cannot correct the error → sends RET_CHUNK.
        /// </summary>
        public enum ErrorMode { None, OneBit, TwoBit }

        /// <summary>Current test error injection mode (set from UI).</summary>
        public ErrorMode TestErrorMode { get; set; } = ErrorMode.None;

        // ── Events ────────────────────────────────────────────────────────────
        public event Action<string, int>  OnFileInfoReceived;  // (fileName, totalChunks)
        public event Action<int, byte[]>  OnChunkReceived;     // (chunkIndex, rawData)
        public event Action               OnTransferComplete;
        public event Action               OnConnected;
        public event Action               OnDisconnected;

        // ── State ─────────────────────────────────────────────────────────────
        public bool IsConnected { get; private set; }

        // ── UI log box ────────────────────────────────────────────────────────
        public RichTextBox LogBox;

        private readonly Coding _coder = new Coding();

        private const byte START       = 0xFF;
        private const int  MAX_RETRIES = 3;

        // ── Write lock — prevents TX-line byte interleaving in full-duplex mode ─
        // Both the DataReceived callback (sending ACK/RET) and the sender thread
        // (sending CHUNKs) share the same SerialPort.Write path on one PC.
        private readonly object _writeLock = new object();

        // ── Chunk ACK synchronisation (used by SendChunk) ─────────────────────
        private volatile bool _waitingForChunkAck = false;
        private volatile bool _chunkAckReceived   = false;
        private volatile bool _chunkRetReceived   = false;
        private readonly ManualResetEventSlim _chunkAckEvent = new ManualResetEventSlim(false);

        // ── Chunk size (raw bytes before Hamming encoding) ────────────────────
        public const int CHUNK_SIZE = 128;

        // ─────────────────────────────────────────────────────────────────────
        // Incoming frame dispatcher
        // ─────────────────────────────────────────────────────────────────────
        public void FrameAction(byte frameTypeByte, SerialPort port)
        {
            if (!Enum.IsDefined(typeof(FrameType), frameTypeByte))
            {
                Log($"[{DateTime.Now:HH:mm:ss}] Неизвестный кадр 0x{frameTypeByte:X2}", Color.OrangeRed);
                return;
            }

            var ft = (FrameType)frameTypeByte;
            Log($"[{DateTime.Now:HH:mm:ss}] ← {ft}", Color.Gray);

            switch (ft)
            {
                // ── Connection ────────────────────────────────────────────────
                case FrameType.UPLINK:
                    SendControlFrame(FrameType.ACK_UPLINK, port);
                    IsConnected = true;
                    OnConnected?.Invoke();
                    break;

                case FrameType.ACK_UPLINK:
                    IsConnected = true;
                    OnConnected?.Invoke();
                    break;

                case FrameType.RET_UPLINK:
                    IsConnected = false;
                    Log($"[{DateTime.Now:HH:mm:ss}] Соединение отклонено.", Color.OrangeRed);
                    break;

                case FrameType.DOWNLINK:
                    IsConnected = false;
                    SendControlFrame(FrameType.ACK_DOWNLINK, port);
                    OnDisconnected?.Invoke();
                    break;

                case FrameType.ACK_DOWNLINK:
                    IsConnected = false;
                    OnDisconnected?.Invoke();
                    break;

                // ── File info ─────────────────────────────────────────────────
                case FrameType.FILE_INFO:
                {
                    try
                    {
                        var lenBuf    = ReadExact(port, 2);
                        int nameLen   = lenBuf[0] | (lenBuf[1] << 8);
                        var nameBuf   = ReadExact(port, nameLen);
                        var chunksBuf = ReadExact(port, 4);
                        string fileName   = Encoding.UTF8.GetString(nameBuf);
                        int totalChunks   = BitConverter.ToInt32(chunksBuf, 0);

                        Log($"[{DateTime.Now:HH:mm:ss}] FILE_INFO: «{fileName}», {totalChunks} блоков", Color.DodgerBlue);
                        SendControlFrame(FrameType.ACK_FILE_INFO, port);
                        OnFileInfoReceived?.Invoke(fileName, totalChunks);
                    }
                    catch (Exception ex)
                    {
                        Log($"[{DateTime.Now:HH:mm:ss}] FILE_INFO ошибка разбора: {ex.Message}", Color.Crimson);
                        SendControlFrame(FrameType.RET_FILE_INFO, port);
                    }
                    break;
                }

                case FrameType.ACK_FILE_INFO:
                    Log($"[{DateTime.Now:HH:mm:ss}] Приёмник готов к получению файла.", Color.DodgerBlue);
                    break;

                case FrameType.RET_FILE_INFO:
                    Log($"[{DateTime.Now:HH:mm:ss}] Приёмник отклонил FILE_INFO.", Color.Crimson);
                    break;

                // ── Chunk ─────────────────────────────────────────────────────
                case FrameType.CHUNK:
                {
                    try
                    {
                        var idxBuf   = ReadExact(port, 4);
                        var lenBuf   = ReadExact(port, 2);
                        int chunkIdx = BitConverter.ToInt32(idxBuf, 0);
                        int dataLen  = lenBuf[0] | (lenBuf[1] << 8);
                        var encoded  = ReadExact(port, dataLen);

                        var decoded = _coder.DecodeBytes(encoded,
                            out bool hadCorrection, out bool hadUncorrectable);

                        if (hadUncorrectable || decoded == null)
                        {
                            Log($"[{DateTime.Now:HH:mm:ss}] CHUNK #{chunkIdx} — 2-битовая ошибка, исправление невозможно → RET_CHUNK", Color.Crimson);
                            SendControlFrame(FrameType.RET_CHUNK, port);
                            break;
                        }

                        if (hadCorrection)
                            Log($"[{DateTime.Now:HH:mm:ss}] CHUNK #{chunkIdx} — 1-битовая ошибка исправлена кодом Хэмминга ✓", Color.FromArgb(255, 200, 80));
                        else
                            Log($"[{DateTime.Now:HH:mm:ss}] CHUNK #{chunkIdx} OK ({decoded.Length} байт)", Color.DodgerBlue);

                        SendControlFrame(FrameType.ACK_CHUNK, port);
                        OnChunkReceived?.Invoke(chunkIdx, decoded);
                    }
                    catch (Exception ex)
                    {
                        Log($"[{DateTime.Now:HH:mm:ss}] CHUNK ошибка чтения: {ex.Message}", Color.Crimson);
                        SendControlFrame(FrameType.RET_CHUNK, port);
                    }
                    break;
                }

                // ── Chunk ACK/RET — signal waiting SendChunk ──────────────────
                case FrameType.ACK_CHUNK:
                    if (_waitingForChunkAck)
                    {
                        _chunkAckReceived = true;
                        _chunkRetReceived = false;
                        _chunkAckEvent.Set();
                    }
                    break;

                case FrameType.RET_CHUNK:
                    if (_waitingForChunkAck)
                    {
                        _chunkRetReceived = true;
                        _chunkAckReceived = false;
                        _chunkAckEvent.Set();
                    }
                    break;

                // ── Transfer done ─────────────────────────────────────────────
                case FrameType.TRANSFER_DONE:
                    SendControlFrame(FrameType.ACK_DONE, port);
                    OnTransferComplete?.Invoke();
                    Log($"[{DateTime.Now:HH:mm:ss}] Передача завершена.", Color.Green);
                    break;

                case FrameType.ACK_DONE:
                    OnTransferComplete?.Invoke();
                    Log($"[{DateTime.Now:HH:mm:ss}] Передача подтверждена.", Color.Green);
                    break;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Send helpers (all writes are protected by _writeLock)
        // ─────────────────────────────────────────────────────────────────────

        public void SendControlFrame(FrameType ft, SerialPort port)
        {
            EnsureOpen(port);
            lock (_writeLock)
                port.Write(new byte[] { START, (byte)ft }, 0, 2);
            Log($"[{DateTime.Now:HH:mm:ss}] → {ft}", Color.SlateGray);
        }

        public void SendFileInfo(string fileName, int totalChunks, SerialPort port)
        {
            EnsureOpen(port);
            var nameBytes = Encoding.UTF8.GetBytes(fileName);
            var payload = new List<byte> { START, (byte)FrameType.FILE_INFO };
            payload.Add((byte)(nameBytes.Length & 0xFF));
            payload.Add((byte)((nameBytes.Length >> 8) & 0xFF));
            payload.AddRange(nameBytes);
            payload.AddRange(BitConverter.GetBytes(totalChunks));
            lock (_writeLock)
                port.Write(payload.ToArray(), 0, payload.Count);
            Log($"[{DateTime.Now:HH:mm:ss}] → FILE_INFO «{fileName}» ({totalChunks} блоков)", Color.SlateGray);
        }

        /// <summary>
        /// Send a single CHUNK with Hamming encoding.
        /// Waits for ACK_CHUNK (signalled by FrameAction via _chunkAckEvent).
        /// Returns true if ACK received within MAX_RETRIES attempts.
        /// </summary>
        public bool SendChunk(int chunkIndex, byte[] rawData, SerialPort port)
        {
            var encoded = _coder.EncodeBytes(rawData);

            for (int attempt = 0; attempt < MAX_RETRIES; attempt++)
            {
                byte[] toSend = (byte[])encoded.Clone();
                if (attempt == 0 && TestErrorMode != ErrorMode.None && toSend.Length > 0)
                {
                    toSend[0] ^= 0x01;
                    string errDesc = "1-битовая ошибка (бит 0 кодового слова 0)";
                    if (TestErrorMode == ErrorMode.TwoBit)
                    {
                        toSend[0] ^= 0x02;
                        errDesc = "2-битовая ошибка (биты 0+1 кодового слова 0)";
                    }
                    Log($"[{DateTime.Now:HH:mm:ss}] ⚡ ТЕСТ: {errDesc} в CHUNK #{chunkIndex}", Color.Orange);
                }

                _chunkAckEvent.Reset();
                _chunkAckReceived   = false;
                _chunkRetReceived   = false;
                _waitingForChunkAck = true;

                EnsureOpen(port);
                var payload = new List<byte> { START, (byte)FrameType.CHUNK };
                payload.AddRange(BitConverter.GetBytes(chunkIndex));
                payload.Add((byte)(toSend.Length & 0xFF));
                payload.Add((byte)((toSend.Length >> 8) & 0xFF));
                payload.AddRange(toSend);
                lock (_writeLock)
                    port.Write(payload.ToArray(), 0, payload.Count);
                Log($"[{DateTime.Now:HH:mm:ss}] → CHUNK #{chunkIndex} ({rawData.Length} байт, попытка {attempt + 1})", Color.SlateGray);

                bool signalled = _chunkAckEvent.Wait(10000);
                _waitingForChunkAck = false;

                if (signalled && _chunkAckReceived)
                {
                    Log($"[{DateTime.Now:HH:mm:ss}] ← ACK_CHUNK #{chunkIndex}", Color.Gray);
                    return true;
                }

                if (signalled && _chunkRetReceived)
                    Log($"[{DateTime.Now:HH:mm:ss}] ← RET_CHUNK #{chunkIndex}, повтор...", Color.OrangeRed);
                else
                    Log($"[{DateTime.Now:HH:mm:ss}] Таймаут ожидания ACK_CHUNK #{chunkIndex}", Color.OrangeRed);
            }

            Log($"[{DateTime.Now:HH:mm:ss}] CHUNK #{chunkIndex} не доставлен после {MAX_RETRIES} попыток.", Color.Crimson);
            return false;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Utility
        // ─────────────────────────────────────────────────────────────────────

        private static byte[] ReadExact(SerialPort port, int count)
        {
            var buf  = new byte[count];
            int read = 0;
            while (read < count)
            {
                int got = port.Read(buf, read, count - read);
                if (got == 0) throw new IOException("Порт закрыт неожиданно.");
                read += got;
            }
            return buf;
        }

        private static void EnsureOpen(SerialPort port)
        {
            if (!port.IsOpen) port.Open();
        }

        private void Log(string text, Color color)
        {
            if (LogBox == null) return;
            try
            {
                if (LogBox.IsHandleCreated && !LogBox.IsDisposed)
                {
                    if (LogBox.InvokeRequired)
                        LogBox.BeginInvoke(new Action(() => AppendLog(text, color)));
                    else
                        AppendLog(text, color);
                }
            }
            catch { }
        }

        private void AppendLog(string text, Color color)
        {
            try
            {
                LogBox.SelectionStart  = LogBox.TextLength;
                LogBox.SelectionLength = 0;
                LogBox.SelectionColor  = color;
                LogBox.AppendText(text + "\n");
                LogBox.ScrollToCaret();
            }
            catch { }
        }
    }
}
