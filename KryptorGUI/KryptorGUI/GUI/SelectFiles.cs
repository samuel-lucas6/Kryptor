using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;

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
    public static class SelectFiles
    {
        public static bool SelectFilesDialog()
        {
            using (var selectFilesDialog = new VistaOpenFileDialog())
            {
                selectFilesDialog.Title = "Select Files";
                selectFilesDialog.Multiselect = true;
                if (selectFilesDialog.ShowDialog() == DialogResult.OK)
                {
                    Globals.SetSelectedFiles(selectFilesDialog.FileNames.ToList());
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool SelectFolderDialog()
        {
            using (var selectFolderDialog = new VistaFolderBrowserDialog())
            {
                selectFolderDialog.Description = "Select Folder";
                if (selectFolderDialog.ShowDialog() == DialogResult.OK)
                {
                    Globals.SetSelectedFiles(new List<string> { selectFolderDialog.SelectedPath });
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
