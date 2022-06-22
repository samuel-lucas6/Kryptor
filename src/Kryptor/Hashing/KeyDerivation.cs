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

public static class KeyDerivation
{
    private static readonly byte[] Personalisation = Encoding.UTF8.GetBytes("Kryptor.Personal");
    
    public static byte[] Blake2b(byte[] ephemeralSharedSecret, byte[] sharedSecret, byte[] salt)
    {
        byte[] inputKeyingMaterial = Arrays.Concat(ephemeralSharedSecret, sharedSecret);
        byte[] keyEncryptionKey = GenericHash.HashSaltPersonal(message: Array.Empty<byte>(), inputKeyingMaterial, salt, Personalisation, Constants.EncryptionKeyLength);
        CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
        CryptographicOperations.ZeroMemory(inputKeyingMaterial);
        return keyEncryptionKey;
    }

    public static byte[] Blake2b(byte[] ephemeralSharedSecret, byte[] salt)
    {
        byte[] keyEncryptionKey = GenericHash.HashSaltPersonal(message: Array.Empty<byte>(), ephemeralSharedSecret, salt, Personalisation, Constants.EncryptionKeyLength);
        CryptographicOperations.ZeroMemory(ephemeralSharedSecret);
        return keyEncryptionKey;
    }
}