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

namespace Kryptor
{
    public static class SelectFiles
    {
        public static bool SelectFilesDialog()
        {
            using (var selectFiles = new VistaOpenFileDialog())
            {
                selectFiles.Title = "Select Files";
                selectFiles.Multiselect = true;
                if (selectFiles.ShowDialog() == DialogResult.OK)
                {
                    Globals.SetSelectedFiles(selectFiles.FileNames.ToList());
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
            using (var selectFolder = new VistaFolderBrowserDialog())
            {
                selectFolder.Description = "Select Folder";
                if (selectFolder.ShowDialog() == DialogResult.OK)
                {
                    Globals.SetSelectedFiles(new List<string> { selectFolder.SelectedPath });
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
