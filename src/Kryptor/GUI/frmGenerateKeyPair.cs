using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Sodium;
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

        private void frmGenerateKeyPair_Load(object sender, EventArgs e)
        {
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
            // Fix label alignment on Linux & macOS
            MonoGUI.AlignLabels(lblPublicKey, lblPrivateKey, null, null);
            DisplayKeyPair();
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            lblPublicKey.ForeColor = Color.White;
            lblPrivateKey.ForeColor = Color.White;
            txtPublicKey.BackColor = Color.DimGray;
            txtPublicKey.ForeColor = Color.White;
            txtPrivateKey.BackColor = Color.DimGray;
            txtPrivateKey.ForeColor = Color.White;
            btnExportPublicKey.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            btnExportPublicKey.ForeColor = Color.White;
            btnExportPublicKey.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnStoredKeys.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            btnStoredKeys.ForeColor = Color.White;
            btnStoredKeys.FlatAppearance.MouseDownBackColor = Color.Transparent;
            SharedContextMenu.DarkContextMenu(cmsKeyPairMenu);
        }

        private void DisplayKeyPair()
        {
            var keyPair = GenerateKeyPair();
            txtPublicKey.Text = keyPair.Item1;
            txtPrivateKey.Text = keyPair.Item2;
        }

        private static (string, string) GenerateKeyPair()
        {
            using (var keyPair = PublicKeyBox.GenerateKeyPair())
            {
                string publicKey = Convert.ToBase64String(keyPair.PublicKey);
                string privateKey = Convert.ToBase64String(keyPair.PrivateKey);
                return (publicKey, privateKey);
            }
        }

        private void tsmiCopyTextbox_Click(object sender, EventArgs e)
        {
            SharedContextMenu.CopyTextbox(sender);
        }

        private void tsmiClearClipboard_Click(object sender, EventArgs e)
        {
            EditClipboard.ClearClipboard();
        }

        private void btnExportPublicKey_Click(object sender, EventArgs e)
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
                    using (var saveFile = new VistaSaveFileDialog())
                    {
                        saveFile.DefaultExt = ".txt";
                        saveFile.FileName = "KRYPTOR PUBLIC KEY";
                        if (saveFile.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllText(saveFile.FileName, publicKey);
                            File.SetAttributes(saveFile.FileName, FileAttributes.ReadOnly);
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

        private void btnStoredKeys_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void picHelp_Click(object sender, EventArgs e)
        {
            const string passwordSharingLink = "https://kryptor.co.uk/Password%20Sharing.html";
            OpenURL.OpenLink(passwordSharingLink);
        }
    }
}
