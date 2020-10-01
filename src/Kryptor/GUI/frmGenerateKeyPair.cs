using System;
using System.IO;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;

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
    public partial class frmGenerateKeyPair : Form
    {
        public frmGenerateKeyPair()
        {
            InitializeComponent();
        }

        private void FrmGenerateKeyPair_Load(object sender, EventArgs e)
        {
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
            RunningOnMono();
            DisplayKeyPair();
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = DarkTheme.BackgroundColour();
            DarkTheme.Labels(lblPublicKey);
            DarkTheme.Labels(lblPrivateKey);
            DarkTheme.TextBoxes(txtPublicKey);
            DarkTheme.TextBoxes(txtPrivateKey);
            DarkTheme.Buttons(btnExportPublicKey);
            DarkTheme.Buttons(btnStoredKeys);
            DarkTheme.ContextMenu(cmsKeyPairMenu);
        }

        private void RunningOnMono()
        {
            if (Constants.RunningOnMono == true)
            {
                MonoGUI.MoveLabelRight(lblPublicKey);
                MonoGUI.MoveLabelRight(lblPrivateKey);
            }
        }

        private void DisplayKeyPair()
        {
            var keyPair = PasswordSharing.GenerateKeyPair();
            txtPublicKey.Text = keyPair.Item1;
            txtPrivateKey.Text = keyPair.Item2;
        }

        private void TsmiCopyTextbox_Click(object sender, EventArgs e)
        {
            SharedContextMenu.CopyTextbox(sender);
        }

        private void TsmiClearClipboard_Click(object sender, EventArgs e)
        {
            EditClipboard.ClearClipboard();
        }

        private void PicHelp_Click(object sender, EventArgs e)
        {
            const string passwordSharingLink = "https://kryptor.co.uk/Password%20Sharing.html";
            VisitLink.OpenLink(passwordSharingLink);
        }

        private void BtnExportPublicKey_Click(object sender, EventArgs e)
        {
            lblPublicKey.Focus();
            ExportPublicKey(txtPublicKey.Text);
        }

        private static void ExportPublicKey(string publicKey)
        {
            try
            {
                if (!string.IsNullOrEmpty(publicKey))
                {
                    using (var saveFileDialog = new VistaSaveFileDialog())
                    {
                        saveFileDialog.Title = "Export Public Key";
                        saveFileDialog.DefaultExt = ".txt";
                        saveFileDialog.FileName = "KRYPTOR PUBLIC KEY";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllText(saveFileDialog.FileName, publicKey);
                            File.SetAttributes(saveFileDialog.FileName, FileAttributes.ReadOnly);
                        }
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to export public key.");
            }
        }

        private void BtnStoredKeys_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
