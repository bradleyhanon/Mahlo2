
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "Maybe fix later", Scope = "type", Target = "~T:MahloService.Logic.ICutRollLogic")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Maybe fix later")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2007:Do not directly await a Task", Justification = "The default ConfigureAwait is desired")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Method is in SignalR hub", Scope = "member", Target = "~M:MahloService.Ipc.MahloHub.BasApplyRecipeAsync(System.String,System.Boolean)~System.Threading.Tasks.Task")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "I like it the way it is", Scope = "member", Target = "~E:MahloService.Logic.SewinQueue.QueueChanged")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "I like it the way it is", Scope = "member", Target = "~E:MahloService.Repository.ProgramState.Saving")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "I like it the way it is", Scope = "member", Target = "~E:MahloService.Logic.MeterLogic`1.RollStarted")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "I like it the way it is", Scope = "member", Target = "~E:MahloService.Logic.MeterLogic`1.RollFinished")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "I like it the way it is", Scope = "member", Target = "~E:MahloService.Logic.IMeterLogic.RollStarted")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "I like it the way it is", Scope = "member", Target = "~E:MahloService.Logic.IMeterLogic.RollFinished")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "I like it the way it is", Scope = "member", Target = "~E:MahloService.Logic.ISewinQueue.QueueChanged")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "It is a system name", Scope = "member", Target = "~E:MahloService.Repository.IProgramState.Saving")]
