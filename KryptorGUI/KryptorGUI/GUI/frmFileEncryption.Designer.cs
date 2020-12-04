namespace KryptorGUI
{
    partial class FrmFileEncryption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFileEncryption));
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.cmsPasswordMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCopyPassword = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearPassword = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.picFilesSelected = new System.Windows.Forms.PictureBox();
            this.cmsClearFilesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiClearSelectedFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.lblFilesSelected = new System.Windows.Forms.Label();
            this.lblDragDrop = new System.Windows.Forms.Label();
            this.lblShowPassword = new System.Windows.Forms.Label();
            this.chkShowPassword = new System.Windows.Forms.CheckBox();
            this.msMenus = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSelectFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSelectFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCreateKeyfile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSelectKeyfile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTools = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPasswordGenerator = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPasswordSharing = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShredFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShredFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBackupSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRestoreSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDocumentation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSourceCode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDonate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrClearClipboard = new System.Windows.Forms.Timer(this.components);
            this.masterPasswordOffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.masterPasswordOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bgwEncryption = new System.ComponentModel.BackgroundWorker();
            this.bgwDecryption = new System.ComponentModel.BackgroundWorker();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.bgwShredFiles = new System.ComponentModel.BackgroundWorker();
            this.lblEntropy = new System.Windows.Forms.Label();
            this.cmsPasswordMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFilesSelected)).BeginInit();
            this.cmsClearFilesMenu.SuspendLayout();
            this.msMenus.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.White;
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.ContextMenuStrip = this.cmsPasswordMenu;
            this.txtPassword.Location = new System.Drawing.Point(12, 101);
            this.txtPassword.MaxLength = 256;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(500, 29);
            this.txtPassword.TabIndex = 49;
            this.txtPassword.TabStop = false;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TextChanged += new System.EventHandler(this.TxtPassword_TextChanged);
            // 
            // cmsPasswordMenu
            // 
            this.cmsPasswordMenu.BackColor = System.Drawing.Color.White;
            this.cmsPasswordMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmsPasswordMenu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmsPasswordMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCopyPassword,
            this.tsmiClearPassword,
            this.tsmiClearClipboard});
            this.cmsPasswordMenu.Name = "passwordContextMenu";
            this.cmsPasswordMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsPasswordMenu.Size = new System.Drawing.Size(258, 82);
            // 
            // tsmiCopyPassword
            // 
            this.tsmiCopyPassword.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tsmiCopyPassword.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiCopyPassword.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsmiCopyPassword.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsmiCopyPassword.Name = "tsmiCopyPassword";
            this.tsmiCopyPassword.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.tsmiCopyPassword.Size = new System.Drawing.Size(257, 26);
            this.tsmiCopyPassword.Text = "Copy Password";
            this.tsmiCopyPassword.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsmiCopyPassword.Click += new System.EventHandler(this.TsmiCopyPassword_Click);
            // 
            // tsmiClearPassword
            // 
            this.tsmiClearPassword.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tsmiClearPassword.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiClearPassword.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsmiClearPassword.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsmiClearPassword.Name = "tsmiClearPassword";
            this.tsmiClearPassword.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.tsmiClearPassword.Size = new System.Drawing.Size(257, 26);
            this.tsmiClearPassword.Text = "Clear Password";
            this.tsmiClearPassword.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsmiClearPassword.Click += new System.EventHandler(this.TsmiClearPassword_Click);
            // 
            // tsmiClearClipboard
            // 
            this.tsmiClearClipboard.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tsmiClearClipboard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiClearClipboard.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsmiClearClipboard.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsmiClearClipboard.Name = "tsmiClearClipboard";
            this.tsmiClearClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.tsmiClearClipboard.Size = new System.Drawing.Size(257, 26);
            this.tsmiClearClipboard.Text = "Clear Clipboard";
            this.tsmiClearClipboard.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsmiClearClipboard.Click += new System.EventHandler(this.TsmiClearClipboard_Click);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.ForeColor = System.Drawing.Color.Black;
            this.lblPassword.Location = new System.Drawing.Point(8, 73);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(79, 21);
            this.lblPassword.TabIndex = 50;
            this.lblPassword.Text = "Password:";
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.BackColor = System.Drawing.Color.Black;
            this.btnEncrypt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnEncrypt.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnEncrypt.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnEncrypt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEncrypt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEncrypt.ForeColor = System.Drawing.Color.White;
            this.btnEncrypt.Location = new System.Drawing.Point(12, 136);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(236, 46);
            this.btnEncrypt.TabIndex = 65;
            this.btnEncrypt.TabStop = false;
            this.btnEncrypt.Text = "Encrypt";
            this.btnEncrypt.UseVisualStyleBackColor = false;
            this.btnEncrypt.Click += new System.EventHandler(this.BtnEncrypt_Click);
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.BackColor = System.Drawing.Color.Black;
            this.btnDecrypt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnDecrypt.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnDecrypt.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btnDecrypt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDecrypt.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDecrypt.ForeColor = System.Drawing.Color.White;
            this.btnDecrypt.Location = new System.Drawing.Point(276, 136);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(236, 46);
            this.btnDecrypt.TabIndex = 66;
            this.btnDecrypt.TabStop = false;
            this.btnDecrypt.Text = "Decrypt";
            this.btnDecrypt.UseVisualStyleBackColor = false;
            this.btnDecrypt.Click += new System.EventHandler(this.BtnDecrypt_Click);
            // 
            // picFilesSelected
            // 
            this.picFilesSelected.BackColor = System.Drawing.Color.Red;
            this.picFilesSelected.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picFilesSelected.BackgroundImage")));
            this.picFilesSelected.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picFilesSelected.ContextMenuStrip = this.cmsClearFilesMenu;
            this.picFilesSelected.Location = new System.Drawing.Point(490, 38);
            this.picFilesSelected.Name = "picFilesSelected";
            this.picFilesSelected.Size = new System.Drawing.Size(22, 22);
            this.picFilesSelected.TabIndex = 67;
            this.picFilesSelected.TabStop = false;
            // 
            // cmsClearFilesMenu
            // 
            this.cmsClearFilesMenu.BackColor = System.Drawing.Color.White;
            this.cmsClearFilesMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmsClearFilesMenu.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmsClearFilesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiClearSelectedFiles});
            this.cmsClearFilesMenu.Name = "passwordContextMenu";
            this.cmsClearFilesMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsClearFilesMenu.Size = new System.Drawing.Size(214, 30);
            // 
            // tsmiClearSelectedFiles
            // 
            this.tsmiClearSelectedFiles.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tsmiClearSelectedFiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiClearSelectedFiles.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsmiClearSelectedFiles.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsmiClearSelectedFiles.Name = "tsmiClearSelectedFiles";
            this.tsmiClearSelectedFiles.Size = new System.Drawing.Size(213, 26);
            this.tsmiClearSelectedFiles.Text = "Clear Selected Files";
            this.tsmiClearSelectedFiles.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsmiClearSelectedFiles.Click += new System.EventHandler(this.TsmiClearSelectedFiles_Click);
            // 
            // lblFilesSelected
            // 
            this.lblFilesSelected.AutoSize = true;
            this.lblFilesSelected.BackColor = System.Drawing.Color.Transparent;
            this.lblFilesSelected.ForeColor = System.Drawing.Color.Black;
            this.lblFilesSelected.Location = new System.Drawing.Point(362, 38);
            this.lblFilesSelected.Name = "lblFilesSelected";
            this.lblFilesSelected.Size = new System.Drawing.Size(106, 21);
            this.lblFilesSelected.TabIndex = 68;
            this.lblFilesSelected.Text = "Files Selected:";
            // 
            // lblDragDrop
            // 
            this.lblDragDrop.AutoSize = true;
            this.lblDragDrop.BackColor = System.Drawing.Color.Transparent;
            this.lblDragDrop.ForeColor = System.Drawing.Color.Black;
            this.lblDragDrop.Location = new System.Drawing.Point(8, 39);
            this.lblDragDrop.Name = "lblDragDrop";
            this.lblDragDrop.Size = new System.Drawing.Size(235, 21);
            this.lblDragDrop.TabIndex = 69;
            this.lblDragDrop.Text = "Drag and drop files/folders here.";
            // 
            // lblShowPassword
            // 
            this.lblShowPassword.AutoSize = true;
            this.lblShowPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblShowPassword.ForeColor = System.Drawing.Color.Black;
            this.lblShowPassword.Location = new System.Drawing.Point(362, 73);
            this.lblShowPassword.Name = "lblShowPassword";
            this.lblShowPassword.Size = new System.Drawing.Size(122, 21);
            this.lblShowPassword.TabIndex = 70;
            this.lblShowPassword.Text = "Show Password:";
            // 
            // chkShowPassword
            // 
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Location = new System.Drawing.Point(497, 79);
            this.chkShowPassword.Name = "chkShowPassword";
            this.chkShowPassword.Size = new System.Drawing.Size(15, 14);
            this.chkShowPassword.TabIndex = 71;
            this.chkShowPassword.TabStop = false;
            this.chkShowPassword.UseVisualStyleBackColor = true;
            this.chkShowPassword.CheckedChanged += new System.EventHandler(this.ShowPassword_CheckedChanged);
            // 
            // msMenus
            // 
            this.msMenus.BackColor = System.Drawing.Color.White;
            this.msMenus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.msMenus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msMenus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiTools,
            this.tsmiHelp});
            this.msMenus.Location = new System.Drawing.Point(0, 0);
            this.msMenus.Name = "msMenus";
            this.msMenus.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.msMenus.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.msMenus.Size = new System.Drawing.Size(524, 29);
            this.msMenus.TabIndex = 72;
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSelectFiles,
            this.tsmiSelectFolder,
            this.tsmiCreateKeyfile,
            this.tsmiSelectKeyfile,
            this.tsmiSettings,
            this.tsmiQuit});
            this.tsmiFile.ForeColor = System.Drawing.Color.Black;
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(46, 25);
            this.tsmiFile.Text = "File";
            // 
            // tsmiSelectFiles
            // 
            this.tsmiSelectFiles.BackColor = System.Drawing.Color.White;
            this.tsmiSelectFiles.ForeColor = System.Drawing.Color.Black;
            this.tsmiSelectFiles.Name = "tsmiSelectFiles";
            this.tsmiSelectFiles.Size = new System.Drawing.Size(175, 26);
            this.tsmiSelectFiles.Text = "Select Files";
            this.tsmiSelectFiles.Click += new System.EventHandler(this.TsmiSelectFiles_Click);
            // 
            // tsmiSelectFolder
            // 
            this.tsmiSelectFolder.BackColor = System.Drawing.Color.White;
            this.tsmiSelectFolder.ForeColor = System.Drawing.Color.Black;
            this.tsmiSelectFolder.Name = "tsmiSelectFolder";
            this.tsmiSelectFolder.Size = new System.Drawing.Size(175, 26);
            this.tsmiSelectFolder.Text = "Select Folder";
            this.tsmiSelectFolder.Click += new System.EventHandler(this.TsmiSelectFolder_Click);
            // 
            // tsmiCreateKeyfile
            // 
            this.tsmiCreateKeyfile.BackColor = System.Drawing.Color.White;
            this.tsmiCreateKeyfile.ForeColor = System.Drawing.Color.Black;
            this.tsmiCreateKeyfile.Name = "tsmiCreateKeyfile";
            this.tsmiCreateKeyfile.Size = new System.Drawing.Size(175, 26);
            this.tsmiCreateKeyfile.Text = "Create Keyfile";
            this.tsmiCreateKeyfile.Click += new System.EventHandler(this.TsmiCreateKeyFile_Click);
            // 
            // tsmiSelectKeyfile
            // 
            this.tsmiSelectKeyfile.BackColor = System.Drawing.Color.White;
            this.tsmiSelectKeyfile.ForeColor = System.Drawing.Color.Black;
            this.tsmiSelectKeyfile.Name = "tsmiSelectKeyfile";
            this.tsmiSelectKeyfile.Size = new System.Drawing.Size(175, 26);
            this.tsmiSelectKeyfile.Text = "Select Keyfile";
            this.tsmiSelectKeyfile.Click += new System.EventHandler(this.TsmiSelectKeyfile_Click);
            // 
            // tsmiSettings
            // 
            this.tsmiSettings.BackColor = System.Drawing.Color.White;
            this.tsmiSettings.ForeColor = System.Drawing.Color.Black;
            this.tsmiSettings.Name = "tsmiSettings";
            this.tsmiSettings.Size = new System.Drawing.Size(175, 26);
            this.tsmiSettings.Text = "Settings";
            this.tsmiSettings.Click += new System.EventHandler(this.TsmiSettings_Click);
            // 
            // tsmiQuit
            // 
            this.tsmiQuit.BackColor = System.Drawing.Color.White;
            this.tsmiQuit.ForeColor = System.Drawing.Color.Black;
            this.tsmiQuit.Name = "tsmiQuit";
            this.tsmiQuit.Size = new System.Drawing.Size(175, 26);
            this.tsmiQuit.Text = "Quit Kryptor";
            this.tsmiQuit.Click += new System.EventHandler(this.TsmiQuit_Click);
            // 
            // tsmiTools
            // 
            this.tsmiTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPasswordGenerator,
            this.tsmiPasswordSharing,
            this.tsmiShredFiles,
            this.tsmiShredFolder,
            this.tsmiBackupSettings,
            this.tsmiRestoreSettings});
            this.tsmiTools.ForeColor = System.Drawing.Color.Black;
            this.tsmiTools.Name = "tsmiTools";
            this.tsmiTools.Size = new System.Drawing.Size(57, 25);
            this.tsmiTools.Text = "Tools";
            // 
            // tsmiPasswordGenerator
            // 
            this.tsmiPasswordGenerator.BackColor = System.Drawing.Color.White;
            this.tsmiPasswordGenerator.ForeColor = System.Drawing.Color.Black;
            this.tsmiPasswordGenerator.Name = "tsmiPasswordGenerator";
            this.tsmiPasswordGenerator.Size = new System.Drawing.Size(220, 26);
            this.tsmiPasswordGenerator.Text = "Password Generator";
            this.tsmiPasswordGenerator.Click += new System.EventHandler(this.TsmiPasswordGenerator_Click);
            // 
            // tsmiPasswordSharing
            // 
            this.tsmiPasswordSharing.BackColor = System.Drawing.Color.White;
            this.tsmiPasswordSharing.ForeColor = System.Drawing.Color.Black;
            this.tsmiPasswordSharing.Name = "tsmiPasswordSharing";
            this.tsmiPasswordSharing.Size = new System.Drawing.Size(220, 26);
            this.tsmiPasswordSharing.Text = "Password Sharing";
            this.tsmiPasswordSharing.Click += new System.EventHandler(this.TsmiPasswordSharing_Click);
            // 
            // tsmiShredFiles
            // 
            this.tsmiShredFiles.BackColor = System.Drawing.Color.White;
            this.tsmiShredFiles.ForeColor = System.Drawing.Color.Black;
            this.tsmiShredFiles.Name = "tsmiShredFiles";
            this.tsmiShredFiles.Size = new System.Drawing.Size(220, 26);
            this.tsmiShredFiles.Text = "Shred Files";
            this.tsmiShredFiles.Click += new System.EventHandler(this.TsmiShredFiles_Click);
            // 
            // tsmiShredFolder
            // 
            this.tsmiShredFolder.BackColor = System.Drawing.Color.White;
            this.tsmiShredFolder.ForeColor = System.Drawing.Color.Black;
            this.tsmiShredFolder.Name = "tsmiShredFolder";
            this.tsmiShredFolder.Size = new System.Drawing.Size(220, 26);
            this.tsmiShredFolder.Text = "Shred Folder";
            this.tsmiShredFolder.Click += new System.EventHandler(this.TsmiShredFolder_Click);
            // 
            // tsmiBackupSettings
            // 
            this.tsmiBackupSettings.BackColor = System.Drawing.Color.White;
            this.tsmiBackupSettings.ForeColor = System.Drawing.Color.Black;
            this.tsmiBackupSettings.Name = "tsmiBackupSettings";
            this.tsmiBackupSettings.Size = new System.Drawing.Size(220, 26);
            this.tsmiBackupSettings.Text = "Backup Settings";
            this.tsmiBackupSettings.Click += new System.EventHandler(this.TsmiBackupSettings_Click);
            // 
            // tsmiRestoreSettings
            // 
            this.tsmiRestoreSettings.BackColor = System.Drawing.Color.White;
            this.tsmiRestoreSettings.ForeColor = System.Drawing.Color.Black;
            this.tsmiRestoreSettings.Name = "tsmiRestoreSettings";
            this.tsmiRestoreSettings.Size = new System.Drawing.Size(220, 26);
            this.tsmiRestoreSettings.Text = "Restore Settings";
            this.tsmiRestoreSettings.Click += new System.EventHandler(this.TsmiRestoreSettings_Click);
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiDocumentation,
            this.tsmiSourceCode,
            this.tsmiDonate,
            this.tsmiCheckForUpdates,
            this.tsmiAbout});
            this.tsmiHelp.ForeColor = System.Drawing.Color.Black;
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.Size = new System.Drawing.Size(54, 25);
            this.tsmiHelp.Text = "Help";
            // 
            // tsmiDocumentation
            // 
            this.tsmiDocumentation.BackColor = System.Drawing.Color.White;
            this.tsmiDocumentation.ForeColor = System.Drawing.Color.Black;
            this.tsmiDocumentation.Name = "tsmiDocumentation";
            this.tsmiDocumentation.Size = new System.Drawing.Size(207, 26);
            this.tsmiDocumentation.Text = "Documentation";
            this.tsmiDocumentation.Click += new System.EventHandler(this.TsmiDocumentation_Click);
            // 
            // tsmiSourceCode
            // 
            this.tsmiSourceCode.BackColor = System.Drawing.Color.White;
            this.tsmiSourceCode.ForeColor = System.Drawing.Color.Black;
            this.tsmiSourceCode.Name = "tsmiSourceCode";
            this.tsmiSourceCode.Size = new System.Drawing.Size(207, 26);
            this.tsmiSourceCode.Text = "Source Code";
            this.tsmiSourceCode.Click += new System.EventHandler(this.TsmiSourceCode_Click);
            // 
            // tsmiDonate
            // 
            this.tsmiDonate.BackColor = System.Drawing.Color.White;
            this.tsmiDonate.ForeColor = System.Drawing.Color.Black;
            this.tsmiDonate.Name = "tsmiDonate";
            this.tsmiDonate.Size = new System.Drawing.Size(207, 26);
            this.tsmiDonate.Text = "Donate";
            this.tsmiDonate.Click += new System.EventHandler(this.TsmiDonate_Click);
            // 
            // tsmiCheckForUpdates
            // 
            this.tsmiCheckForUpdates.BackColor = System.Drawing.Color.White;
            this.tsmiCheckForUpdates.ForeColor = System.Drawing.Color.Black;
            this.tsmiCheckForUpdates.Name = "tsmiCheckForUpdates";
            this.tsmiCheckForUpdates.Size = new System.Drawing.Size(207, 26);
            this.tsmiCheckForUpdates.Text = "Check for Updates";
            this.tsmiCheckForUpdates.Click += new System.EventHandler(this.TsmiCheckForUpdates_Click);
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.BackColor = System.Drawing.Color.White;
            this.tsmiAbout.ForeColor = System.Drawing.Color.Black;
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(207, 26);
            this.tsmiAbout.Text = "About";
            this.tsmiAbout.Click += new System.EventHandler(this.TsmiAbout_Click);
            // 
            // tmrClearClipboard
            // 
            this.tmrClearClipboard.Interval = 30000;
            this.tmrClearClipboard.Tick += new System.EventHandler(this.TmrClearClipboard_Tick);
            // 
            // masterPasswordOffToolStripMenuItem
            // 
            this.masterPasswordOffToolStripMenuItem.Name = "masterPasswordOffToolStripMenuItem";
            this.masterPasswordOffToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // masterPasswordOnToolStripMenuItem
            // 
            this.masterPasswordOnToolStripMenuItem.Name = "masterPasswordOnToolStripMenuItem";
            this.masterPasswordOnToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // bgwEncryption
            // 
            this.bgwEncryption.WorkerReportsProgress = true;
            this.bgwEncryption.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgwEncryption_DoWork);
            this.bgwEncryption.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BgwEncryption_ProgressChanged);
            this.bgwEncryption.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgwEncryption_RunWorkerCompleted);
            // 
            // bgwDecryption
            // 
            this.bgwDecryption.WorkerReportsProgress = true;
            this.bgwDecryption.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgwDecryption_DoWork);
            this.bgwDecryption.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BgwDecryption_ProgressChanged);
            this.bgwDecryption.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgwDecryption_RunWorkerCompleted);
            // 
            // prgProgress
            // 
            this.prgProgress.BackColor = System.Drawing.Color.DimGray;
            this.prgProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.prgProgress.ForeColor = System.Drawing.Color.Lime;
            this.prgProgress.Location = new System.Drawing.Point(0, 190);
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(524, 6);
            this.prgProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.prgProgress.TabIndex = 73;
            this.prgProgress.Visible = false;
            // 
            // bgwShredFiles
            // 
            this.bgwShredFiles.WorkerReportsProgress = true;
            this.bgwShredFiles.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgwShredFiles_DoWork);
            this.bgwShredFiles.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BgwShredFiles_ProgressChanged);
            this.bgwShredFiles.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgwShredFiles_RunWorkerCompleted);
            // 
            // lblEntropy
            // 
            this.lblEntropy.AutoSize = true;
            this.lblEntropy.BackColor = System.Drawing.Color.Transparent;
            this.lblEntropy.ForeColor = System.Drawing.Color.Black;
            this.lblEntropy.Location = new System.Drawing.Point(86, 73);
            this.lblEntropy.Name = "lblEntropy";
            this.lblEntropy.Size = new System.Drawing.Size(48, 21);
            this.lblEntropy.TabIndex = 74;
            this.lblEntropy.Text = "0 bits";
            this.lblEntropy.Visible = false;
            // 
            // FrmFileEncryption
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(524, 196);
            this.Controls.Add(this.lblEntropy);
            this.Controls.Add(this.prgProgress);
            this.Controls.Add(this.chkShowPassword);
            this.Controls.Add(this.lblShowPassword);
            this.Controls.Add(this.lblDragDrop);
            this.Controls.Add(this.lblFilesSelected);
            this.Controls.Add(this.picFilesSelected);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.msMenus);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msMenus;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FrmFileEncryption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kryptor";
            this.Load += new System.EventHandler(this.Kryptor_Load);
            this.cmsPasswordMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picFilesSelected)).EndInit();
            this.cmsClearFilesMenu.ResumeLayout(false);
            this.msMenus.ResumeLayout(false);
            this.msMenus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.PictureBox picFilesSelected;
        private System.Windows.Forms.Label lblFilesSelected;
        private System.Windows.Forms.Label lblDragDrop;
        private System.Windows.Forms.Label lblShowPassword;
        private System.Windows.Forms.MenuStrip msMenus;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectFiles;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem tsmiCheckForUpdates;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private System.Windows.Forms.ToolStripMenuItem tsmiSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiTools;
        private System.Windows.Forms.ToolStripMenuItem tsmiPasswordGenerator;
        private System.Windows.Forms.ToolStripMenuItem tsmiDonate;
        private System.Windows.Forms.ContextMenuStrip cmsPasswordMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopyPassword;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearPassword;
        private System.Windows.Forms.ToolStripMenuItem tsmiCreateKeyfile;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectKeyfile;
        private System.Windows.Forms.ToolStripMenuItem tsmiDocumentation;
        private System.Windows.Forms.ToolStripMenuItem tsmiShredFiles;
        private System.Windows.Forms.ToolStripMenuItem tsmiShredFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiQuit;
        private System.Windows.Forms.ToolStripMenuItem tsmiSourceCode;
        private System.Windows.Forms.ContextMenuStrip cmsClearFilesMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearSelectedFiles;
        private System.Windows.Forms.ToolStripMenuItem masterPasswordOffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem masterPasswordOnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearClipboard;
        private System.Windows.Forms.ProgressBar prgProgress;
        private System.Windows.Forms.ToolStripMenuItem tsmiSelectFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmiBackupSettings;
        private System.Windows.Forms.ToolStripMenuItem tsmiPasswordSharing;
        private System.ComponentModel.BackgroundWorker bgwDecryption;
        private System.ComponentModel.BackgroundWorker bgwShredFiles;
        private System.ComponentModel.BackgroundWorker bgwEncryption;
        private System.Windows.Forms.Label lblEntropy;
        public System.Windows.Forms.Timer tmrClearClipboard;
        public System.Windows.Forms.CheckBox chkShowPassword;
        private System.Windows.Forms.ToolStripMenuItem tsmiRestoreSettings;
    }
}

