using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace KR.Forms
{
    public partial class HistoryForm : Form
    {
        public string Filepath;

        public HistoryForm()
        {
            InitializeComponent();
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            var sr = new StreamReader(Filepath, Encoding.Default);
            richTextBox1.Text = sr.ReadToEnd();
            sr.Close();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
