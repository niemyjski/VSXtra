// ================================================================================================
// CommandGroup.cs
//
// Created: 2008.07.11, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra.Commands;

namespace DeepDiver.MultiToolWindow
{
  // ================================================================================================
  /// <summary>
  /// Logical command group for all commands in this package
  /// </summary>
  // ================================================================================================
  [Guid(GuidList.guidMultiToolWindowCmdSetString)]
  public sealed class CommandGroup: CommandGroup<MultiToolWindowPackage>
  {
    [CommandId(CmdIDs.cmdidShowFirstWindow)]
    [ShowToolWindowAction(typeof(NumberStackWindowPane1))]
    public sealed class ShowStackWindow1: MenuCommandHandler {}

    [CommandId(CmdIDs.cmdidShowSecondWindow)]
    [ShowToolWindowAction(typeof(NumberStackWindowPane2))]
    public sealed class ShowStackWindow2 : MenuCommandHandler { }

    [CommandId(CmdIDs.cmdidStackWindowToolbar1)]
    public sealed class StackWindowToolbar1 : ToolbarDefinition { }

    [CommandId(CmdIDs.cmdidStackWindowToolbar2)]
    public sealed class StackWindowToolbar2 : ToolbarDefinition { }
  }
}