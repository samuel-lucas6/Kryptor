using System;
using System.Collections.Generic;
using System.IO;

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

namespace KryptorCLI
{
    public static class KryptorSettings
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
                DisplayMessage.Error(ex.GetType().Name, "Unable to read the settings file.");
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
            }
            catch (IndexOutOfRangeException ex)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Bug);
                DisplayMessage.Error(ex.GetType().Name, "Error loading settings.");
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
                DisplayMessage.Error(ex.GetType().Name, $"Unable to convert {setting} setting to integer.");
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
                DisplayMessage.Error(ex.GetType().Name, $"Unable to convert {setting} setting to boolean.");
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
                    };
                    using var streamWriter = new StreamWriter(_settingsFile);
                    foreach (var keyValuePair in settings)
                    {
                        streamWriter.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value}");
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
                DisplayMessage.Error(ex.GetType().Name, "Unable to save settings.");
            }
        }

        public static void EditSettings(string[] arguments)
        {
            if (arguments != null && !string.IsNullOrEmpty(arguments[0]))
            {
                if (!string.IsNullOrEmpty(arguments[1]))
                {
                    Globals.MemoryEncryption = SetBooleanSetting("memory-encryption".ToUpperInvariant(), arguments) ?? Globals.MemoryEncryption;
                    Globals.AnonymousRename = SetBooleanSetting("anonymous-rename".ToUpperInvariant(), arguments) ?? Globals.AnonymousRename;
                    Globals.OverwriteFiles = SetBooleanSetting("overwrite-files".ToUpperInvariant(), arguments) ?? Globals.OverwriteFiles;
                    Globals.MemorySize = SetIntegerSetting("memory-size".ToUpperInvariant(), arguments, 32, 500) * Constants.Mebibyte ?? Globals.MemorySize;
                    Globals.Iterations = SetIntegerSetting("iterations".ToUpperInvariant(), arguments, 3, 128) ?? Globals.Iterations;
                    SaveSettings();
                }
                else
                {
                    Console.WriteLine("Error: No value has been specified.");
                }
            }
        }

        private static int? SetIntegerSetting(string argument, string[] arguments, int minimum, int maximum)
        {
            if (string.Equals(arguments[0].ToUpperInvariant(), argument))
            {
                int? setting = LoadIntegerSetting(arguments[1]);
                if (setting != null && setting >= minimum && setting <= maximum)
                {
                    Console.WriteLine("This setting has been changed.");
                    return setting;
                }
                else
                {
                    Console.WriteLine($"Error: Invalid value - must be between {minimum}-{maximum}.");
                }
            }
            return null;
        }

        private static bool? SetBooleanSetting(string argument, string[] arguments)
        {
            if (string.Equals(arguments[0].ToUpperInvariant(), argument))
            {
                bool? setting = LoadBooleanSetting(arguments[1]);
                if (setting != null)
                {
                    Console.WriteLine("This setting has been changed.");
                    return setting;
                }
                else
                {
                    Console.WriteLine("Error: Invalid value - must be True or False.");
                }
            }
            return null;
        }
    }
}
