using System.IO;
using System.ComponentModel;
using System.Security.Cryptography;

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
    public static class AesAlgorithms
    {
        private const CipherMode _cbcMode = CipherMode.CBC;
        private const PaddingMode _pkcs7Padding = PaddingMode.PKCS7;

        public static void EncryptAesCBC(FileStream plaintext, FileStream ciphertext, long headersLength, byte[] fileBytes, byte[] nonce, byte[] key, BackgroundWorker bgwEncryption)
        {
            NullChecks.FileEncryption(plaintext, ciphertext, fileBytes, nonce, key);
            using (var aes = new AesCryptoServiceProvider() { Mode = _cbcMode, Padding = _pkcs7Padding })
            {
                using (var cryptoStream = new CryptoStream(ciphertext, aes.CreateEncryptor(key, nonce), CryptoStreamMode.Write))
                {
                    int bytesRead;
                    while ((bytesRead = plaintext.Read(fileBytes, 0, fileBytes.Length)) > 0)
                    {
                        cryptoStream.Write(fileBytes, 0, bytesRead);
                        // Report progress if encrypting a single file
                        ReportProgress.ReportEncryptionProgress(ciphertext.Position, plaintext.Length + headersLength, bgwEncryption);
                    }
                }
            }
        }

        public static void DecryptAesCBC(FileStream plaintext, FileStream ciphertext, byte[] fileBytes, byte[] nonce, byte[] key, BackgroundWorker bgwDecryption)
        {
            NullChecks.FileEncryption(plaintext, ciphertext, fileBytes, nonce, key);
            using (var aes = new AesCryptoServiceProvider() { Mode = _cbcMode, Padding = _pkcs7Padding })
            {
                using (var cryptoStream = new CryptoStream(ciphertext, aes.CreateDecryptor(key, nonce), CryptoStreamMode.Read))
                {
                    int bytesRead;
                    while ((bytesRead = cryptoStream.Read(fileBytes, 0, fileBytes.Length)) > 0)
                    {
                        plaintext.Write(fileBytes, 0, bytesRead);
                        // Report progress if encrypting a single file
                        ReportProgress.ReportEncryptionProgress(plaintext.Position, ciphertext.Length, bgwDecryption);
                    }
                }
            }
        }
    }
}
