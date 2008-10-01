using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.SolutionNodeAnalyzer
{
  // ================================================================================================
  /// <summary>
  /// This type represents a command group owned by the PersistedToolWindowPackage.
  /// </summary>
  // ================================================================================================
  [Guid(GuidList.guidSolutionNodeAnalyzerCmdSetString)]
  public sealed class AnalyzerCommandGroup : CommandGroup<SolutionNodeAnalyzerPackage>
  {
    // ================================================================================================
    /// <summary>
    /// This class implements the command to display the Persisted Tool Window
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidShowNodeAnalyzer)]
    [ShowToolWindowAction(typeof(NodeAnalyzerToolWindow))]
    public sealed class ShowToolCommand : MenuCommandHandler { }

    // ================================================================================================
    /// <summary>
    /// Toolbar used by the NodeAnalyzerToolWindow
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.AnalyzerWindowToolbar)]
    public sealed class AnalyzerWindowToolbar : ToolbarDefinition { }
  }
}