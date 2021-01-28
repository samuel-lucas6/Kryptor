using System.IO;
using System.Text;

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
    public static class ObfuscateFileName
    {
        public static string ReplaceFilePath(string filePath)
        {
            string originalFileName = Path.GetFileName(filePath);
            string randomFileName = GetRandomFileName();
            return filePath.Replace(originalFileName, randomFileName);
        }

        public static string GetRandomFileName()
        {
            return GenerateRandomFileName() + GenerateRandomFileName();
        }

        private static string GenerateRandomFileName()
        {
            // Remove the generated extension
            return Path.GetRandomFileName().Replace(".", string.Empty);
        }

        public static void AppendFileName(string filePath)
        {
            Encoding fileEncoding = FileHandling.GetFileEncoding(filePath);
            string fileName = Path.GetFileName(filePath);
            byte[] fileNameBytes = fileEncoding.GetBytes(fileName);
            using var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, Constants.FileBufferSize, FileOptions.RandomAccess);
            fileStream.Write(fileNameBytes, offset: 0, fileNameBytes.Length);
        }
    }
}
