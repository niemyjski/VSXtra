// ================================================================================================
// RdtEventsCommandGroup.cs
//
// Created: 2008.08.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra.Commands;

namespace DeepDiver.RdtEventsWindow
{
  [Guid(GuidList.guidRdtEventsWindowCmdSetString)]
  public class RdtEventsCommandGroup: CommandGroup<RdtEventsWindowPackage>
  {
    // ================================================================================================
    /// <summary>
    /// Displays the RDT Event Explorer tool window
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidShowRdtEventWindow)]
    [ShowToolWindowAction(typeof(RdtEventsWindowPane))]
    public sealed class ShowRdtEventExplore: MenuCommandHandler {}

    // ================================================================================================
    /// <summary>
    /// Represents the RDT Event Explorer toolbar
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.RdtEventsToolbar)]
    public sealed class RdtEventExplorerToolbar : ToolbarDefinition { }
  }
}