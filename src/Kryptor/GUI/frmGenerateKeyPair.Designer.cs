namespace Kryptor
{
    partial class frmGenerateKeyPair
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGenerateKeyPair));
            this.btnStoredKeys = new System.Windows.Forms.Button();
            this.txtPrivateKey = new System.Windows.Forms.TextBox();
            this.cmsKeyPairMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopyTextbox = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExportPublicKey = new System.Windows.Forms.Button();
            this.lblPublicKey = new System.Windows.Forms.Label();
            this.txtPublicKey = new System.Windows.Forms.TextBox();
            this.lblPrivateKey = new System.Windows.Forms.Label();
            this.picHelp = new System.Windows.Forms.PictureBox();
            this.cmsKeyPairMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHelp)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStoredKeys
            // 
            this.btnStoredKeys.BackColor = System.Drawing.Color.Black;
            this.btnStoredKeys.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnStoredKeys.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnStoredKeys.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnStoredKeys.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStoredKeys.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnStoredKeys.ForeColor = System.Drawing.Color.White;
            this.btnStoredKeys.Location = new System.Drawing.Point(276, 138);
            this.btnStoredKeys.Name = "btnStoredKeys";
            this.btnStoredKeys.Size = new System.Drawing.Size(236, 46);
            this.btnStoredKeys.TabIndex = 86;
            this.btnStoredKeys.TabStop = false;
            this.btnStoredKeys.Text = "I have stored my keys";
            this.btnStoredKeys.UseVisualStyleBackColor = false;
            this.btnStoredKeys.Click += new System.EventHandler(this.btnStoredKeys_Click);
            // 
            // txtPrivateKey
            // 
            this.txtPrivateKey.BackColor = System.Drawing.Color.White;
            this.txtPrivateKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrivateKey.ContextMenuStrip = this.cmsKeyPairMenu;
            this.txtPrivateKey.Location = new System.Drawing.Point(12, 103);
            this.txtPrivateKey.MaxLength = 2147483647;
            this.txtPrivateKey.Name = "txtPrivateKey";
            this.txtPrivateKey.ReadOnly = true;
            this.txtPrivateKey.Size = new System.Drawing.Size(500, 29);
            this.txtPrivateKey.TabIndex = 85;
            this.txtPrivateKey.TabStop = false;
            // 
            // cmsKeyPairMenu
            // 
            this.cmsKeyPairMenu.BackColor = System.Drawing.Color.White;
            this.cmsKeyPairMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmsKeyPairMenu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmsKeyPairMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyTextbox,
            this.tsmiClearClipboard});
            this.cmsKeyPairMenu.Name = "passwordContextMenu";
            this.cmsKeyPairMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsKeyPairMenu.Size = new System.Drawing.Size(189, 78);
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
            this.tsmiCopyTextbox.Click += new System.EventHandler(this.tsmiCopyTextbox_Click);
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
            this.tsmiClearClipboard.Click += new System.EventHandler(this.tsmiClearClipboard_Click);
            // 
            // btnExportPublicKey
            // 
            this.btnExportPublicKey.BackColor = System.Drawing.Color.Black;
            this.btnExportPublicKey.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnExportPublicKey.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnExportPublicKey.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnExportPublicKey.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportPublicKey.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnExportPublicKey.ForeColor = System.Drawing.Color.White;
            this.btnExportPublicKey.Location = new System.Drawing.Point(12, 138);
            this.btnExportPublicKey.Name = "btnExportPublicKey";
            this.btnExportPublicKey.Size = new System.Drawing.Size(236, 46);
            this.btnExportPublicKey.TabIndex = 84;
            this.btnExportPublicKey.TabStop = false;
            this.btnExportPublicKey.Text = "Export Public Key";
            this.btnExportPublicKey.UseVisualStyleBackColor = false;
            this.btnExportPublicKey.Click += new System.EventHandler(this.btnExportPublicKey_Click);
            // 
            // lblPublicKey
            // 
            this.lblPublicKey.AutoSize = true;
            this.lblPublicKey.BackColor = System.Drawing.Color.Transparent;
            this.lblPublicKey.ForeColor = System.Drawing.Color.Black;
            this.lblPublicKey.Location = new System.Drawing.Point(8, 10);
            this.lblPublicKey.Name = "lblPublicKey";
            this.lblPublicKey.Size = new System.Drawing.Size(84, 21);
            this.lblPublicKey.TabIndex = 83;
            this.lblPublicKey.Text = "Public Key:";
            // 
            // txtPublicKey
            // 
            this.txtPublicKey.BackColor = System.Drawing.Color.White;
            this.txtPublicKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPublicKey.ContextMenuStrip = this.cmsKeyPairMenu;
            this.txtPublicKey.Location = new System.Drawing.Point(12, 34);
            this.txtPublicKey.MaxLength = 2147483647;
            this.txtPublicKey.Name = "txtPublicKey";
            this.txtPublicKey.ReadOnly = true;
            this.txtPublicKey.Size = new System.Drawing.Size(500, 29);
            this.txtPublicKey.TabIndex = 81;
            this.txtPublicKey.TabStop = false;
            // 
            // lblPrivateKey
            // 
            this.lblPrivateKey.AutoSize = true;
            this.lblPrivateKey.BackColor = System.Drawing.Color.Transparent;
            this.lblPrivateKey.ForeColor = System.Drawing.Color.Black;
            this.lblPrivateKey.Location = new System.Drawing.Point(8, 79);
            this.lblPrivateKey.Name = "lblPrivateKey";
            this.lblPrivateKey.Size = new System.Drawing.Size(90, 21);
            this.lblPrivateKey.TabIndex = 82;
            this.lblPrivateKey.Text = "Private Key:";
            // 
            // picHelp
            // 
            this.picHelp.BackColor = System.Drawing.Color.Transparent;
            this.picHelp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picHelp.BackgroundImage")));
            this.picHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picHelp.Location = new System.Drawing.Point(487, 9);
            this.picHelp.Name = "picHelp";
            this.picHelp.Size = new System.Drawing.Size(25, 22);
            this.picHelp.TabIndex = 87;
            this.picHelp.TabStop = false;
            this.picHelp.Click += new System.EventHandler(this.picHelp_Click);
            // 
            // frmGenerateKeyPair
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(524, 196);
            this.Controls.Add(this.picHelp);
            this.Controls.Add(this.btnStoredKeys);
            this.Controls.Add(this.txtPrivateKey);
            this.Controls.Add(this.btnExportPublicKey);
            this.Controls.Add(this.lblPublicKey);
            this.Controls.Add(this.txtPublicKey);
            this.Controls.Add(this.lblPrivateKey);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frmGenerateKeyPair";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Generate Key Pair";
            this.Load += new System.EventHandler(this.frmGenerateKeyPair_Load);
            this.cmsKeyPairMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picHelp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStoredKeys;
        private System.Windows.Forms.TextBox txtPrivateKey;
        private System.Windows.Forms.Button btnExportPublicKey;
        private System.Windows.Forms.Label lblPublicKey;
        private System.Windows.Forms.TextBox txtPublicKey;
        private System.Windows.Forms.Label lblPrivateKey;
        private System.Windows.Forms.ContextMenuStrip cmsKeyPairMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyTextbox;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearClipboard;
        private System.Windows.Forms.PictureBox picHelp;
    }
}