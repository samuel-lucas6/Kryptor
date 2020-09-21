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

namespace Kryptor
{
    public static class MonoGUI
    {
        public static void AlignLabels(Label firstLabel, Label secondLabel, Label thirdLabel, LinkLabel linkLabel)
        {
            if (Constants.RunningOnMono == true)
            {
                const int adjustment = 4;
                if (firstLabel != null & secondLabel != null)
                {
                    firstLabel.Left = firstLabel.Location.X + adjustment;
                    secondLabel.Left = secondLabel.Location.X + adjustment;
                }
                if (thirdLabel != null)
                {
                    thirdLabel.Left = thirdLabel.Location.X + adjustment;
                }
                if (linkLabel != null)
                {
                    linkLabel.Left = linkLabel.Location.X - adjustment;
                }
            }
        }
    }
}
