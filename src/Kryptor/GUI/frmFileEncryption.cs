using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
    public partial class FrmFileEncryption : Form
    {
        public FrmFileEncryption()
        {
            InitializeComponent();
        }

        private void Kryptor_Load(object sender, EventArgs e)
        {
            this.DragEnter += new DragEventHandler(FileDragEnter);
            this.DragDrop += new DragEventHandler(FileDragDrop);
            // Make adjustments for Linux & macOS
            RunningOnMono();
            // Check whether Kryptor has been opened by clicking on an encrypted file
            CheckForSelectedFiles();
            LoadSettings();
            CheckForUpdates();
        }

        private void FileDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void FileDragDrop(object sender, DragEventArgs e)
        {
            string[] selectedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            Globals.SetSelectedFiles(selectedFiles.ToList());
            Array.Clear(selectedFiles, 0, selectedFiles.Length);
            picFilesSelected.BackColor = Color.LawnGreen;
        }

        private void RunningOnMono()
        {
            if (Constants.RunningOnMono == true)
            {
                this.TopMost = true;
                msMenus.RenderMode = ToolStripRenderMode.System;
                MonoGUI.MoveLabelRight(lblDragDrop);
                MonoGUI.MoveLabelRight(lblPassword);
                MonoGUI.MoveLabelRight(lblEntropy);
            }
        }

        private void CheckForSelectedFiles()
        {
            if (Globals.GetSelectedFiles() != null)
            {
                picFilesSelected.BackColor = Color.LawnGreen;
            }
        }

        private void LoadSettings()
        {
            Settings.ReadSettings();
            chkShowPassword.Checked = Globals.ShowPasswordByDefault;
            tmrClearClipboard.Interval = Globals.ClearClipboardInterval;
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
        }

        private void ApplyDarkTheme()
        {
            if (Constants.RunningOnMono == false)
            {
                msMenus.Renderer = new ToolStripProfessionalRenderer(new DarkColourTable());
            }
            this.BackColor = DarkTheme.BackgroundColour();
            msMenus.BackColor = DarkTheme.BackgroundColour();
            DarkTheme.ToolStripMenuItems(tsmiFile);
            DarkTheme.ToolStripMenuItems(tsmiTools);
            DarkTheme.ToolStripMenuItems(tsmiHelp);
            DarkTheme.ToolStripMenuItems(tsmiHelp);
            DarkTheme.Menu(tsmiFile);
            DarkTheme.Menu(tsmiTools);
            DarkTheme.Menu(tsmiHelp);
            DarkTheme.Labels(lblFilesSelected);
            DarkTheme.Labels(lblShowPassword);
            DarkTheme.Labels(lblDragDrop);
            DarkTheme.Labels(lblPassword);
            DarkTheme.TextBoxes(txtPassword);
            DarkTheme.Buttons(btnEncrypt);
            DarkTheme.Buttons(btnDecrypt);
            DarkTheme.ContextMenu(cmsClearFilesMenu);
            DarkTheme.ContextMenu(cmsPasswordMenu);
        }

        private static void CheckForUpdates()
        {
            if (Globals.CheckForUpdates == true)
            {
                bool displayUpToDate = false;
                Updates.UpdateKryptor(displayUpToDate);
            }
        }

        private void ShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (chkShowPassword.Checked == true)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void TsmiCopyPassword_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPassword.Text))
            {
                EditClipboard.SetClipboard(txtPassword.Text);
                EditClipboard.AutoClearClipboard();
            }
        }

        private void TsmiClearPassword_Click(object sender, EventArgs e)
        {
            ClearPasswordTextbox();
        }

        private void TsmiClearClipboard_Click(object sender, EventArgs e)
        {
            EditClipboard.ClearClipboard();
        }

        private void TsmiClearSelectedFiles_Click(object sender, EventArgs e)
        {
            ClearSelectedFiles();
        }

        private void TmrClearClipboard_Tick(object sender, EventArgs e)
        {
            EditClipboard.ClearClipboard();
            tmrClearClipboard.Stop();
        }

        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            PasswordEvaluation.DisplayPasswordEntropy(txtPassword.Text, lblEntropy);
        }

        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            lblPassword.Focus();
            bool encryption = true;
            GetPasswordInput(encryption);
        }

        private void GetPasswordInput(bool encryption)
        {
            char[] password = txtPassword.Text.ToCharArray();
            ValidateUserInput(encryption, password);
        }

        private void ValidateUserInput(bool encryption, char[] password)
        {
            if (!bgwEncryption.IsBusy && !bgwDecryption.IsBusy && !bgwShredFiles.IsBusy)
            {
                if (Globals.GetSelectedFiles() != null)
                {
                    if (password.Length >= 8 || !string.IsNullOrEmpty(Globals.KeyfilePath))
                    {
                        GetPasswordBytes(encryption, password);
                    }
                    else
                    {
                        DisplayMessage.InformationMessageBox("Please enter a password (8+ characters long) or select/generate a keyfile.");
                    }
                }
                else
                {
                    DisplayMessage.InformationMessageBox("Please select files to encrypt.");
                }
            }
            else
            {
                DisplayMessage.InformationMessageBox("Please wait for the current operation to finish.");
            }
        }

        private void GetPasswordBytes(bool encryption, char[] password)
        {
            byte[] passwordBytes = FileEncryption.GetPasswordBytes(password);
            Utilities.ZeroArray(password);
            AutoClearPassword();
            StartBackgroundWorker(encryption, passwordBytes);
        }

        private void AutoClearPassword()
        {
            if (Globals.AutoClearPassword == true)
            {
                ClearPasswordTextbox();
            }
        }

        private void ClearPasswordTextbox()
        {
            txtPassword.Clear();
            txtPassword.ClearUndo();
        }

        private void StartBackgroundWorker(bool encryption, byte[] passwordBytes)
        {
            BeforeBackgroundWorker();
            if (encryption == true)
            {
                bgwEncryption.RunWorkerAsync(passwordBytes);
            }
            else
            {
                bgwDecryption.RunWorkerAsync(passwordBytes);
            }
        }

        private void BeforeBackgroundWorker()
        {
            prgProgress.Visible = true;
            DisableMenus();
        }

        private void DisableMenus()
        {
            // Prevent the user altering settings, etc whilst encryption/decryption is running
            tsmiFile.Enabled = false;
            tsmiTools.Enabled = false;
            cmsClearFilesMenu.Enabled = false;
        }

        private void BgwEncryption_DoWork(object sender, DoWorkEventArgs e)
        {
            bool encryption = true;
            byte[] passwordBytes = (byte[])e.Argument;
            FileEncryption.StartEncryption(encryption, passwordBytes, bgwEncryption);
        }

        private void BgwEncryption_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressBar(e.ProgressPercentage);
        }

        private void UpdateProgressBar(int percentage)
        {
            try
            {
                // This solution fixes some update lag
                prgProgress.Value = percentage;
                prgProgress.Value = percentage - 1;
                prgProgress.Value = percentage;
            }
            catch (ArgumentException ex)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Bug);
                DisplayMessage.ErrorResultsText(string.Empty, ex.GetType().Name, "Invalid progress bar value. This is a bug - please report it.");
            }
        }

        private void BgwEncryption_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            const string outputMessage = "encrypted";
            FileEncryptionCompleted(outputMessage);
        }

        private void FileEncryptionCompleted(string outputMessage)
        {
            ClearSelectedFiles();
            // Deallocate Argon2 memory
            GC.Collect();
            BackgroundWorkerCompleted(outputMessage);
        }

        private void ClearSelectedFiles()
        {
            if (Globals.SuccessfulCount == Globals.TotalCount)
            {
                Globals.SetSelectedFiles(null);
                tsmiSelectFiles.Checked = false;
                tsmiSelectFolder.Checked = false;
                picFilesSelected.BackColor = Color.Red;
                Globals.KeyfilePath = null;
                tsmiSelectKeyfile.Checked = false;
            }
        }

        private void BackgroundWorkerCompleted(string outputMessage)
        {
            EnableMenus();
            prgProgress.Visible = false;
            prgProgress.Value = 0;
            Globals.ResultsText += Environment.NewLine + $"Successfully {outputMessage}: {Invariant.ToString(Globals.SuccessfulCount)}/{Invariant.ToString(Globals.TotalCount)}";
            using (var results = new FrmResults())
            {
                results.ShowDialog();
            }
        }

        private void EnableMenus()
        {
            // Allow the user to access all features after encryption/decryption has finished
            tsmiFile.Enabled = true;
            tsmiTools.Enabled = true;
            cmsClearFilesMenu.Enabled = true;
        }

        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            lblPassword.Focus();
            bool encryption = false;
            GetPasswordInput(encryption);
        }

        private void BgwDecryption_DoWork(object sender, DoWorkEventArgs e)
        {
            bool encryption = false;
            byte[] passwordBytes = (byte[])e.Argument;
            FileEncryption.StartEncryption(encryption, passwordBytes, bgwDecryption);
        }

        private void BgwDecryption_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressBar(e.ProgressPercentage);
        }

        private void BgwDecryption_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            const string outputMessage = "decrypted";
            FileEncryptionCompleted(outputMessage);
        }

        private void TsmiSelectFiles_Click(object sender, EventArgs e)
        {
            bool filesSelected = SelectFiles.SelectFilesDialog();
            FilesSelected(filesSelected);
        }

        private void FilesSelected(bool filesSelected)
        {
            if (filesSelected == true)
            {
                picFilesSelected.BackColor = Color.LawnGreen;
                tsmiSelectFiles.Checked = true;
                tsmiSelectFolder.Checked = false;
            }
        }

        private void TsmiSelectFolder_Click(object sender, EventArgs e)
        {
            bool folderSelected = SelectFiles.SelectFolderDialog();
            FolderSelected(folderSelected);
        }

        private void FolderSelected(bool folderSelected)
        {
            if (folderSelected == true)
            {
                picFilesSelected.BackColor = Color.LawnGreen;
                tsmiSelectFolder.Checked = true;
                tsmiSelectFiles.Checked = false;
            }
        }

        private void TsmiCreateKeyFile_Click(object sender, EventArgs e)
        {
            bool keyfileGenerated = Keyfiles.CreateKeyfile();
            KeyfileSelected(keyfileGenerated);
        }

        private void TsmiSelectKeyfile_Click(object sender, EventArgs e)
        {
            bool keyfileSelected = Keyfiles.SelectKeyfile();
            KeyfileSelected(keyfileSelected);
        }

        private void KeyfileSelected(bool keyfileSelected)
        {
            if (keyfileSelected == true)
            {
                tsmiSelectKeyfile.Checked = keyfileSelected;
            }
        }

        private void TsmiSettings_Click(object sender, EventArgs e)
        {
            using (var settings = new FrmSettings())
            {
                settings.ShowDialog();
            }
        }

        private void TsmiQuit_Click(object sender, EventArgs e)
        {
            if (!bgwEncryption.IsBusy && !bgwDecryption.IsBusy && !bgwShredFiles.IsBusy)
            {
                Application.Exit();
            }
            else
            {
                DisplayMessage.InformationMessageBox("Please wait for the current operation to finish.");
            }
        }

        private void TsmiPasswordGenerator_Click(object sender, EventArgs e)
        {
            using (var passwordGenerator = new frmPasswordGenerator())
            {
                passwordGenerator.ShowDialog();
            }
        }

        private void TsmiPasswordSharing_Click(object sender, EventArgs e)
        {
            using (var passwordSharing = new FrmPasswordSharing())
            {
                passwordSharing.ShowDialog();
            }
        }

        private void TsmiShredFiles_Click(object sender, EventArgs e)
        {
            bool filesSelected = SelectFiles.SelectFilesDialog();
            CallShredFiles(filesSelected);
        }

        private void TsmiShredFolder_Click(object sender, EventArgs e)
        {
            bool folderSelected = SelectFiles.SelectFolderDialog();
            CallShredFiles(folderSelected);
        }

        private void CallShredFiles(bool filesSelected)
        {
            if (filesSelected == true)
            {
                BeforeBackgroundWorker();
                bgwShredFiles.RunWorkerAsync();
            }
        }

        private void BgwShredFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            ShredFiles.ShredSelectedFiles(bgwShredFiles);
        }

        private void BgwShredFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgressBar(e.ProgressPercentage);
        }

        private void BgwShredFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            const string outputMessage = "shredded";
            BackgroundWorkerCompleted(outputMessage);
        }

        private void TsmiBackupSettings_Click(object sender, EventArgs e)
        {
            Settings.BackupSettings();
        }

        private void TsmiRestoreSettings_Click(object sender, EventArgs e)
        {
            Settings.RestoreSettings();
        }

        private void TsmiDocumentation_Click(object sender, EventArgs e)
        {
            const string documentationLink = "https://kryptor.co.uk/Documentation.html";
            VisitLink.OpenLink(documentationLink);
        }

        private void TsmiSourceCode_Click(object sender, EventArgs e)
        {
            const string sourceCodeLink = "https://github.com/samuel-lucas6/Kryptor";
            VisitLink.OpenLink(sourceCodeLink);
        }

        private void TsmiDonate_Click(object sender, EventArgs e)
        {
            const string donateLink = "https://kryptor.co.uk/Donate.html";
            VisitLink.OpenLink(donateLink);
        }

        private void TsmiCheckForUpdates_Click(object sender, EventArgs e)
        {
            bool displayUpToDate = true;
            Updates.UpdateKryptor(displayUpToDate);
        }

        private void TsmiAbout_Click(object sender, EventArgs e)
        {
            using (var about = new FrmAboutKryptor())
            {
                about.ShowDialog();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (bgwEncryption.IsBusy || bgwDecryption.IsBusy || bgwShredFiles.IsBusy)
            {
                if (e != null)
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnFormClosing(e);
        }
    }
}
