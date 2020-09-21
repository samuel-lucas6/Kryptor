using System;
using System.Drawing;
using System.Reflection;
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
    public partial class frmAboutKryptor : Form
    {
        public frmAboutKryptor()
        {
            InitializeComponent();
        }

        private void frmAboutKryptor_Load(object sender, EventArgs e)
        {
            DisplayVersion();
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
        }

        private void DisplayVersion()
        {
            lblVersion.Text = $"Version {Assembly.GetExecutingAssembly().GetName().Version} Beta";
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            txtAbout.BackColor = Color.DimGray;
            txtAbout.ForeColor = Color.White;
            lblTitle.ForeColor = Color.White;
            lblVersion.ForeColor = Color.White;
            lblProgramDescription.ForeColor = Color.White;
        }

        private void txtAbout_GotFocus(object sender, EventArgs e)
        {
            // Hide the cursor
            txtAbout.Enabled = false;
            txtAbout.Enabled = true;
        }
    }
}
