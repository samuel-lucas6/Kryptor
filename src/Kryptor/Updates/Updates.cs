/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
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
using System.Net;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Kryptor;

public static class Updates
{
    private static readonly string LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private const string ExecutableFileName = "kryptor";
    private const string WindowsExecutableFileName = $"{ExecutableFileName}.exe";

    public static bool CheckForUpdates(out string latestVersion)
    {
        string assemblyVersion = Program.GetVersion();
        latestVersion = GetLatestVersion();
        return new Version(latestVersion).CompareTo(new Version(assemblyVersion)) > 0;
    }

    private static string GetLatestVersion()
    {
        const string versionFileLink = "https://raw.githubusercontent.com/samuel-lucas6/Kryptor/master/version.txt";
        string downloadFilePath = Path.Combine(LocalAppDataPath, Path.GetFileName(versionFileLink));
        string signatureFilePath = downloadFilePath + Constants.SignatureExtension;
        
        DownloadFile(versionFileLink + Constants.SignatureExtension, signatureFilePath);
        DownloadFile(versionFileLink, downloadFilePath);
        
        string latestVersion = File.ReadAllText(downloadFilePath).Trim('\n').Trim();
        VerifyDownloadSignature(signatureFilePath, downloadFilePath, latestVersion);
        File.Delete(downloadFilePath);
        return latestVersion;
    }

    private static void DownloadFile(string link, string filePath)
    {
        using var webClient = new WebClient();
        webClient.DownloadFile(link, filePath);
    }

    public static void Update(string latestVersion)
    {
        if (RuntimeInformation.OSArchitecture is not (Architecture.X64 or Architecture.Arm64) || !OperatingSystem.IsWindows() & !OperatingSystem.IsLinux() & !OperatingSystem.IsMacOS()) {
            throw new PlatformNotSupportedException("There are no official releases for your operating system/architecture.");
        }
        Console.WriteLine("Downloading update...");
        byte[] downloadedExecutable = GetLatestExecutable(latestVersion);
        Console.WriteLine("Applying update...");
        ReplaceExecutable(downloadedExecutable);
    }

    private static byte[] GetLatestExecutable(string latestVersion)
    {
        string downloadFilePath = Path.Combine(LocalAppDataPath, "kryptor-latest.zip");
        string downloadLink = GetReleaseDownloadLink(latestVersion);
        string signatureFilePath = downloadFilePath + Constants.SignatureExtension;
        
        DownloadFile(downloadLink + Constants.SignatureExtension, signatureFilePath);
        DownloadFile(downloadLink, downloadFilePath);
        VerifyDownloadSignature(signatureFilePath, downloadFilePath, latestVersion);
        
        string extractedDirectoryPath = Path.Combine(LocalAppDataPath, ExecutableFileName);
        if (!Directory.Exists(extractedDirectoryPath)) {
            Directory.CreateDirectory(extractedDirectoryPath);
        }
        ZipFile.ExtractToDirectory(downloadFilePath, extractedDirectoryPath, overwriteFiles: true);
        File.Delete(downloadFilePath);
        
        string executableFilePath = Path.Combine(extractedDirectoryPath, OperatingSystem.IsWindows() ? WindowsExecutableFileName : ExecutableFileName);
        byte[] downloadedExecutable = File.ReadAllBytes(executableFilePath);
        Directory.Delete(extractedDirectoryPath, recursive: true);
        return downloadedExecutable;
    }

    private static string GetReleaseDownloadLink(string latestVersion)
    {
        string downloadLink = $"https://github.com/samuel-lucas6/Kryptor/releases/download/v{latestVersion}/";
        if (OperatingSystem.IsWindows()) { return downloadLink + "kryptor-windows-amd64.zip"; }
        if (OperatingSystem.IsLinux() && RuntimeInformation.OSArchitecture == Architecture.X64) { return downloadLink + "kryptor-linux-amd64.zip"; }
        if (OperatingSystem.IsLinux() && RuntimeInformation.OSArchitecture == Architecture.Arm64) { return downloadLink + "kryptor-linux-arm64.zip"; }
        if (OperatingSystem.IsMacOS() && RuntimeInformation.OSArchitecture == Architecture.X64) { return downloadLink + "kryptor-macos-amd64.zip"; }
        if (OperatingSystem.IsMacOS() && RuntimeInformation.OSArchitecture == Architecture.Arm64) { return downloadLink + "kryptor-macos-arm64.zip"; }
        return null;
    }

    private static void VerifyDownloadSignature(string signatureFilePath, string downloadFilePath, string latestVersion)
    {
        Span<byte> publicKey = AsymmetricKeyValidation.SigningPublicKeyString("Ed5udj7GpRdUxpojSmgHBOoNGUoD37H0WOUMAcT0yZcobg==");
        bool validSignature = DigitalSignatures.VerifySignature(signatureFilePath, downloadFilePath, publicKey, out string comment);
        File.Delete(signatureFilePath);
        if (validSignature && comment.Equals($"Kryptor v{latestVersion}")) {
            return;
        }
        File.Delete(downloadFilePath);
        throw new CryptographicException("Bad signature. Update aborted.");
    }

    private static void ReplaceExecutable(byte[] downloadedExecutable)
    {
        string executableFilePath = Environment.ProcessPath ?? throw new ArgumentNullException(nameof(executableFilePath));
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) {
            File.WriteAllBytes(executableFilePath, downloadedExecutable);
        }
        else {
            string newExecutableFilePath = Path.Combine(LocalAppDataPath, WindowsExecutableFileName);
            string batFilePath = Path.Combine(LocalAppDataPath, $"{ExecutableFileName}.bat");
            File.WriteAllBytes(newExecutableFilePath, downloadedExecutable);
            using (var batFile = new StreamWriter(File.Create(batFilePath)))
            {
                batFile.WriteLine("@ECHO OFF");
                batFile.WriteLine("TIMEOUT /t 1 /nobreak > NUL");
                batFile.WriteLine($"TASKKILL /IM \"{Path.GetFileName(executableFilePath)}\" > NUL");
                batFile.WriteLine($"MOVE \"{newExecutableFilePath}\" \"{executableFilePath}\"");
                batFile.WriteLine("DEL \"%~f0\"");
            }
            var startInfo = new ProcessStartInfo(batFilePath)
            {
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(executableFilePath)
            };
            Process.Start(startInfo);
        }
        DisplayMessage.WriteLine("Update complete. Please specify -a|--about to check the version.", ConsoleColor.Green);
        Environment.Exit(exitCode: 0);
    }
}