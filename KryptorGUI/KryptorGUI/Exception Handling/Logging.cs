using System;
using System.IO;

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
    public static class Logging
    {
        public enum Severity
        {
            Low, // User error or unimportant
            Medium, // Minor impact
            High, // Critical impact
            Bug // Shouldn't happen
        }

        public static void LogException(string exceptionMessage, Severity severity)
        {
            try
            {
                const string logFileName = "error log.txt";
                string logFilePath = Path.Combine(Constants.KryptorDirectory, logFileName);
                string logMessage = $"[Error] Severity = {severity}" + Environment.NewLine + exceptionMessage + Environment.NewLine;
                File.AppendAllText(logFilePath, logMessage);    
            }
            catch (Exception ex) when (ExceptionFilters.FileAccessExceptions(ex))
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to log exception.");
            }
        }
    }
}
