using System;
using System.ComponentModel;

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

namespace KryptorGUI
{
    public static class ReportProgress
    {
        private static int _previousPercentage;

        public static void ReportEncryptionProgress(long bytesWritten, long fileSize, BackgroundWorker backgroundWorker)
        {
            if (Globals.TotalCount == 1)
            {
                BackgroundWorkerReportProgress(bytesWritten, fileSize, backgroundWorker);
            }
        }

        public static void IncrementProgress(ref int progress, BackgroundWorker backgroundWorker)
        {
            if (Globals.TotalCount > 1)
            {
                BackgroundWorkerReportProgress(progress, Globals.TotalCount, backgroundWorker);
                progress++;
            }
        }

        public static void BackgroundWorkerReportProgress(long progress, long total, BackgroundWorker backgroundWorker)
        {
            try
            {
                NullChecks.BackgroundWorkers(backgroundWorker);
                int percentage = (int)Math.Round((double)((double)progress / total) * 100);
                // Prevent unnecessary calls
                if (percentage != 0 && percentage != _previousPercentage)
                {
                    backgroundWorker.ReportProgress(percentage);
                }
                _previousPercentage = percentage;
            }
            catch (Exception ex) when (ExceptionFilters.ReportProgressExceptions(ex))
            {
                Logging.LogException(ex.ToString(), Logging.Severity.Bug);
                DisplayMessage.ErrorResultsText(string.Empty, ex.GetType().Name, "Background worker report progress exception. This is a bug - please report it.");
            }
        }
    }
}
