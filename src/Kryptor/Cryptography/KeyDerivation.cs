using Sodium;

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

namespace Kryptor
{
    public static class KeyDerivation
    {
        public static (byte[], byte[]) DeriveKeys(byte[] passwordBytes, byte[] salt, int iterations, int memorySize)
        {
            var argon2id = PasswordHash.ArgonAlgorithm.Argon_2ID13;
            MemoryEncryption.DecryptByteArray(ref passwordBytes);
            // 256-bit encryption key
            byte[] encryptionKey = PasswordHash.ArgonHashBinary(passwordBytes, salt, iterations, memorySize, Constants.EncryptionKeySize, argon2id);
            // 512-bit MAC key
            byte[] macKey = PasswordHash.ArgonHashBinary(passwordBytes, salt, iterations, memorySize, Constants.MACKeySize, argon2id);
            MemoryEncryption.EncryptByteArray(ref passwordBytes);
            MemoryEncryption.EncryptByteArray(ref encryptionKey);
            MemoryEncryption.EncryptByteArray(ref macKey);
            return (encryptionKey, macKey);
        }
    }
}
