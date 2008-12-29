// ================================================================================================
// SortedUIHierarchyWindow.cs
//
// Created: 2008.12.15, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Commands;
using VSXtra.Hierarchy;
using VSXtra.Shell;
using VSXtra.Windows;

namespace DeepDiver.HierarchySort
{
  [InitialCaption("Sorted Hierarchy Window")]
  [BitmapResourceId(301)]
  [Toolbar(typeof(CommandGroup.HierarchyWindowToolbar))]
  [LinesAtRoot]
  [InitWithHiddenRootHierarchy]
  public sealed class SortedUIHierarchyWindow : UIHierarchyToolWindow<HierarchySortPackage>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to set up the initial hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override HierarchyManager<HierarchySortPackage> CreateInitialHierarchy()
    {
      return new HiddenHierarchyManager<HierarchySortPackage>();
    }

    [CommandExecMethod]
    [CommandId(GuidList.guidHierarchySortCmdSetString, CmdIDs.AddHierarchy)]
    private void AddNewHierarchy()
    {
      AddUIHierarchy(new NumberHierarchyManager());
    }

    [CommandExecMethod]
    [CommandId(GuidList.guidHierarchySortCmdSetString, CmdIDs.RemoveHierarchy)]
    private void RemoveSelectedHierarchy()
    {
      var root = FindCommonSelectionRoot();
      if (root == null)
      {
        VsMessageBox.Show("No common root");
      }
      else
      {
        VsMessageBox.Show("Common root: " + root.HierarchyRoot.Caption);
      }
    }

    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      AddUIHierarchy(new NumberHierarchyManager());
      AddUIHierarchy(new NumberHierarchyManager());
      AddUIHierarchy(new NumberHierarchyManager());
    }
  }
}