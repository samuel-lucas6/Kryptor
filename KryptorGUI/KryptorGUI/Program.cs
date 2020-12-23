using System;
using System.IO;
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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace KryptorGUI
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            CheckArguments(args);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RunKryptor();
            ExitClearClipboard();
        }

        private static void CheckArguments(string[] args)
        {
            if (args.Length > 0)
            {
                if (!args.Contains(Application.ExecutablePath))
                {
                    // Select clicked encrypted file
                    Globals.SetSelectedFiles(args.ToList());
                }
            }
        }

        private static void RunKryptor()
        {
            // Perform Argon2 benchmark if first run 
            string firstRunPath = Path.Combine(Constants.KryptorDirectory, "first run.tmp");
            if (File.Exists(firstRunPath))
            {
                Application.Run(new FrmArgon2Benchmark());
            }
            Application.Run(new FrmFileEncryption());
        }

        private static void ExitClearClipboard()
        {
            if (Globals.ExitClearClipboard == true)
            {
                EditClipboard.ClearClipboard();
            }
        }
    }
}
