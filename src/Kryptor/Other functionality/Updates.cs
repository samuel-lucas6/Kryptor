using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

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

namespace Kryptor
{
    public static class Updates
    {
        public static void UpdateKryptor(bool displayUpToDate)
        {
            bool updateAvailable = CheckForUpdates();
            if (updateAvailable == true)
            {
                AskToUpdate();
            }
            else
            {
                // Only show this message if manually checking for updates
                if (displayUpToDate == true)
                {
                    DisplayMessage.InformationMessageBox("Kryptor is up-to-date.", $"Version {Assembly.GetExecutingAssembly().GetName().Version}");
                }
            }
        }

        private static bool CheckForUpdates()
        {
            try
            {
                bool updateAvailable = false;
                // Compare assembly version to online version file
                string downloadPath = Path.Combine(Constants.KryptorDirectory, "version.txt");
                DownloadVersionFile(downloadPath);
                string programVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                // Remove new line char & any leading/trailing whitespace
                string latestVersion = File.ReadAllText(downloadPath).Trim('\n').Trim();
                if (programVersion != latestVersion)
                {
                    updateAvailable = true;
                }
                return updateAvailable;
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex) || ex is WebException)
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Medium);
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to check for updates.");
                return false;
            }
        }

        private static void DownloadVersionFile(string downloadPath)
        {
            using (var webClient = new WebClient())
            {
                const string versionLink = "https://raw.githubusercontent.com/Kryptor-Software/Kryptor/master/version.txt";
                webClient.DownloadFile(versionLink, downloadPath);
            }
        }

        private static void AskToUpdate()
        {
            if (MessageBox.Show("An update is available for Kryptor. Would you like to download it now?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                const string downloadLink = "https://kryptor.co.uk/Downloads.html";
                OpenURL.OpenLink(downloadLink);
            }
        }
    }
}
