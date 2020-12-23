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
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/. 
*/

namespace KryptorGUI
{
    public static class DarkTheme
    {
        private const int _red = 44;
        private const int _green = 47;
        private const int _blue = 51;

        public static Color BackgroundColour()
        {
            return Color.FromArgb(_red, _green,_blue);
        }

        public static void Labels(Label label)
        {
            if (label != null)
            {
                label.ForeColor = Color.White;
            }
        }

        public static void ComboBoxes(ComboBox comboBox)
        {
            if (comboBox != null)
            {
                comboBox.BackColor = Color.DimGray;
                comboBox.ForeColor = Color.White;
            }
        }

        public static void Buttons(Button button)
        {
            if (button != null)
            {
                button.BackColor = Color.FromArgb(_red, _green,_blue);
                button.ForeColor = Color.White;
                button.FlatAppearance.MouseDownBackColor = Color.Transparent;
            }
        }

        public static void TextBoxes(TextBox textBox)
        {
            if (textBox != null)
            {
                textBox.BackColor = Color.DimGray;
                textBox.ForeColor = Color.White;
            }
        }

        public static void ToolStripMenuItems(ToolStripMenuItem toolStripMenuItem)
        {
            if (toolStripMenuItem != null)
            {
                toolStripMenuItem.ForeColor = Color.White;
                toolStripMenuItem.BackColor = Color.FromArgb(_red, _green, _blue);
            }
        }

        public static void CheckBoxes(CheckBox checkBox)
        {
            if (checkBox != null)
            {
                checkBox.BackColor = Color.FromArgb(_red, _green, _blue);
                checkBox.ForeColor = Color.White;
            }
        }

        public static void GroupBoxes(GroupBox groupBox)
        {
            if (groupBox != null)
            {
                groupBox.ForeColor = Color.White;
            }
        }

        public static void ContextMenu(ContextMenuStrip contextMenu)
        {
            if (contextMenu != null)
            {
                contextMenu.BackColor = Color.FromArgb(_red, _green, _blue);
                foreach (ToolStripItem toolStripItem in contextMenu.Items)
                {
                    toolStripItem.ForeColor = Color.White;
                    toolStripItem.BackColor = Color.FromArgb(_red, _green, _blue);
                }
            }
        }

        public static void Menu(ToolStripMenuItem menu)
        {
            if (menu != null)
            {
                foreach (ToolStripMenuItem toolStripItem in menu.DropDownItems)
                {
                    ToolStripMenuItems(toolStripItem);
                }
            }
        }

        public static void NumericUpDown(NumericUpDown numericUpDown)
        {
            if (numericUpDown != null)
            {
                numericUpDown.BackColor = Color.DimGray;
                numericUpDown.ForeColor = Color.White;
            }
        }

        public static void SettingsButtons(Button button)
        {
            if (button != null)
            {
                button.BackColor = Color.DimGray;
                button.ForeColor = Color.White;
                button.FlatAppearance.MouseDownBackColor = Color.DimGray;
            }
        }

        public static void LinkLabels(LinkLabel linkLabel)
        {
            if (linkLabel != null)
            {
                linkLabel.LinkColor = Color.White;
                linkLabel.ActiveLinkColor = Color.White;
            }
        }
    }
}
