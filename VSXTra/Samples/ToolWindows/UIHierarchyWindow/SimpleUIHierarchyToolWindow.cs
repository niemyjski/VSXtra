// ================================================================================================
// SimpleUIHierarchyToolWindow.cs
//
// Created: 2008.09.02, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;
using VSXtra.Windows;

namespace DeepDiver.UIHierarchyWindow
{
  // ================================================================================================
  /// <summary>
  /// This class implements a simple hierarchy window
  /// </summary>
  // ================================================================================================
  [InitialCaption("Simple UI Hierarchy window")]
  [BitmapResourceId(301)]
  [LinesAtRoot]
  [RouteCmdidDelete]
  public class SimpleUIHierarchyToolWindow : UIHierarchyToolWindow<UIHierarchyWindowPackage>
  {
    protected override HierarchyManager<UIHierarchyWindowPackage> HierarchyManager
    {
      get { return new FileHierarchyManager("C:\\"); }
    }
  }
}