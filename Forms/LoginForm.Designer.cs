namespace KR.Forms
{
    partial class LoginForm
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
            this.panelTop     = new System.Windows.Forms.Panel();
            this.lblTitle     = new System.Windows.Forms.Label();
            this.lblSubtitle  = new System.Windows.Forms.Label();
            this.panelBody    = new System.Windows.Forms.Panel();
            this.lblPort      = new System.Windows.Forms.Label();
            this.cmbPort      = new System.Windows.Forms.ComboBox();
            this.lblBaud      = new System.Windows.Forms.Label();
            this.cmbBaud      = new System.Windows.Forms.ComboBox();
            this.lblDataBits  = new System.Windows.Forms.Label();
            this.cmbDataBits  = new System.Windows.Forms.ComboBox();
            this.lblParity    = new System.Windows.Forms.Label();
            this.cmbParity    = new System.Windows.Forms.ComboBox();
            this.lblStopBits  = new System.Windows.Forms.Label();
            this.cmbStopBits  = new System.Windows.Forms.ComboBox();
            this.btnOk        = new System.Windows.Forms.Button();
            this.btnExit      = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.panelBody.SuspendLayout();
            this.SuspendLayout();

            var bg1  = System.Drawing.Color.FromArgb(22,  27,  46);
            var bg2  = System.Drawing.Color.FromArgb(30,  37,  64);
            var acc  = System.Drawing.Color.FromArgb(67,  97, 238);
            var grn  = System.Drawing.Color.FromArgb(76, 201, 160);
            var txt  = System.Drawing.Color.FromArgb(224, 230, 240);
            var sub  = System.Drawing.Color.FromArgb(136, 153, 187);
            var fontUI   = new System.Drawing.Font("Segoe UI",          9.5F);
            var fontSemi = new System.Drawing.Font("Segoe UI Semibold", 9.5F);
            var fontBold = new System.Drawing.Font("Segoe UI",         14F, System.Drawing.FontStyle.Bold);

            // ── panelTop ─────────────────────────────────────────────────────
            this.panelTop.BackColor = acc;
            this.panelTop.Dock      = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height    = 80;
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Controls.Add(this.lblSubtitle);

            this.lblTitle.AutoSize  = false;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font      = fontBold;
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location  = new System.Drawing.Point(0, 10);
            this.lblTitle.Size      = new System.Drawing.Size(420, 30);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Text      = "Двунаправленная передача файлов";

            this.lblSubtitle.AutoSize  = false;
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Font      = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(200, 220, 255);
            this.lblSubtitle.Location  = new System.Drawing.Point(0, 44);
            this.lblSubtitle.Size      = new System.Drawing.Size(420, 22);
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSubtitle.Text      = "Вариант 5  ·  [7,4]-код Хэмминга  ·  RS232C нуль-модем";

            // ── panelBody ─────────────────────────────────────────────────────
            this.panelBody.BackColor = bg1;
            this.panelBody.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.panelBody.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblPort, this.cmbPort,
                this.lblBaud, this.cmbBaud,
                this.lblDataBits, this.cmbDataBits,
                this.lblParity, this.cmbParity,
                this.lblStopBits, this.cmbStopBits,
                this.btnOk, this.btnExit });

            // ── Helper: label ─────────────────────────────────────────────────
            void StyleLabel(System.Windows.Forms.Label l, string text, int x, int y)
            {
                l.AutoSize  = false;
                l.BackColor = System.Drawing.Color.Transparent;
                l.Font      = fontUI;
                l.ForeColor = sub;
                l.Location  = new System.Drawing.Point(x, y);
                l.Size      = new System.Drawing.Size(110, 24);
                l.Text      = text;
                l.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            }

            void StyleCombo(System.Windows.Forms.ComboBox c, int x, int y, int w = 220)
            {
                c.BackColor     = bg2;
                c.ForeColor     = txt;
                c.Font          = fontUI;
                c.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                c.Location      = new System.Drawing.Point(x, y);
                c.Size          = new System.Drawing.Size(w, 24);
                c.FlatStyle     = System.Windows.Forms.FlatStyle.Flat;
            }

            int lx = 16, cx = 134, row0 = 14, step = 34;

            StyleLabel(this.lblPort,     "COM-порт:", lx, row0);
            this.cmbPort.BackColor     = bg2;
            this.cmbPort.ForeColor     = txt;
            this.cmbPort.Font          = fontUI;
            this.cmbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort.Location      = new System.Drawing.Point(cx, row0);
            this.cmbPort.Size          = new System.Drawing.Size(120, 24);
            this.cmbPort.FlatStyle     = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPort.Items.AddRange(new object[] {
                "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9","COM10" });
            this.cmbPort.SelectedIndex = 0;

            StyleLabel(this.lblBaud, "Скорость:", lx, row0 + step);
            StyleCombo(this.cmbBaud, cx, row0 + step, 140);
            this.cmbBaud.Items.AddRange(new object[] {
                "1200","2400","4800","9600","14400","19200","38400","57600","115200" });
            this.cmbBaud.SelectedIndex = 3; // 9600

            StyleLabel(this.lblDataBits, "Биты данных:", lx, row0 + step * 2);
            StyleCombo(this.cmbDataBits, cx, row0 + step * 2, 80);
            this.cmbDataBits.Items.AddRange(new object[] { "5","6","7","8" });
            this.cmbDataBits.SelectedIndex = 3; // 8

            StyleLabel(this.lblParity, "Чётность:", lx, row0 + step * 3);
            StyleCombo(this.cmbParity, cx, row0 + step * 3, 150);
            this.cmbParity.Items.AddRange(new object[] {
                "None (нет)", "Odd (нечёт)", "Even (чёт)", "Mark", "Space" });
            this.cmbParity.SelectedIndex = 0;

            StyleLabel(this.lblStopBits, "Стоп-биты:", lx, row0 + step * 4);
            StyleCombo(this.cmbStopBits, cx, row0 + step * 4, 120);
            this.cmbStopBits.Items.AddRange(new object[] { "1", "1.5", "2" });
            this.cmbStopBits.SelectedIndex = 0;

            // ── OK button ─────────────────────────────────────────────────────
            this.btnOk.BackColor = grn;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.FlatAppearance.BorderSize = 0;
            this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(95, 220, 180);
            this.btnOk.Font      = fontSemi;
            this.btnOk.ForeColor = System.Drawing.Color.White;
            this.btnOk.Location  = new System.Drawing.Point(cx, row0 + step * 5 + 6);
            this.btnOk.Size      = new System.Drawing.Size(220, 36);
            this.btnOk.Text      = "▶  Открыть порт";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnOk.Click    += new System.EventHandler(this.BtnOk_Click);

            // ── Exit button ───────────────────────────────────────────────────
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(40, 46, 70);
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(70, 80, 120);
            this.btnExit.FlatAppearance.BorderSize  = 1;
            this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(200, 60, 60);
            this.btnExit.Font      = fontUI;
            this.btnExit.ForeColor = sub;
            this.btnExit.Location  = new System.Drawing.Point(lx, row0 + step * 5 + 6);
            this.btnExit.Size      = new System.Drawing.Size(100, 36);
            this.btnExit.Text      = "Отмена";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnExit.Click    += new System.EventHandler(this.BtnExit_Click);

            // ── LoginForm ─────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = bg1;
            this.ClientSize          = new System.Drawing.Size(420, 330);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
            this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text                = "Настройка COM-порта — Вариант 5";
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelTop);
            this.panelTop.ResumeLayout(false);
            this.panelBody.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel    panelTop;
        private System.Windows.Forms.Panel    panelBody;
        private System.Windows.Forms.Label    lblTitle;
        private System.Windows.Forms.Label    lblSubtitle;
        private System.Windows.Forms.Label    lblPort;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Label    lblBaud;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.Label    lblDataBits;
        private System.Windows.Forms.ComboBox cmbDataBits;
        private System.Windows.Forms.Label    lblParity;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.Label    lblStopBits;
        private System.Windows.Forms.ComboBox cmbStopBits;
        private System.Windows.Forms.Button   btnOk;
        private System.Windows.Forms.Button   btnExit;
    }
}
