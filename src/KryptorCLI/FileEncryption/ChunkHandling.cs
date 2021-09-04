using System;

/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2021 Samuel Lucas

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
    public static class ChunkHandling
    {
        public static byte[] GetPreviousTag(byte[] ciphertextChunk)
        {
            byte[] previousTag = new byte[Constants.TagLength];
            Array.Copy(ciphertextChunk, ciphertextChunk.Length - previousTag.Length, previousTag, destinationIndex: 0, previousTag.Length);
            return previousTag;
        }
    }
}
