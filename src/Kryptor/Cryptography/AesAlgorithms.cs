using System;
using System.IO;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Linq;

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
        public static void EncryptAesCBC(FileStream plaintext, FileStream ciphertext, byte[] fileBytes, byte[] nonce, byte[] key, BackgroundWorker bgwEncryption)
        {
            NullChecks.FileEncryption(plaintext, ciphertext, fileBytes, nonce, key);
            using (var aes = new AesCryptoServiceProvider() { Mode = CipherMode.CBC, Padding = PaddingMode.ISO10126 })
            {
                using (var cs = new CryptoStream(ciphertext, aes.CreateEncryptor(key, nonce), CryptoStreamMode.Write))
                {
                    int bytesRead;
                    while ((bytesRead = plaintext.Read(fileBytes, 0, fileBytes.Length)) > 0)
                    {
                        cs.Write(fileBytes, 0, bytesRead);
                        // Report progress if encrypting a single file
                        ReportProgress.ReportEncryptionProgress(ciphertext.Position, plaintext.Length, bgwEncryption);
                    }
                }
            }
        }

        public static void DecryptAesCBC(FileStream plaintext, FileStream ciphertext, byte[] fileBytes, byte[] nonce, byte[] key, BackgroundWorker bgwDecryption)
        {
            NullChecks.FileEncryption(plaintext, ciphertext, fileBytes, nonce, key);
            using (var aes = new AesCryptoServiceProvider() { Mode = CipherMode.CBC, Padding = PaddingMode.ISO10126 })
            {
                using (var cs = new CryptoStream(ciphertext, aes.CreateDecryptor(key, nonce), CryptoStreamMode.Read))
                {
                    int bytesRead;
                    while ((bytesRead = cs.Read(fileBytes, 0, fileBytes.Length)) > 0)
                    {
                        plaintext.Write(fileBytes, 0, bytesRead);
                        // Report progress if encrypting a single file
                        ReportProgress.ReportEncryptionProgress(plaintext.Position, ciphertext.Length, bgwDecryption);
                    }
                }
            }
        }

        public static void AesCTR(FileStream inputStream, FileStream outputStream, byte[] fileBytes, byte[] nonce, byte[] key, BackgroundWorker backgroundWorker)
        {
            NullChecks.FileEncryption(inputStream, outputStream, fileBytes, nonce, key);
            // Use nonce as counter
            byte[] counter = new byte[nonce.Length];
            Array.Copy(nonce, counter, nonce.Length);
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider() { Mode = CipherMode.ECB, Padding = PaddingMode.None })
            {
                byte[] emptyIV = new byte[counter.Length];
                using (var encryptor = aes.CreateEncryptor(key, emptyIV))
                {
                    int bytesRead;
                    while ((bytesRead = inputStream.Read(fileBytes, 0, fileBytes.Length)) > 0)
                    {
                        byte[] keystream = GenerateKeystream(fileBytes, ref counter, encryptor);
                        byte[] xoredBytes = Utilities.XorBytes(keystream, fileBytes);
                        outputStream.Write(xoredBytes, 0, bytesRead);
                        // Report progress if encrypting a single file
                        ReportProgress.ReportEncryptionProgress(outputStream.Position, inputStream.Length, backgroundWorker);
                    }
                }
            }
        }

        private static byte[] GenerateKeystream(byte[] fileBytes, ref byte[] counter, ICryptoTransform encryptor)
        {           
            byte[] keystream = Array.Empty<byte>();
            // Round up
            int iterations = (int)Math.Ceiling((decimal)fileBytes.Length / counter.Length);
            for (int i = 0; i < iterations; i++)
            {
                byte[] keystreamBlock = new byte[counter.Length];
                encryptor.TransformBlock(counter, 0, counter.Length, keystreamBlock, 0);
                counter = Sodium.Utilities.Increment(counter);
                keystream = keystream.Concat(keystreamBlock).ToArray();
            }
            return keystream;
        }
    }
}
