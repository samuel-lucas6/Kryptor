using System;
using System.IO;
using System.Net;
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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace KryptorCLI
{
    public static class Updates
    {
        public static string UpdateKryptor(bool displayUpToDate)
        {
            string message = string.Empty;
            string kryptorVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            // Convert to semantic versioning - 3 numbers instead of 4
            kryptorVersion = kryptorVersion.Substring(0, kryptorVersion.Length - 2);
            bool? updateAvailable = CheckForUpdates(kryptorVersion);
            if (updateAvailable == true)
            {
                message = "An update is available for Kryptor. Visit <https://github.com/samuel-lucas6/Kryptor/releases> to update.";
            }
            else
            {
                if (updateAvailable == null)
                {
                    message = "Error: Unable to check for updates.";
                }
                else if (displayUpToDate == true)
                {
                    // Only show this message if manually checking for updates
                    message = $"Kryptor is up-to-date. Version {kryptorVersion}";
                }
            }
            return message;
        }

        private static bool? CheckForUpdates(string kryptorVersion)
        {
            try
            {
                bool? updateAvailable = false;
                // Compare assembly version to online version file
                string downloadFilePath = Path.Combine(Constants.KryptorDirectory, "version.txt");
                DownloadVersionFile(downloadFilePath);
                // Remove new line char & any leading/trailing whitespace
                string latestVersion = File.ReadAllText(downloadFilePath).Trim('\n').Trim();
                if (kryptorVersion != latestVersion)
                {
                    updateAvailable = true;
                }
                File.Delete(downloadFilePath);
                return updateAvailable;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex) || ex is WebException)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                return null;
            }
        }

        private static void DownloadVersionFile(string downloadFilePath)
        {
            using var webClient = new WebClient();
            const string versionLink = "https://raw.githubusercontent.com/samuel-lucas6/Kryptor/master/version.txt";
            webClient.DownloadFile(versionLink, downloadFilePath);
        }
    }
}
