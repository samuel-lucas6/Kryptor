using System;
using System.ComponentModel;
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
    public partial class FrmSettings : Form
    {
        private static bool FormLoad { get; set; } = true;

        public FrmSettings()
        {
            InitializeComponent();
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
            SetSettingsControls();
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = DarkTheme.BackgroundColour();
            DarkTheme.GroupBoxes(grbFileEncryption);
            DarkTheme.GroupBoxes(grbKeyDerivation);
            DarkTheme.GroupBoxes(grbOtherSettings);
            DarkTheme.Labels(lblEncryptionAlgorithm);
            DarkTheme.Labels(lblMemoryEncryption);
            DarkTheme.Labels(lblAnonymousRename);
            DarkTheme.Labels(lblOverwriteFiles);
            DarkTheme.Labels(lblArgon2MemorySize);
            DarkTheme.Labels(lblArgon2Iterations);
            DarkTheme.Labels(lblShowPassword);
            DarkTheme.Labels(lblAutoClearPassword);
            DarkTheme.Labels(lblAutoClearClipboard);
            DarkTheme.Labels(lblExitClearClipboard);
            DarkTheme.Labels(lblShredFilesMethod);
            DarkTheme.Labels(lblCheckForUpdates);
            DarkTheme.Labels(lblTheme);
            DarkTheme.ComboBoxes(cmbEncryptionAlgorithm);
            DarkTheme.ComboBoxes(cmbMemoryEncryption);
            DarkTheme.ComboBoxes(cmbAnonymousRename);
            DarkTheme.ComboBoxes(cmbOverwriteFiles);
            DarkTheme.ComboBoxes(cmbAnonymousRename);
            DarkTheme.ComboBoxes(cmbShowPassword);
            DarkTheme.ComboBoxes(cmbAutoClearPassword);
            DarkTheme.ComboBoxes(cmbAutoClearClipboard);
            DarkTheme.ComboBoxes(cmbExitClearClipboard);
            DarkTheme.ComboBoxes(cmbShredFilesMethod);
            DarkTheme.ComboBoxes(cmbCheckForUpdates);
            DarkTheme.ComboBoxes(cmbTheme);
            DarkTheme.NumericUpDown(nudArgon2MemorySize);
            DarkTheme.NumericUpDown(nudArgon2Iterations);
            DarkTheme.SettingsButtons(btnArgon2Benchmark);
            DarkTheme.SettingsButtons(btnTestParameters);
        }

        private void SetSettingsControls()
        {
            // Set the value of each control
            DisplaySettings.SetEncryptionAlgorithm(cmbEncryptionAlgorithm);
            DisplaySettings.SetMemoryEncryption(cmbMemoryEncryption);
            DisplaySettings.SetAnonymousRename(cmbAnonymousRename);
            DisplaySettings.SetOverwriteFiles(cmbOverwriteFiles);
            DisplaySettings.SetMemorySize(nudArgon2MemorySize);
            DisplaySettings.SetIterations(nudArgon2Iterations);
            DisplaySettings.SetShowPassword(cmbShowPassword);
            DisplaySettings.SetAutoClearPassword(cmbAutoClearPassword);
            DisplaySettings.SetAutoClearClipboard(cmbAutoClearClipboard);
            DisplaySettings.SetExitClearClipboard(cmbExitClearClipboard);
            DisplaySettings.SetShredFilesMethod(cmbShredFilesMethod);
            DisplaySettings.SetCheckForUpdates(cmbCheckForUpdates);
            DisplaySettings.SetTheme(cmbTheme);
            FormLoad = false;
        }

        private void CmbEncryptionAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                lblEncryptionAlgorithm.Focus();
                Globals.EncryptionAlgorithm = cmbEncryptionAlgorithm.SelectedIndex;
            }
        }

        private void CmbEncryptionAlgorithm_DropDownClosed(object sender, EventArgs e)
        {
            lblEncryptionAlgorithm.Focus();
        }

        private void CmbMemoryEncryption_SelectedIndexChanged(object sender, EventArgs e)
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

        private void CmbMemoryEncryption_DropDownClosed(object sender, EventArgs e)
        {
            lblMemoryEncryption.Focus();
        }

        private void CmbAnonymousRename_SelectedIndexChanged(object sender, EventArgs e)
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

        private void CmbAnonymousRename_DropDownClosed(object sender, EventArgs e)
        {
            lblAnonymousRename.Focus();
        }

        private void CmbOverwriteFiles_SelectedIndexChanged(object sender, EventArgs e)
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

        private void CmbOverwriteFiles_DropDownClosed(object sender, EventArgs e)
        {
            lblOverwriteFiles.Focus();
        }

        private void BtnArgon2Benchmark_Click(object sender, EventArgs e)
        {
            lblArgon2MemorySize.Focus();
            RunArgon2Benchmark();
        }

        private void RunArgon2Benchmark()
        {
            foreach (Form form in Application.OpenForms)
            {
                form.Hide();
            }
            using (var argon2Benchmark = new FrmArgon2Benchmark())
            {
                argon2Benchmark.ShowDialog();
            }
            nudArgon2MemorySize.Value = Globals.MemorySize / Constants.Mebibyte;
        }

        private void BtnTestParameters_Click(object sender, EventArgs e)
        {
            lblArgon2MemorySize.Focus();
            if (!bgwTestArgon2Parameters.IsBusy)
            {
                bgwTestArgon2Parameters.RunWorkerAsync();
            }
        }

        private void BgwTestArgon2Parameters_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Argon2Benchmark.TestArgon2Parameters();
        }

        private void BgwTestArgon2Parameters_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DisplayMessage.InformationMessageBox($"{e.Result} ms delay per file.", "Argon2 Parameter Results");
            // Deallocate RAM for Argon2
            GC.Collect();
        }

        private void NudArgon2MemorySize_ValueChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                Globals.MemorySize = (int)nudArgon2MemorySize.Value * Constants.Mebibyte;
            }
        }

        private void NudArgon2Iterations_ValueChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                Globals.Iterations = (int)nudArgon2Iterations.Value;
            }
        }

        private void CmbShredFilesMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                Globals.ShredFilesMethod = cmbShredFilesMethod.SelectedIndex;
            }
        }

        private void CmbShredFilesMethod_DropDownClosed(object sender, EventArgs e)
        {
            lblShredFilesMethod.Focus();
        }

        private void CmbShowPassword_SelectedIndexChanged(object sender, EventArgs e)
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

        private void CmbShowPassword_DropDownClosed(object sender, EventArgs e)
        {
            lblShowPassword.Focus();
        }

        private static void ApplyShowPassword()
        {
            FrmFileEncryption fileEncryption = (FrmFileEncryption)Application.OpenForms["frmFileEncryption"];
            fileEncryption.chkShowPassword.Checked = Globals.ShowPasswordByDefault;
        }

        private void CmbAutoClearPassword_SelectedIndexChanged(object sender, EventArgs e)
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

        private void CmbAutoClearPassword_DropDownClosed(object sender, EventArgs e)
        {
            lblAutoClearPassword.Focus();
        }

        private void CmbAutoClearClipboard_SelectedIndexChanged(object sender, EventArgs e)
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

        private void CmbAutoClearClipboard_DropDownClosed(object sender, EventArgs e)
        {
            lblAutoClearClipboard.Focus();
        }

        private static void ApplyClearClipboardInterval()
        {
            FrmFileEncryption mainForm = (FrmFileEncryption)Application.OpenForms["frmFileEncryption"];
            mainForm.tmrClearClipboard.Interval = Globals.ClearClipboardInterval;
        }

        private void CmbExitClearClipboard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FormLoad == false)
            {
                if (cmbExitClearClipboard.SelectedIndex == 0)
                {
                    Globals.ExitClearClipboard = true;
                }
                else
                {
                    Globals.ExitClearClipboard = false;
                }
            }
        }

        private void CmbExitClearClipboard_DropDownClosed(object sender, EventArgs e)
        {
            lblExitClearClipboard.Focus();
        }

        private void CmbCheckForUpdates_SelectedIndexChanged(object sender, EventArgs e)
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

        private void CmbCheckForUpdates_DropDownClosed(object sender, EventArgs e)
        {
            lblCheckForUpdates.Focus();
        }

        private void CmbTheme_SelectedIndexChanged(object sender, EventArgs e)
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

        private void CmbTheme_DropDownClosed(object sender, EventArgs e)
        {
            lblTheme.Focus();
        }

        private void FrmSettings_FormClosing(object sender, EventArgs e)
        {
            // Write settings to file on close
            Settings.SaveSettings();
        }
    }
}
