using System;
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
    public static class EditClipboard
    {
        public static void SetClipboard(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    Clipboard.SetText(text);
                }
            }
            catch (Exception ex) when (ExceptionFilters.ClipboardExceptions(ex))
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to copy text to the clipboard.");
            }
        }

        public static void ClearClipboard()
        {
            try
            {
                Clipboard.Clear();
            }
            catch (Exception ex) when (ExceptionFilters.ClipboardExceptions(ex))
            {
                DisplayMessage.ErrorMessageBox(ex.GetType().Name, "Unable to clear the clipboard.");
            }
        }

        public static void AutoClearClipboard()
        {
            FrmFileEncryption fileEncryption = (FrmFileEncryption)Application.OpenForms["frmFileEncryption"];
            if (Globals.ClearClipboardInterval != 1)
            {
                fileEncryption.tmrClearClipboard.Start();
            }
        }
    }
}
