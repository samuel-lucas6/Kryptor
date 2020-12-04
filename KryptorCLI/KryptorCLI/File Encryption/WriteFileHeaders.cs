using System.IO;
using System.Text;

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
    public static class WriteFileHeaders
    {
        public static void WriteHeaders(FileStream ciphertext, byte[] salt, byte[] nonce)
        {
            NullChecks.FileHeaders(ciphertext, salt, nonce);
            byte[] memorySizeFlag = Encoding.UTF8.GetBytes(Constants.MemorySizeFlag + Invariant.ToString(Globals.MemorySize));
            byte[] iterationsFlag = Encoding.UTF8.GetBytes(Constants.IterationsFlag + Invariant.ToString(Globals.Iterations));
            byte[] endFlag = Encoding.UTF8.GetBytes(Constants.EndFlag);
            ciphertext.Write(memorySizeFlag, 0, memorySizeFlag.Length);
            ciphertext.Write(iterationsFlag, 0, iterationsFlag.Length);
            ciphertext.Write(endFlag, 0, endFlag.Length);
            ciphertext.Write(salt, 0, salt.Length);
            ciphertext.Write(nonce, 0, nonce.Length);
        }
    }
}
