using System;
using System.Collections.Generic;
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
    public static class Settings
    {
        private static readonly string _settingsFile = Path.Combine(Constants.KryptorDirectory, "settings.ini");

        public static void ReadSettings()
        {
            try
            {
                if (File.Exists(_settingsFile))
                {
                    string[] settings = File.ReadAllLines(_settingsFile);
                    var settingsList = new List<string>();
                    foreach (string line in settings)
                    {
                        string[] splitLine = line.Split(':');
                        // Ignore the name of each setting - only store settings values
                        settingsList.Add(splitLine[1]);
                    }
                    LoadSettings(settingsList);
                }
                else
                {
                    // Create settings file with default settings
                    SaveSettings();
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to read settings file.");
            }
        }

        private static void LoadSettings(List<string> settingsList)
        {
            try
            {
                LoadEncryptionAlgorithm(settingsList[0]);
                LoadMemoryEncryption(settingsList[1]);
                LoadAnonymousRename(settingsList[2]);
                LoadOverwriteFiles(settingsList[3]);
                LoadParallelism(settingsList[4]);
                LoadMemorySize(settingsList[5]);
                LoadIterations(settingsList[6]);
                LoadShowPasword(settingsList[7]);
                LoadAutoClearPassword(settingsList[8]);
                LoadClearClipboardInterval(settingsList[9]);
                LoadClearClipboardOnExit(settingsList[10]);
                LoadShredFilesMethod(settingsList[11]);
                LoadCheckForUpdates(settingsList[12]);
                LoadTheme(settingsList[13]);
            }
            catch (IndexOutOfRangeException ex)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Bug);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load some settings.");
            }
        }

        private static void LoadEncryptionAlgorithm(string setting)
        {
            try
            {
                Globals.EncryptionAlgorithm = Invariant.ToInt(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load encryption algorithm setting. The default encryption algorithm will be used.");
            }
        }

        private static void LoadMemoryEncryption(string setting)
        {
            try
            {
                Globals.MemoryEncryption = Invariant.ToBoolean(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load memory encryption setting. The default setting will be used.");
            }
        }

        private static void LoadAnonymousRename(string setting)
        {
            try
            {
                Globals.AnonymousRename = Invariant.ToBoolean(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load anonymous rename setting. The default setting will be used.");
            }
        }

        private static void LoadOverwriteFiles(string setting)
        {
            try
            {
                Globals.OverwriteFiles = Invariant.ToBoolean(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load overwrite original files setting. The default setting will be used.");
            }
        }
        private static void LoadParallelism(string setting)
        {
            try
            {
                Globals.Parallelism = Invariant.ToInt(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load Argon2 parallelism setting. The default setting will be used.");
            }
        }

        private static void LoadMemorySize(string setting)
        {
            try
            {
                Globals.MemorySize = Invariant.ToInt(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load Argon2 memory size setting. The default setting will be used.");
            }
        }

        private static void LoadIterations(string setting)
        {
            try
            {
                Globals.Iterations = Invariant.ToInt(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load Argon2 iterations setting. The default setting will be used.");
            }
        }

        private static void LoadShowPasword(string setting)
        {
            try
            {
                Globals.ShowPasswordByDefault = Invariant.ToBoolean(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load show password setting. The default setting will be used.");
            }
        }

        private static void LoadAutoClearPassword(string setting)
        {
            try
            {
                Globals.AutoClearPassword = Invariant.ToBoolean(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load auto clear password setting. The default setting will be used.");
            }
        }

        private static void LoadClearClipboardInterval(string setting)
        {
            try
            {
                Globals.ClearClipboardInterval = Invariant.ToInt(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load clear clipboard setting. The default setting will be used.");
            }
        }

        private static void LoadClearClipboardOnExit(string setting)
        {
            try
            {
                Globals.ClearClipboardOnExit = Invariant.ToBoolean(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load clear clipboard on exit setting. The default setting will be used.");
            }
        }

        private static void LoadShredFilesMethod(string setting)
        {
            try
            {
                Globals.ShredFilesMethod = Invariant.ToInt(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load shred files method setting. The default setting will be used.");
            }
        }

        private static void LoadCheckForUpdates(string setting)
        {
            try
            {
                Globals.CheckForUpdates = Invariant.ToBoolean(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load check for updates setting. The default setting will be used.");
            }
        }

        private static void LoadTheme(string setting)
        {
            try
            {
                Globals.DarkTheme = Invariant.ToBoolean(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to load theme setting. The default setting will be used.");
            }
        }

        public static void SaveSettings()
        {
            try
            {
                if (File.Exists(_settingsFile))
                {
                    var settings = new Dictionary<string, string>
                    {
                        { "Encryption Algorithm", Invariant.ToString(Globals.EncryptionAlgorithm) },
                        { "Memory Encryption", Globals.MemoryEncryption.ToString() },
                        { "Anonymous Rename", Globals.AnonymousRename.ToString() },
                        { "Overwrite Files", Globals.OverwriteFiles.ToString() },
                        { "Argon2 Parallelism", Invariant.ToString(Globals.Parallelism) },
                        { "Argon2 Memory Size", Invariant.ToString(Globals.MemorySize) },
                        { "Argon2 Iterations", Invariant.ToString(Globals.Iterations) },
                        { "Show Password", Globals.ShowPasswordByDefault.ToString() },
                        { "Auto Clear Password", Globals.AutoClearPassword.ToString() },
                        { "Auto Clear Clipboard", Invariant.ToString(Globals.ClearClipboardInterval) },
                        { "Exit Clipboard Clear", Globals.ClearClipboardOnExit.ToString() },
                        { "Shred Files Method", Invariant.ToString(Globals.ShredFilesMethod) },
                        { "Check for Updates", Globals.CheckForUpdates.ToString() },
                        { "Dark Theme", Globals.DarkTheme.ToString() }
                    };
                    using (var streamWriter = new StreamWriter(_settingsFile))
                    {
                        foreach (var keyValuePair in settings)
                        {
                            streamWriter.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value}");
                        }
                    }
                }
                else
                {
                    File.Create(_settingsFile).Close();
                    SaveSettings();
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.High);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to save settings.");
            }
        }

        public static void BackupSettings()
        {
            try
            {
                using (var settingsBackup = new VistaFolderBrowserDialog())
                {
                    settingsBackup.Description = "Settings Backup";
                    if (settingsBackup.ShowDialog() == DialogResult.OK)
                    {
                        string backupSettingsFile = Path.Combine(settingsBackup.SelectedPath, Path.GetFileName(_settingsFile));
                        File.Copy(_settingsFile, backupSettingsFile, true);
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to backup settings file.");
            }
        }

        public static void RestoreSettings()
        {
            try
            {
                using (var settingsRestore = new VistaOpenFileDialog())
                {
                    settingsRestore.Title = "Settings Restore";
                    if (settingsRestore.ShowDialog() == DialogResult.OK)
                    {
                        if (settingsRestore.FileName.EndsWith(".ini", StringComparison.Ordinal))
                        {
                            File.Copy(settingsRestore.FileName, _settingsFile, true);
                            // Reload settings
                            ReadSettings();
                            DisplayMessage.InformationMessageBox("Your settings have been restored.", "Settings Restored");
                        }
                        else
                        {
                            DisplayMessage.ErrorMessageBox("The selected file is not an INI file. Please select a 'settings.ini' file.");
                        }
                    }
                }
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to restore settings from the selected file.");
            }
        }
    }
}
