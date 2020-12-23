using Sodium;
using System;

/*
    Kryptor: Free and open source file encryption software.
    Copyright(C) 2020 Samuel Lucas

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
    public static class KeyDerivation
    {
        public static (byte[] encryptionKey, byte[] macKey) DeriveKeys(byte[] passwordBytes, byte[] salt, int iterations, int memorySize)
        {
            var argon2id = PasswordHash.ArgonAlgorithm.Argon_2ID13;
            MemoryEncryption.DecryptByteArray(ref passwordBytes);
            // Derive a 96 byte key
            byte[] derivedKey = PasswordHash.ArgonHashBinary(passwordBytes, salt, iterations, memorySize, Constants.Argon2KeySize, argon2id);
            // 256-bit encryption key
            byte[] encryptionKey = new byte[Constants.EncryptionKeySize];
            Array.Copy(derivedKey, encryptionKey, encryptionKey.Length);
            // 512-bit MAC key
            byte[] macKey = new byte[Constants.MACKeySize];
            Array.Copy(derivedKey, encryptionKey.Length, macKey, 0, macKey.Length);
            Utilities.ZeroArray(derivedKey);
            MemoryEncryption.EncryptByteArray(ref passwordBytes);
            MemoryEncryption.EncryptByteArray(ref encryptionKey);
            MemoryEncryption.EncryptByteArray(ref macKey);
            return (encryptionKey, macKey);
        }
    }
}
