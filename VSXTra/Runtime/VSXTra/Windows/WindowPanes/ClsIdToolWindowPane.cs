// ================================================================================================
// ClsIdToolWindowPane.cs
//
// Created: 2008.09.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Package;

namespace VSXtra.Windows
{
  // ================================================================================================
  /// <summary>
  /// This class defines a Tool Window Pane based on a CLSID.
  /// </summary>
  // ================================================================================================
  public abstract class ClsIdToolWindowPane<TPackage> :
    ToolWindowPane<TPackage, WindowPanePlaceHolderControl>
    where TPackage : PackageBase
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Derived tool window classes must define this property to set up the CLSID of the tool 
    /// window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected abstract Guid ToolWindowClassGuid { get; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the CLSID as defined by this window class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override Guid ToolClsid
    {
      get { return ToolWindowClassGuid; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sites the window before setting the frame.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override WindowFrame Frame
    {
      set
      {
        if (value != null && value.OleServiceProvider != null)
          ((IVsWindowPane) this).SetSite(value.OleServiceProvider);
        base.Frame = value;
      }
    }
  }
}