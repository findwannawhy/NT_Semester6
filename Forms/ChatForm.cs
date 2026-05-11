using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KR.Forms
{
    /// <summary>
    /// Variant 5 — main application form.
    ///
    /// Two equal peers connected via RS232C null-modem cable.
    /// Each PC simultaneously acts as both Sender and Receiver.
    ///
    /// Connection protocol (symmetric):
    ///   Both sides send UPLINK on connect.
    ///   Receiving UPLINK → send ACK_UPLINK, mark connected.
    ///   Receiving ACK_UPLINK → mark connected.
    ///
    /// All data is protected with [7,4] Hamming code (SECDED).
    /// Write-lock in Frames prevents TX-line interleaving during simultaneous transfer.
    /// </summary>
    public partial class ChatForm : Form
    {
        // ── Port & protocol ───────────────────────────────────────────────────
        private PortSetup?  _portSetup;
        private SerialPort? _port;
        private readonly Frames _frames = new Frames();

        // ── Sender state ──────────────────────────────────────────────────────
        private string? _fileToSend;
        private byte[]? _fileData;
        private int     _totalChunks;

        // ── Receiver state ────────────────────────────────────────────────────
        private string?                  _receivedFileName;
        private int                      _totalChunksExpected;
        private int                      _chunksReceived;
        private readonly Dictionary<int, byte[]> _receivedChunks = new Dictionary<int, byte[]>();

        // ── Connection state ──────────────────────────────────────────────────
        private bool _connected;

        // ─────────────────────────────────────────────────────────────────────
        public ChatForm()
        {
            InitializeComponent();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Form load — show COM port settings dialog
        // ─────────────────────────────────────────────────────────────────────
        private void ChatForm_Load(object sender, EventArgs e)
        {
            var settings = new LoginForm();
            if (settings.ShowDialog() != DialogResult.OK)
            {
                Environment.Exit(0);
                return;
            }

            _portSetup = settings.PortConfig!;

            lblPortInfo.Text = $"Порт: {_portSetup.PortName}  |  {_portSetup.SettingsDescription}";

            // Wire up Frames events
            _frames.LogBox             = rtbLog;
            _frames.OnConnected       += HandleConnected;
            _frames.OnDisconnected    += HandleDisconnected;
            _frames.OnFileInfoReceived += HandleFileInfoReceived;
            _frames.OnChunkReceived   += HandleChunkReceived;
            _frames.OnTransferComplete += HandleTransferComplete;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Connection
        // ─────────────────────────────────────────────────────────────────────
        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _port = _portSetup.OpenPort();
                _port.DataReceived  += Port_DataReceived;
                _port.ErrorReceived += Port_ErrorReceived;

                AppendLog($"[{DateTime.Now:HH:mm:ss}] Порт {_portSetup.PortName} открыт. Отправка UPLINK...", Color.Cyan);

                // Both peers send UPLINK — whoever receives it first sends ACK_UPLINK back.
                // Both sides end up connected regardless of who connects first.
                _frames.SendControlFrame(Frames.FrameType.UPLINK, _port);

                btnConnect.Enabled    = false;
                btnDisconnect.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть порт {_portSetup.PortName}:\n{ex.Message}",
                    "Ошибка порта", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (_port != null && _port.IsOpen)
                {
                    _frames.SendControlFrame(Frames.FrameType.DOWNLINK, _port);
                    Thread.Sleep(300);
                    _port.DataReceived  -= Port_DataReceived;
                    _port.ErrorReceived -= Port_ErrorReceived;
                    _port.Close();
                    _port.Dispose();
                    _port = null;
                }
            }
            catch { }

            SetDisconnectedState();
            AppendLog($"[{DateTime.Now:HH:mm:ss}] Соединение закрыто.", Color.OrangeRed);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Serial port events
        // ─────────────────────────────────────────────────────────────────────
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = (SerialPort)sender;
            try
            {
                int b = port.ReadByte();
                if (b == 0xFF)
                {
                    int ft = port.ReadByte();
                    if (ft >= 0)
                        _frames.FrameAction((byte)ft, port);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"[{DateTime.Now:HH:mm:ss}] Ошибка чтения порта: {ex.Message}", Color.Crimson);
            }
        }

        private void Port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            AppendLog($"[{DateTime.Now:HH:mm:ss}] Ошибка порта: {e.EventType}", Color.Crimson);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Frames event handlers
        // ─────────────────────────────────────────────────────────────────────
        private void HandleConnected()
        {
            _connected = true;
            SafeInvoke(() =>
            {
                lblStatus.Text      = "● Подключено";
                lblStatus.ForeColor = Color.FromArgb(76, 201, 160);
                lblStatus.BackColor = Color.FromArgb(20, 76, 201, 160);
                btnSendFile.Enabled = !string.IsNullOrEmpty(_fileToSend);
                AppendLog($"[{DateTime.Now:HH:mm:ss}] Соединение установлено.", Color.FromArgb(76, 201, 160));
            });
        }

        private void HandleDisconnected()
        {
            _connected = false;
            SafeInvoke(SetDisconnectedState);
        }

        private void HandleFileInfoReceived(string fileName, int totalChunks)
        {
            SafeInvoke(() =>
            {
                _receivedFileName    = fileName;
                _totalChunksExpected = totalChunks;
                _chunksReceived      = 0;
                _receivedChunks.Clear();

                txtReceivedFileName.Text = fileName;
                progressBarRecv.Value    = 0;
                lblProgressRecv.Text     = "0 %";
                lblRecvStatus.Text       = $"Принимаю: {fileName} ({totalChunks} блоков)";
                btnSaveFile.Enabled      = false;

                AppendLog($"[{DateTime.Now:HH:mm:ss}] Начало приёма «{fileName}» ({totalChunks} блоков).", Color.Cyan);
            });
        }

        private void HandleChunkReceived(int chunkIndex, byte[] rawData)
        {
            SafeInvoke(() =>
            {
                _receivedChunks[chunkIndex] = rawData;
                _chunksReceived++;

                if (_totalChunksExpected > 0)
                {
                    int pct = (int)(100.0 * _chunksReceived / _totalChunksExpected);
                    progressBarRecv.Value = Math.Min(pct, 100);
                    lblProgressRecv.Text  = $"{pct} %";
                    lblRecvStatus.Text    = $"Принято блоков: {_chunksReceived} / {_totalChunksExpected}";
                }
            });
        }

        private void HandleTransferComplete()
        {
            // ACK_DONE also fires this event on the sender side — ignore it there.
            if (_chunksReceived == 0) return;

            SafeInvoke(() =>
            {
                progressBarRecv.Value = 100;
                lblProgressRecv.Text  = "100 %";
                btnSaveFile.Enabled   = true;
                lblRecvStatus.Text    = $"Получен файл: {_receivedFileName}. Нажмите «Сохранить».";
                AppendLog($"[{DateTime.Now:HH:mm:ss}] Передача завершена. Принято {_chunksReceived} блоков.", Color.LightGreen);
                MessageBox.Show($"Файл «{_receivedFileName}» успешно принят!\nНажмите «Сохранить» для записи на диск.",
                    "Приём завершён", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // Sender controls
        // ─────────────────────────────────────────────────────────────────────
        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _fileToSend      = openFileDialog1.FileName;
                txtFilePath.Text = _fileToSend;
                _fileData        = File.ReadAllBytes(_fileToSend);

                btnSendFile.Enabled = _connected;
                AppendLog($"[{DateTime.Now:HH:mm:ss}] Файл выбран: {Path.GetFileName(_fileToSend)} ({_fileData.Length} байт)", Color.Cyan);
            }
        }

        private void BtnSendFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_fileToSend) || _fileData == null)
            {
                MessageBox.Show("Сначала выберите файл.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!_connected || _port == null || !_port.IsOpen)
            {
                MessageBox.Show("Нет активного соединения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSendFile.Enabled = false;
            btnBrowse.Enabled   = false;

            var thread = new Thread(SendFileThread) { IsBackground = true };
            thread.Start();
        }

        private void SendFileThread()
        {
            try
            {
                string fileName = Path.GetFileName(_fileToSend);
                _totalChunks = (int)Math.Ceiling((double)_fileData.Length / Frames.CHUNK_SIZE);

                SafeInvoke(() =>
                {
                    progressBar.Value = 0;
                    lblProgress.Text  = "0 %";
                    AppendLog($"[{DateTime.Now:HH:mm:ss}] Отправка «{fileName}», {_totalChunks} блоков...", Color.Cyan);
                });

                _frames.SendFileInfo(fileName, _totalChunks, _port);
                Thread.Sleep(500); // wait for ACK_FILE_INFO

                bool transferOk = true;
                for (int i = 0; i < _totalChunks; i++)
                {
                    int offset = i * Frames.CHUNK_SIZE;
                    int len    = Math.Min(Frames.CHUNK_SIZE, _fileData.Length - offset);
                    var chunk  = new byte[len];
                    Array.Copy(_fileData, offset, chunk, 0, len);

                    bool ok = _frames.SendChunk(i, chunk, _port);
                    if (!ok)
                    {
                        int fi = i;
                        SafeInvoke(() =>
                            AppendLog($"[{DateTime.Now:HH:mm:ss}] Блок #{fi} не доставлен. Передача прервана.", Color.Crimson));
                        transferOk = false;
                        break;
                    }

                    int pct = (int)(100.0 * (i + 1) / _totalChunks);
                    SafeInvoke(() =>
                    {
                        progressBar.Value = Math.Min(pct, 100);
                        lblProgress.Text  = $"{pct} %";
                    });
                }

                if (!transferOk)
                {
                    SafeInvoke(() => { btnSendFile.Enabled = true; btnBrowse.Enabled = true; });
                    return;
                }

                _frames.SendControlFrame(Frames.FrameType.TRANSFER_DONE, _port);

                SafeInvoke(() =>
                {
                    progressBar.Value   = 100;
                    lblProgress.Text    = "100 %";
                    btnSendFile.Enabled = true;
                    btnBrowse.Enabled   = true;
                    AppendLog($"[{DateTime.Now:HH:mm:ss}] Файл «{fileName}» отправлен.", Color.LightGreen);
                    MessageBox.Show("Файл успешно отправлен!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
            }
            catch (Exception ex)
            {
                SafeInvoke(() =>
                {
                    btnSendFile.Enabled = true;
                    btnBrowse.Enabled   = true;
                    AppendLog($"[{DateTime.Now:HH:mm:ss}] Ошибка передачи: {ex.Message}", Color.Crimson);
                    MessageBox.Show($"Ошибка передачи:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        private void CmbErrorMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            _frames.TestErrorMode = cmbErrorMode.SelectedIndex switch
            {
                1 => Frames.ErrorMode.OneBit,
                2 => Frames.ErrorMode.TwoBit,
                _ => Frames.ErrorMode.None,
            };
            string desc = cmbErrorMode.SelectedIndex switch
            {
                1 => "⚡ Режим: 1-битовая ошибка (Хэмминг исправит)",
                2 => "💥 Режим: 2-битовая ошибка (RET_CHUNK, повтор)",
                _ => "✅ Режим: без ошибок",
            };
            AppendLog($"[{DateTime.Now:HH:mm:ss}] {desc}", Color.FromArgb(255, 200, 80));
        }

        // ─────────────────────────────────────────────────────────────────────
        // Receiver controls
        // ─────────────────────────────────────────────────────────────────────
        private void BtnSaveFile_Click(object sender, EventArgs e)
        {
            if (_receivedChunks.Count == 0)
            {
                MessageBox.Show("Нет принятых данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            saveFileDialog1.FileName = _receivedFileName ?? "received";
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            try
            {
                var allData = new List<byte>();
                for (int i = 0; i < _totalChunksExpected; i++)
                {
                    if (_receivedChunks.TryGetValue(i, out byte[] chunk))
                        allData.AddRange(chunk);
                }

                File.WriteAllBytes(saveFileDialog1.FileName, allData.ToArray());
                AppendLog($"[{DateTime.Now:HH:mm:ss}] Файл сохранён: {saveFileDialog1.FileName}", Color.LightGreen);
                MessageBox.Show($"Файл сохранён:\n{saveFileDialog1.FileName}", "Сохранено",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения:\n{ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Menu
        // ─────────────────────────────────────────────────────────────────────
        private void MenuItemHelp_Click(object sender, EventArgs e)
        {
            new CreatorsForm().Show();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────
        private void SetDisconnectedState()
        {
            _connected            = false;
            lblStatus.Text        = "● Не подключено";
            lblStatus.ForeColor   = Color.FromArgb(224, 82, 82);
            lblStatus.BackColor   = Color.FromArgb(40, 224, 82, 82);
            btnConnect.Enabled    = true;
            btnDisconnect.Enabled = false;
            btnSendFile.Enabled   = false;
        }

        private void AppendLog(string text, Color color)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action(() => AppendLog(text, color)));
                return;
            }
            rtbLog.SelectionStart  = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor  = color;
            rtbLog.AppendText(text + "\n");
            rtbLog.ScrollToCaret();
        }

        private void SafeInvoke(Action action)
        {
            if (IsDisposed) return;
            if (InvokeRequired) Invoke(action);
            else action();
        }
    }
}
