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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace KryptorGUI
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
                    string[] settingsLines = File.ReadAllLines(_settingsFile);
                    var settings = new List<string>();
                    foreach (string line in settingsLines)
                    {
                        string[] splitLine = line.Split(':');
                        // Ignore the name of each setting - only store values
                        settings.Add(splitLine[1]);
                    }
                    LoadSettings(settings);
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
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to read the settings file.");
            }
        }

        private static void LoadSettings(List<string> settings)
        {
            try
            {
                Globals.MemoryEncryption = LoadBooleanSetting(settings[0]) ?? Globals.MemoryEncryption;
                Globals.AnonymousRename = LoadBooleanSetting(settings[1]) ?? Globals.AnonymousRename;
                Globals.OverwriteFiles = LoadBooleanSetting(settings[2]) ?? Globals.OverwriteFiles;
                Globals.MemorySize = LoadIntegerSetting(settings[3]) ?? Globals.MemorySize;
                Globals.Iterations = LoadIntegerSetting(settings[4]) ?? Globals.Iterations;
                Globals.ShowPasswordByDefault = LoadBooleanSetting(settings[5]) ?? Globals.ShowPasswordByDefault;
                Globals.AutoClearPassword = LoadBooleanSetting(settings[6]) ?? Globals.AutoClearPassword;
                Globals.ClearClipboardInterval = LoadIntegerSetting(settings[7]) ?? Globals.ClearClipboardInterval;
                Globals.ExitClearClipboard = LoadBooleanSetting(settings[8]) ?? Globals.ExitClearClipboard;
                Globals.CheckForUpdates = LoadBooleanSetting(settings[9]) ?? Globals.CheckForUpdates;
                Globals.DarkTheme = LoadBooleanSetting(settings[10]) ?? Globals.DarkTheme;
            }
            catch (IndexOutOfRangeException ex)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Bug);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Error loading settings.");
            }
        }

        private static int? LoadIntegerSetting(string setting)
        {
            try
            {
                return Invariant.ToInt(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, $"Unable to load {setting} setting. The default setting will be used.");
                return null;
            }
        }

        private static bool? LoadBooleanSetting(string setting)
        {
            try
            {
                return Invariant.ToBoolean(setting);
            }
            catch (Exception ex) when (ExceptionFilters.SettingsExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Low);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, $"Unable to load {setting} setting. The default setting will be used.");
                return null;
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
                        { "Memory Encryption", Globals.MemoryEncryption.ToString() },
                        { "Anonymous Rename", Globals.AnonymousRename.ToString() },
                        { "Overwrite Files", Globals.OverwriteFiles.ToString() },
                        { "Argon2 Memory Size", Invariant.ToString(Globals.MemorySize) },
                        { "Argon2 Iterations", Invariant.ToString(Globals.Iterations) },
                        { "Show Password", Globals.ShowPasswordByDefault.ToString() },
                        { "Auto Clear Password", Globals.AutoClearPassword.ToString() },
                        { "Auto Clear Clipboard", Invariant.ToString(Globals.ClearClipboardInterval) },
                        { "Exit Clipboard Clear", Globals.ExitClearClipboard.ToString() },
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
                using (var selectFolderDialog = new VistaFolderBrowserDialog())
                {
                    selectFolderDialog.Description = "Settings Backup";
                    if (selectFolderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string backupFile = Path.Combine(selectFolderDialog.SelectedPath, Path.GetFileName(_settingsFile));
                        File.Copy(_settingsFile, backupFile, true);
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
                using (var selectFileDialog = new VistaOpenFileDialog())
                {
                    selectFileDialog.Title = "Settings Restore";
                    if (selectFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (selectFileDialog.FileName.EndsWith(".ini", StringComparison.Ordinal))
                        {
                            File.Copy(selectFileDialog.FileName, _settingsFile, true);
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
