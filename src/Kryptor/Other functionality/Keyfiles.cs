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
    public static class Keyfiles
    {
        public static bool CreateKeyfile()
        {
            using (var createKeyfile = new VistaSaveFileDialog())
            {
                createKeyfile.Title = "Generate Keyfile";
                createKeyfile.DefaultExt = ".key";
                createKeyfile.AddExtension = true;
                createKeyfile.FileName = AnonymousRename.GenerateRandomFileName();
                if (createKeyfile.ShowDialog() == DialogResult.OK)
                {
                    Globals.KeyfilePath = createKeyfile.FileName;
                    bool success = GenerateKeyfile(Globals.KeyfilePath);
                    return success;
                }
                else
                {
                    return false;
                }
            }
        }

        private static bool GenerateKeyfile(string filePath)
        {
            try
            {
                byte[] keyfileBytes = RandomNumberGenerator.GenerateRandomBytes(Constants.HMACKeySize);
                File.WriteAllBytes(filePath, keyfileBytes);
                File.SetAttributes(filePath, FileAttributes.ReadOnly);
                Utilities.ZeroArray(keyfileBytes);
                return true;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to generate keyfile.");
                return false;
            }
        }

        public static bool SelectKeyfile()
        {
            using (var selectKeyfile = new VistaOpenFileDialog())
            {
                selectKeyfile.Title = "Select Keyfile";
                if (selectKeyfile.ShowDialog() == DialogResult.OK)
                {
                    Globals.KeyfilePath = selectKeyfile.FileName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static byte[] ReadKeyfile(string keyfilePath)
        {
            try
            {
                File.SetAttributes(keyfilePath, FileAttributes.Normal);
                byte[] keyfileBytes = new byte[Constants.HMACKeySize];
                // Read the first 128 bytes of a keyfile
                using (var keyfile = new FileStream(keyfilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    keyfile.Read(keyfileBytes, 0, keyfileBytes.Length);
                }
                File.SetAttributes(keyfilePath, FileAttributes.ReadOnly);
                return keyfileBytes;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorResultsText(string.Empty, ex.GetType().Name, "Unable to read keyfile. The selected keyfile has not been used.");
                return null;
            }
        }
    }
}
