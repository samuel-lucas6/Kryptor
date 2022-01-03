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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;

namespace KryptorCLI;

public static class Updates
{
    private const string VersionFileName = "version.txt";
    private const string VersionFileLink = "https://raw.githubusercontent.com/samuel-lucas6/Kryptor/master/version.txt";
    private const string LatestReleaseFileName = "kryptor-latest.zip";
    private const string WindowsDownloadFileName = "kryptor-windows.zip";
    private const string LinuxDownloadFileName = "kryptor-linux.zip";
    private const string MacOSDownloadFileName = "kryptor-macos.zip";
    private const string ExecutableFileName = "kryptor";
    private const string ExeExtension = ".exe";

    public static bool CheckForUpdates(out string latestVersion)
    {
        string assemblyVersion = Program.GetVersion();
        latestVersion = GetLatestVersion();
        return new Version(latestVersion).CompareTo(new Version(assemblyVersion)) == 1;
    }

    private static string GetLatestVersion()
    {
        string downloadFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), VersionFileName);
        string signatureFilePath = downloadFilePath + Constants.SignatureExtension;
        DownloadFile(VersionFileLink + Constants.SignatureExtension, signatureFilePath);
        DownloadFile(VersionFileLink, downloadFilePath);
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
        if (!Environment.Is64BitOperatingSystem || !OperatingSystem.IsWindows() & !OperatingSystem.IsLinux() & !OperatingSystem.IsMacOS())
        {
            throw new PlatformNotSupportedException("There are no official releases for your operating system.");
        }
        Console.WriteLine($"Downloading {latestVersion} update...");
        byte[] downloadedExecutable = GetLatestExecutable(latestVersion);
        Console.WriteLine($"Applying {latestVersion} update...");
        ReplaceExecutable(downloadedExecutable);
    }

    private static byte[] GetLatestExecutable(string latestVersion)
    {
        string downloadFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), LatestReleaseFileName);
        string downloadLink = GetReleaseDownloadLink(latestVersion);
        string signatureFilePath = downloadFilePath + Constants.SignatureExtension;
        DownloadFile(downloadLink + Constants.SignatureExtension, signatureFilePath);
        DownloadFile(downloadLink, downloadFilePath);
        VerifyDownloadSignature(signatureFilePath, downloadFilePath, latestVersion);
        string extractedDirectoryPath = Path.Combine(Path.GetDirectoryName(downloadFilePath), ExecutableFileName);
        if (!Directory.Exists(extractedDirectoryPath)) { Directory.CreateDirectory(extractedDirectoryPath); }
        ZipFile.ExtractToDirectory(downloadFilePath, extractedDirectoryPath, overwriteFiles: true);
        File.Delete(downloadFilePath);
        string executableFilePath = Path.Combine(extractedDirectoryPath, OperatingSystem.IsWindows() ? ExecutableFileName + ExeExtension : ExecutableFileName);
        byte[] downloadedExecutable = File.ReadAllBytes(executableFilePath);
        Directory.Delete(extractedDirectoryPath, recursive: true);
        return downloadedExecutable;
    }

    private static string GetReleaseDownloadLink(string latestVersion)
    {
        string downloadLink = $"https://github.com/samuel-lucas6/Kryptor/releases/download/v{latestVersion}/";
        if (OperatingSystem.IsWindows()) { return downloadLink + WindowsDownloadFileName; }
        if (OperatingSystem.IsLinux()) { return downloadLink + LinuxDownloadFileName; }
        if (OperatingSystem.IsMacOS()) { return downloadLink + MacOSDownloadFileName; }
        return null;
    }

    private static void VerifyDownloadSignature(string signatureFilePath, string downloadFilePath, string latestVersion)
    {
        byte[] publicKey = AsymmetricKeyValidation.SigningPublicKeyString("RWRudj7GpRdUxpojSmgHBOoNGUoD37H0WOUMAcT0yZcobg==".ToCharArray());
        bool validSignature = DigitalSignatures.VerifySignature(signatureFilePath, downloadFilePath, publicKey, out string comment);
        File.Delete(signatureFilePath);
        if (validSignature && string.Equals(comment, $"Kryptor v{latestVersion}")) { return; }
        File.Delete(downloadFilePath);
        throw new CryptographicException("Bad signature. Update aborted.");
    }

    private static void ReplaceExecutable(byte[] downloadedExecutable)
    {
        string executableFilePath = Environment.ProcessPath;
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            File.WriteAllBytes(executableFilePath, downloadedExecutable);
        }
        else
        {
            string newExecutableFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ExecutableFileName + ExeExtension);
            string batFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{ExecutableFileName}.bat");
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
        Environment.Exit(0);
    }
}