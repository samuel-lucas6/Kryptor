using System;
using System.ComponentModel;
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
    public partial class frmSettings : Form
    {
        private bool FormLoad { get; set; } = true;

        public frmSettings()
        {
            InitializeComponent();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
            DisplaySettings();
        }

        private void ApplyDarkTheme()
        {
            // Main form 
            this.BackColor = Color.FromArgb(Constants.Red, Constants.Green, Constants.Blue);
            grbFileEncryption.ForeColor = Color.White;
            grbKeyDerivation.ForeColor = Color.White;
            grbOtherSettings.ForeColor = Color.White;

            // File Encryption group box
            lblEncryptionAlgorithm.ForeColor = Color.White;
            lblMemoryEncryption.ForeColor = Color.White;
            cmbEncryptionAlgorithm.BackColor = Color.DimGray;
            cmbEncryptionAlgorithm.ForeColor = Color.White;
            cmbMemoryEncryption.BackColor = Color.DimGray;
            cmbMemoryEncryption.ForeColor = Color.White;
            lblAnonymousRename.ForeColor = Color.White;
            cmbAnonymousRename.BackColor = Color.DimGray;
            cmbAnonymousRename.ForeColor = Color.White;
            lblOverwriteFiles.ForeColor = Color.White;
            cmbOverwriteFiles.BackColor = Color.DimGray;
            cmbOverwriteFiles.ForeColor = Color.White;

            // Key Derivation group box
            lblArgon2Parallelism.ForeColor = Color.White;
            lblArgon2MemorySize.ForeColor = Color.White;
            lblArgon2Iterations.ForeColor = Color.White;
            nudArgon2Parallelism.BackColor = Color.DimGray;
            nudArgon2Parallelism.ForeColor = Color.White;
            nudArgon2MemorySize.BackColor = Color.DimGray;
            nudArgon2MemorySize.ForeColor = Color.White;
            nudArgon2Iterations.BackColor = Color.DimGray;
            nudArgon2Iterations.ForeColor = Color.White;
            btnArgon2Benchmark.BackColor = Color.DimGray;
            btnArgon2Benchmark.FlatAppearance.MouseDownBackColor = Color.DimGray;
            btnArgon2Benchmark.ForeColor = Color.White;
            btnTestParameters.BackColor = Color.DimGray;
            btnTestParameters.FlatAppearance.MouseDownBackColor = Color.DimGray;
            btnTestParameters.ForeColor = Color.White;

            // Other Settings group box
            lblShowPassword.ForeColor = Color.White;
            lblAutoClearPassword.ForeColor = Color.White;
            lblAutoClearClipboard.ForeColor = Color.White;
            lblExitClipboardClear.ForeColor = Color.White;
            lblShredFilesMethod.ForeColor = Color.White;
            lblCheckForUpdates.ForeColor = Color.White;
            lblTheme.ForeColor = Color.White;
            cmbAnonymousRename.BackColor = Color.DimGray;
            cmbAnonymousRename.ForeColor = Color.White;
            cmbShowPassword.BackColor = Color.DimGray;
            cmbShowPassword.ForeColor = Color.White;
            cmbAutoClearPassword.BackColor = Color.DimGray;
            cmbAutoClearPassword.ForeColor = Color.White;
            cmbAutoClearClipboard.BackColor = Color.DimGray;
            cmbAutoClearClipboard.ForeColor = Color.White;
            cmbExitClipboardClear.BackColor = Color.DimGray;
            cmbExitClipboardClear.ForeColor = Color.White;
            cmbShredFilesMethod.BackColor = Color.DimGray;
            cmbShredFilesMethod.ForeColor = Color.White;
            cmbCheckForUpdates.BackColor = Color.DimGray;
            cmbCheckForUpdates.ForeColor = Color.White;
            cmbTheme.BackColor = Color.DimGray;
            cmbTheme.ForeColor = Color.White;
        }

        private void DisplaySettings()
        {
            // Set the value of each control
            SetEncryptionAlgorithm();
            SetMemoryEncryption();
            SetAnonymousRename();
            SetOverwriteFiles();
            SetParallelism();
            SetMemorySize();
            SetIterations();
            SetShowPassword();
            SetAutoClearPassword();
            SetAutoClearClipboard();
            SetClearClipboardOnExit();
            SetShredFilesMethod();
            SetCheckForUpdates();
            SetTheme();
            FormLoad = false;
        }

        private void SetEncryptionAlgorithm()
        {
            try
            {
                cmbEncryptionAlgorithm.SelectedIndex = Globals.EncryptionAlgorithm;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Encryption Algorithm setting. The default setting will be used instead.");
                Globals.EncryptionAlgorithm = (int)Cipher.XChaCha20;
                Settings.SaveSettings();
                SetEncryptionAlgorithm();
            }
        }

        private void SetMemoryEncryption()
        {
            try
            {
                if (Globals.MemoryEncryption == true)
                {
                    cmbMemoryEncryption.SelectedIndex = 0;
                }
                else
                {
                    cmbMemoryEncryption.SelectedIndex = 1;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Memory Encryption setting. This is a bug - please report it.");
            }
        }

        private void SetAnonymousRename()
        {
            try
            {
                if (Globals.AnonymousRename == true)
                {
                    cmbAnonymousRename.SelectedIndex = 0;
                }
                else
                {
                    cmbAnonymousRename.SelectedIndex = 1;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Anonymous Rename setting. This is a bug - please report it.");
            }
        }

        private void SetOverwriteFiles()
        {
            try
            {
                if (Globals.OverwriteFiles == true)
                {
                    cmbOverwriteFiles.SelectedIndex = 0;
                }
                else
                {
                    cmbOverwriteFiles.SelectedIndex = 1;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Overwrite Original Files setting. This is a bug - please report it.");
            }
        }

        private void SetParallelism()
        {
            try
            {
                nudArgon2Parallelism.Value = Globals.Parallelism;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Argon2 Parallelism setting. The default setting will be used instead.");
                Globals.Parallelism = Constants.DefaultParallelism;
                Settings.SaveSettings();
                SetParallelism();
            }
        }

        private void SetMemorySize()
        {
            try
            {
                nudArgon2MemorySize.Value = Globals.MemorySize / Constants.Mebibyte;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Argon2 Memory Size setting. The default setting will be used instead.");
                Globals.MemorySize = Constants.Mebibyte;
                Settings.SaveSettings();
                SetMemorySize();
            }
        }

        private void SetIterations()
        {
            try
            {
                nudArgon2Iterations.Value = Globals.Iterations;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Argon2 Iterations setting. The default setting will be used instead.");
                Globals.Iterations = 1;
                Settings.SaveSettings();
                SetIterations();
            }
        }

        private void SetShowPassword()
        {
            try
            {
                if (Globals.ShowPasswordByDefault == true)
                {
                    cmbShowPassword.SelectedIndex = 0;
                }
                else
                {
                    cmbShowPassword.SelectedIndex = 1;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Show Password setting. This is a bug - please report it.");
            }
        }

        private void SetAutoClearPassword()
        {
            try
            {
                if (Globals.AutoClearPassword == true)
                {
                    cmbAutoClearPassword.SelectedIndex = 0;
                }
                else
                {
                    cmbAutoClearPassword.SelectedIndex = 1;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Auto Clear Password setting. This is a bug - please report it.");
            }
        }

        private void SetAutoClearClipboard()
        {
            try
            {
                if (Globals.ClearClipboardInterval == 1)
                {
                    cmbAutoClearClipboard.SelectedIndex = 0;
                }
                else if (Globals.ClearClipboardInterval == 15000)
                {
                    cmbAutoClearClipboard.SelectedIndex = 1;
                }
                else if (Globals.ClearClipboardInterval == 30000)
                {
                    cmbAutoClearClipboard.SelectedIndex = 2;
                }
                else if (Globals.ClearClipboardInterval == 60000)
                {
                    cmbAutoClearClipboard.SelectedIndex = 3;
                }
                else if (Globals.ClearClipboardInterval == 90000)
                {
                    cmbAutoClearClipboard.SelectedIndex = 4;
                }
                else if (Globals.ClearClipboardInterval == 120000)
                {
                    cmbAutoClearClipboard.SelectedIndex = 5;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Auto Clear Clipboard setting. The default setting will be used instead.");
                Globals.ClearClipboardInterval = 30000;
                Settings.SaveSettings();
                SetAutoClearClipboard();
            }
        }

        private void SetClearClipboardOnExit()
        {
            try
            {
                if (Globals.ClearClipboardOnExit == true)
                {
                    cmbExitClipboardClear.SelectedIndex = 0;
                }
                else
                {
                    cmbExitClipboardClear.SelectedIndex = 1;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Clear Clipboard on Exit setting. This is a bug - please report it.");
            }
        }

        private void SetShredFilesMethod()
        {
            try
            {
                cmbShredFilesMethod.SelectedIndex = Globals.ShredFilesMethod;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Shred Files Method setting. The default setting will be used instead.");
                Globals.ShredFilesMethod = 2;
                Settings.SaveSettings();
                SetIterations();
            }
        }

        private void SetCheckForUpdates()
        {
            try
            {
                if (Globals.CheckForUpdates == true)
                {
                    cmbCheckForUpdates.SelectedIndex = 0;
                }
                else
                {
                    cmbCheckForUpdates.SelectedIndex = 1;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Check for Updates setting. This is a bug - please report it.");
            }
        }

        private void SetTheme()
        {
            try
            {
                if (Globals.DarkTheme == true)
                {
                    cmbTheme.SelectedIndex = 0;
                }
                else
                {
                    cmbTheme.SelectedIndex = 1;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid Theme setting. This is a bug - please report it.");
            }
        }

        private void cmbEncryptionAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                lblEncryptionAlgorithm.Focus();
                Globals.EncryptionAlgorithm = cmbEncryptionAlgorithm.SelectedIndex;
            }
        }

        private void cmbEncryptionAlgorithm_DropDownClosed(object sender, EventArgs e)
        {
            lblEncryptionAlgorithm.Focus();
        }

        private void cmbMemoryEncryption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                if (cmbMemoryEncryption.SelectedIndex == 0)
                {
                    Globals.MemoryEncryption = true;
                }
                else
                {
                    Globals.MemoryEncryption = false;
                }
            }
        }

        private void cmbMemoryEncryption_DropDownClosed(object sender, EventArgs e)
        {
            lblMemoryEncryption.Focus();
        }

        private void cmbAnonymousRename_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                if (cmbAnonymousRename.SelectedIndex == 0)
                {
                    Globals.AnonymousRename = true;
                }
                else
                {
                    Globals.AnonymousRename = false;
                }
            }
        }

        private void cmbAnonymousRename_DropDownClosed(object sender, EventArgs e)
        {
            lblAnonymousRename.Focus();
        }

        private void cmbOverwriteFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                if (cmbOverwriteFiles.SelectedIndex == 0)
                {
                    Globals.OverwriteFiles = true;
                }
                else
                {
                    Globals.OverwriteFiles = false;
                }
            }
        }

        private void cmbOverwriteFiles_DropDownClosed(object sender, EventArgs e)
        {
            lblOverwriteFiles.Focus();
        }

        private void btnArgon2Benchmark_Click(object sender, EventArgs e)
        {
            lblArgon2Parallelism.Focus();
            RunArgon2Benchmark();
        }

        private void RunArgon2Benchmark()
        {
            foreach (Form form in Application.OpenForms)
            {
                form.Hide();
            }
            using (var argon2Benchmark = new frmArgon2Benchmark())
            {
                argon2Benchmark.ShowDialog();
            }
            nudArgon2MemorySize.Value = Globals.MemorySize / Constants.Mebibyte;
        }

        private void btnTestParameters_Click(object sender, EventArgs e)
        {
            lblArgon2Parallelism.Focus();
            if (!bgwTestArgon2Parameters.IsBusy)
            {
                bgwTestArgon2Parameters.RunWorkerAsync();
            }
        }

        private void bgwTestArgon2Parameters_DoWork(object sender, DoWorkEventArgs e)
        {
            Argon2Benchmark.TestArgon2Parameters();
        }

        private void bgwTestArgon2Parameters_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Deallocate RAM for Argon2
            GC.Collect();
        }

        private void nudArgon2Parallelism_ValueChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                Globals.Parallelism = (int)nudArgon2Parallelism.Value;
            }
        }

        private void nudArgon2MemorySize_ValueChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                Globals.MemorySize = (int)nudArgon2MemorySize.Value * Constants.Mebibyte;
                Argon2Warning();
            }
        }

        private void nudArgon2Iterations_ValueChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                Globals.Iterations = (int)nudArgon2Iterations.Value;
                Argon2Warning();
            }
        }

        private void Argon2Warning()
        {
            bool warning = false;
            // 40+ MiB memory size & 6+ iterations or 100+ MiB memory size & 4+ iterations
            if ((Globals.MemorySize >= 40960 & Globals.Iterations > 5) | (Globals.MemorySize >= 102400 & Globals.Iterations > 3) | (Globals.MemorySize >= 10240 & Globals.Iterations > 30))
            {
                lblArgon2Warning.Visible = true;
                lblArgon2Warning.Text = "Warning: Encryption will be slow.";
                warning = true;
            }
            if (warning == false)
            {
                lblArgon2Warning.Visible = false;
            }
        }

        private void cmbShredFilesMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                Globals.ShredFilesMethod = cmbShredFilesMethod.SelectedIndex;
            }
        }

        private void cmbShredFilesMethod_DropDownClosed(object sender, EventArgs e)
        {
            lblShredFilesMethod.Focus();
        }

        private void cmbShowPassword_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                if (cmbShowPassword.SelectedIndex == 0)
                {
                    Globals.ShowPasswordByDefault = true;
                }
                else
                {
                    Globals.ShowPasswordByDefault = false;
                }
                ApplyShowPassword();
            }
        }

        private void cmbShowPassword_DropDownClosed(object sender, EventArgs e)
        {
            lblShowPassword.Focus();
        }

        private static void ApplyShowPassword()
        {
            frmKryptor mainForm = (frmKryptor)Application.OpenForms["frmKryptor"];
            mainForm.chkShowPassword.Checked = Globals.ShowPasswordByDefault;
        }

        private void cmbAutoClearPassword_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                if (cmbAutoClearPassword.SelectedIndex == 0)
                {
                    Globals.AutoClearPassword = true;
                }
                else
                {
                    Globals.AutoClearPassword = false;
                }
            }
        }

        private void cmbAutoClearPassword_DropDownClosed(object sender, EventArgs e)
        {
            lblAutoClearPassword.Focus();
        }

        private void cmbAutoClearClipboard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                switch (cmbAutoClearClipboard.SelectedIndex)
                {
                    case 0:
                        Globals.ClearClipboardInterval = 1;
                        break;
                    case 1:
                        Globals.ClearClipboardInterval = 15000;
                        break;
                    case 2:
                        Globals.ClearClipboardInterval = 30000;
                        break;
                    case 3:
                        Globals.ClearClipboardInterval = 60000;
                        break;
                    case 4:
                        Globals.ClearClipboardInterval = 90000;
                        break;
                    case 5:
                        Globals.ClearClipboardInterval = 120000;
                        break;
                }
                ApplyClearClipboardInterval();
            }
        }

        private void cmbAutoClearClipboard_DropDownClosed(object sender, EventArgs e)
        {
            lblAutoClearClipboard.Focus();
        }

        private static void ApplyClearClipboardInterval()
        {
            frmKryptor mainForm = (frmKryptor)Application.OpenForms["frmKryptor"];
            mainForm.tmrClearClipboard.Interval = Globals.ClearClipboardInterval;
        }

        private void cmbExitClipboardClear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                if (cmbExitClipboardClear.SelectedIndex == 0)
                {
                    Globals.ClearClipboardOnExit = true;
                }
                else
                {
                    Globals.ClearClipboardOnExit = false;
                }
            }
        }

        private void cmbExitClipboardClear_DropDownClosed(object sender, EventArgs e)
        {
            lblExitClipboardClear.Focus();
        }

        private void cmbCheckForUpdates_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                if (cmbCheckForUpdates.SelectedIndex == 0)
                {
                    Globals.CheckForUpdates = true;
                }
                else
                {
                    Globals.CheckForUpdates = false;
                }
            }
        }

        private void cmbCheckForUpdates_DropDownClosed(object sender, EventArgs e)
        {
            lblCheckForUpdates.Focus();
        }

        private void cmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                bool previousTheme = Globals.DarkTheme;
                if (cmbTheme.SelectedIndex == 0)
                {
                    Globals.DarkTheme = true;
                }
                else
                {
                    Globals.DarkTheme = false;
                }
                if (previousTheme != Globals.DarkTheme)
                {
                    Application.Restart();
                }
            }
        }

        private void cmbTheme_DropDownClosed(object sender, EventArgs e)
        {
            lblTheme.Focus();
        }

        private void frmSettings_FormClosing(object sender, EventArgs e)
        {
            // Write settings to file on close
            Settings.SaveSettings();
        }
    }
}
