using System;
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
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
            DisplayVersion();
            RunningOnMono();
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = DarkTheme.BackgroundColour();
            DarkTheme.TextBoxes(txtAbout);
            DarkTheme.Labels(lblTitle);
            DarkTheme.Labels(lblVersion);
            DarkTheme.Labels(lblProgramDescription);
        }

        private void DisplayVersion()
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            lblVersion.Text = $"Version {assemblyVersion.Substring(0, assemblyVersion.Length - 2)} Beta";
        }

        private void RunningOnMono()
        {
            if (Constants.RunningOnMono == true)
            {
                lblTitle.Focus();
                MonoGUI.MoveLabelLeft(lblProgramDescription);
                txtAbout.Cursor = Cursors.IBeam;
            }
        }

        private void txtAbout_GotFocus(object sender, EventArgs e)
        {
            // Hide the cursor
            lblTitle.Focus();
        }
    }
}
