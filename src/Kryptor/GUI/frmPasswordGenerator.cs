using System;
using System.Drawing;
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
    public partial class frmPasswordGenerator : Form
    {
        public frmPasswordGenerator()
        {
            InitializeComponent();
        }

        // Enum for code readability instead of referencing numbers
        private enum Generate
        {
            Password,
            Passphrase
        }

        private void frmPasswordGenerator_Load(object sender, EventArgs e)
        {
            cmbGenerateType.SelectedIndex = (int)Generate.Password;
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
            // Fix label alignment on Linux & macOS
            MonoGUI.AlignLabels(lblPassword, lblEntropy, null, null);
            GeneratePasswordOnLoad();
        }

        private void ApplyDarkTheme()
        {
            // Main form 
            this.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            grbOptions.ForeColor = Color.White;

            // Controls outside group box
            txtGeneratedPassword.BackColor = Color.DimGray;
            txtGeneratedPassword.ForeColor = Color.White;
            lblEntropy.ForeColor = Color.White;
            lblPassword.ForeColor = Color.White;
            btnRegeneratePassword.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            btnRegeneratePassword.ForeColor = Color.White;
            btnRegeneratePassword.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnCopyPassword.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            btnCopyPassword.ForeColor = Color.White;
            btnCopyPassword.FlatAppearance.MouseDownBackColor = Color.Transparent;

            // Options group box
            cmbGenerateType.ForeColor = Color.White;
            cmbGenerateType.BackColor = Color.DimGray;
            lblLength.ForeColor = Color.White;
            nudLength.ForeColor = Color.White;
            nudLength.BackColor = Color.DimGray;
            chkLowercase.ForeColor = Color.White;
            chkLowercase.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            chkUppercase.ForeColor = Color.White;
            chkUppercase.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            chkNumbers.ForeColor = Color.White;
            chkNumbers.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            chkSymbols.ForeColor = Color.White;
            chkSymbols.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
        }

        private void GeneratePasswordOnLoad()
        {
            int length = 30;
            // Use all character types
            char[] password = PasswordGenerator.GenerateRandomPassword(length, true, true, true, true);
            DisplayGeneratedPassword(password);
        }

        private void DisplayGeneratedPassword(char[] password)
        {
            if (password.Length > 0)
            {
                txtGeneratedPassword.Text = new string(password);
                PasswordEvaluation.DisplayPasswordEntropy(txtGeneratedPassword.Text, lblEntropy);
            }
        }

        private void btnRegeneratePassword_Click(object sender, EventArgs e)
        {
            lblPassword.Focus();
            RegeneratePassword();
        }

        private void RegeneratePassword()
        {
            char[] password;
            int length = (int)nudLength.Value;
            bool lowercase = chkLowercase.Checked;
            bool uppercase = chkUppercase.Checked;
            bool numbers = chkNumbers.Checked;
            bool symbols = chkSymbols.Checked;
            if (cmbGenerateType.SelectedIndex == (int)Generate.Password)
            {
                password = PasswordGenerator.GenerateRandomPassword(length, lowercase, uppercase, numbers, symbols);
            }
            else
            {
                password = PasswordGenerator.GenerateRandomPassphrase(length, uppercase, numbers);
            }
            DisplayGeneratedPassword(password);
            Utilities.ZeroArray(password);
        }

        private void btnCopyPassword_Click(object sender, EventArgs e)
        {
            lblPassword.Focus();
            CopyPassword(txtGeneratedPassword.Text);
        }

        private void CopyPassword(string password)
        {
            EditClipboard.SetClipboard(password);
            EditClipboard.AutoClearClipboard();
            SetPasswordTextbox(password);
            this.Close();
        }

        private static void SetPasswordTextbox(string password)
        {
            frmKryptor mainForm = (frmKryptor)Application.OpenForms["frmKryptor"];
            TextBox txtPassword = (TextBox)mainForm.Controls["txtPassword"];
            txtPassword.Text = password;
            txtPassword.SelectionStart = txtPassword.Text.Length;
        }

        private void cmbGenerateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGenerateType.SelectedIndex == (int)Generate.Password)
            {
                // Password
                lblLength.Text = "Length:";
                nudLength.Maximum = 128;
                nudLength.Minimum = 12;
                nudLength.Value = 30;
            }
            else
            {
                // Passphrase
                lblLength.Text = "Words:";
                nudLength.Maximum = 20;
                nudLength.Minimum = 4;
                nudLength.Value = 6;
                chkLowercase.Checked = true;
                chkSymbols.Checked = true;
            }
            cmbGenerateType.Select(0, 0);
            RegeneratePassword();
        }

        private void cmbGenerateType_DropDownClosed(object sender, EventArgs e)
        {
            // Remove highlight
            lblEntropy.Focus();
        }

        private void nudLength_ValueChanged(object sender, EventArgs e)
        {
            RegeneratePassword();
        }

        private void chkLowercase_CheckedChanged(object sender, EventArgs e)
        {
            // Prevent unchecking
            if (cmbGenerateType.SelectedIndex == (int)Generate.Passphrase | chkUppercase.Checked == false & chkNumbers.Checked == false & chkSymbols.Checked == false)
            {
                chkLowercase.Checked = true;
            }
            RegeneratePassword();
        }

        private void chkUppercase_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLowercase.Checked == false & chkNumbers.Checked == false & chkSymbols.Checked == false)
            {
                chkUppercase.Checked = true;
            }
            RegeneratePassword();
        }

        private void chkNumbers_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLowercase.Checked == false & chkUppercase.Checked == false & chkSymbols.Checked == false)
            {
                chkNumbers.Checked = true;
            }
            RegeneratePassword();
        }

        private void chkSymbols_CheckedChanged(object sender, EventArgs e)
        {
            if (cmbGenerateType.SelectedIndex == (int)Generate.Passphrase | chkLowercase.Checked == false & chkUppercase.Checked == false & chkNumbers.Checked == false)
            {
                chkSymbols.Checked = true;
            }
            RegeneratePassword();
        }
    }
}
