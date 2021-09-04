using System;
using System.IO;
using System.Net;

/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2021 Samuel Lucas

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
    public static class Updates
    {
        public static bool CheckForUpdates()
        {
            string downloadFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "version.txt");
            DownloadVersionFile(downloadFilePath);
            string assemblyVersion = Program.GetVersion();
            string latestVersion = GetLatestVersion(downloadFilePath);
            return !CompareVersions(assemblyVersion, latestVersion);
        }

        private static void DownloadVersionFile(string downloadFilePath)
        {
            using var webClient = new WebClient();
            const string versionLink = "https://raw.githubusercontent.com/samuel-lucas6/Kryptor/master/version.txt";
            webClient.DownloadFile(versionLink, downloadFilePath);
        }

        private static string GetLatestVersion(string downloadFilePath)
        {
            string latestVersion = File.ReadAllText(downloadFilePath).Trim('\n').Trim();
            File.Delete(downloadFilePath);
            return latestVersion;
        }

        private static bool CompareVersions(string assemblyVersion, string latestVersion)
        {
            return string.Equals(assemblyVersion, latestVersion, StringComparison.Ordinal);
        }
    }
}
