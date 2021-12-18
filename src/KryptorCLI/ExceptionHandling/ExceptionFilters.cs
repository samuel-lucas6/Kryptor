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

using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Net;

namespace KryptorCLI;

public static class ExceptionFilters
{
    public static bool FileAccess(Exception ex)
    {
        return ex is IOException or UnauthorizedAccessException or ArgumentException or SecurityException or NotSupportedException;
    }

    public static bool Cryptography(Exception ex)
    {
        return ex is CryptographicException || FileAccess(ex);
    }

    public static bool AsymmetricKeyHandling(Exception ex)
    {
        return ex is FormatException || FileAccess(ex);
    }

    public static bool CheckForUpdates(Exception ex)
    {
        return ex is WebException or PlatformNotSupportedException || FileAccess(ex);
    }
}