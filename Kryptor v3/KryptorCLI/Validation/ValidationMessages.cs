/*
    Kryptor: Modern and secure file encryption.
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
    public static class ValidationMessages
    {
        public static readonly string FilePath = "Please specify a file/folder.";
        public static readonly string PrivateKeyFile = "Please specify a valid private key file.";
        public static readonly string PublicKeyFile = "Please specify a valid public key file.";
        public static readonly string PublicKeyString = "Please specify a valid public key.";
    }
}
