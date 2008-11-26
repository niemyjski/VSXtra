// ================================================================================================
// UIHierarchyToolWindow.cs
//
// Created: 2008.09.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Package;

namespace VSXtra.Windows
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
    #region Private Fields

    private readonly List<HierarchyWindowAttribute> _Attributes;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new hierarchy tool window instance and sets up its style according to the 
    /// attributes defined.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected UIHierarchyToolWindow()
    {
      _Attributes = new List<HierarchyWindowAttribute>(GetType().AttributesOfType<HierarchyWindowAttribute>());
    }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the CLSID_VsUIHierarchyWindow class ID.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override Guid ToolWindowClassGuid 
    {
      get { return VSConstants.CLSID_VsUIHierarchyWindow; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the tool window with the specified style and initial hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      __UIHWINFLAGS flags = 0;
      // ReSharper disable AccessToModifiedClosure
      _Attributes.ForEach(attr => flags |= attr.StyleFlag);
      // ReSharper restore AccessToModifiedClosure
      SetUIWindowStyle(ref flags);
      object unkObj;
      HierarchyWindow.Init(InitialHierarchy, (uint)flags, out unkObj);
    }

    #endregion

    #region Virtual methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the style of the hierarchy window.
    /// </summary>
    /// <param name="style">Style to set</param>
    /// <remarks>
    /// Override this method to set style flags manually.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual void SetUIWindowStyle(ref __UIHWINFLAGS style)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to set up the initial hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual IVsUIHierarchy InitialHierarchy
    {
      get { return null; }
    }

    #endregion

    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the IVsUIHierarchyWindow instance representing the native hierarchy window 
    /// behind this tool window instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
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

    #endregion
  }
}