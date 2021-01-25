using System.Collections.Generic;
using System.IO;

/*
    Kryptor: Free and open source file encryption.
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
    public static class SignValidation
    {
        public static bool Sign(string privateKeyPath, string comment, string[] filePaths)
        {
            IEnumerable<string> errorMessages = GetSignErrors(privateKeyPath, comment, filePaths);
            return DisplayMessage.AnyErrors(errorMessages);
        }

        private static IEnumerable<string> GetSignErrors(string privateKeyPath, string comment, string[] filePaths)
        {
            if (!privateKeyPath.EndsWith(Constants.PrivateKeyExtension) || !File.Exists(privateKeyPath))
            {
                yield return ValidationMessages.PrivateKeyFile;
            }
            if (comment.Length > 500)
            {
                yield return "Please enter a shorter comment.";
            }
            if (filePaths == null)
            {
                yield return "Please specify a file to sign.";
            }
        }
    }
}
