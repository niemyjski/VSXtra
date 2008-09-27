// ================================================================================================
// CommandGroup.cs
//
// Created: 2008.07.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.DynamicToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This type represents a command group owned by the DynamicToolWindowPackage.
  /// </summary>
  // ================================================================================================
  [Guid(GuidList.guidDynamicToolWindowCmdSetString)]
  public sealed class DynamicToolWindowCommandGroup: CommandGroup<DynamicToolWindowPackage>
  {
    // ================================================================================================
    /// <summary>
    /// This class implements the command to display the Dynamic Tool Window
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidMyTool)]
    [ShowToolWindowAction(typeof(DynamicWindowPane))]
    public sealed class ShowToolCommand : MenuCommandHandler { }
  }
}
