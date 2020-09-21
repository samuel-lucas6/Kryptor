using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kryptor
{
    public partial class frmSelectBenchmarkMode : Form
    {
        public frmSelectBenchmarkMode()
        {
            InitializeComponent();
        }

        private void frmSelectBenchmarkMode_Load(object sender, EventArgs e)
        {
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            btnSpeedMode.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            btnSpeedMode.ForeColor = Color.White;
            btnSpeedMode.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnSecurityMode.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            btnSecurityMode.ForeColor = Color.White;
            btnSecurityMode.FlatAppearance.MouseDownBackColor = Color.Transparent;
        }

        private void btnSpeedMode_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnSecurityMode_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void frmSelectBenchmarkMode_FormClosing(object sender, EventArgs e)
        {
            if (this.DialogResult != DialogResult.Yes | this.DialogResult != DialogResult.No)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
