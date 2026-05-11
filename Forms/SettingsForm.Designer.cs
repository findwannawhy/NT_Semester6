namespace KR.Forms
{
    partial class SettingsForm
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
            var lbl = new System.Windows.Forms.Label
            {
                Text      = "Параметры COM-порта задаются при запуске приложения\n(диалог настройки порта).",
                AutoSize  = false,
                Location  = new System.Drawing.Point(12, 20),
                Size      = new System.Drawing.Size(360, 50),
                Font      = new System.Drawing.Font("Segoe UI", 10F),
                ForeColor = System.Drawing.Color.White,
                BackColor = System.Drawing.Color.Transparent,
            };

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(30, 30, 60);
            this.ClientSize          = new System.Drawing.Size(384, 90);
            this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox         = false;
            this.MinimizeBox         = false;
            this.Text                = "Настройки порта — Вариант 5";
            this.Controls.Add(lbl);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
