// ================================================================================================
// DynamicUIHierarchyWindow.cs
//
// Created: 2008.12.07, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;
using VSXtra.Windows;

namespace DeepDiver.DynamicHierarchy
{
  // ================================================================================================
  /// <summary>
  /// Sample dynamic hierarchy window
  /// </summary>
  // ================================================================================================
  [InitialCaption("Dynamic Hierarchy Window")]
  [BitmapResourceId(301)]
  [LinesAtRoot]
  [DoNotSortRootNodes]
  public sealed class DynamicUIHierarchyWindow : UIHierarchyToolWindow<DynamicHierarchyPackage>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to set up the initial hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override HierarchyManager<DynamicHierarchyPackage> HierarchyManager
    {
      get { return new FileHierarchyManager("C:\\"); }
    }
  }
}
