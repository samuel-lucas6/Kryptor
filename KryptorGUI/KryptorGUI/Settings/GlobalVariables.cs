using System.Collections.Generic;

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
    public static class Globals
    {
        // File Encryption/Shred Files
        private static List<string> _selectedFiles;

        public static List<string> GetSelectedFiles()
        {
            return _selectedFiles;
        }

        public static void SetSelectedFiles(List<string> value)
        {
            _selectedFiles = value;
        }

        public static string ResultsText { get; set; } = string.Empty;
        public static int SuccessfulCount { get; set; }
        public static int TotalCount { get; set; }
        public static string KeyfilePath { get; set; } = string.Empty;

        // Settings
        public static int EncryptionAlgorithm { get; set; } = (int)Cipher.XChaCha20;
        public static bool MemoryEncryption { get; set; } = true;
        public static bool AnonymousRename { get; set; } = true;
        public static bool OverwriteFiles { get; set; } = true;
        public static int MemorySize { get; set; } = Constants.DefaultMemorySize;
        public static int Iterations { get; set; } = Constants.DefaultIterations;
        public static int ShredFilesMethod { get; set; } = 2; // Default to 1 pass of pseudorandom data
        public static bool ShowPasswordByDefault { get; set; } = true;
        public static bool AutoClearPassword { get; set; } = true;
        public static int ClearClipboardInterval { get; set; } = 30000; // Default to 30 seconds
        public static bool ExitClearClipboard { get; set; } // Defaults to false
        public static bool CheckForUpdates { get; set; } = true;
        public static bool DarkTheme { get; set; } = true;
    }
}
