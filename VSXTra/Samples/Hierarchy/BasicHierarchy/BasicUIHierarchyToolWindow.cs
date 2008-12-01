// ================================================================================================
// BasicUIHierarchyToolWindow.cs
//
// Created: 2008.12.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;
using VSXtra.Windows;

namespace DeepDiver.BasicHierarchy
{
  // ================================================================================================
  /// <summary>
  /// This class represents a Basic UI hierarchy window
  /// </summary>
  // ================================================================================================
  [InitialCaption("Basic UI Hierarchy window")]
  [BitmapResourceId(301)]
  [LinesAtRoot]
  [RouteCmdidDelete]
  [DoNotSortRootNodes]
  public class BasicUIHierarchyToolWindow: UIHierarchyToolWindow<BasicHierarchyPackage>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets up the initial hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override HierarchyManager<BasicHierarchyPackage> HierarchyManager
    {
      get { return new FileHierarchyManager("C:\\"); }
    }
  }
}