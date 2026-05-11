using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace KR.Forms
{
    /// <summary>
    /// Variant 5: COM-port configuration dialog.
    /// Each user independently sets their own port name, baud rate, and parameters.
    /// </summary>
    public partial class LoginForm : Form
    {
        public PortSetup? PortConfig { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbPort.Text))
            {
                MessageBox.Show("Введите имя COM-порта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(cmbBaud.Text, out int baud) || baud <= 0)
            {
                MessageBox.Show("Выберите скорость обмена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PortConfig = new PortSetup
            {
                PortName = cmbPort.Text.Trim().ToUpper(),
                BaudRate = baud,
                DataBits = int.Parse(cmbDataBits.Text),
                Parity   = IndexToParity(cmbParity.SelectedIndex),
                StopBits = IndexToStopBits(cmbStopBits.SelectedIndex),
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private static Parity IndexToParity(int index) => index switch
        {
            1 => Parity.Odd,
            2 => Parity.Even,
            3 => Parity.Mark,
            4 => Parity.Space,
            _ => Parity.None,
        };

        private static StopBits IndexToStopBits(int index) => index switch
        {
            1 => StopBits.OnePointFive,
            2 => StopBits.Two,
            _ => StopBits.One,
        };
    }
}
