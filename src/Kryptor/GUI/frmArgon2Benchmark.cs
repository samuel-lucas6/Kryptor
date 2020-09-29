using System;
using System.ComponentModel;
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
    public partial class frmArgon2Benchmark : Form
    {
        public frmArgon2Benchmark()
        {
            InitializeComponent();
        }

        private void frmArgon2Benchmark_Load(object sender, EventArgs e)
        {
            RunningOnMono();
            this.Hide();
            bool? speedMode = GetBenchmarkMode();
            if (speedMode != null)
            {
                this.Show();
                bgwArgon2Benchmark.RunWorkerAsync(speedMode);
            }
            else
            {
                // Cancel benchmark
                ShowOtherForms();
                this.Close();
            }
        }

        private void RunningOnMono()
        {
            if (Constants.RunningOnMono == true)
            {
                DarkTheme.Labels(lblMessage);
            }
        }

        private static bool? GetBenchmarkMode()
        {
            using (var selectBenchmarkMode = new frmSelectBenchmarkMode())
            {
                DialogResult dialogResult = selectBenchmarkMode.ShowDialog();
                if (dialogResult == DialogResult.Yes)
                {
                    // 150 ms delay per file
                    return true;
                }
                else if (dialogResult == DialogResult.No)
                {
                    // 250 ms delay per file
                    return false;
                }
                else
                {
                    // Cancel if the user closes the form
                    return null;
                }
            }
        }

        private void bgwArgon2Benchmark_DoWork(object sender, DoWorkEventArgs e)
        {
            bool speedMode = (bool)e.Argument;
            Argon2Benchmark.RunBenchmark(speedMode);
        }

        private void bgwArgon2Benchmark_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Settings.SaveSettings();
            // Deallocate RAM for Argon2
            GC.Collect();
            Argon2Benchmark.DeleteFirstRunFile();
            this.Hide();
            DisplayMessage.InformationMessageBox($"Argon2 will use a memory size of {Invariant.ToString(Globals.MemorySize / Constants.Mebibyte)} MiB. This value can be changed in the settings.{Environment.NewLine}{Environment.NewLine}For the full benchmark results, please view: {Constants.KryptorDirectory}\\benchmark.txt", "Argon2 Benchmark Results");
            ShowOtherForms();
            this.Close();
        }

        private static void ShowOtherForms()
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name != "frmArgon2Benchmark")
                {
                    form.Show();
                }
            }
        }
    }
}
