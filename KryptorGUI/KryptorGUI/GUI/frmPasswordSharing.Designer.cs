namespace KryptorGUI
{
    partial class FrmPasswordSharing
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPasswordSharing));
            this.btnEncryptPassword = new System.Windows.Forms.Button();
            this.lblAsymmetricKey = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.cmsTextboxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopyTextbox = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearTextbox = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnDecryptPassword = new System.Windows.Forms.Button();
            this.llbGenerateKeyPair = new System.Windows.Forms.LinkLabel();
            this.picHelp = new System.Windows.Forms.PictureBox();
            this.cmsTextboxMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHelp)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEncryptPassword
            // 
            this.btnEncryptPassword.BackColor = System.Drawing.Color.Black;
            this.btnEncryptPassword.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnEncryptPassword.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnEncryptPassword.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnEncryptPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEncryptPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnEncryptPassword.ForeColor = System.Drawing.Color.White;
            this.btnEncryptPassword.Location = new System.Drawing.Point(12, 138);
            this.btnEncryptPassword.Name = "btnEncryptPassword";
            this.btnEncryptPassword.Size = new System.Drawing.Size(236, 46);
            this.btnEncryptPassword.TabIndex = 78;
            this.btnEncryptPassword.TabStop = false;
            this.btnEncryptPassword.Text = "Encrypt Password";
            this.btnEncryptPassword.UseVisualStyleBackColor = false;
            this.btnEncryptPassword.Click += new System.EventHandler(this.BtnEncryptPassword_Click);
            // 
            // lblAsymmetricKey
            // 
            this.lblAsymmetricKey.AutoSize = true;
            this.lblAsymmetricKey.BackColor = System.Drawing.Color.Transparent;
            this.lblAsymmetricKey.ForeColor = System.Drawing.Color.Black;
            this.lblAsymmetricKey.Location = new System.Drawing.Point(8, 10);
            this.lblAsymmetricKey.Name = "lblAsymmetricKey";
            this.lblAsymmetricKey.Size = new System.Drawing.Size(125, 21);
            this.lblAsymmetricKey.TabIndex = 77;
            this.lblAsymmetricKey.Text = "Asymmetric Key:";
            // 
            // txtKey
            // 
            this.txtKey.BackColor = System.Drawing.Color.White;
            this.txtKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKey.ContextMenuStrip = this.cmsTextboxMenu;
            this.txtKey.Location = new System.Drawing.Point(12, 34);
            this.txtKey.MaxLength = 2147483647;
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(500, 29);
            this.txtKey.TabIndex = 74;
            this.txtKey.TabStop = false;
            // 
            // cmsTextboxMenu
            // 
            this.cmsTextboxMenu.BackColor = System.Drawing.Color.White;
            this.cmsTextboxMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmsTextboxMenu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmsTextboxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyTextbox,
            this.tsmiClearTextbox,
            this.tsmiClearClipboard});
            this.cmsTextboxMenu.Name = "passwordContextMenu";
            this.cmsTextboxMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsTextboxMenu.Size = new System.Drawing.Size(189, 104);
            // 
            // tsmiCopyTextbox
            // 
            this.tsmiCopyTextbox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tsmiCopyTextbox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiCopyTextbox.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsmiCopyTextbox.Name = "tsmiCopyTextbox";
            this.tsmiCopyTextbox.Size = new System.Drawing.Size(188, 26);
            this.tsmiCopyTextbox.Text = "Copy Textbox";
            this.tsmiCopyTextbox.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsmiCopyTextbox.Click += new System.EventHandler(this.TsmiCopyTextbox_Click);
            // 
            // tsmiClearTextbox
            // 
            this.tsmiClearTextbox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tsmiClearTextbox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiClearTextbox.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsmiClearTextbox.Name = "tsmiClearTextbox";
            this.tsmiClearTextbox.Size = new System.Drawing.Size(188, 26);
            this.tsmiClearTextbox.Text = "Clear Textbox";
            this.tsmiClearTextbox.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsmiClearTextbox.Click += new System.EventHandler(this.TsmiClearTextbox_Click);
            // 
            // tsmiClearClipboard
            // 
            this.tsmiClearClipboard.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tsmiClearClipboard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiClearClipboard.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsmiClearClipboard.Name = "tsmiClearClipboard";
            this.tsmiClearClipboard.Size = new System.Drawing.Size(188, 26);
            this.tsmiClearClipboard.Text = "Clear Clipboard";
            this.tsmiClearClipboard.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsmiClearClipboard.Click += new System.EventHandler(this.TsmiClearClipboard_Click);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.ForeColor = System.Drawing.Color.Black;
            this.lblPassword.Location = new System.Drawing.Point(8, 79);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(79, 21);
            this.lblPassword.TabIndex = 75;
            this.lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.White;
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.ContextMenuStrip = this.cmsTextboxMenu;
            this.txtPassword.Location = new System.Drawing.Point(12, 103);
            this.txtPassword.MaxLength = 2147483647;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(500, 29);
            this.txtPassword.TabIndex = 79;
            this.txtPassword.TabStop = false;
            // 
            // btnDecryptPassword
            // 
            this.btnDecryptPassword.BackColor = System.Drawing.Color.Black;
            this.btnDecryptPassword.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDecryptPassword.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnDecryptPassword.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnDecryptPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDecryptPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnDecryptPassword.ForeColor = System.Drawing.Color.White;
            this.btnDecryptPassword.Location = new System.Drawing.Point(276, 138);
            this.btnDecryptPassword.Name = "btnDecryptPassword";
            this.btnDecryptPassword.Size = new System.Drawing.Size(236, 46);
            this.btnDecryptPassword.TabIndex = 80;
            this.btnDecryptPassword.TabStop = false;
            this.btnDecryptPassword.Text = "Decrypt Password";
            this.btnDecryptPassword.UseVisualStyleBackColor = false;
            this.btnDecryptPassword.Click += new System.EventHandler(this.BtnDecryptPassword_Click);
            // 
            // llbGenerateKeyPair
            // 
            this.llbGenerateKeyPair.ActiveLinkColor = System.Drawing.Color.Black;
            this.llbGenerateKeyPair.AutoSize = true;
            this.llbGenerateKeyPair.BackColor = System.Drawing.Color.Transparent;
            this.llbGenerateKeyPair.ForeColor = System.Drawing.Color.Black;
            this.llbGenerateKeyPair.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.llbGenerateKeyPair.LinkColor = System.Drawing.Color.Black;
            this.llbGenerateKeyPair.Location = new System.Drawing.Point(309, 66);
            this.llbGenerateKeyPair.Name = "llbGenerateKeyPair";
            this.llbGenerateKeyPair.Size = new System.Drawing.Size(203, 21);
            this.llbGenerateKeyPair.TabIndex = 0;
            this.llbGenerateKeyPair.TabStop = true;
            this.llbGenerateKeyPair.Text = "Recipient: Generate Key Pair";
            this.llbGenerateKeyPair.VisitedLinkColor = System.Drawing.Color.DimGray;
            this.llbGenerateKeyPair.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlbGenerateKeyPair_LinkClicked);
            // 
            // picHelp
            // 
            this.picHelp.BackColor = System.Drawing.Color.Transparent;
            this.picHelp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picHelp.BackgroundImage")));
            this.picHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picHelp.Location = new System.Drawing.Point(487, 10);
            this.picHelp.Name = "picHelp";
            this.picHelp.Size = new System.Drawing.Size(25, 22);
            this.picHelp.TabIndex = 82;
            this.picHelp.TabStop = false;
            this.picHelp.Click += new System.EventHandler(this.PicHelp_Click);
            // 
            // frmPasswordSharing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(524, 196);
            this.Controls.Add(this.picHelp);
            this.Controls.Add(this.llbGenerateKeyPair);
            this.Controls.Add(this.btnDecryptPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnEncryptPassword);
            this.Controls.Add(this.lblAsymmetricKey);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.lblPassword);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.MaximizeBox = false;
            this.Name = "frmPasswordSharing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Password Sharing";
            this.Load += new System.EventHandler(this.FrmPasswordSharing_Load);
            this.cmsTextboxMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picHelp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnEncryptPassword;
        private System.Windows.Forms.Label lblAsymmetricKey;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnDecryptPassword;
        private System.Windows.Forms.LinkLabel llbGenerateKeyPair;
        private System.Windows.Forms.ContextMenuStrip cmsTextboxMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyTextbox;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearTextbox;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearClipboard;
        private System.Windows.Forms.PictureBox picHelp;
    }
}