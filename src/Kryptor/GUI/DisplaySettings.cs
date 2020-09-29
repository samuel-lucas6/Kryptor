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
    public static class DisplaySettings
    {
        public static void SetEncryptionAlgorithm(ComboBox cmbEncryptionAlgorithm)
        {
            try
            {
                NullChecks.ComboBoxes(cmbEncryptionAlgorithm);
                cmbEncryptionAlgorithm.SelectedIndex = Globals.EncryptionAlgorithm;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid 'Encryption Algorithm' setting. The default setting will be used instead.");
                Globals.EncryptionAlgorithm = (int)Cipher.XChaCha20;
                Settings.SaveSettings();
                SetEncryptionAlgorithm(cmbEncryptionAlgorithm);
            }
        }

        public static void SetMemoryEncryption(ComboBox cmbMemoryEncryption)
        {
            NullChecks.ComboBoxes(cmbMemoryEncryption);
            if (Globals.MemoryEncryption == true)
            {
                cmbMemoryEncryption.SelectedIndex = 0;
            }
            else
            {
                cmbMemoryEncryption.SelectedIndex = 1;
            }
        }

        public static void SetAnonymousRename(ComboBox cmbAnonymousRename)
        {
            NullChecks.ComboBoxes(cmbAnonymousRename);
            if (Globals.AnonymousRename == true)
            {
                cmbAnonymousRename.SelectedIndex = 0;
            }
            else
            {
                cmbAnonymousRename.SelectedIndex = 1;
            }
        }

        public static void SetOverwriteFiles(ComboBox cmbOverwriteFiles)
        {
            NullChecks.ComboBoxes(cmbOverwriteFiles);
            if (Globals.OverwriteFiles == true)
            {
                cmbOverwriteFiles.SelectedIndex = 0;
            }
            else
            {
                cmbOverwriteFiles.SelectedIndex = 1;
            }
        }

        public static void SetMemorySize(NumericUpDown nudArgon2MemorySize)
        {
            try
            {
                NullChecks.NumericUpDowns(nudArgon2MemorySize);
                nudArgon2MemorySize.Value = Globals.MemorySize / Constants.Mebibyte;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid 'Memory Size' setting. The default setting will be used instead.");
                Globals.MemorySize = Constants.DefaultMemorySize;
                Settings.SaveSettings();
                SetMemorySize(nudArgon2MemorySize);
            }
        }

        public static void SetIterations(NumericUpDown nudArgon2Iterations)
        {
            try
            {
                NullChecks.NumericUpDowns(nudArgon2Iterations);
                nudArgon2Iterations.Value = Globals.Iterations;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid 'Iterations' setting. The default setting will be used instead.");
                Globals.Iterations = Constants.DefaultIterations;
                Settings.SaveSettings();
                SetIterations(nudArgon2Iterations);
            }
        }

        public static void SetShowPassword(ComboBox cmbShowPassword)
        {
            NullChecks.ComboBoxes(cmbShowPassword);
            if (Globals.ShowPasswordByDefault == true)
            {
                cmbShowPassword.SelectedIndex = 0;
            }
            else
            {
                cmbShowPassword.SelectedIndex = 1;
            }
        }

        public static void SetAutoClearPassword(ComboBox cmbShredFilesMethod)
        {
            NullChecks.ComboBoxes(cmbShredFilesMethod);
            if (Globals.AutoClearPassword == true)
            {
                cmbShredFilesMethod.SelectedIndex = 0;
            }
            else
            {
                cmbShredFilesMethod.SelectedIndex = 1;
            }
        }

        public static void SetAutoClearClipboard(ComboBox cmbAutoClearClipboard)
        {
            NullChecks.ComboBoxes(cmbAutoClearClipboard);
            if (Globals.ClearClipboardInterval == 15000)
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
            else
            {
                Globals.ClearClipboardInterval = 1;
                cmbAutoClearClipboard.SelectedIndex = 0;
            }
        }

        public static void SetExitClearClipboard(ComboBox cmbExitClearClipboard)
        {
            NullChecks.ComboBoxes(cmbExitClearClipboard);
            if (Globals.ExitClearClipboard == true)
            {
                cmbExitClearClipboard.SelectedIndex = 0;
            }
            else
            {
                cmbExitClearClipboard.SelectedIndex = 1;
            }
        }

        public static void SetShredFilesMethod(ComboBox cmbShredFilesMethod)
        {
            try
            {
                NullChecks.ComboBoxes(cmbShredFilesMethod);
                cmbShredFilesMethod.SelectedIndex = Globals.ShredFilesMethod;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Invalid 'Shred Files Method' setting. The default setting will be used instead.");
                Globals.ShredFilesMethod = 2;
                Settings.SaveSettings();
                SetShredFilesMethod(cmbShredFilesMethod);
            }
        }

        public static void SetCheckForUpdates(ComboBox cmbCheckForUpdates)
        {
            NullChecks.ComboBoxes(cmbCheckForUpdates);
            if (Globals.CheckForUpdates == true)
            {
                cmbCheckForUpdates.SelectedIndex = 0;
            }
            else
            {
                cmbCheckForUpdates.SelectedIndex = 1;
            }
        }

        public static void SetTheme(ComboBox cmbTheme)
        {
            NullChecks.ComboBoxes(cmbTheme);
            if (Globals.DarkTheme == true)
            {
                cmbTheme.SelectedIndex = 0;
            }
            else
            {
                cmbTheme.SelectedIndex = 1;
            }
        }
    }
}
