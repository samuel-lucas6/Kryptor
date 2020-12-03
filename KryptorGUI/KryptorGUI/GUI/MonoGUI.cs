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
        private const int _xAxisAdjustment = 4;
        public static void MoveLabelRight(Label label)
        {
            if (label != null)
            {
                label.Left = label.Location.X + _xAxisAdjustment;
            }
        }

        public static void MoveCheckBoxLeft(CheckBox checkBox)
        {
            if (checkBox != null)
            {
                checkBox.Left = checkBox.Location.X - _xAxisAdjustment;
            }
        }

        public static void MoveLabelLeft(Label label)
        {
            if (label != null)
            {
                label.Left = label.Location.X - _xAxisAdjustment;
            }
        }

        public static void MoveLinkLabelLeft(LinkLabel linkLabel)
        {
            if (linkLabel != null)
            {
                linkLabel.Left = linkLabel.Location.X - _xAxisAdjustment;
            }
        }
    }
}
