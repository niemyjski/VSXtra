// ================================================================================================
// CommandGroup.cs
//
// Created: 2008.07.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra.Commands;

namespace DeepDiver.PersistedToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This type represents a command group owned by the PersistedToolWindowPackage.
  /// </summary>
  // ================================================================================================
  [Guid(GuidList.guidPersistedToolWindowCmdSetString)]
  public sealed class PersistedToolWindowCommandGroup : CommandGroup<PersistedToolWindowPackage>
  {
    // ================================================================================================
    /// <summary>
    /// This class implements the command to display the Persisted Tool Window
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidPersistedWindow)]
    [ShowToolWindowAction(typeof(PersistedWindowPane))]
    public sealed class ShowToolCommand : MenuCommandHandler { }

    // ================================================================================================
    /// <summary>
    /// This class implements the command to display the Persisted Tool Window
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.IDM_PersistedWindowToolbar)]
    public sealed class PersistedWindowToolbar : ToolbarDefinition {}
  }
}
