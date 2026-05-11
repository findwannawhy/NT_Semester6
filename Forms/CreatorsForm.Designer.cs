namespace KR.Forms
{
    partial class CreatorsForm
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
            this.label1  = new System.Windows.Forms.Label();
            this.label2  = new System.Windows.Forms.Label();
            this.label3  = new System.Windows.Forms.Label();
            this.label4  = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();

            var nameFont = new System.Drawing.Font("Palatino Linotype", 18F,
                System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);

            // ── Крюков В.А. ───────────────────────────────────────────────────
            this.label1.AutoSize  = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font      = nameFont;
            this.label1.ForeColor = System.Drawing.Color.DarkViolet;
            this.label1.Location  = new System.Drawing.Point(100, 30);
            this.label1.Name      = "label1";
            this.label1.TabIndex  = 0;
            this.label1.Text      = "Крюков В.А.";
            this.label1.Click    += new System.EventHandler(this.label1_Click);

            // ── Баршин С.Л. ───────────────────────────────────────────────────
            this.label2.AutoSize  = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font      = nameFont;
            this.label2.ForeColor = System.Drawing.Color.DarkViolet;
            this.label2.Location  = new System.Drawing.Point(100, 68);
            this.label2.Name      = "label2";
            this.label2.TabIndex  = 1;
            this.label2.Text      = "Баршин С.Л.";
            this.label2.Click    += new System.EventHandler(this.label2_Click);

            // ── Санников Н.А. ─────────────────────────────────────────────────
            this.label3.AutoSize  = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font      = nameFont;
            this.label3.ForeColor = System.Drawing.Color.DarkViolet;
            this.label3.Location  = new System.Drawing.Point(100, 106);
            this.label3.Name      = "label3";
            this.label3.TabIndex  = 2;
            this.label3.Text      = "Санников Н.А.";

            // ── ИУ5-61Б 2026г ─────────────────────────────────────────────────
            this.label4.AutoSize  = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font      = nameFont;
            this.label4.ForeColor = System.Drawing.Color.DarkViolet;
            this.label4.Location  = new System.Drawing.Point(100, 152);
            this.label4.Name      = "label4";
            this.label4.TabIndex  = 3;
            this.label4.Text      = "ИУ5-61Б   2026г";

            // ── OK ────────────────────────────────────────────────────────────
            this.button1.BackColor = System.Drawing.Color.Lavender;
            this.button1.Font      = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.button1.Location  = new System.Drawing.Point(9, 210);
            this.button1.Margin    = new System.Windows.Forms.Padding(2);
            this.button1.Name      = "button1";
            this.button1.Size      = new System.Drawing.Size(416, 44);
            this.button1.TabIndex  = 4;
            this.button1.Text      = "OK";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click    += new System.EventHandler(this.button1_Click);

            // ── CreatorsForm ──────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(22, 27, 46);
            this.ClientSize          = new System.Drawing.Size(436, 268);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin          = new System.Windows.Forms.Padding(2);
            this.Name            = "CreatorsForm";
            this.Text            = "Разработчики";
            this.Load           += new System.EventHandler(this.CreatorsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label  label1;
        private System.Windows.Forms.Label  label2;
        private System.Windows.Forms.Label  label3;
        private System.Windows.Forms.Label  label4;
        private System.Windows.Forms.Button button1;
    }
}
