// ================================================================================================
// NodeAnalyzerToolWindow.cs
//
// Created: 2008.09.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.SolutionNodeAnalyzer
{
  [Guid("cf60a169-de7c-4c68-9fe7-cfc6bf9d016a")]
  [InitialCaption("$ToolWindowTitle")]
  [BitmapResourceId(301)]
  [Toolbar(typeof(AnalyzerCommandGroup.AnalyzerWindowToolbar))]
  public class NodeAnalyzerToolWindow : ToolWindowPane<SolutionNodeAnalyzerPackage, NodeAnalyzerControl>
  {
    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      UIControl.SelectionTracker = SelectionTracker;
      RefreshList();
    }

    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidRefreshHierarchy)]
    private void RefreshList()
    {
      UIControl.RefreshList(Solution.TraverseItems());
    }
  }
}