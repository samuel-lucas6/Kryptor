/*
    Kryptor: A simple, modern, and secure encryption and signing tool.
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

using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace Kryptor;

public static class ExceptionFilters
{
    public static bool FileAccess(Exception ex) => ex is IOException or UnauthorizedAccessException or ArgumentException or SecurityException or NotSupportedException;

    public static bool Cryptography(Exception ex) => ex is CryptographicException || FileAccess(ex);

    public static bool StringKey(Exception ex) => ex is FormatException || Cryptography(ex);
}