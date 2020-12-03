namespace Kryptor
{
    partial class FrmAboutKryptor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAboutKryptor));
            this.lblProgramDescription = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.picKryptorLogo = new System.Windows.Forms.PictureBox();
            this.txtAbout = new System.Windows.Forms.TextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picKryptorLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProgramDescription
            // 
            this.lblProgramDescription.AutoSize = true;
            this.lblProgramDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblProgramDescription.ForeColor = System.Drawing.Color.Black;
            this.lblProgramDescription.Location = new System.Drawing.Point(103, 54);
            this.lblProgramDescription.Name = "lblProgramDescription";
            this.lblProgramDescription.Size = new System.Drawing.Size(329, 21);
            this.lblProgramDescription.TabIndex = 74;
            this.lblProgramDescription.Text = "Free and open source file encryption software.";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(97, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(125, 45);
            this.lblTitle.TabIndex = 73;
            this.lblTitle.Text = "Kryptor";
            // 
            // picKryptorLogo
            // 
            this.picKryptorLogo.BackColor = System.Drawing.Color.Transparent;
            this.picKryptorLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picKryptorLogo.BackgroundImage")));
            this.picKryptorLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picKryptorLogo.Location = new System.Drawing.Point(12, 12);
            this.picKryptorLogo.Name = "picKryptorLogo";
            this.picKryptorLogo.Size = new System.Drawing.Size(79, 77);
            this.picKryptorLogo.TabIndex = 72;
            this.picKryptorLogo.TabStop = false;
            // 
            // txtAbout
            // 
            this.txtAbout.BackColor = System.Drawing.Color.White;
            this.txtAbout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAbout.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtAbout.ForeColor = System.Drawing.Color.Black;
            this.txtAbout.Location = new System.Drawing.Point(12, 97);
            this.txtAbout.Multiline = true;
            this.txtAbout.Name = "txtAbout";
            this.txtAbout.ReadOnly = true;
            this.txtAbout.ShortcutsEnabled = false;
            this.txtAbout.Size = new System.Drawing.Size(520, 134);
            this.txtAbout.TabIndex = 75;
            this.txtAbout.TabStop = false;
            this.txtAbout.Text = resources.GetString("txtAbout.Text");
            this.txtAbout.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtAbout.GotFocus += new System.EventHandler(this.TxtAbout_GotFocus);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblVersion.ForeColor = System.Drawing.Color.Black;
            this.lblVersion.Location = new System.Drawing.Point(399, 9);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(133, 21);
            this.lblVersion.TabIndex = 76;
            this.lblVersion.Text = "Version 2.2.0 Beta";
            // 
            // FrmAboutKryptor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(544, 243);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.txtAbout);
            this.Controls.Add(this.lblProgramDescription);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.picKryptorLogo);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAboutKryptor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Kryptor";
            this.Load += new System.EventHandler(this.FrmAboutKryptor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picKryptorLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblProgramDescription;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox picKryptorLogo;
        private System.Windows.Forms.TextBox txtAbout;
        private System.Windows.Forms.Label lblVersion;
    }
}