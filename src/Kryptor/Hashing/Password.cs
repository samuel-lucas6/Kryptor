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
using System.Text;
using System.Security.Cryptography;
using Sodium;

namespace Kryptor;

public static class Password
{
    public static char[] GetNewPassword(char[] password)
    {
        if (password.Length == 0) { return PasswordPrompt.EnterNewPassword(); }
        if (password.Length == 1 && Arrays.Compare(password, new[] { ' ' })) { return PasswordPrompt.UseRandomPassphrase(); }
        return password;
    }

    public static byte[] Prehash(char[] password, string keyfilePath)
    {
        var passwordBytes = Prehash(password);
        return string.IsNullOrEmpty(keyfilePath) ? passwordBytes : UseKeyfile(passwordBytes, keyfilePath);
    }

    public static byte[] Prehash(char[] password)
    {
        if (password.Length == 0) { return null; }
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        Arrays.ZeroMemory(password);
        return GenericHash.Hash(passwordBytes, key: null, Constants.HashLength);
    }

    private static byte[] UseKeyfile(byte[] passwordBytes, string keyfilePath)
    {
        try
        {
            var keyfileBytes = Keyfiles.ReadKeyfile(keyfilePath);
            return passwordBytes == null ? keyfileBytes : PepperPassword(passwordBytes, keyfileBytes);
        }
        catch (Exception ex) when (ExceptionFilters.FileAccess(ex))
        {
            DisplayMessage.Exception(ex.GetType().Name, "Unable to read the keyfile, so it has not been used.");
            return passwordBytes;
        }
    }

    private static byte[] PepperPassword(byte[] passwordBytes, byte[] keyfileBytes)
    {
        passwordBytes = GenericHash.Hash(passwordBytes, keyfileBytes, Constants.HashLength);
        CryptographicOperations.ZeroMemory(keyfileBytes);
        return passwordBytes;
    }
}