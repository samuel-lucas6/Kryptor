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

namespace Kryptor
{
    public partial class frmResults : Form
    {
        public frmResults()
        {
            InitializeComponent();
        }

        private void FrmResults_Load(object sender, EventArgs e)
        {
            if (Globals.DarkTheme == true)
            {
                ApplyDarkTheme();
            }
            DisplayResults();
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = DarkTheme.BackgroundColour();
            DarkTheme.TextBoxes(txtResults);
        }

        private void DisplayResults()
        {
            txtResults.Text = Globals.ResultsText;
            if (txtResults.Lines.Length > 16 || txtResults.Text.Length > 1000)
            {
                txtResults.ScrollBars = ScrollBars.Vertical;
            }
            txtResults.SelectionStart = txtResults.Text.Length;
        }

        private void FrmResults_FormClosing(object sender, EventArgs e)
        {
            Globals.ResultsText = string.Empty;
            Globals.SuccessfulCount = 0;
            Globals.TotalCount = 0;
            txtResults.Clear();
        }
    }
}
