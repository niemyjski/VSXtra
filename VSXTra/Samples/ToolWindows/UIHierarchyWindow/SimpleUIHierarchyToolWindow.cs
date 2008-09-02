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
  public class SimpleUIHierarchyToolWindow : UIHierarchyToolWindow<UIHierarchyWindowPackage>
  {
    public override void OnToolWindowCreated()
    {
      // Initialize the hierarchy window with desired styles...
      object unkObj;
      uint grfUIHWF = (uint) __UIHWINFLAGS.UIHWF_DoNotSortRootNodes |
                      //(uint) __UIHWINFLAGS.UIHWF_SupportToolWindowToolbars |
                      (uint)__UIHWINFLAGS.UIHWF_LinesAtRoot |
                      (uint)__UIHWINFLAGS.UIHWF_RouteCmdidDelete |
                      (uint) __UIHWINFLAGS.UIHWF_ActAsProjectTypeWin;

      // Initialize with custom hierarchy
      var hierarchy = new SimpleHierarchy(HierarchyWindow) as IVsUIHierarchy;
      HierarchyWindow.Init(hierarchy, grfUIHWF, out unkObj);
    }
  }
}