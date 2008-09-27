// ================================================================================================
// SimpleUIHierarchyToolWindow.cs
//
// Created: 2008.09.02, by Istvan Novak (DeepDiver)
// ================================================================================================
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra;

namespace DeepDiver.UIHierarchyWindow
{
  [InitialCaption("Simple UI Hierarchy window")]
  [BitmapResourceId(301)]
  [DoNotSortRootNodes]
  [LinesAtRoot]
  [RouteCmdidDelete]
  [ActAsProjectTypeWindow]
  public class SimpleUIHierarchyToolWindow : UIHierarchyToolWindow<UIHierarchyWindowPackage>
  {
    protected override IVsUIHierarchy InitialHierarchy
    {
      get { return new SimpleHierarchy(HierarchyWindow); }
    }
  }
}