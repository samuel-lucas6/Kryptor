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

using System.IO;
using System.Text;

namespace KryptorCLI;

public static class ObfuscateFileName
{
    public static string ReplaceFilePath(string filePath) => filePath.Replace(Path.GetFileName(filePath), GetRandomFileName());

    public static string GetRandomFileName() => string.Concat(Path.GetRandomFileName(), Path.GetRandomFileName()).Replace(".", string.Empty);

    public static void AppendFileName(string filePath)
    {
        File.SetAttributes(filePath, FileAttributes.Normal);
        using var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, Constants.FileStreamBufferSize, FileOptions.RandomAccess);
        var fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(filePath));
        fileStream.Write(fileNameBytes, offset: 0, fileNameBytes.Length);
    }
}