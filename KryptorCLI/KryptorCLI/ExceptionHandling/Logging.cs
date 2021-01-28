using System;
using System.IO;

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
    public static class Logging
    {
        private static readonly string _logdirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Kryptor");
        private static readonly string _logFilePath = Path.Combine(_logdirectoryPath, "error log.txt");

        public enum Severity
        {
            Info,
            Warning,
            Error
        }

        public static void LogException(string exceptionMessage, Severity severity)
        {
            try
            {
                string logMessage = $"Exception Severity = {severity}" + Environment.NewLine + exceptionMessage + Environment.NewLine;
                if (!Directory.Exists(_logdirectoryPath))
                {
                    Directory.CreateDirectory(_logdirectoryPath);
                }
                File.AppendAllText(_logFilePath, logMessage);    
            }
            catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
            {
                DisplayMessage.Exception(ex.GetType().Name, "Unable to log exception.");
            }
        }
    }
}
