using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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

namespace KryptorCLI
{
    public static class CommandLine
    {
        public static void ValidateFileEncryptionInput(bool encryption, char[] password, string keyfilePath, string[] filePaths)
        {
            if (filePaths != null)
            {
                bool keyfileSelected = !string.IsNullOrEmpty(keyfilePath);
                if (encryption == true && password == null && keyfileSelected == false)
                {
                    const int words = 6;
                    password = PasswordGenerator.GenerateRandomPassphrase(words, true, true);
                    DisplayRandomPassphrase(password);
                }
                if ((password != null && password.Length >= 8) || keyfileSelected)
                {
                    CallEncryption(encryption, password, keyfilePath, filePaths);
                }
                else
                {
                    Console.WriteLine("Error: Please enter a password (8+ characters long) or select/generate a keyfile.");
                }
            }
            else
            {
                Console.WriteLine("Error: Please select files/folders to encrypt/decrypt.");
            }
        }

        private static void DisplayRandomPassphrase(char[] password)
        {
            Console.Write($"Randomly generated passphrase: ");
            foreach (char character in password)
            {
                Console.Write(character);
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void CallEncryption(bool encryption, char[] password, string keyfilePath, string[] filePaths)
        {
            byte[] passwordBytes = FileEncryption.GetPasswordBytes(password);
            Utilities.ZeroArray(password);
            FileEncryption.StartEncryption(encryption, filePaths, passwordBytes, keyfilePath);
            EncryptionCompleted(encryption);
        }

        private static void EncryptionCompleted(bool encryption)
        {
            GC.Collect();
            string outputMessage;
            if (encryption)
            {
                outputMessage = "encrypted";
            }
            else
            {
                outputMessage = "decrypted";
            }
            Console.WriteLine();
            Console.WriteLine($"Successfully {outputMessage}: {Invariant.ToString(Globals.SuccessfulCount)}/{Invariant.ToString(Globals.TotalCount)}");
        }

        public static void CreateKeyfile(string filePath)
        {
            NullChecks.Strings(filePath);
            const string keyfileExtension = ".key";
            // Generate a random name if the path is a directory
            if (Directory.Exists(filePath))
            {
                string randomFileName = AnonymousRename.GenerateRandomFileName() + keyfileExtension;
                filePath = Path.Combine(filePath, randomFileName);
            }
            if (!File.Exists(filePath))
            {
                // Append .key extension if missing
                if (!filePath.EndsWith(keyfileExtension, StringComparison.InvariantCulture))
                {
                    filePath += keyfileExtension;
                }
                Keyfiles.GenerateKeyfile(filePath);
            }
            else
            {
                Console.WriteLine("Error: A file with this name already exists.");
            }
        }

        public static void GenerateRandomPassword(int length)
        {
            if (length >= 8 && length <= 128)
            {
                char[] password = PasswordGenerator.GenerateRandomPassword(length, true, true, true, true);
                Console.WriteLine(password);
                Utilities.ZeroArray(password);
            }
            else
            {
                Console.WriteLine("Error: Invalid length. Randomly generated passwords must be between 8-128 characters.");
            }
        }

        public static void GenerateRandomPassphrase(int length)
        {
            if (length >= 4 && length <= 20)
            {
                char[] passphrase = PasswordGenerator.GenerateRandomPassphrase(length, true, true);
                Console.WriteLine(passphrase);
                Utilities.ZeroArray(passphrase);
            }
            else
            {
                Console.WriteLine("Error: Invalid number of words. Randomly generated passphrases must be between 4-20 words.");
            }
        }

        public static void GenerateRecipientKeyPair()
        {
            var keyPair = PasswordSharing.GenerateKeyPair();
            Console.WriteLine($"PUBLIC KEY: {keyPair.Item1}");
            Console.WriteLine($"PRIVATE KEY: {keyPair.Item2}");
        }

        public static void ValidatePasswordSharingInput(bool encryption, string[] arguments)
        {
            if (arguments != null)
            {
                const int keyLength = 44;
                if (arguments[0].Length == keyLength)
                {
                    char[] message = PasswordSharing.ConvertUserInput(encryption, arguments[0].ToCharArray(), arguments[1].ToCharArray());
                    if (message != Array.Empty<char>())
                    {
                        Console.WriteLine(message);
                    }
                }
                else
                {
                    Console.WriteLine("Error: Invalid key length. Type the public or private key followed by the password to encrypt/decrypt.");
                }
            }
            else
            {
                Console.WriteLine("Error: Please enter a public or private key followed by the password to encrypt/decrypt.");
            }
        }

        public static void CallShredFiles(string[] filePaths)
        {
            if (filePaths != null)
            {
                ShredFiles.ShredSelectedFiles(filePaths);
            }
            else
            {
                Console.WriteLine("Error: Please select files/folders to shred.");
            }
        }

        public static void DisplaySettings(string[] arguments)
        {
            if (arguments == null)
            {
                Console.WriteLine($"encryption-algorithm: {KryptorSettings.GetCipherName()}");
                Console.WriteLine($"memory-encryption: {Globals.MemoryEncryption}");
                Console.WriteLine($"anonymous-rename: {Globals.AnonymousRename}");
                Console.WriteLine($"overwrite-files: {Globals.OverwriteFiles}");
                Console.WriteLine($"memory-size: {Globals.MemorySize / Constants.Mebibyte} MiB");
                Console.WriteLine($"iterations: {Globals.Iterations}");
                Console.WriteLine($"shred-files-method: {KryptorSettings.GetShredFilesMethod()}");
                Console.WriteLine();
                Console.WriteLine("Use --settings [setting name] [value] to change a setting.");
            }
            else
            {
                KryptorSettings.EditSettings(arguments);
            }
        }

        public static void SelectBenchmarkMode()
        {
            Console.WriteLine("Please select a benchmark mode:");
            Console.WriteLine("1) I want encryption to be as fast as possible (150 ms delay per file).");
            Console.WriteLine("2) I want encryption to be more secure (250 ms delay per file).");
            Console.WriteLine("3) Benchmark current Argon2 settings.");
            Console.WriteLine("4) Cancel the benchmark.");
            string userInput = Console.ReadLine();
            if (int.TryParse(userInput, out int benchmarkMode) && benchmarkMode >= 1 && benchmarkMode <= 4)
            {
                if (benchmarkMode == 1 || benchmarkMode == 2)
                {
                    int delayPerFile = 250;
                    if (benchmarkMode == 1)
                    {
                        delayPerFile = 150;
                    }
                    Argon2Benchmark.PerformBenchmark(delayPerFile);
                }
                else if (benchmarkMode == 3)
                {
                    Argon2Benchmark.TestArgon2Parameters();
                }
            }
            else
            {
                Console.WriteLine("Error: Invalid value - must be between 1-4.");
            }
        }

        public static void OpenDocumentation()
        {
            const string documentationLink = "https://kryptor.co.uk/Documentation.html";
            VisitLink.OpenLink(documentationLink);
        }

        public static void OpenSourceCode()
        {
            const string sourceCodeLink = "https://github.com/samuel-lucas6/Kryptor";
            VisitLink.OpenLink(sourceCodeLink);
        }

        public static void OpenDonate()
        {
            const string donateLink = "https://kryptor.co.uk/Donate.html";
            VisitLink.OpenLink(donateLink);
        }

        public static void UpdateCheck(bool displayUpToDate)
        {
            string message = Updates.UpdateKryptor(displayUpToDate);
            Console.WriteLine(message);
        }

        public static void DisplayAbout()
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine($"Kryptor {assemblyVersion.Substring(0, assemblyVersion.Length - 2)} Beta");
            Console.WriteLine("Copyright(C) 2020 Samuel Lucas");
            Console.WriteLine("License GPLv3+: GNU GPL version 3 or later <https://gnu.org/licenses/gpl.html>");
            Console.WriteLine("This is free software: you are free to change and redistribute it.");
            Console.WriteLine("There is NO WARRANTY, to the extent permitted by law.");
        }

        public static void OpenKryptorDirectory()
        {
            ProcessStartInfo kryptorDirectory = new ProcessStartInfo(Constants.KryptorDirectory)
            {
                UseShellExecute = true
            };
            Process.Start(kryptorDirectory);
        }

        public static void DisplayEasterEgg()
        {
            Console.WriteLine("Mjc1OTZtNzUyMDc3NjU3MjY1MjA2MjZtNzI2bDIwNjE3NDIwNjEyMDcwNzI2NTc0NzQ3OTIwNjM3MjYxNzA3MDc5MjA3NDY5Nms2NTIwNjk2bDIwNjg2OTczNzQ2bTcyNzkybDIwNDE2bDY0MjA2OTc0MjA2ajZtNm02aTczMjA2ajY5Nmk2NTIwNzQ2ODY5Nmw2NzczMjA2MTcyNjUyMDZtNmw2ajc5MjA2NzZtNmw2bDYxMjA2NzY1NzQyMDc3Nm03MjczNjUyMDY2NzI2bTZrMjA2ODY1NzI2NTIwNm02bDIwNm03NTc0MmwyNzBrMGgyazIwNTA2MTcyN2g2OTc2NjE2ajJqMjA1MjY1NjE2NDc5MjA1MDZqNjE3OTY1NzIyMDRtNmw2NQ==");
        }
    }
}
