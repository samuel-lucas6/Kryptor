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

namespace KryptorCLI
{
    public static class DisplayMessage
    {
        public static void Error(string filePath, string exceptionName, string errorMessage)
        {
            string resultsMessage = $"Error: {exceptionName} - {errorMessage}";
            string fileName = Path.GetFileName(filePath);
            Console.WriteLine($"{fileName} - {resultsMessage}");
        }

        public static void Error(string exceptionName, string errorMessage)
        {
            Console.WriteLine($"Error: {exceptionName} - {errorMessage}");
        }

        public static void FileError(string filePath, bool encryption, bool kryptorExtension)
        {
            string errorMessage;
            if (encryption == false && kryptorExtension == false)
            {
                errorMessage = "This file is missing the '.kryptor' extension.";
            }
            else
            {
                errorMessage = "This file is already encrypted.";
            }
            Console.WriteLine($"{Path.GetFileName(filePath)} - Error: {errorMessage}");
        }
    }
}
