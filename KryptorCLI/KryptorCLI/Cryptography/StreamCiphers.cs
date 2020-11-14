using Sodium;
using System.IO;

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
    public static class StreamCiphers
    {
        public static void Encrypt(FileStream plaintext, FileStream ciphertext, byte[] fileBytes, byte[] nonce, byte[] key)
        {
            NullChecks.FileEncryption(plaintext, ciphertext, fileBytes, nonce, key);
            int bytesRead;
            while ((bytesRead = plaintext.Read(fileBytes, 0, fileBytes.Length)) > 0)
            {
                byte[] encryptedBytes = EncryptFileBytes(fileBytes, nonce, key);
                ciphertext.Write(encryptedBytes, 0, bytesRead);
            }
        }

        private static byte[] EncryptFileBytes(byte[] fileBytes, byte[] nonce, byte[] key)
        {
            byte[] encryptedBytes = new byte[fileBytes.Length];
            if (Globals.EncryptionAlgorithm == (int)Cipher.XChaCha20)
            {
                encryptedBytes = StreamEncryption.EncryptXChaCha20(fileBytes, nonce, key);
            }
            else if (Globals.EncryptionAlgorithm == (int)Cipher.XSalsa20)
            {
                encryptedBytes = StreamEncryption.Encrypt(fileBytes, nonce, key);
            }
            return encryptedBytes;
        }

        public static void Decrypt(FileStream plaintext, FileStream ciphertext, byte[] fileBytes, byte[] nonce, byte[] key)
        {
            NullChecks.FileEncryption(plaintext, ciphertext, fileBytes, nonce, key);
            int bytesRead;
            while ((bytesRead = ciphertext.Read(fileBytes, 0, fileBytes.Length)) > 0)
            {
                byte[] decryptedBytes = DecryptFileBytes(fileBytes, nonce, key);
                plaintext.Write(decryptedBytes, 0, bytesRead);
            }
        }

        private static byte[] DecryptFileBytes(byte[] fileBytes, byte[] nonce, byte[] key)
        {
            byte[] decryptedBytes = new byte[fileBytes.Length];
            if (Globals.EncryptionAlgorithm == (int)Cipher.XChaCha20)
            {
                decryptedBytes = StreamEncryption.DecryptXChaCha20(fileBytes, nonce, key);
            }
            else if (Globals.EncryptionAlgorithm == (int)Cipher.XSalsa20)
            {
                decryptedBytes = StreamEncryption.Decrypt(fileBytes, nonce, key);
            }
            return decryptedBytes;
        }
    }
}
