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
using Geralt;

namespace Kryptor;

public static class Password
{
    public static char[] GetNewPassword(char[] password)
    {
        return password.Length switch
        {
            0 => PasswordPrompt.EnterNewPassword(),
            1 when ConstantTime.Equals(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(" ")) => PasswordPrompt.UseRandomPassphrase(),
            _ => password
        };
    }
    
    public static Span<byte> Prehash(char[] password, Span<byte> pepper = default)
    {
        if (password.Length == 0) {
            return default;
        }
        Span<byte> passwordBytes = GC.AllocateArray<byte>(Encoding.UTF8.GetMaxByteCount(password.Length), pinned: true);
        int bytesEncoded = Encoding.UTF8.GetBytes(password, passwordBytes);
        Array.Clear(password);
        Span<byte> hash = GC.AllocateArray<byte>(BLAKE2b.MaxHashSize, pinned: true);
        if (pepper == default) {
            BLAKE2b.ComputeHash(hash, passwordBytes[..bytesEncoded]);
        }
        else {
            BLAKE2b.ComputeTag(hash, passwordBytes[..bytesEncoded], pepper);
        }
        CryptographicOperations.ZeroMemory(passwordBytes);
        CryptographicOperations.ZeroMemory(pepper);
        return hash;
    }
}