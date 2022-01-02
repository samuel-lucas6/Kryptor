/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2022 Samuel Lucas

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

using System;
using System.IO;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace KryptorCLI;

[HelpOption("-h|--help", ShowInHelpText = false)]
[Command(ExtendedHelpText = @"  -h|--help       show help information

Examples:
  --encrypt [file]
  --encrypt -p [file]
  --encrypt [-y recipient's public key] [file]
  --decrypt [-y sender's public key] [file]
  --sign [-c comment] [file]
  --verify [-y public key] [file]

File names/paths that contain a space must be surrounded by ""speech marks"".

Stuck? Read the tutorial at <https://www.kryptor.co.uk/tutorial>.

Please report bugs at <https://github.com/samuel-lucas6/Kryptor/issues>.")]
public class Program
{
    [Option("-e|--encrypt", "encrypt files/folders", CommandOptionType.NoValue)]
    private bool Encrypt { get; }

    [Option("-d|--decrypt", "decrypt files/folders", CommandOptionType.NoValue)]
    private bool Decrypt { get; }

    [Option("-p|--password", "specify a password (empty for interactive entry)", CommandOptionType.SingleOrNoValue)]
    private (bool hasValue, string value) Password { get; }

    [Option("-k|--keyfile", "specify or randomly generate a keyfile", CommandOptionType.SingleValue)]
    private string Keyfile { get; }

    [Option("-x|--private", "specify your private key (unused or empty for default key)", CommandOptionType.SingleOrNoValue)]
    private (bool hasValue, string value) PrivateKey { get; }

    [Option("-y|--public", "specify a public key", CommandOptionType.SingleValue)]
    private string PublicKey { get; }

    [Option("-n|--names", "encrypt file/folder names", CommandOptionType.NoValue)]
    private bool EncryptFileNames { get; }

    [Option("-o|--overwrite", "overwrite files", CommandOptionType.NoValue)]
    private bool Overwrite { get; }

    [Option("-g|--generate", "generate a new key pair", CommandOptionType.NoValue)]
    private bool GenerateKeys { get; }

    [Option("-r|--recover", "recover your public key from your private key", CommandOptionType.NoValue)]
    private bool RecoverPublicKey { get; }

    [Option("-s|--sign", "create a signature", CommandOptionType.NoValue)]
    private bool Sign { get; }

    [Option("-c|--comment", "add a comment to a signature", CommandOptionType.SingleValue)]
    private string Comment { get; }

    [Option("-l|--prehash", "sign large files by prehashing", CommandOptionType.NoValue)]
    private bool Prehash { get; }

    [Option("-v|--verify", "verify a signature", CommandOptionType.NoValue)]
    private bool Verify { get; }

    [Option("-t|--signature", "specify a signature file (unused for default name)", CommandOptionType.SingleValue)]
    private string Signature { get; }

    [Option("-u|--update", "check for updates", CommandOptionType.NoValue)]
    private bool CheckForUpdates { get; }

    [Option("-a|--about", "view the program version and license", CommandOptionType.NoValue)]
    private bool About { get; }

    [Argument(0, Name = "file", Description = "specify a file/folder path")]
    private string[] FilePaths { get; }

    public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

    private void OnExecute()
    {
        ExtractVisualCRuntime();
        Globals.Overwrite = Overwrite;
        Globals.EncryptFileNames = EncryptFileNames;
        Globals.TotalCount = FilePaths?.Length ?? 0;
        Console.WriteLine();
        if (Encrypt)
        {
            CommandLine.Encrypt((Password.hasValue, GetPassword(Password.value)), Keyfile, (PrivateKey.hasValue, GetEncryptionPrivateKey(PrivateKey.value)), PublicKey, FilePaths);
        }
        else if (Decrypt)
        {
            CommandLine.Decrypt((Password.hasValue, GetPassword(Password.value)), Keyfile, (PrivateKey.hasValue, GetEncryptionPrivateKey(PrivateKey.value)), PublicKey, FilePaths);
        }
        else if (GenerateKeys)
        {
            CommandLine.GenerateNewKeyPair(GetPassword(Password.value), FilePaths == null ? Constants.DefaultKeyDirectory : FilePaths[0]);
        }
        else if (RecoverPublicKey)
        {
            CommandLine.RecoverPublicKey(PrivateKey.value, GetPassword(Password.value));
        }
        else if (Sign)
        {
            CommandLine.Sign(GetSigningPrivateKey(PrivateKey.value), GetPassword(Password.value), Comment, Prehash, Signature, FilePaths);
        }
        else if (Verify)
        {
            CommandLine.Verify(PublicKey, Signature, FilePaths);
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
            DisplayMessage.Error("Unknown command. Please specify -h|--help for a list of options and examples.");
        }
    }
    
    private static void ExtractVisualCRuntime()
    {
        try
        {
            string vcruntimeFilePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "vcruntime140.dll");
            if (!OperatingSystem.IsWindows() || File.Exists(vcruntimeFilePath)) { return; }
            if (Environment.Is64BitOperatingSystem)
            {
                File.WriteAllBytes(vcruntimeFilePath, Properties.Resources.vcruntime140x64);
                return;
            }
            File.WriteAllBytes(vcruntimeFilePath, Properties.Resources.vcruntime140x86);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, "Unable to extract the 'vcruntime140.dll' file, which is required for the libsodium library to function on Windows.");
        }
    }

    private static char[] GetPassword(string password) => string.IsNullOrEmpty(password) ? Array.Empty<char>() : password.ToCharArray();

    private static string GetEncryptionPrivateKey(string privateKey) => string.IsNullOrEmpty(privateKey) ? Constants.DefaultEncryptionPrivateKeyPath : privateKey;

    private static string GetSigningPrivateKey(string privateKey) => string.IsNullOrEmpty(privateKey) ? Constants.DefaultSigningPrivateKeyPath : privateKey;

    public static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString()[..^2];
}