/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
    Copyright (C) 2020-2023 Samuel Lucas

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
using System.Text;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace Kryptor;

[HelpOption("-h|--help", ShowInHelpText = false)]
[Command(ExtendedHelpText = @"
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
    [Option("-e|--encrypt", "encrypt files/directories", CommandOptionType.NoValue)]
    private bool Encrypt { get; }

    [Option("-d|--decrypt", "decrypt files/directories", CommandOptionType.NoValue)]
    private bool Decrypt { get; }

    [Option("-p|--passphrase", "specify a passphrase (empty for interactive entry)", CommandOptionType.SingleOrNoValue)]
    private (bool optionSpecified, string value) Passphrase { get; }

    [Option("-k|--key", "specify or randomly generate a symmetric key or keyfile", CommandOptionType.SingleValue)]
    private string SymmetricKey { get; }

    [Option("-x|--private", "specify your private key (unused or empty for default key)", CommandOptionType.SingleOrNoValue)]
    private (bool optionSpecified, string value) PrivateKey { get; }

    [Option("-y|--public", "specify a public key", CommandOptionType.MultipleValue)]
    private string[] PublicKeys { get; }

    [Option("-n|--names", "encrypt file/directory names", CommandOptionType.NoValue)]
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

    [Option("-t|--signature", "specify a signature file (unused for default name)", CommandOptionType.MultipleValue)]
    private string[] Signatures { get; }

    [Option("-u|--update", "check for updates", CommandOptionType.NoValue)]
    private bool Update { get; }

    [Option("-a|--about", "view the program version and license", CommandOptionType.NoValue)]
    private bool About { get; }

    [Argument(order: 0, Name = "file", Description = "specify a file/directory path")]
    private string[] FilePaths { get; }

    public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

    private int OnExecute()
    {
        ExtractVisualCRuntime();
        Globals.Overwrite = Overwrite;
        Globals.EncryptFileNames = EncryptFileNames;
        Globals.TotalCount = FilePaths?.Length ?? 0;
        Console.WriteLine();
        try
        {
            if (GenerateKeys) {
                CommandLine.GenerateNewKeyPair(FilePaths?[0] ?? Constants.DefaultKeyDirectory, GetPassphrase(Passphrase.value), Comment, Encrypt, Sign);
            }
            else if (Encrypt) {
                CommandLine.Encrypt(Passphrase.optionSpecified, GetPassphrase(Passphrase.value), SymmetricKey, PrivateKey.optionSpecified, GetEncryptionPrivateKey(PrivateKey.value), PublicKeys, FilePaths);
            }
            else if (Decrypt) {
                CommandLine.Decrypt(Passphrase.optionSpecified, GetPassphrase(Passphrase.value), SymmetricKey, PrivateKey.optionSpecified, GetEncryptionPrivateKey(PrivateKey.value), PublicKeys, FilePaths);
            }
            else if (RecoverPublicKey) {
                CommandLine.RecoverPublicKey(PrivateKey.value, GetPassphrase(Passphrase.value));
            }
            else if (Sign) {
                CommandLine.Sign(GetSigningPrivateKey(PrivateKey.value), GetPassphrase(Passphrase.value), Comment, Prehash, Signatures, FilePaths);
            }
            else if (Verify) {
                CommandLine.Verify(PublicKeys, Signatures, FilePaths);
            }
            else if (Update) {
                CommandLine.CheckForUpdates();
            }
            else if (About) {
                DisplayMessage.About();
            }
            else {
                DisplayMessage.Error("Unknown command. Specify -h|--help for a list of options and examples.");
            }
        }
        catch (Exception ex) when (ex is UserInputException or PlatformNotSupportedException or InsufficientMemoryException)
        {
            if (ex is PlatformNotSupportedException) {
                DisplayMessage.Exception(ex.GetType().Name, "Missing the Microsoft Visual C++ Redistributable for Visual Studio 2015-2022 on Windows. Please install this runtime or move the Kryptor executable to a directory that doesn't require administrative privileges.");
            }
            else if (ex is InsufficientMemoryException) {
                DisplayMessage.Exception(ex.GetType().Name, "This machine doesn't have enough memory to perform password-based key derivation.");
            }
        }
        return Environment.ExitCode;
    }
    
    private static void ExtractVisualCRuntime()
    {
        try
        {
            string vcruntimeFilePath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "vcruntime140.dll");
            if (!OperatingSystem.IsWindows() || File.Exists(vcruntimeFilePath)) {
                return;
            }
            if (Environment.Is64BitOperatingSystem) {
                File.WriteAllBytes(vcruntimeFilePath, Properties.Resources.vcruntime140x64);
                return;
            }
            File.WriteAllBytes(vcruntimeFilePath, Properties.Resources.vcruntime140x86);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
        }
    }

    private static Span<byte> GetPassphrase(string passphrase) => string.IsNullOrEmpty(passphrase) ? Span<byte>.Empty : Encoding.UTF8.GetBytes(passphrase);

    private static string GetEncryptionPrivateKey(string privateKeyPath) => string.IsNullOrEmpty(privateKeyPath) ? Constants.DefaultEncryptionPrivateKeyPath : privateKeyPath;

    private static string GetSigningPrivateKey(string privateKeyPath) => string.IsNullOrEmpty(privateKeyPath) ? Constants.DefaultSigningPrivateKeyPath : privateKeyPath;

    public static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString(fieldCount: 3);
}