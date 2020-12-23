// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

// Arguments have to be properties
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:KryptorCLI.Program.EncryptPassword")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:KryptorCLI.Program.DecryptPassword")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:KryptorCLI.Program.Arguments")]

// False warning
[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorCLI.Program.OnExecute")]

// Less readable
[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorCLI.CommandLine.DisplayAbout")]
[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorCLI.Updates.UpdateKryptor(System.Boolean)~System.String")]
[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorCLI.ReadFileHeaders.RetrieveArgon2Parameters(System.String,System.String@,System.String@)")]
[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "<Pending>", Scope = "member", Target = "~M:KryptorCLI.ReadFileHeaders.RemoveParameterFlag(System.String)~System.String")]
