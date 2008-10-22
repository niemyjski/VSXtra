// ================================================================================================
// NodeAnalyzerToolWindow.cs
//
// Created: 2008.09.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
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
      Console.WriteLine("__VSHPROPID values:");
      DisplayEnumValues(typeof(__VSHPROPID));
      Console.WriteLine("__VSHPROPID2 values:");
      DisplayEnumValues(typeof(__VSHPROPID2));
      Console.WriteLine("__VSHPROPID3 values:");
      DisplayEnumValues(typeof(__VSHPROPID3));
      Console.WriteLine("NIL: {0}", VSConstants.VSITEMID_NIL);
    }

    private static void DisplayEnumValues(Type enumType)
    {
      foreach (var item in Enum.GetNames(enumType))
      {
        Console.WriteLine("{0} = {1}", item, (int)Enum.Parse(enumType, item));
      }
    }
  }
}