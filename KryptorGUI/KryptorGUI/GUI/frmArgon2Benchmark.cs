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

namespace KryptorGUI
{
    public partial class FrmArgon2Benchmark : Form
    {
        public FrmArgon2Benchmark()
        {
            InitializeComponent();
        }

        private void FrmArgon2Benchmark_Load(object sender, EventArgs e)
        {
            RunningOnMono();
            this.Hide();
            int delayPerFile = GetBenchmarkMode();
            if (delayPerFile != 0)
            {
                this.Show();
                bgwArgon2Benchmark.RunWorkerAsync(delayPerFile);
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

        private static int GetBenchmarkMode()
        {
            using (var selectBenchmarkMode = new FrmSelectBenchmarkMode())
            {
                DialogResult dialogResult = selectBenchmarkMode.ShowDialog();
                int delayPerFile = 0;
                if (dialogResult == DialogResult.Yes)
                {
                    // 150 ms
                    delayPerFile = 150;
                }
                else if (dialogResult == DialogResult.No)
                {
                    // 250 ms
                    delayPerFile = 250;
                }
                return delayPerFile;
            }
        }

        private void BgwArgon2Benchmark_DoWork(object sender, DoWorkEventArgs e)
        {
            int delayPerFile = (int)e.Argument;
            Argon2Benchmark.RunBenchmark(delayPerFile);
        }

        private void BgwArgon2Benchmark_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
