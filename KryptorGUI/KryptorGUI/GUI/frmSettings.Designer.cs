namespace Kryptor
{
    partial class FrmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSettings));
            this.lblShredFilesMethod = new System.Windows.Forms.Label();
            this.cmbShredFilesMethod = new System.Windows.Forms.ComboBox();
            this.lblAutoClearClipboard = new System.Windows.Forms.Label();
            this.cmbAutoClearClipboard = new System.Windows.Forms.ComboBox();
            this.lblMemoryEncryption = new System.Windows.Forms.Label();
            this.cmbMemoryEncryption = new System.Windows.Forms.ComboBox();
            this.lblEncryptionAlgorithm = new System.Windows.Forms.Label();
            this.cmbEncryptionAlgorithm = new System.Windows.Forms.ComboBox();
            this.lblAutoClearPassword = new System.Windows.Forms.Label();
            this.cmbAutoClearPassword = new System.Windows.Forms.ComboBox();
            this.lblAnonymousRename = new System.Windows.Forms.Label();
            this.cmbAnonymousRename = new System.Windows.Forms.ComboBox();
            this.lblExitClearClipboard = new System.Windows.Forms.Label();
            this.cmbExitClearClipboard = new System.Windows.Forms.ComboBox();
            this.lblShowPassword = new System.Windows.Forms.Label();
            this.cmbShowPassword = new System.Windows.Forms.ComboBox();
            this.grbFileEncryption = new System.Windows.Forms.GroupBox();
            this.cmbOverwriteFiles = new System.Windows.Forms.ComboBox();
            this.lblOverwriteFiles = new System.Windows.Forms.Label();
            this.grbKeyDerivation = new System.Windows.Forms.GroupBox();
            this.btnTestParameters = new System.Windows.Forms.Button();
            this.lblArgon2Iterations = new System.Windows.Forms.Label();
            this.btnArgon2Benchmark = new System.Windows.Forms.Button();
            this.nudArgon2MemorySize = new System.Windows.Forms.NumericUpDown();
            this.lblArgon2MemorySize = new System.Windows.Forms.Label();
            this.nudArgon2Iterations = new System.Windows.Forms.NumericUpDown();
            this.grbOtherSettings = new System.Windows.Forms.GroupBox();
            this.lblTheme = new System.Windows.Forms.Label();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.lblCheckForUpdates = new System.Windows.Forms.Label();
            this.cmbCheckForUpdates = new System.Windows.Forms.ComboBox();
            this.bgwTestArgon2Parameters = new System.ComponentModel.BackgroundWorker();
            this.grbFileEncryption.SuspendLayout();
            this.grbKeyDerivation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudArgon2MemorySize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudArgon2Iterations)).BeginInit();
            this.grbOtherSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblShredFilesMethod
            // 
            this.lblShredFilesMethod.AutoSize = true;
            this.lblShredFilesMethod.ForeColor = System.Drawing.Color.Black;
            this.lblShredFilesMethod.Location = new System.Drawing.Point(12, 127);
            this.lblShredFilesMethod.Name = "lblShredFilesMethod";
            this.lblShredFilesMethod.Size = new System.Drawing.Size(147, 21);
            this.lblShredFilesMethod.TabIndex = 16;
            this.lblShredFilesMethod.Text = "Shred Files Method:";
            // 
            // cmbShredFilesMethod
            // 
            this.cmbShredFilesMethod.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbShredFilesMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShredFilesMethod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbShredFilesMethod.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbShredFilesMethod.ForeColor = System.Drawing.Color.Black;
            this.cmbShredFilesMethod.FormattingEnabled = true;
            this.cmbShredFilesMethod.Items.AddRange(new object[] {
            "16 KiB",
            "Zero fill",
            "1 Pass",
            "Encryption",
            "HMG IS5",
            "5 Passes"});
            this.cmbShredFilesMethod.Location = new System.Drawing.Point(188, 124);
            this.cmbShredFilesMethod.Name = "cmbShredFilesMethod";
            this.cmbShredFilesMethod.Size = new System.Drawing.Size(136, 29);
            this.cmbShredFilesMethod.TabIndex = 8;
            this.cmbShredFilesMethod.TabStop = false;
            this.cmbShredFilesMethod.SelectedIndexChanged += new System.EventHandler(this.CmbShredFilesMethod_SelectedIndexChanged);
            this.cmbShredFilesMethod.DropDownClosed += new System.EventHandler(this.CmbShredFilesMethod_DropDownClosed);
            // 
            // lblAutoClearClipboard
            // 
            this.lblAutoClearClipboard.AutoSize = true;
            this.lblAutoClearClipboard.ForeColor = System.Drawing.Color.Black;
            this.lblAutoClearClipboard.Location = new System.Drawing.Point(12, 84);
            this.lblAutoClearClipboard.Name = "lblAutoClearClipboard";
            this.lblAutoClearClipboard.Size = new System.Drawing.Size(158, 21);
            this.lblAutoClearClipboard.TabIndex = 8;
            this.lblAutoClearClipboard.Text = "Auto Clear Clipboard:";
            // 
            // cmbAutoClearClipboard
            // 
            this.cmbAutoClearClipboard.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbAutoClearClipboard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutoClearClipboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAutoClearClipboard.ForeColor = System.Drawing.Color.Black;
            this.cmbAutoClearClipboard.FormattingEnabled = true;
            this.cmbAutoClearClipboard.Items.AddRange(new object[] {
            "Never",
            "15 seconds",
            "30 seconds",
            "60 seconds",
            "90 seconds",
            "120 seconds"});
            this.cmbAutoClearClipboard.Location = new System.Drawing.Point(188, 81);
            this.cmbAutoClearClipboard.Name = "cmbAutoClearClipboard";
            this.cmbAutoClearClipboard.Size = new System.Drawing.Size(136, 29);
            this.cmbAutoClearClipboard.TabIndex = 9;
            this.cmbAutoClearClipboard.TabStop = false;
            this.cmbAutoClearClipboard.SelectedIndexChanged += new System.EventHandler(this.CmbAutoClearClipboard_SelectedIndexChanged);
            this.cmbAutoClearClipboard.DropDownClosed += new System.EventHandler(this.CmbAutoClearClipboard_DropDownClosed);
            // 
            // lblMemoryEncryption
            // 
            this.lblMemoryEncryption.AutoSize = true;
            this.lblMemoryEncryption.ForeColor = System.Drawing.Color.Black;
            this.lblMemoryEncryption.Location = new System.Drawing.Point(12, 85);
            this.lblMemoryEncryption.Name = "lblMemoryEncryption";
            this.lblMemoryEncryption.Size = new System.Drawing.Size(150, 21);
            this.lblMemoryEncryption.TabIndex = 6;
            this.lblMemoryEncryption.Text = "Memory Encryption:";
            // 
            // cmbMemoryEncryption
            // 
            this.cmbMemoryEncryption.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbMemoryEncryption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMemoryEncryption.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMemoryEncryption.ForeColor = System.Drawing.Color.Black;
            this.cmbMemoryEncryption.FormattingEnabled = true;
            this.cmbMemoryEncryption.Items.AddRange(new object[] {
            "Enabled",
            "Disabled"});
            this.cmbMemoryEncryption.Location = new System.Drawing.Point(188, 81);
            this.cmbMemoryEncryption.Name = "cmbMemoryEncryption";
            this.cmbMemoryEncryption.Size = new System.Drawing.Size(136, 29);
            this.cmbMemoryEncryption.TabIndex = 2;
            this.cmbMemoryEncryption.TabStop = false;
            this.cmbMemoryEncryption.SelectedIndexChanged += new System.EventHandler(this.CmbMemoryEncryption_SelectedIndexChanged);
            this.cmbMemoryEncryption.DropDownClosed += new System.EventHandler(this.CmbMemoryEncryption_DropDownClosed);
            // 
            // lblEncryptionAlgorithm
            // 
            this.lblEncryptionAlgorithm.AutoSize = true;
            this.lblEncryptionAlgorithm.ForeColor = System.Drawing.Color.Black;
            this.lblEncryptionAlgorithm.Location = new System.Drawing.Point(12, 41);
            this.lblEncryptionAlgorithm.Name = "lblEncryptionAlgorithm";
            this.lblEncryptionAlgorithm.Size = new System.Drawing.Size(161, 21);
            this.lblEncryptionAlgorithm.TabIndex = 2;
            this.lblEncryptionAlgorithm.Text = "Encryption Algorithm:";
            // 
            // cmbEncryptionAlgorithm
            // 
            this.cmbEncryptionAlgorithm.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbEncryptionAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncryptionAlgorithm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEncryptionAlgorithm.ForeColor = System.Drawing.Color.Black;
            this.cmbEncryptionAlgorithm.FormattingEnabled = true;
            this.cmbEncryptionAlgorithm.Items.AddRange(new object[] {
            "XChaCha20",
            "XSalsa20",
            "AES-CBC"});
            this.cmbEncryptionAlgorithm.Location = new System.Drawing.Point(188, 38);
            this.cmbEncryptionAlgorithm.Name = "cmbEncryptionAlgorithm";
            this.cmbEncryptionAlgorithm.Size = new System.Drawing.Size(136, 29);
            this.cmbEncryptionAlgorithm.TabIndex = 1;
            this.cmbEncryptionAlgorithm.TabStop = false;
            this.cmbEncryptionAlgorithm.SelectedIndexChanged += new System.EventHandler(this.CmbEncryptionAlgorithm_SelectedIndexChanged);
            this.cmbEncryptionAlgorithm.DropDownClosed += new System.EventHandler(this.CmbEncryptionAlgorithm_DropDownClosed);
            // 
            // lblAutoClearPassword
            // 
            this.lblAutoClearPassword.AutoSize = true;
            this.lblAutoClearPassword.ForeColor = System.Drawing.Color.Black;
            this.lblAutoClearPassword.Location = new System.Drawing.Point(364, 41);
            this.lblAutoClearPassword.Name = "lblAutoClearPassword";
            this.lblAutoClearPassword.Size = new System.Drawing.Size(156, 21);
            this.lblAutoClearPassword.TabIndex = 20;
            this.lblAutoClearPassword.Text = "Auto Clear Password:";
            // 
            // cmbAutoClearPassword
            // 
            this.cmbAutoClearPassword.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbAutoClearPassword.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutoClearPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAutoClearPassword.ForeColor = System.Drawing.Color.Black;
            this.cmbAutoClearPassword.FormattingEnabled = true;
            this.cmbAutoClearPassword.Items.AddRange(new object[] {
            "Enabled",
            "Disabled"});
            this.cmbAutoClearPassword.Location = new System.Drawing.Point(540, 38);
            this.cmbAutoClearPassword.Name = "cmbAutoClearPassword";
            this.cmbAutoClearPassword.Size = new System.Drawing.Size(136, 29);
            this.cmbAutoClearPassword.TabIndex = 10;
            this.cmbAutoClearPassword.TabStop = false;
            this.cmbAutoClearPassword.SelectedIndexChanged += new System.EventHandler(this.CmbAutoClearPassword_SelectedIndexChanged);
            this.cmbAutoClearPassword.DropDownClosed += new System.EventHandler(this.CmbAutoClearPassword_DropDownClosed);
            // 
            // lblAnonymousRename
            // 
            this.lblAnonymousRename.AutoSize = true;
            this.lblAnonymousRename.ForeColor = System.Drawing.Color.Black;
            this.lblAnonymousRename.Location = new System.Drawing.Point(12, 128);
            this.lblAnonymousRename.Name = "lblAnonymousRename";
            this.lblAnonymousRename.Size = new System.Drawing.Size(158, 21);
            this.lblAnonymousRename.TabIndex = 18;
            this.lblAnonymousRename.Text = "Anonymous Rename:";
            // 
            // cmbAnonymousRename
            // 
            this.cmbAnonymousRename.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbAnonymousRename.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnonymousRename.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAnonymousRename.ForeColor = System.Drawing.Color.Black;
            this.cmbAnonymousRename.FormattingEnabled = true;
            this.cmbAnonymousRename.Items.AddRange(new object[] {
            "Enabled",
            "Disabled"});
            this.cmbAnonymousRename.Location = new System.Drawing.Point(188, 124);
            this.cmbAnonymousRename.Name = "cmbAnonymousRename";
            this.cmbAnonymousRename.Size = new System.Drawing.Size(136, 29);
            this.cmbAnonymousRename.TabIndex = 7;
            this.cmbAnonymousRename.TabStop = false;
            this.cmbAnonymousRename.SelectedIndexChanged += new System.EventHandler(this.CmbAnonymousRename_SelectedIndexChanged);
            this.cmbAnonymousRename.DropDownClosed += new System.EventHandler(this.CmbAnonymousRename_DropDownClosed);
            // 
            // lblExitClearClipboard
            // 
            this.lblExitClearClipboard.AutoSize = true;
            this.lblExitClearClipboard.ForeColor = System.Drawing.Color.Black;
            this.lblExitClearClipboard.Location = new System.Drawing.Point(364, 84);
            this.lblExitClearClipboard.Name = "lblExitClearClipboard";
            this.lblExitClearClipboard.Size = new System.Drawing.Size(149, 21);
            this.lblExitClearClipboard.TabIndex = 24;
            this.lblExitClearClipboard.Text = "Exit Clear Clipboard:";
            // 
            // cmbExitClearClipboard
            // 
            this.cmbExitClearClipboard.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbExitClearClipboard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExitClearClipboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbExitClearClipboard.ForeColor = System.Drawing.Color.Black;
            this.cmbExitClearClipboard.FormattingEnabled = true;
            this.cmbExitClearClipboard.Items.AddRange(new object[] {
            "Enabled",
            "Disabled"});
            this.cmbExitClearClipboard.Location = new System.Drawing.Point(540, 81);
            this.cmbExitClearClipboard.Name = "cmbExitClearClipboard";
            this.cmbExitClearClipboard.Size = new System.Drawing.Size(136, 29);
            this.cmbExitClearClipboard.TabIndex = 12;
            this.cmbExitClearClipboard.TabStop = false;
            this.cmbExitClearClipboard.SelectedIndexChanged += new System.EventHandler(this.CmbExitClearClipboard_SelectedIndexChanged);
            this.cmbExitClearClipboard.DropDownClosed += new System.EventHandler(this.CmbExitClearClipboard_DropDownClosed);
            // 
            // lblShowPassword
            // 
            this.lblShowPassword.AutoSize = true;
            this.lblShowPassword.ForeColor = System.Drawing.Color.Black;
            this.lblShowPassword.Location = new System.Drawing.Point(12, 41);
            this.lblShowPassword.Name = "lblShowPassword";
            this.lblShowPassword.Size = new System.Drawing.Size(122, 21);
            this.lblShowPassword.TabIndex = 22;
            this.lblShowPassword.Text = "Show Password:";
            // 
            // cmbShowPassword
            // 
            this.cmbShowPassword.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbShowPassword.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShowPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbShowPassword.ForeColor = System.Drawing.Color.Black;
            this.cmbShowPassword.FormattingEnabled = true;
            this.cmbShowPassword.Items.AddRange(new object[] {
            "Enabled",
            "Disabled"});
            this.cmbShowPassword.Location = new System.Drawing.Point(188, 38);
            this.cmbShowPassword.Name = "cmbShowPassword";
            this.cmbShowPassword.Size = new System.Drawing.Size(136, 29);
            this.cmbShowPassword.TabIndex = 11;
            this.cmbShowPassword.TabStop = false;
            this.cmbShowPassword.SelectedIndexChanged += new System.EventHandler(this.CmbShowPassword_SelectedIndexChanged);
            this.cmbShowPassword.DropDownClosed += new System.EventHandler(this.CmbShowPassword_DropDownClosed);
            // 
            // grbFileEncryption
            // 
            this.grbFileEncryption.BackColor = System.Drawing.Color.Transparent;
            this.grbFileEncryption.Controls.Add(this.cmbOverwriteFiles);
            this.grbFileEncryption.Controls.Add(this.cmbMemoryEncryption);
            this.grbFileEncryption.Controls.Add(this.lblOverwriteFiles);
            this.grbFileEncryption.Controls.Add(this.lblMemoryEncryption);
            this.grbFileEncryption.Controls.Add(this.lblEncryptionAlgorithm);
            this.grbFileEncryption.Controls.Add(this.cmbAnonymousRename);
            this.grbFileEncryption.Controls.Add(this.cmbEncryptionAlgorithm);
            this.grbFileEncryption.Controls.Add(this.lblAnonymousRename);
            this.grbFileEncryption.ForeColor = System.Drawing.Color.Black;
            this.grbFileEncryption.Location = new System.Drawing.Point(12, 13);
            this.grbFileEncryption.Name = "grbFileEncryption";
            this.grbFileEncryption.Size = new System.Drawing.Size(341, 221);
            this.grbFileEncryption.TabIndex = 25;
            this.grbFileEncryption.TabStop = false;
            this.grbFileEncryption.Text = "File Encryption";
            // 
            // cmbOverwriteFiles
            // 
            this.cmbOverwriteFiles.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbOverwriteFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOverwriteFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbOverwriteFiles.ForeColor = System.Drawing.Color.Black;
            this.cmbOverwriteFiles.FormattingEnabled = true;
            this.cmbOverwriteFiles.Items.AddRange(new object[] {
            "Enabled",
            "Disabled"});
            this.cmbOverwriteFiles.Location = new System.Drawing.Point(188, 168);
            this.cmbOverwriteFiles.Name = "cmbOverwriteFiles";
            this.cmbOverwriteFiles.Size = new System.Drawing.Size(136, 29);
            this.cmbOverwriteFiles.TabIndex = 30;
            this.cmbOverwriteFiles.TabStop = false;
            this.cmbOverwriteFiles.SelectedIndexChanged += new System.EventHandler(this.CmbOverwriteFiles_SelectedIndexChanged);
            this.cmbOverwriteFiles.DropDownClosed += new System.EventHandler(this.CmbOverwriteFiles_DropDownClosed);
            // 
            // lblOverwriteFiles
            // 
            this.lblOverwriteFiles.AutoSize = true;
            this.lblOverwriteFiles.ForeColor = System.Drawing.Color.Black;
            this.lblOverwriteFiles.Location = new System.Drawing.Point(12, 172);
            this.lblOverwriteFiles.Name = "lblOverwriteFiles";
            this.lblOverwriteFiles.Size = new System.Drawing.Size(117, 21);
            this.lblOverwriteFiles.TabIndex = 31;
            this.lblOverwriteFiles.Text = "Overwrite Files:";
            // 
            // grbKeyDerivation
            // 
            this.grbKeyDerivation.BackColor = System.Drawing.Color.Transparent;
            this.grbKeyDerivation.Controls.Add(this.btnTestParameters);
            this.grbKeyDerivation.Controls.Add(this.lblArgon2Iterations);
            this.grbKeyDerivation.Controls.Add(this.btnArgon2Benchmark);
            this.grbKeyDerivation.Controls.Add(this.nudArgon2MemorySize);
            this.grbKeyDerivation.Controls.Add(this.lblArgon2MemorySize);
            this.grbKeyDerivation.Controls.Add(this.nudArgon2Iterations);
            this.grbKeyDerivation.ForeColor = System.Drawing.Color.Black;
            this.grbKeyDerivation.Location = new System.Drawing.Point(364, 13);
            this.grbKeyDerivation.Name = "grbKeyDerivation";
            this.grbKeyDerivation.Size = new System.Drawing.Size(341, 221);
            this.grbKeyDerivation.TabIndex = 29;
            this.grbKeyDerivation.TabStop = false;
            this.grbKeyDerivation.Text = "Key Derivation";
            // 
            // btnTestParameters
            // 
            this.btnTestParameters.BackColor = System.Drawing.Color.Gainsboro;
            this.btnTestParameters.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnTestParameters.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.btnTestParameters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestParameters.ForeColor = System.Drawing.Color.Black;
            this.btnTestParameters.Location = new System.Drawing.Point(16, 168);
            this.btnTestParameters.Name = "btnTestParameters";
            this.btnTestParameters.Size = new System.Drawing.Size(308, 29);
            this.btnTestParameters.TabIndex = 23;
            this.btnTestParameters.TabStop = false;
            this.btnTestParameters.Text = "Test Parameters";
            this.btnTestParameters.UseVisualStyleBackColor = true;
            this.btnTestParameters.Click += new System.EventHandler(this.BtnTestParameters_Click);
            // 
            // lblArgon2Iterations
            // 
            this.lblArgon2Iterations.AutoSize = true;
            this.lblArgon2Iterations.ForeColor = System.Drawing.Color.Black;
            this.lblArgon2Iterations.Location = new System.Drawing.Point(12, 85);
            this.lblArgon2Iterations.Name = "lblArgon2Iterations";
            this.lblArgon2Iterations.Size = new System.Drawing.Size(117, 21);
            this.lblArgon2Iterations.TabIndex = 21;
            this.lblArgon2Iterations.Text = "Iteration Count:";
            // 
            // btnArgon2Benchmark
            // 
            this.btnArgon2Benchmark.BackColor = System.Drawing.Color.Gainsboro;
            this.btnArgon2Benchmark.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnArgon2Benchmark.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.btnArgon2Benchmark.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnArgon2Benchmark.ForeColor = System.Drawing.Color.Black;
            this.btnArgon2Benchmark.Location = new System.Drawing.Point(16, 124);
            this.btnArgon2Benchmark.Name = "btnArgon2Benchmark";
            this.btnArgon2Benchmark.Size = new System.Drawing.Size(308, 29);
            this.btnArgon2Benchmark.TabIndex = 7;
            this.btnArgon2Benchmark.TabStop = false;
            this.btnArgon2Benchmark.Text = "Benchmark";
            this.btnArgon2Benchmark.UseVisualStyleBackColor = true;
            this.btnArgon2Benchmark.Click += new System.EventHandler(this.BtnArgon2Benchmark_Click);
            // 
            // nudArgon2MemorySize
            // 
            this.nudArgon2MemorySize.BackColor = System.Drawing.Color.Gainsboro;
            this.nudArgon2MemorySize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudArgon2MemorySize.ForeColor = System.Drawing.Color.Black;
            this.nudArgon2MemorySize.Location = new System.Drawing.Point(188, 38);
            this.nudArgon2MemorySize.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudArgon2MemorySize.Minimum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudArgon2MemorySize.Name = "nudArgon2MemorySize";
            this.nudArgon2MemorySize.Size = new System.Drawing.Size(136, 29);
            this.nudArgon2MemorySize.TabIndex = 17;
            this.nudArgon2MemorySize.TabStop = false;
            this.nudArgon2MemorySize.Tag = "";
            this.nudArgon2MemorySize.ThousandsSeparator = true;
            this.nudArgon2MemorySize.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudArgon2MemorySize.ValueChanged += new System.EventHandler(this.NudArgon2MemorySize_ValueChanged);
            // 
            // lblArgon2MemorySize
            // 
            this.lblArgon2MemorySize.AutoSize = true;
            this.lblArgon2MemorySize.ForeColor = System.Drawing.Color.Black;
            this.lblArgon2MemorySize.Location = new System.Drawing.Point(12, 42);
            this.lblArgon2MemorySize.Name = "lblArgon2MemorySize";
            this.lblArgon2MemorySize.Size = new System.Drawing.Size(145, 21);
            this.lblArgon2MemorySize.TabIndex = 20;
            this.lblArgon2MemorySize.Text = "Memory Size (MiB):";
            // 
            // nudArgon2Iterations
            // 
            this.nudArgon2Iterations.BackColor = System.Drawing.Color.Gainsboro;
            this.nudArgon2Iterations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudArgon2Iterations.ForeColor = System.Drawing.Color.Black;
            this.nudArgon2Iterations.Location = new System.Drawing.Point(188, 81);
            this.nudArgon2Iterations.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudArgon2Iterations.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudArgon2Iterations.Name = "nudArgon2Iterations";
            this.nudArgon2Iterations.Size = new System.Drawing.Size(136, 29);
            this.nudArgon2Iterations.TabIndex = 18;
            this.nudArgon2Iterations.TabStop = false;
            this.nudArgon2Iterations.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudArgon2Iterations.ValueChanged += new System.EventHandler(this.NudArgon2Iterations_ValueChanged);
            // 
            // grbOtherSettings
            // 
            this.grbOtherSettings.BackColor = System.Drawing.Color.Transparent;
            this.grbOtherSettings.Controls.Add(this.lblTheme);
            this.grbOtherSettings.Controls.Add(this.cmbTheme);
            this.grbOtherSettings.Controls.Add(this.lblCheckForUpdates);
            this.grbOtherSettings.Controls.Add(this.cmbAutoClearClipboard);
            this.grbOtherSettings.Controls.Add(this.cmbCheckForUpdates);
            this.grbOtherSettings.Controls.Add(this.lblAutoClearClipboard);
            this.grbOtherSettings.Controls.Add(this.cmbAutoClearPassword);
            this.grbOtherSettings.Controls.Add(this.cmbShredFilesMethod);
            this.grbOtherSettings.Controls.Add(this.lblExitClearClipboard);
            this.grbOtherSettings.Controls.Add(this.cmbExitClearClipboard);
            this.grbOtherSettings.Controls.Add(this.lblShredFilesMethod);
            this.grbOtherSettings.Controls.Add(this.lblShowPassword);
            this.grbOtherSettings.Controls.Add(this.cmbShowPassword);
            this.grbOtherSettings.Controls.Add(this.lblAutoClearPassword);
            this.grbOtherSettings.ForeColor = System.Drawing.Color.Black;
            this.grbOtherSettings.Location = new System.Drawing.Point(12, 238);
            this.grbOtherSettings.Name = "grbOtherSettings";
            this.grbOtherSettings.Size = new System.Drawing.Size(693, 221);
            this.grbOtherSettings.TabIndex = 27;
            this.grbOtherSettings.TabStop = false;
            this.grbOtherSettings.Text = "Other Settings";
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.ForeColor = System.Drawing.Color.Black;
            this.lblTheme.Location = new System.Drawing.Point(12, 170);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(116, 21);
            this.lblTheme.TabIndex = 28;
            this.lblTheme.Text = "Kryptor Theme:";
            // 
            // cmbTheme
            // 
            this.cmbTheme.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTheme.ForeColor = System.Drawing.Color.Black;
            this.cmbTheme.FormattingEnabled = true;
            this.cmbTheme.Items.AddRange(new object[] {
            "Dark",
            "Light"});
            this.cmbTheme.Location = new System.Drawing.Point(188, 167);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(136, 29);
            this.cmbTheme.TabIndex = 26;
            this.cmbTheme.TabStop = false;
            this.cmbTheme.SelectedIndexChanged += new System.EventHandler(this.CmbTheme_SelectedIndexChanged);
            this.cmbTheme.DropDownClosed += new System.EventHandler(this.CmbTheme_DropDownClosed);
            // 
            // lblCheckForUpdates
            // 
            this.lblCheckForUpdates.AutoSize = true;
            this.lblCheckForUpdates.ForeColor = System.Drawing.Color.Black;
            this.lblCheckForUpdates.Location = new System.Drawing.Point(364, 127);
            this.lblCheckForUpdates.Name = "lblCheckForUpdates";
            this.lblCheckForUpdates.Size = new System.Drawing.Size(140, 21);
            this.lblCheckForUpdates.TabIndex = 27;
            this.lblCheckForUpdates.Text = "Check for Updates:";
            // 
            // cmbCheckForUpdates
            // 
            this.cmbCheckForUpdates.BackColor = System.Drawing.Color.Gainsboro;
            this.cmbCheckForUpdates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCheckForUpdates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCheckForUpdates.ForeColor = System.Drawing.Color.Black;
            this.cmbCheckForUpdates.FormattingEnabled = true;
            this.cmbCheckForUpdates.Items.AddRange(new object[] {
            "Enabled",
            "Disabled"});
            this.cmbCheckForUpdates.Location = new System.Drawing.Point(540, 124);
            this.cmbCheckForUpdates.Name = "cmbCheckForUpdates";
            this.cmbCheckForUpdates.Size = new System.Drawing.Size(136, 29);
            this.cmbCheckForUpdates.TabIndex = 25;
            this.cmbCheckForUpdates.TabStop = false;
            this.cmbCheckForUpdates.SelectedIndexChanged += new System.EventHandler(this.CmbCheckForUpdates_SelectedIndexChanged);
            this.cmbCheckForUpdates.DropDownClosed += new System.EventHandler(this.CmbCheckForUpdates_DropDownClosed);
            // 
            // bgwTestArgon2Parameters
            // 
            this.bgwTestArgon2Parameters.WorkerReportsProgress = true;
            this.bgwTestArgon2Parameters.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgwTestArgon2Parameters_DoWork);
            this.bgwTestArgon2Parameters.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgwTestArgon2Parameters_RunWorkerCompleted);
            // 
            // FrmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(717, 471);
            this.Controls.Add(this.grbKeyDerivation);
            this.Controls.Add(this.grbOtherSettings);
            this.Controls.Add(this.grbFileEncryption);
            this.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSettings_FormClosing);
            this.Load += new System.EventHandler(this.FrmSettings_Load);
            this.grbFileEncryption.ResumeLayout(false);
            this.grbFileEncryption.PerformLayout();
            this.grbKeyDerivation.ResumeLayout(false);
            this.grbKeyDerivation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudArgon2MemorySize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudArgon2Iterations)).EndInit();
            this.grbOtherSettings.ResumeLayout(false);
            this.grbOtherSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbEncryptionAlgorithm;
        private System.Windows.Forms.Label lblEncryptionAlgorithm;
        private System.Windows.Forms.Label lblMemoryEncryption;
        private System.Windows.Forms.ComboBox cmbMemoryEncryption;
        private System.Windows.Forms.Label lblAutoClearClipboard;
        private System.Windows.Forms.ComboBox cmbAutoClearClipboard;
        private System.Windows.Forms.Label lblShredFilesMethod;
        private System.Windows.Forms.ComboBox cmbShredFilesMethod;
        private System.Windows.Forms.Label lblAutoClearPassword;
        private System.Windows.Forms.ComboBox cmbAutoClearPassword;
        private System.Windows.Forms.Label lblAnonymousRename;
        private System.Windows.Forms.ComboBox cmbAnonymousRename;
        private System.Windows.Forms.Label lblExitClearClipboard;
        private System.Windows.Forms.ComboBox cmbExitClearClipboard;
        private System.Windows.Forms.Label lblShowPassword;
        private System.Windows.Forms.ComboBox cmbShowPassword;
        private System.Windows.Forms.GroupBox grbFileEncryption;
        private System.Windows.Forms.GroupBox grbOtherSettings;
        private System.Windows.Forms.Button btnArgon2Benchmark;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.Label lblCheckForUpdates;
        private System.Windows.Forms.ComboBox cmbCheckForUpdates;
        private System.Windows.Forms.Label lblArgon2Iterations;
        private System.Windows.Forms.NumericUpDown nudArgon2MemorySize;
        private System.Windows.Forms.Label lblArgon2MemorySize;
        private System.Windows.Forms.NumericUpDown nudArgon2Iterations;
        private System.Windows.Forms.GroupBox grbKeyDerivation;
        private System.Windows.Forms.ComboBox cmbOverwriteFiles;
        private System.Windows.Forms.Label lblOverwriteFiles;
        private System.Windows.Forms.Button btnTestParameters;
        private System.ComponentModel.BackgroundWorker bgwTestArgon2Parameters;
    }
}