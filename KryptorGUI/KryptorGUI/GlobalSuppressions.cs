// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// These will be disposed when the form is closed
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Program.Main(System.String[])")]
[assembly: SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Program.RunKryptor")]

// Currently only supporting English - CA1303 will be fixed if other languages are implemented using a resource table
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Settings.RestoreSettings")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Settings.BackupSettings")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.frmPasswordGenerator.CmbGenerateType_SelectedIndexChanged(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.FrmGenerateKeyPair.ExportPublicKey(System.String)")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.SelectFiles.SelectFolderDialog~System.Boolean")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.SelectFiles.SelectFilesDialog~System.Boolean")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Keyfiles.SelectKeyfile~System.Boolean")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Keyfiles.CreateKeyfile~System.Boolean")]

// Simple using statement is less readable - it's not clear when things go out of scope
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.AesAlgorithms.DecryptAesCBC(System.IO.FileStream,System.IO.FileStream,System.Byte[],System.Byte[],System.Byte[],System.ComponentModel.BackgroundWorker)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.AesAlgorithms.EncryptAesCBC(System.IO.FileStream,System.IO.FileStream,System.Int64,System.Byte[],System.Byte[],System.Byte[],System.ComponentModel.BackgroundWorker)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.FrmFileEncryption.BackgroundWorkerCompleted(System.String)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.FrmFileEncryption.TsmiAbout_Click(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.FrmFileEncryption.TsmiPasswordGenerator_Click(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.FrmFileEncryption.TsmiPasswordSharing_Click(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.FrmFileEncryption.TsmiSettings_Click(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.FrmGenerateKeyPair.ExportPublicKey(System.String)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.FrmPasswordSharing.LlbGenerateKeyPair_LinkClicked(System.Object,System.Windows.Forms.LinkLabelLinkClickedEventArgs)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.HashingAlgorithms.Blake2(System.IO.FileStream,System.Byte[])~System.Byte[]")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Keyfiles.CreateKeyfile~System.Boolean")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Keyfiles.SelectKeyfile~System.Boolean")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.OriginalFileName.EncodeFileName(System.String,System.String,System.Byte[]@,System.Byte[]@)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.OriginalFileName.RemoveStoredFileName(System.String,System.String)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.PasswordSharing.DecryptPassword(System.Byte[],System.Byte[])~System.Char[]")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.PasswordSharing.GenerateKeyPair~System.ValueTuple{System.String,System.String}")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.ReadFileHeaders.RetrieveArgon2Parameters(System.String,System.String@,System.String@)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.SelectFiles.SelectFilesDialog~System.Boolean")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.SelectFiles.SelectFolderDialog~System.Boolean")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Settings.BackupSettings")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Settings.RestoreSettings")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Settings.SaveSettings")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.ShredFilesMethods.FirstLast16KiB(System.String)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.ShredFilesMethods.PseudorandomData(System.String,System.ComponentModel.BackgroundWorker)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.ShredFilesMethods.ZeroFill(System.String,System.Boolean,System.ComponentModel.BackgroundWorker)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.Updates.DownloadVersionFile(System.String)")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorGUI.FrmArgon2Benchmark.GetBenchmarkMode~System.Int32")]
