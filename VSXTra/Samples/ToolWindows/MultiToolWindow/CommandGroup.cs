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
  public sealed class CommandGroup : CommandGroup<MultiToolWindowPackage>
  {
    #region Nested type: ShowStackWindow1

    [CommandId(CmdIDs.cmdidShowFirstWindow)]
    [ShowToolWindowAction(typeof (NumberStackWindowPane1))]
    public sealed class ShowStackWindow1 : MenuCommandHandler
    {
    }

    #endregion

    #region Nested type: ShowStackWindow2

    [CommandId(CmdIDs.cmdidShowSecondWindow)]
    [ShowToolWindowAction(typeof (NumberStackWindowPane2))]
    public sealed class ShowStackWindow2 : MenuCommandHandler
    {
    }

    #endregion

    #region Nested type: StackWindowToolbar1

    [CommandId(CmdIDs.cmdidStackWindowToolbar1)]
    public sealed class StackWindowToolbar1 : ToolbarDefinition
    {
    }

    #endregion

    #region Nested type: StackWindowToolbar2

    [CommandId(CmdIDs.cmdidStackWindowToolbar2)]
    public sealed class StackWindowToolbar2 : ToolbarDefinition
    {
    }

    #endregion
  }
}