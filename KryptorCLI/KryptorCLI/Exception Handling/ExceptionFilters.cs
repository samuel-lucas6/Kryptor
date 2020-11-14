using System;
using System.IO;
using System.Security;
using System.Text;
using System.Security.Cryptography;
using Sodium.Exceptions;
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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace KryptorCLI
{
    public static class ExceptionFilters
    {
        public static bool FileAccessExceptions(Exception ex)
        {
            return ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException || ex is SecurityException || ex is NotSupportedException;
        }

        public static bool SettingsExceptions(Exception ex)
        {
            return ex is FormatException || ex is OverflowException;
        }

        public static bool CharacterEncodingExceptions(Exception ex)
        {
            return ex is ArgumentException || ex is EncoderFallbackException || ex is OverflowException || ex is IOException;
        }

        public static bool FileEncryptionExceptions(Exception ex)
        {
            return ex is CryptographicException || FileAccessExceptions(ex) || ex is PlatformNotSupportedException;
        }

        public static bool PasswordSharingExceptions(Exception ex)
        {
            return ex is CryptographicException || ex is KeyOutOfRangeException || ex is SeedOutOfRangeException || ex is OverflowException || ex is DecoderFallbackException;
        }

        public static bool MemoryEncryptionExceptions(Exception ex)
        {
            return ex is NotSupportedException || ex is CryptographicException;
        }

        public static bool OpenLinkExceptions(Exception ex)
        {
            return ex is Win32Exception || ex is InvalidOperationException || ex is FileNotFoundException;
        }
    }
}
