using System;
using System.Windows.Forms;

/*  
    Kryptor: Free and open source file encryption software.
    Copyright(C) 2020 Samuel Lucas

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace Kryptor
{
    public partial class frmSelectBenchmarkMode : Form
    {
        public frmSelectBenchmarkMode()
        {
            InitializeComponent();
        }

        private void FrmSelectBenchmarkMode_Load(object sender, EventArgs e)
        {
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = DarkTheme.BackgroundColour();
            DarkTheme.Buttons(btnSpeedMode);
            DarkTheme.Buttons(btnSecurityMode);
        }

        private void BtnSpeedMode_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            CloseForm();
        }

        private void BtnSecurityMode_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            CloseForm();
        }

        private void CloseForm()
        {
            this.Close();
        }

        private void FrmSelectBenchmarkMode_FormClosing(object sender, EventArgs e)
        {
            if (this.DialogResult != DialogResult.Yes || this.DialogResult != DialogResult.No)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
