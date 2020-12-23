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
    public static class SharedContextMenu
    {
        public static void CopyTextbox(object sender)
        {
            Control sourceControl = GetSourceControl(sender);
            if (sourceControl != null && !string.IsNullOrEmpty(sourceControl.Text))
            {
                EditClipboard.SetClipboard(sourceControl.Text);
                EditClipboard.AutoClearClipboard();
            }
        }

        public static void ClearTextbox(object sender)
        {
            Control sourceControl = GetSourceControl(sender);
            if (sourceControl != null)
            {
                sourceControl.Text = string.Empty;
            }
        }

        private static Control GetSourceControl(object sender)
        {
            // Return which textbox the context menu was used on
            if (sender is ToolStripItem toolStripItem)
            {
                if (toolStripItem.Owner is ContextMenuStrip contextMenuStrip)
                {
                    Control sourceControl = contextMenuStrip.SourceControl;
                    return sourceControl;
                }
            }
            return null;
        }
    }
}
