using System;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

/*
    Kryptor: Free and open source file encryption.
    Copyright(C) 2020-2021 Samuel Lucas

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
    [HelpOption("-h|--help", ShowInHelpText = false)]
    [Command(ExtendedHelpText = @"  -h|--help       show help information

Examples:
  --encrypt -p [file]
  --encrypt [-x sender private key] [-y recipient public key] [file]
  --decrypt [-x recipient private key] [-y sender public key]  [file]
  --sign [-x private key] [-c comment] [file]
  --verify [-y public key] [signature] [file]

Please report bugs to <https://github.com/samuel-lucas6/Kryptor/issues>.

Still need help? Read the tutorial <https://kryptor.co.uk>.")]
    public class Program
    {
        [Option("-e|--encrypt", "encrypt files/folders", CommandOptionType.NoValue)]
        public bool Encrypt { get; }

        [Option("-d|--decrypt", "decrypt files/folders", CommandOptionType.NoValue)]
        public bool Decrypt { get; }

        [Option("-p|--password", "use a password", CommandOptionType.NoValue)]
        public bool Password { get; }

        [Option("-k|--keyfile", "specify a keyfile", CommandOptionType.SingleValue)]
        public string Keyfile { get; }

        [Option("-x|--private", "specify your private key", CommandOptionType.SingleOrNoValue)]
        public (bool hasValue, string value) PrivateKey { get; }

        [Option("-y|--public", "specify a public key", CommandOptionType.SingleOrNoValue)]
        public (bool hasValue, string value) PublicKey { get; }

        [Option("-f|--obfuscate", "obfuscate file names", CommandOptionType.NoValue)]
        public bool ObfuscateFileNames { get; }

        [Option("-o|--overwrite", "overwrite input files", CommandOptionType.NoValue)]
        public bool Overwrite { get; }

        [Option("-g|--generate", "generate a new key pair", CommandOptionType.NoValue)]
        public bool GenerateKeys { get; }

        [Option("-r|--recover", "recover your public key from your private key", CommandOptionType.NoValue)]
        public bool RecoverPublicKey { get; }

        [Option("-s|--sign", "create a signature", CommandOptionType.NoValue)]
        public bool Sign { get; }

        [Option("-c|--comment", "add a comment to a signature", CommandOptionType.SingleValue)]
        public string Comment { get; }

        [Option("-h|--prehash", "sign large files by prehashing", CommandOptionType.NoValue)]
        public bool Prehash { get; }

        [Option("-v|--verify", "verify a signature", CommandOptionType.NoValue)]
        public bool Verify { get; }

        [Option("-u|--update", "check for updates", CommandOptionType.NoValue)]
        public bool CheckForUpdates { get; }

        [Option("-a|--about", "view the program version and license", CommandOptionType.NoValue)]
        public bool About { get; }

        [Argument(0, Name = "file", Description = "specify a file path")]
        public string[] FilePaths { get; }

        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private void OnExecute()
        {
            Console.WriteLine();
            SetSettings(Overwrite, ObfuscateFileNames);
            if (Encrypt)
            {
                string privateKey = GetEncryptionPrivateKey(PrivateKey.value);
                CommandLine.Encrypt(Password, Keyfile, privateKey, PublicKey.value, FilePaths);
            }
            else if (Decrypt)
            {
                string privateKey = GetEncryptionPrivateKey(PrivateKey.value);
                CommandLine.Decrypt(Password, Keyfile, privateKey, PublicKey.value, FilePaths);
            }
            else if (GenerateKeys)
            {
                CommandLine.GenerateNewKeyPair((FilePaths != null) ? FilePaths[0] : Constants.DefaultKeyDirectory);
            }
            else if (RecoverPublicKey)
            {
                CommandLine.RecoverPublicKey(PrivateKey.value);
            }
            else if (Sign)
            {
                string privateKey = GetSigningPrivateKey(PrivateKey.value);
                CommandLine.Sign(privateKey, Comment, Prehash, FilePaths);
            }
            else if (Verify)
            {
                string publicKey = GetSigningPublicKey(PublicKey.value);
                CommandLine.Verify(publicKey, FilePaths);
            }
            else if (CheckForUpdates)
            {
                CommandLine.CheckForUpdates();
            }
            else if (About)
            {
                CommandLine.DisplayAbout();
            }
            else
            {
                DisplayMessage.Error("Unknown command. Specify --help for a list of options and examples.");
            }
        }

        private static void SetSettings(bool overwrite, bool obfuscateFileNames)
        {
            // Don't overwrite or obfuscate by default
            Globals.Overwrite = overwrite;
            Globals.ObfuscateFileNames = obfuscateFileNames;
        }

        private static string GetEncryptionPrivateKey(string privateKey)
        {
            return string.IsNullOrEmpty(privateKey) ? Constants.DefaultEncryptionPrivateKeyPath : privateKey;
        }

        private static string GetSigningPrivateKey(string privateKey)
        {
            return string.IsNullOrEmpty(privateKey) ? Constants.DefaultSigningPrivateKeyPath : privateKey;
        }

        private static string GetSigningPublicKey(string publicKey)
        {
            return string.IsNullOrEmpty(publicKey) ? Constants.DefaultSigningPublicKeyPath : publicKey;
        }

        public static string GetVersion()
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return assemblyVersion.Substring(startIndex: 0, assemblyVersion.Length - 2);
        }
    }
}
