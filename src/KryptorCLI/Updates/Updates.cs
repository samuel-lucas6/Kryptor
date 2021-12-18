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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace KryptorCLI;

public static class Updates
{
    private const string VersionFileName = "version.txt";
    private const string VersionFileLink = "https://raw.githubusercontent.com/samuel-lucas6/Kryptor/master/version.txt";

    public static bool CheckForUpdates()
    {
        string assemblyVersion = Program.GetVersion();
        string latestVersion = GetLatestVersion();
        return !string.Equals(assemblyVersion, latestVersion, StringComparison.Ordinal);
    }

    private static string GetLatestVersion()
    {
        string downloadFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), VersionFileName);
        DownloadVersionFile(downloadFilePath);
        string latestVersion = File.ReadAllText(downloadFilePath).Trim('\n').Trim();
        File.Delete(downloadFilePath);
        return latestVersion;
    }

    private static void DownloadVersionFile(string downloadFilePath)
    {
        using var webClient = new WebClient();
        webClient.DownloadFile(VersionFileLink, downloadFilePath);
    }
}