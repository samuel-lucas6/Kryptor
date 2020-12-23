using System;
using System.IO;
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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace KryptorGUI
{
    public static class DisplayMessage
    {
        private const string _errorCaption = "Error";

        public static void ErrorMessageBox(string exceptionName, string errorMessage)
        {
            MessageBox.Show($"{exceptionName}: {errorMessage}", _errorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ErrorMessageBox(string errorMessage)
        {
            MessageBox.Show(errorMessage, _errorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void InformationMessageBox(string errorMessage)
        {
            MessageBox.Show(errorMessage, _errorCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void InformationMessageBox(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ErrorResultsText(string filePath, string exceptionName, string errorMessage)
        {
            string resultsMessage = $"Error: {exceptionName} - {errorMessage}";
            if (!string.IsNullOrEmpty(filePath))
            {
                string fileName = Path.GetFileName(filePath);
                Globals.ResultsText += $"{fileName} - {resultsMessage}";
            }
            else
            {
                Globals.ResultsText += resultsMessage;
            }
            Globals.ResultsText += Environment.NewLine;
        }
    }
}
