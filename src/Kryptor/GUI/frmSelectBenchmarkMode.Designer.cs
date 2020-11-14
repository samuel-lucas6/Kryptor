namespace Kryptor
{
    partial class FrmSelectBenchmarkMode
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSelectBenchmarkMode));
            this.btnSecurityMode = new System.Windows.Forms.Button();
            this.btnSpeedMode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSecurityMode
            // 
            this.btnSecurityMode.BackColor = System.Drawing.Color.Black;
            this.btnSecurityMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSecurityMode.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnSecurityMode.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnSecurityMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSecurityMode.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnSecurityMode.ForeColor = System.Drawing.Color.White;
            this.btnSecurityMode.Location = new System.Drawing.Point(12, 68);
            this.btnSecurityMode.Name = "btnSecurityMode";
            this.btnSecurityMode.Size = new System.Drawing.Size(462, 50);
            this.btnSecurityMode.TabIndex = 71;
            this.btnSecurityMode.TabStop = false;
            this.btnSecurityMode.Text = "I want encryption to be more secure.";
            this.btnSecurityMode.UseVisualStyleBackColor = false;
            this.btnSecurityMode.Click += new System.EventHandler(this.BtnSecurityMode_Click);
            // 
            // btnSpeedMode
            // 
            this.btnSpeedMode.BackColor = System.Drawing.Color.Black;
            this.btnSpeedMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSpeedMode.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnSpeedMode.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnSpeedMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSpeedMode.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnSpeedMode.ForeColor = System.Drawing.Color.White;
            this.btnSpeedMode.Location = new System.Drawing.Point(12, 12);
            this.btnSpeedMode.Name = "btnSpeedMode";
            this.btnSpeedMode.Size = new System.Drawing.Size(462, 50);
            this.btnSpeedMode.TabIndex = 70;
            this.btnSpeedMode.TabStop = false;
            this.btnSpeedMode.Text = "I want encryption to be as fast as possible.";
            this.btnSpeedMode.UseVisualStyleBackColor = false;
            this.btnSpeedMode.Click += new System.EventHandler(this.BtnSpeedMode_Click);
            // 
            // frmSelectBenchmarkMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(486, 130);
            this.Controls.Add(this.btnSecurityMode);
            this.Controls.Add(this.btnSpeedMode);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frmSelectBenchmarkMode";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Benchmark Mode";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmSelectBenchmarkMode_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSecurityMode;
        private System.Windows.Forms.Button btnSpeedMode;
    }
}