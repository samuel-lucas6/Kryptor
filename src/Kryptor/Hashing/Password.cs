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
    public static Span<byte> GetNewPassword(Span<byte> password)
    {
        return password.Length switch
        {
            0 => PasswordPrompt.EnterNewPassword(),
            1 when ConstantTime.Equals(password, Encoding.UTF8.GetBytes(" ")) => PasswordPrompt.UseRandomPassphrase(),
            _ => password
        };
    }
    
    public static Span<byte> Pepper(Span<byte> password, Span<byte> pepper)
    {
        if (password.Length == 0 || pepper == default) {
            return default;
        }
        Span<byte> hash = GC.AllocateArray<byte>(BLAKE2b.MaxHashSize, pinned: true);
        BLAKE2b.ComputeTag(hash, password, pepper);
        CryptographicOperations.ZeroMemory(password);
        CryptographicOperations.ZeroMemory(pepper);
        return hash;
    }
}