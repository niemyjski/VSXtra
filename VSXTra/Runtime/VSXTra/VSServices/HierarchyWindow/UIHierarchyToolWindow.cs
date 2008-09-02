// ================================================================================================
// UIHierarchyToolWindow.cs
//
// Created: 2008.09.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class represents a UI hierarchy tool window.
  /// </summary>
  /// <typeparam name="TPackage">Package owning the tool window.</typeparam>
  // ================================================================================================
  public abstract class UIHierarchyToolWindow<TPackage> :
    ClsIdToolWindowPane<TPackage>
    where TPackage: PackageBase
  {
    protected override Guid ToolWindowClassGuid 
    {
      get { return VSConstants.CLSID_VsUIHierarchyWindow; }
    }

    public IVsUIHierarchyWindow HierarchyWindow
    {
      get
      {
        var frame = Frame as IVsWindowFrame;
        if (frame != null)
        {
          Object docView;
          int hr = frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out docView);
          if (hr == VSConstants.S_OK)
            return (IVsUIHierarchyWindow)docView;
        }
        return null;
      }
    }
  }
}