namespace KryptorGUI
{
    partial class frmPasswordGenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPasswordGenerator));
            this.txtGeneratedPassword = new System.Windows.Forms.TextBox();
            this.btnRegeneratePassword = new System.Windows.Forms.Button();
            this.btnCopyPassword = new System.Windows.Forms.Button();
            this.grbOptions = new System.Windows.Forms.GroupBox();
            this.lblType = new System.Windows.Forms.Label();
            this.chkSymbols = new System.Windows.Forms.CheckBox();
            this.chkNumbers = new System.Windows.Forms.CheckBox();
            this.chkUppercase = new System.Windows.Forms.CheckBox();
            this.chkLowercase = new System.Windows.Forms.CheckBox();
            this.nudLength = new System.Windows.Forms.NumericUpDown();
            this.lblLength = new System.Windows.Forms.Label();
            this.cmbGenerateType = new System.Windows.Forms.ComboBox();
            this.lblEntropy = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grbOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).BeginInit();
            this.SuspendLayout();
            // 
            // txtGeneratedPassword
            // 
            this.txtGeneratedPassword.BackColor = System.Drawing.SystemColors.Window;
            this.txtGeneratedPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtGeneratedPassword.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGeneratedPassword.Location = new System.Drawing.Point(12, 24);
            this.txtGeneratedPassword.Name = "txtGeneratedPassword";
            this.txtGeneratedPassword.ReadOnly = true;
            this.txtGeneratedPassword.ShortcutsEnabled = false;
            this.txtGeneratedPassword.Size = new System.Drawing.Size(500, 26);
            this.txtGeneratedPassword.TabIndex = 3;
            this.txtGeneratedPassword.TabStop = false;
            // 
            // btnRegeneratePassword
            // 
            this.btnRegeneratePassword.BackColor = System.Drawing.Color.Black;
            this.btnRegeneratePassword.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnRegeneratePassword.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnRegeneratePassword.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnRegeneratePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegeneratePassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnRegeneratePassword.ForeColor = System.Drawing.Color.White;
            this.btnRegeneratePassword.Location = new System.Drawing.Point(12, 56);
            this.btnRegeneratePassword.Name = "btnRegeneratePassword";
            this.btnRegeneratePassword.Size = new System.Drawing.Size(236, 46);
            this.btnRegeneratePassword.TabIndex = 1;
            this.btnRegeneratePassword.TabStop = false;
            this.btnRegeneratePassword.Text = "Regenerate Password";
            this.btnRegeneratePassword.UseVisualStyleBackColor = false;
            this.btnRegeneratePassword.Click += new System.EventHandler(this.BtnRegeneratePassword_Click);
            // 
            // btnCopyPassword
            // 
            this.btnCopyPassword.BackColor = System.Drawing.Color.Black;
            this.btnCopyPassword.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnCopyPassword.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCopyPassword.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnCopyPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopyPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnCopyPassword.ForeColor = System.Drawing.Color.White;
            this.btnCopyPassword.Location = new System.Drawing.Point(276, 56);
            this.btnCopyPassword.Name = "btnCopyPassword";
            this.btnCopyPassword.Size = new System.Drawing.Size(236, 46);
            this.btnCopyPassword.TabIndex = 2;
            this.btnCopyPassword.TabStop = false;
            this.btnCopyPassword.Text = "Copy Password";
            this.btnCopyPassword.UseVisualStyleBackColor = false;
            this.btnCopyPassword.Click += new System.EventHandler(this.BtnCopyPassword_Click);
            // 
            // grbOptions
            // 
            this.grbOptions.BackColor = System.Drawing.Color.Transparent;
            this.grbOptions.Controls.Add(this.label1);
            this.grbOptions.Controls.Add(this.lblType);
            this.grbOptions.Controls.Add(this.chkSymbols);
            this.grbOptions.Controls.Add(this.chkNumbers);
            this.grbOptions.Controls.Add(this.chkUppercase);
            this.grbOptions.Controls.Add(this.chkLowercase);
            this.grbOptions.Controls.Add(this.nudLength);
            this.grbOptions.Controls.Add(this.lblLength);
            this.grbOptions.Controls.Add(this.cmbGenerateType);
            this.grbOptions.ForeColor = System.Drawing.Color.Black;
            this.grbOptions.Location = new System.Drawing.Point(12, 111);
            this.grbOptions.Name = "grbOptions";
            this.grbOptions.Size = new System.Drawing.Size(500, 97);
            this.grbOptions.TabIndex = 68;
            this.grbOptions.TabStop = false;
            this.grbOptions.Text = "Options";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(13, 33);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(45, 21);
            this.lblType.TabIndex = 78;
            this.lblType.Text = "Type:";
            // 
            // chkSymbols
            // 
            this.chkSymbols.AutoSize = true;
            this.chkSymbols.Checked = true;
            this.chkSymbols.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSymbols.Location = new System.Drawing.Point(405, 66);
            this.chkSymbols.Name = "chkSymbols";
            this.chkSymbols.Size = new System.Drawing.Size(89, 25);
            this.chkSymbols.TabIndex = 10;
            this.chkSymbols.TabStop = false;
            this.chkSymbols.Text = "Symbols";
            this.chkSymbols.UseVisualStyleBackColor = true;
            this.chkSymbols.CheckedChanged += new System.EventHandler(this.ChkSymbols_CheckedChanged);
            // 
            // chkNumbers
            // 
            this.chkNumbers.AutoSize = true;
            this.chkNumbers.Checked = true;
            this.chkNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNumbers.Location = new System.Drawing.Point(299, 66);
            this.chkNumbers.Name = "chkNumbers";
            this.chkNumbers.Size = new System.Drawing.Size(94, 25);
            this.chkNumbers.TabIndex = 9;
            this.chkNumbers.TabStop = false;
            this.chkNumbers.Text = "Numbers";
            this.chkNumbers.UseVisualStyleBackColor = true;
            this.chkNumbers.CheckedChanged += new System.EventHandler(this.ChkNumbers_CheckedChanged);
            // 
            // chkUppercase
            // 
            this.chkUppercase.AutoSize = true;
            this.chkUppercase.Checked = true;
            this.chkUppercase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUppercase.Location = new System.Drawing.Point(185, 66);
            this.chkUppercase.Name = "chkUppercase";
            this.chkUppercase.Size = new System.Drawing.Size(102, 25);
            this.chkUppercase.TabIndex = 8;
            this.chkUppercase.TabStop = false;
            this.chkUppercase.Text = "Uppercase";
            this.chkUppercase.UseVisualStyleBackColor = true;
            this.chkUppercase.CheckedChanged += new System.EventHandler(this.ChkUppercase_CheckedChanged);
            // 
            // chkLowercase
            // 
            this.chkLowercase.AutoSize = true;
            this.chkLowercase.Checked = true;
            this.chkLowercase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLowercase.Location = new System.Drawing.Point(72, 66);
            this.chkLowercase.Name = "chkLowercase";
            this.chkLowercase.Size = new System.Drawing.Size(102, 25);
            this.chkLowercase.TabIndex = 7;
            this.chkLowercase.TabStop = false;
            this.chkLowercase.Text = "Lowercase";
            this.chkLowercase.UseVisualStyleBackColor = true;
            this.chkLowercase.CheckedChanged += new System.EventHandler(this.ChkLowercase_CheckedChanged);
            // 
            // nudLength
            // 
            this.nudLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudLength.ForeColor = System.Drawing.Color.Black;
            this.nudLength.Location = new System.Drawing.Point(417, 29);
            this.nudLength.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudLength.Minimum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.nudLength.Name = "nudLength";
            this.nudLength.Size = new System.Drawing.Size(69, 29);
            this.nudLength.TabIndex = 5;
            this.nudLength.TabStop = false;
            this.nudLength.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudLength.ValueChanged += new System.EventHandler(this.NudLength_ValueChanged);
            // 
            // lblLength
            // 
            this.lblLength.AutoSize = true;
            this.lblLength.Location = new System.Drawing.Point(349, 32);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(61, 21);
            this.lblLength.TabIndex = 4;
            this.lblLength.Text = "Length:";
            // 
            // cmbGenerateType
            // 
            this.cmbGenerateType.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbGenerateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGenerateType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbGenerateType.ForeColor = System.Drawing.Color.Black;
            this.cmbGenerateType.FormattingEnabled = true;
            this.cmbGenerateType.Items.AddRange(new object[] {
            "Password",
            "Passphrase"});
            this.cmbGenerateType.Location = new System.Drawing.Point(64, 30);
            this.cmbGenerateType.Name = "cmbGenerateType";
            this.cmbGenerateType.Size = new System.Drawing.Size(273, 29);
            this.cmbGenerateType.TabIndex = 2;
            this.cmbGenerateType.TabStop = false;
            this.cmbGenerateType.SelectedIndexChanged += new System.EventHandler(this.CmbGenerateType_SelectedIndexChanged);
            this.cmbGenerateType.DropDownClosed += new System.EventHandler(this.CmbGenerateType_DropDownClosed);
            // 
            // lblEntropy
            // 
            this.lblEntropy.AutoSize = true;
            this.lblEntropy.BackColor = System.Drawing.Color.Transparent;
            this.lblEntropy.ForeColor = System.Drawing.Color.Black;
            this.lblEntropy.Location = new System.Drawing.Point(86, 0);
            this.lblEntropy.Name = "lblEntropy";
            this.lblEntropy.Size = new System.Drawing.Size(48, 21);
            this.lblEntropy.TabIndex = 75;
            this.lblEntropy.Text = "0 bits";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.ForeColor = System.Drawing.Color.Black;
            this.lblPassword.Location = new System.Drawing.Point(8, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(79, 21);
            this.lblPassword.TabIndex = 76;
            this.lblPassword.Text = "Password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 21);
            this.label1.TabIndex = 79;
            this.label1.Text = "Chars:";
            // 
            // frmPasswordGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(524, 217);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblEntropy);
            this.Controls.Add(this.grbOptions);
            this.Controls.Add(this.btnCopyPassword);
            this.Controls.Add(this.btnRegeneratePassword);
            this.Controls.Add(this.txtGeneratedPassword);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPasswordGenerator";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Password Generator";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmPasswordGenerator_Load);
            this.grbOptions.ResumeLayout(false);
            this.grbOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtGeneratedPassword;
        private System.Windows.Forms.Button btnRegeneratePassword;
        private System.Windows.Forms.Button btnCopyPassword;
        private System.Windows.Forms.GroupBox grbOptions;
        private System.Windows.Forms.ComboBox cmbGenerateType;
        private System.Windows.Forms.NumericUpDown nudLength;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.CheckBox chkSymbols;
        private System.Windows.Forms.CheckBox chkNumbers;
        private System.Windows.Forms.CheckBox chkUppercase;
        private System.Windows.Forms.CheckBox chkLowercase;
        private System.Windows.Forms.Label lblEntropy;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label label1;
    }
}