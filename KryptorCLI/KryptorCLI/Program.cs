using McMaster.Extensions.CommandLineUtils;
using System;

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
    [HelpOption("-h|--help", ShowInHelpText = false)]
    [Command(ExtendedHelpText = @"  -h|--help              show help information

Examples:
  -e -p [password] [filepath]
  -e -k [keyfile path] [filepath]
  -e -p [password] -k [keyfile path] [filepath]
  --generate-password [length]
  --encrypt-password [public key] [password]
  --settings encryption-algorithm [value]

Please report bugs to <https://github.com/samuel-lucas6/Kryptor/issues>.")]
    public class Program
    {
        [Option("-e|--encrypt", "encrypt files/folders", CommandOptionType.NoValue)]
        public bool Encrypt { get; }

        [Option("-d|--decrypt", "decrypt files/folders", CommandOptionType.NoValue)]
        public bool Decrypt { get; }

        [Option("-p|--password", "specify a password for file encryption/decryption", CommandOptionType.SingleValue)]
        public string Password { get; }

        [Option("-k|--keyfile", "specify a keyfile for file encryption/decryption", CommandOptionType.SingleValue)]
        public string Keyfile { get; }

        [Option("--generate-keyfile", "generate a random keyfile at the specified path", CommandOptionType.SingleValue)]
        public string GenerateKeyfile { get; }

        [Option("--generate-password", "generate a random password of a specified length", CommandOptionType.SingleValue)]
        public int GeneratePassword { get; }

        [Option("--generate-passphrase", "generate a random passphrase of a specified length", CommandOptionType.SingleValue)]
        public int GeneratePassphrase { get; }

        [Option("--generate-keys", "generate a recipient key pair for password sharing", CommandOptionType.NoValue)]
        public bool GenerateKeys { get; }

        [Option("--encrypt-password", "encrypt a password using the recipient's public key", CommandOptionType.NoValue)]
        public bool EncryptPassword { get; }

        [Option("--decrypt-password", "decrypt a ciphertext password using your private key", CommandOptionType.NoValue)]
        public bool DecryptPassword { get; }

        [Option("--shred", "shred files/folders", CommandOptionType.NoValue)]
        public bool Shred { get; }

        [Option("--settings", "view/edit your settings", CommandOptionType.NoValue)]
        public bool Settings { get; }

        [Option("--benchmark", "perform the Argon2 benchmark", CommandOptionType.NoValue)]
        public bool Benchmark { get; }

        [Option("--documentation", "view the documentation", CommandOptionType.NoValue)]
        public bool Documentation { get; }

        [Option("--source", "view the source code", CommandOptionType.NoValue)]
        public bool SourceCode { get; }

        [Option("--donate", "find out how to donate", CommandOptionType.NoValue)]
        public bool Donate { get; }

        [Option("--update", "check for updates", CommandOptionType.NoValue)]
        public bool CheckForUpdates { get; }

        [Option("--about", "view the program version and license", CommandOptionType.NoValue)]
        public bool About { get; }

        [Option("--error-log", CommandOptionType.NoValue, ShowInHelpText = false)]
        public bool ErrorLog { get; }

        [Option("--easter-egg", CommandOptionType.NoValue, ShowInHelpText = false)]
        public bool EasterEgg { get; }

        [Argument(0)]
        public string[] Arguments { get; }

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            Console.WriteLine();
            KryptorSettings.ReadSettings();
            if (Encrypt)
            {
                bool encryption = true;
                char[] password = ConvertPassword();
                CommandLine.ValidateFileEncryptionInput(encryption, password, Keyfile, Arguments);
            }
            else if (Decrypt)   
            {
                bool encryption = false;
                char[] password = ConvertPassword();
                CommandLine.ValidateFileEncryptionInput(encryption, password, Keyfile, Arguments);
            }
            else if (!string.IsNullOrEmpty(GenerateKeyfile))
            {
                CommandLine.CreateKeyfile(GenerateKeyfile);
            }
            else if (GeneratePassword != 0)
            {
                CommandLine.GenerateRandomPassword(GeneratePassword);
            }
            else if (GeneratePassphrase != 0)
            {
                CommandLine.GenerateRandomPassphrase(GeneratePassphrase);
            }
            else if (GenerateKeys)
            {
                CommandLine.GenerateRecipientKeyPair();
            }
            else if (EncryptPassword)
            {
                bool encryption = true;
                CommandLine.ValidatePasswordSharingInput(encryption, Arguments);
            }
            else if (DecryptPassword)
            {
                bool encryption = false;
                CommandLine.ValidatePasswordSharingInput(encryption, Arguments);
            }
            else if (Shred)
            {
                CommandLine.CallShredFiles(Arguments);
            }
            else if (Settings)
            {
                CommandLine.DisplaySettings(Arguments);
            }
            else if (Benchmark)
            {
                CommandLine.SelectBenchmarkMode();
            }
            else if (Documentation)
            {
                CommandLine.OpenDocumentation();
            }
            else if (SourceCode)
            {
                CommandLine.OpenSourceCode();
            }
            else if (Donate)
            {
                CommandLine.OpenDonate();
            }
            else if (CheckForUpdates)
            {
                CommandLine.UpdateCheck(displayUpToDate:true);
            }
            else if (About)
            {
                CommandLine.DisplayAbout();
            }
            else if (ErrorLog)
            {
                CommandLine.OpenKryptorDirectory();
            }
            else if (EasterEgg)
            {
                CommandLine.DisplayEasterEgg();
            }
            else
            {
                Console.WriteLine("Error: Unrecognized option. Specify --help for a list of available options and commands.");
            }
        }

        private char[] ConvertPassword()
        {
            // Avoid unnecessary string copies of the password
            char[] password = null;
            if (!string.IsNullOrEmpty(Password))
            {
                password = Password.ToCharArray();
            }
            return password;
        }
    }
}
