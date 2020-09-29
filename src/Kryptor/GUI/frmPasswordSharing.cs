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
    public partial class frmPasswordSharing : Form
    {
        public frmPasswordSharing()
        {
            InitializeComponent();
        }

        private void frmPasswordSharing_Load(object sender, EventArgs e)
        {
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
            RunningOnMono();
        }

        private void ApplyDarkTheme() 
        {
            this.BackColor = DarkTheme.BackgroundColour();
            DarkTheme.Labels(lblAsymmetricKey);
            DarkTheme.Labels(lblPassword);
            DarkTheme.LinkLabels(llbGenerateKeyPair);
            DarkTheme.TextBoxes(txtKey);
            DarkTheme.TextBoxes(txtPassword);
            DarkTheme.Buttons(btnEncryptPassword);
            DarkTheme.Buttons(btnDecryptPassword);
            DarkTheme.ContextMenu(cmsTextboxMenu);
        }

        private void RunningOnMono()
        {
            if (Constants.RunningOnMono == true)
            {
                lblAsymmetricKey.Focus();
                MonoGUI.MoveLabelRight(lblAsymmetricKey);
                MonoGUI.MoveLabelRight(lblPassword);
                MonoGUI.MoveLinkLabelLeft(llbGenerateKeyPair);
            }
        }

        private void tsmiCopyTextbox_Click(object sender, EventArgs e)
        {
            SharedContextMenu.CopyTextbox(sender);
        }

        private void tsmiClearTextbox_Click(object sender, EventArgs e)
        {
            SharedContextMenu.ClearTextbox(sender);
        }

        private void tsmiClearClipboard_Click(object sender, EventArgs e)
        {
            EditClipboard.ClearClipboard();
        }

        private void picHelp_Click(object sender, EventArgs e)
        {
            const string passwordSharingLink = "https://kryptor.co.uk/Password%20Sharing.html";
            VisitLink.OpenLink(passwordSharingLink);
        }

        private void llbGenerateKeyPair_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lblAsymmetricKey.Focus();
            using (var generateKeyPair = new frmGenerateKeyPair())
            {
                generateKeyPair.ShowDialog();
            }
        }

        private void btnEncryptPassword_Click(object sender, EventArgs e)
        {
            lblAsymmetricKey.Focus();
            bool encryption = true;
            GetUserInput(encryption);
        }

        private void btnDecryptPassword_Click(object sender, EventArgs e)
        {
            lblPassword.Focus();
            bool encryption = false;
            GetUserInput(encryption);
        }

        private void GetUserInput(bool encryption)
        {
            char[] key = txtKey.Text.ToCharArray();
            char[] password = txtPassword.Text.ToCharArray();
            if (key.Length > 0 && password.Length > 0)
            {
                char[] message = PasswordSharing.ConvertUserInput(encryption, key, password);
                if (message.Length > 0)
                {
                    txtPassword.Text = new string(message);
                    Utilities.ZeroArray(message);
                }
                Utilities.ZeroArray(key);
                Utilities.ZeroArray(password);
            }
            else
            {
                DisplayUserInputError(encryption);
            }
        }

        private static void DisplayUserInputError(bool encryption)
        {
            string errorMessage;
            if (encryption == true)
            {
                errorMessage = "Sender: Please enter the recipient's public key and a plaintext password to encrypt.";
            }
            else
            {
                errorMessage = "Recipient: Please enter your private key and a ciphertext password to decrypt.";
            }
            DisplayMessage.InformationMessageBox(errorMessage);
        }
    }
}
