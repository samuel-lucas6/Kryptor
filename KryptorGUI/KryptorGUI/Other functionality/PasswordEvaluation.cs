using System;
using System.Drawing;
using System.Windows.Forms;

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

namespace KryptorGUI
{
    public static class PasswordEvaluation
    {
        private static readonly Zxcvbn.Zxcvbn _zxcvbn = new Zxcvbn.Zxcvbn();

        public static void DisplayPasswordEntropy(string password, Label lblEntropy)
        {
            if (lblEntropy != null)
            {
                if (!string.IsNullOrEmpty(password))
                {
                    int entropy = CalculateEntropy(password);
                    if (entropy < 80)
                    {
                        lblEntropy.ForeColor = Color.Red;
                    }
                    else if (entropy >= 80 && entropy < 112)
                    {
                        lblEntropy.ForeColor = Color.Orange;
                    }
                    else if (entropy >= 112)
                    {
                        lblEntropy.ForeColor = Color.LimeGreen;
                    }
                    lblEntropy.Text = $"{Invariant.ToString(entropy)} bits";
                    lblEntropy.Visible = true;
                }
                else
                {
                    lblEntropy.Visible = false;
                }
            }
        }

        private static int CalculateEntropy(string password)
        {
            var passwordEvaluation = _zxcvbn.EvaluatePassword(password);
            int entropy = (int)Math.Round(passwordEvaluation.Entropy);
            return entropy;
        }
    }
}
