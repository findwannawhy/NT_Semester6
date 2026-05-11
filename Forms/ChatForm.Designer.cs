namespace KR.Forms
{
    partial class ChatForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            // ── Declare controls ──────────────────────────────────────────────
            this.menuStrip1       = new System.Windows.Forms.MenuStrip();
            this.menuItemHelp     = new System.Windows.Forms.ToolStripMenuItem();

            this.panelHeader      = new System.Windows.Forms.Panel();
            this.lblPortInfo      = new System.Windows.Forms.Label();
            this.lblStatus        = new System.Windows.Forms.Label();
            this.btnConnect       = new System.Windows.Forms.Button();
            this.btnDisconnect    = new System.Windows.Forms.Button();

            this.panelMain        = new System.Windows.Forms.Panel();

            this.grpSender        = new System.Windows.Forms.GroupBox();
            this.lblFilePath      = new System.Windows.Forms.Label();
            this.txtFilePath      = new System.Windows.Forms.TextBox();
            this.btnBrowse        = new System.Windows.Forms.Button();
            this.btnSendFile      = new System.Windows.Forms.Button();
            this.lblErrorMode     = new System.Windows.Forms.Label();
            this.cmbErrorMode     = new System.Windows.Forms.ComboBox();
            this.progressBar      = new System.Windows.Forms.ProgressBar();
            this.lblProgress      = new System.Windows.Forms.Label();

            this.grpReceiver      = new System.Windows.Forms.GroupBox();
            this.lblReceivedFile  = new System.Windows.Forms.Label();
            this.txtReceivedFileName = new System.Windows.Forms.TextBox();
            this.btnSaveFile      = new System.Windows.Forms.Button();
            this.progressBarRecv  = new System.Windows.Forms.ProgressBar();
            this.lblProgressRecv  = new System.Windows.Forms.Label();
            this.lblRecvStatus    = new System.Windows.Forms.Label();

            this.grpLog           = new System.Windows.Forms.GroupBox();
            this.rtbLog           = new System.Windows.Forms.RichTextBox();

            this.openFileDialog1  = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1  = new System.Windows.Forms.SaveFileDialog();

            this.menuStrip1.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.grpSender.SuspendLayout();
            this.grpReceiver.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.SuspendLayout();

            // ── Color palette ─────────────────────────────────────────────────
            var bg0 = System.Drawing.Color.FromArgb(15,  17,  23);
            var bg1 = System.Drawing.Color.FromArgb(22,  27,  46);
            var bg2 = System.Drawing.Color.FromArgb(30,  37,  64);
            var acc = System.Drawing.Color.FromArgb(67,  97, 238);
            var grn = System.Drawing.Color.FromArgb(76, 201, 160);
            var red = System.Drawing.Color.FromArgb(224, 82,  82);
            var txt = System.Drawing.Color.FromArgb(224, 230, 240);
            var sub = System.Drawing.Color.FromArgb(136, 153, 187);

            var fontUI   = new System.Drawing.Font("Segoe UI",          9.5F);
            var fontSemi = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            var fontBold = new System.Drawing.Font("Segoe UI",          9.5F, System.Drawing.FontStyle.Bold);
            var fontMono = new System.Drawing.Font("Consolas",          9F);

            // ── menuStrip1 ────────────────────────────────────────────────────
            this.menuStrip1.BackColor = bg0;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.menuItemHelp });
            this.menuStrip1.Location  = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name      = "menuStrip1";
            this.menuStrip1.Size      = new System.Drawing.Size(960, 24);
            this.menuStrip1.Padding   = new System.Windows.Forms.Padding(4, 0, 0, 0);

            this.menuItemHelp.ForeColor = sub;
            this.menuItemHelp.Font      = fontUI;
            this.menuItemHelp.Name      = "menuItemHelp";
            this.menuItemHelp.Text      = "Справка";
            this.menuItemHelp.Click    += new System.EventHandler(this.MenuItemHelp_Click);

            // ── panelHeader ───────────────────────────────────────────────────
            this.panelHeader.BackColor = bg1;
            this.panelHeader.Location  = new System.Drawing.Point(0, 24);
            this.panelHeader.Size      = new System.Drawing.Size(960, 56);
            this.panelHeader.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblPortInfo, this.lblStatus, this.btnConnect, this.btnDisconnect });

            this.lblPortInfo.AutoSize  = false;
            this.lblPortInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblPortInfo.Font      = fontUI;
            this.lblPortInfo.ForeColor = sub;
            this.lblPortInfo.Location  = new System.Drawing.Point(12, 19);
            this.lblPortInfo.Size      = new System.Drawing.Size(500, 18);
            this.lblPortInfo.Text      = "Порт: —";

            this.lblStatus.AutoSize  = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(40, 224, 82, 82);
            this.lblStatus.Font      = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = red;
            this.lblStatus.Location  = new System.Drawing.Point(530, 17);
            this.lblStatus.Size      = new System.Drawing.Size(130, 22);
            this.lblStatus.Text      = "● Не подключено";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.btnConnect.BackColor = acc;
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.FlatAppearance.BorderSize = 0;
            this.btnConnect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(86, 116, 255);
            this.btnConnect.Font      = fontSemi;
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location  = new System.Drawing.Point(760, 12);
            this.btnConnect.Size      = new System.Drawing.Size(90, 32);
            this.btnConnect.Text      = "Подключить";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnConnect.Click    += new System.EventHandler(this.BtnConnect_Click);

            this.btnDisconnect.BackColor = red;
            this.btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisconnect.FlatAppearance.BorderSize = 0;
            this.btnDisconnect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(240, 100, 100);
            this.btnDisconnect.Font      = fontSemi;
            this.btnDisconnect.ForeColor = System.Drawing.Color.White;
            this.btnDisconnect.Location  = new System.Drawing.Point(858, 12);
            this.btnDisconnect.Size      = new System.Drawing.Size(90, 32);
            this.btnDisconnect.Text      = "Отключить";
            this.btnDisconnect.Enabled   = false;
            this.btnDisconnect.UseVisualStyleBackColor = false;
            this.btnDisconnect.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnDisconnect.Click    += new System.EventHandler(this.BtnDisconnect_Click);

            // ── panelMain ─────────────────────────────────────────────────────
            this.panelMain.BackColor = bg0;
            this.panelMain.Location  = new System.Drawing.Point(0, 80);
            this.panelMain.Size      = new System.Drawing.Size(960, 560);
            this.panelMain.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.grpSender, this.grpReceiver, this.grpLog });

            // ── Helper: style a GroupBox ──────────────────────────────────────
            void StyleGroup(System.Windows.Forms.GroupBox g, string title,
                            int x, int y, int w, int h)
            {
                g.BackColor = bg1;
                g.ForeColor = sub;
                g.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
                g.Location  = new System.Drawing.Point(x, y);
                g.Size      = new System.Drawing.Size(w, h);
                g.Text      = title;
                g.Padding   = new System.Windows.Forms.Padding(8);
            }

            // ── grpSender  (left column, always visible) ──────────────────────
            StyleGroup(this.grpSender, "Отправка файла", 8, 8, 460, 176);
            this.grpSender.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblFilePath, this.txtFilePath, this.btnBrowse, this.btnSendFile,
                this.lblErrorMode, this.cmbErrorMode,
                this.progressBar, this.lblProgress });

            this.lblFilePath.AutoSize  = false;
            this.lblFilePath.BackColor = System.Drawing.Color.Transparent;
            this.lblFilePath.Font      = fontUI;
            this.lblFilePath.ForeColor = sub;
            this.lblFilePath.Location  = new System.Drawing.Point(10, 26);
            this.lblFilePath.Size      = new System.Drawing.Size(44, 22);
            this.lblFilePath.Text      = "Файл:";
            this.lblFilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtFilePath.Font        = fontMono;
            this.txtFilePath.Location    = new System.Drawing.Point(58, 26);
            this.txtFilePath.Size        = new System.Drawing.Size(280, 22);
            this.txtFilePath.ReadOnly    = true;
            this.txtFilePath.BackColor   = bg2;
            this.txtFilePath.ForeColor   = txt;
            this.txtFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.btnBrowse.BackColor = bg2;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(60, 70, 110);
            this.btnBrowse.FlatAppearance.BorderSize  = 1;
            this.btnBrowse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50, 60, 100);
            this.btnBrowse.Font      = fontUI;
            this.btnBrowse.ForeColor = txt;
            this.btnBrowse.Location  = new System.Drawing.Point(346, 25);
            this.btnBrowse.Size      = new System.Drawing.Size(100, 24);
            this.btnBrowse.Text      = "Обзор...";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnBrowse.Click    += new System.EventHandler(this.BtnBrowse_Click);

            this.btnSendFile.BackColor = grn;
            this.btnSendFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendFile.FlatAppearance.BorderSize = 0;
            this.btnSendFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(95, 220, 180);
            this.btnSendFile.Font      = fontSemi;
            this.btnSendFile.ForeColor = System.Drawing.Color.White;
            this.btnSendFile.Location  = new System.Drawing.Point(10, 58);
            this.btnSendFile.Size      = new System.Drawing.Size(436, 34);
            this.btnSendFile.Text      = "▶  Отправить файл";
            this.btnSendFile.Enabled   = false;
            this.btnSendFile.UseVisualStyleBackColor = false;
            this.btnSendFile.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnSendFile.Click    += new System.EventHandler(this.BtnSendFile_Click);

            this.lblErrorMode.AutoSize  = false;
            this.lblErrorMode.BackColor = System.Drawing.Color.Transparent;
            this.lblErrorMode.Font      = fontUI;
            this.lblErrorMode.ForeColor = sub;
            this.lblErrorMode.Location  = new System.Drawing.Point(10, 102);
            this.lblErrorMode.Size      = new System.Drawing.Size(110, 22);
            this.lblErrorMode.Text      = "Режим ошибок:";
            this.lblErrorMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.cmbErrorMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbErrorMode.Font          = fontUI;
            this.cmbErrorMode.BackColor     = bg2;
            this.cmbErrorMode.ForeColor     = txt;
            this.cmbErrorMode.Location      = new System.Drawing.Point(124, 102);
            this.cmbErrorMode.Size          = new System.Drawing.Size(322, 22);
            this.cmbErrorMode.Items.AddRange(new object[] {
                "Нет ошибок (нормальная передача)",
                "1 бит — Хэмминг исправит автоматически",
                "2 бита — ошибка не исправима, RET_CHUNK" });
            this.cmbErrorMode.SelectedIndex = 0;
            this.cmbErrorMode.SelectedIndexChanged += new System.EventHandler(this.CmbErrorMode_SelectedIndexChanged);

            this.progressBar.Location = new System.Drawing.Point(10, 138);
            this.progressBar.Size     = new System.Drawing.Size(380, 14);
            this.progressBar.Minimum  = 0;
            this.progressBar.Maximum  = 100;
            this.progressBar.Value    = 0;
            this.progressBar.Style    = System.Windows.Forms.ProgressBarStyle.Continuous;

            this.lblProgress.AutoSize  = false;
            this.lblProgress.BackColor = System.Drawing.Color.Transparent;
            this.lblProgress.Font      = fontUI;
            this.lblProgress.ForeColor = grn;
            this.lblProgress.Location  = new System.Drawing.Point(396, 136);
            this.lblProgress.Size      = new System.Drawing.Size(50, 18);
            this.lblProgress.Text      = "0 %";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── grpReceiver (right column, always visible) ────────────────────
            StyleGroup(this.grpReceiver, "Приём файла", 476, 8, 476, 176);
            this.grpReceiver.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblReceivedFile, this.txtReceivedFileName, this.btnSaveFile,
                this.progressBarRecv, this.lblProgressRecv, this.lblRecvStatus });

            this.lblReceivedFile.AutoSize  = false;
            this.lblReceivedFile.BackColor = System.Drawing.Color.Transparent;
            this.lblReceivedFile.Font      = fontUI;
            this.lblReceivedFile.ForeColor = sub;
            this.lblReceivedFile.Location  = new System.Drawing.Point(10, 26);
            this.lblReceivedFile.Size      = new System.Drawing.Size(100, 22);
            this.lblReceivedFile.Text      = "Принятый файл:";
            this.lblReceivedFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtReceivedFileName.Font        = fontMono;
            this.txtReceivedFileName.Location    = new System.Drawing.Point(114, 26);
            this.txtReceivedFileName.Size        = new System.Drawing.Size(224, 22);
            this.txtReceivedFileName.ReadOnly    = true;
            this.txtReceivedFileName.BackColor   = bg2;
            this.txtReceivedFileName.ForeColor   = txt;
            this.txtReceivedFileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.btnSaveFile.BackColor = System.Drawing.Color.FromArgb(67, 97, 238);
            this.btnSaveFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveFile.FlatAppearance.BorderSize = 0;
            this.btnSaveFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(86, 116, 255);
            this.btnSaveFile.Font      = fontUI;
            this.btnSaveFile.ForeColor = System.Drawing.Color.White;
            this.btnSaveFile.Location  = new System.Drawing.Point(346, 25);
            this.btnSaveFile.Size      = new System.Drawing.Size(116, 24);
            this.btnSaveFile.Text      = "Сохранить";
            this.btnSaveFile.Enabled   = false;
            this.btnSaveFile.UseVisualStyleBackColor = false;
            this.btnSaveFile.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnSaveFile.Click    += new System.EventHandler(this.BtnSaveFile_Click);

            this.progressBarRecv.Location = new System.Drawing.Point(10, 60);
            this.progressBarRecv.Size     = new System.Drawing.Size(380, 14);
            this.progressBarRecv.Minimum  = 0;
            this.progressBarRecv.Maximum  = 100;
            this.progressBarRecv.Value    = 0;
            this.progressBarRecv.Style    = System.Windows.Forms.ProgressBarStyle.Continuous;

            this.lblProgressRecv.AutoSize  = false;
            this.lblProgressRecv.BackColor = System.Drawing.Color.Transparent;
            this.lblProgressRecv.Font      = fontUI;
            this.lblProgressRecv.ForeColor = grn;
            this.lblProgressRecv.Location  = new System.Drawing.Point(396, 58);
            this.lblProgressRecv.Size      = new System.Drawing.Size(50, 18);
            this.lblProgressRecv.Text      = "0 %";
            this.lblProgressRecv.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblRecvStatus.AutoSize  = false;
            this.lblRecvStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblRecvStatus.Font      = fontUI;
            this.lblRecvStatus.ForeColor = sub;
            this.lblRecvStatus.Location  = new System.Drawing.Point(10, 86);
            this.lblRecvStatus.Size      = new System.Drawing.Size(452, 72);
            this.lblRecvStatus.Text      = "Ожидание входящей передачи...";
            this.lblRecvStatus.TextAlign = System.Drawing.ContentAlignment.TopLeft;

            // ── grpLog ────────────────────────────────────────────────────────
            StyleGroup(this.grpLog, "Служебный журнал  (кадры · события · ошибки)", 8, 192, 944, 360);
            this.grpLog.Controls.Add(this.rtbLog);

            this.rtbLog.BackColor   = bg0;
            this.rtbLog.ForeColor   = sub;
            this.rtbLog.Font        = fontMono;
            this.rtbLog.Location    = new System.Drawing.Point(6, 18);
            this.rtbLog.Size        = new System.Drawing.Size(930, 334);
            this.rtbLog.ReadOnly    = true;
            this.rtbLog.ScrollBars  = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;

            // ── Dialogs ───────────────────────────────────────────────────────
            this.openFileDialog1.Filter = "Все файлы (*.*)|*.*";
            this.openFileDialog1.Title  = "Выберите файл для отправки";

            this.saveFileDialog1.Filter = "Все файлы (*.*)|*.*";
            this.saveFileDialog1.Title  = "Сохранить принятый файл";

            // ── ChatForm ──────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = bg0;
            this.ClientSize          = new System.Drawing.Size(960, 640);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip       = this.menuStrip1;
            this.MaximizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "Двунаправленная передача файлов по RS232C — Вариант 5";
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelMain);
            this.Load += new System.EventHandler(this.ChatForm_Load);

            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.grpSender.ResumeLayout(false);
            this.grpReceiver.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // ── Controls ──────────────────────────────────────────────────────────
        private System.Windows.Forms.MenuStrip         menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuItemHelp;

        private System.Windows.Forms.Panel  panelHeader;
        private System.Windows.Forms.Panel  panelMain;
        private System.Windows.Forms.Label  lblPortInfo;
        private System.Windows.Forms.Label  lblStatus;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;

        private System.Windows.Forms.GroupBox  grpSender;
        private System.Windows.Forms.Label     lblFilePath;
        private System.Windows.Forms.TextBox   txtFilePath;
        private System.Windows.Forms.Button    btnBrowse;
        private System.Windows.Forms.Button    btnSendFile;
        private System.Windows.Forms.Label     lblErrorMode;
        private System.Windows.Forms.ComboBox  cmbErrorMode;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label     lblProgress;

        private System.Windows.Forms.GroupBox  grpReceiver;
        private System.Windows.Forms.Label     lblReceivedFile;
        private System.Windows.Forms.TextBox   txtReceivedFileName;
        private System.Windows.Forms.Button    btnSaveFile;
        private System.Windows.Forms.ProgressBar progressBarRecv;
        private System.Windows.Forms.Label     lblProgressRecv;
        private System.Windows.Forms.Label     lblRecvStatus;

        private System.Windows.Forms.GroupBox    grpLog;
        private System.Windows.Forms.RichTextBox rtbLog;

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}
