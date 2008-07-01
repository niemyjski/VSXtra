// ================================================================================================
// ToolWindowPane.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new Managed Package Framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created by: Istvan Novak (DiveDeeper), 05/05/2008
// ================================================================================================
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Properties;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This interface represents the behavior of a tool window pane.
  /// </summary>
  // ================================================================================================
  public interface IToolWindowPaneBehavior : IWindowPaneBehavior
  {
    CommandID ToolBar { get; set; }
    Guid ToolClsid { get; set; }
    string Caption { get; set; }
    object Frame { get; set; }
    int ToolBarLocation { get; set; }
    void OnToolBarAdded();
  }

  // ================================================================================================
  /// <summary>
  /// This class implements a tool window pane by specializing a simple window pane.
  /// </summary>
  /// <typeparam name="TPackage">The type of the package owning this window pane.</typeparam>
  /// <typeparam name="TUIControl">The type of the user control represnting the UI.</typeparam>
  // ================================================================================================
  [ComVisible(true)]
  public abstract class ToolWindowPane<TPackage, TUIControl> : 
    WindowPane<TPackage, TUIControl>,
    IToolWindowPaneBehavior
    where TPackage : PackageBase
    where TUIControl : UserControl, new()
  {
    #region Private fields

    /// <summary>Caption of the tool window</summary>
    private string _Caption;

    /// <summary>Window frame hosting this tool window</summary>
    private IVsWindowFrame _Frame;

    /// <summary>Command ID of the toolbar belonging to this tool window</summary>
    private CommandID _ToolBarCommandID;

    /// <summary>Location of the toolbar belonging to this tool window</summary>
    private VSTWT_LOCATION _ToolBarLocation;

    /// <summary>ID of the bitmap resource belonging to this tool window</summary>
    private int _BitmapResourceID;

    /// <summary>Index of the bitmap</summary>
    private int _BitmapIndex;
    
    private Guid _ToolClsid;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of this class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected ToolWindowPane()
    {
      _ToolClsid = Guid.Empty;
      _BitmapIndex = -1;
      _BitmapResourceID = -1;
      _ToolBarLocation = VSTWT_LOCATION.VSTWT_TOP;

      // --- Obtain attributes of the class
      foreach (object attr in GetType().GetCustomAttributes(false))
      {
        var captionAttr = attr as InitialCaptionAttribute;
        if (captionAttr != null)
        {
          Caption = StringResolver<TPackage>.Resolve(captionAttr.Value);
          continue;
        }
        var resIdAttr = attr as BitmapResourceIdAttribute;
        if (resIdAttr != null)
        {
          BitmapResourceID = resIdAttr.ResourceId;
          BitmapIndex = resIdAttr.BitmapIndex;
          continue;
        }
      }
    }

    #endregion

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the caption of the tool window.
    /// </summary>
    /// <value>The caption of the tool window.</value>
    // --------------------------------------------------------------------------------------------
    public string Caption
    {
      get { return _Caption; }
      set
      {
        _Caption = value;
        if (_Frame != null && _Caption != null)
        {
          // --- Since at this time the window is already created, set the coresponding property
          int hr;
          try
          {
            hr = _Frame.SetProperty((int) __VSFPROPID.VSFPROPID_Caption, _Caption);
          }
          catch (COMException e)
          {
            hr = e.ErrorCode;
          }
          VsDebug.Assert(hr >= 0, "Failed to set caption on toolwindow");
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the window frame hosting this tool window.
    /// </summary>
    /// <value>The window frame hosting this tool window.</value>
    /// <remarks>
    /// Fires the OnToolWindowCreated event when the frame is changed.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public object Frame
    {
      get { return _Frame; }
      set
      {
        _Frame = (IVsWindowFrame) value;
        OnToolWindowCreated();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the tool bar belonging to this tool window.
    /// </summary>
    /// <value>The command ID of tool bar belonging to this tool window.</value>
    /// <remarks>
    /// If the toolwindow has a ToolBar, it is described by this parameter.
    /// Otherwise this is null
    /// </remarks>
    /// <include file="doc\ToolWindowPane.uex" path="docs/doc[@for=&quot;ToolWindowPane.ToolBar&quot;]/*"/>
    // --------------------------------------------------------------------------------------------
    public CommandID ToolBar
    {
      get { return _ToolBarCommandID; }
      set
      {
        if (_Frame != null)
          throw new Exception(Resources.ToolWindow_TooLateToAddToolbar);
        _ToolBarCommandID = value;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets where the toolbar should be in the tool window (Up, down, left, right).
    /// This parameter is based on VSTWT_LOCATION
    /// </summary>
    /// <value>The tool bar location.</value>
    // --------------------------------------------------------------------------------------------
    public int ToolBarLocation
    {
      get { return (int) _ToolBarLocation; }
      set
      {
        if (_Frame != null)
          throw new Exception(Resources.ToolWindow_TooLateToAddToolbar);
        _ToolBarLocation = (VSTWT_LOCATION) value;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This is used to specify the CLSID of a tool that should be used for this toolwindow
    /// </summary>
    /// <value>The tool CLSID.</value>
    // --------------------------------------------------------------------------------------------
    // TODO: Think it over if we need this propertyat all
    public Guid ToolClsid
    {
      get { return _ToolClsid; }
      set
      {
        if (_Frame != null)
          throw new Exception(Resources.ToolWindow_TooLateToAddTool);
        _ToolClsid = value;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Get or Set the resource ID for the bitmap strip from which to take the window frame icon.
    /// </summary>
    /// <value>The bitmap resource ID.</value>
    // --------------------------------------------------------------------------------------------
    public int BitmapResourceID
    {
      get { return _BitmapResourceID; }
      set
      {
        _BitmapResourceID = value;
        if (_Frame != null && _BitmapResourceID != -1)
        {
          int hr;
          // --- Since the window is already created, set the coresponding property
          try
          {
            hr = _Frame.SetProperty((int) __VSFPROPID.VSFPROPID_BitmapResource, _BitmapResourceID);
          }
          catch (COMException e)
          {
            hr = e.ErrorCode;
          }
          VsDebug.Assert(hr >= 0, "Failed to set bitmap resource on toolwindow");
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Get or Set the index of the image to use in the bitmap strip for the window frame icon.
    /// </summary>
    /// <value>The index of the bitmap.</value>
    // --------------------------------------------------------------------------------------------
    public int BitmapIndex
    {
      get { return _BitmapIndex; }
      set
      {
        _BitmapIndex = value;
        if (_Frame != null && _BitmapIndex != -1)
        {
          int hr;
          // --- Since the window is already created, set the coresponding property
          try
          {
            hr = _Frame.SetProperty((int) __VSFPROPID.VSFPROPID_BitmapIndex, _BitmapIndex);
          }
          catch (COMException e)
          {
            hr = e.ErrorCode;
          }
          VsDebug.Assert(hr >= 0, "Failed to set bitmap index on toolwindow");
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method can be overriden by the derived class to execute any code that needs to run 
    /// after the IVsWindowFrame is created. If the toolwindow has a toolbar with a combobox, it 
    /// should make sure its command handler are set by the time they return from this method. 
    /// This is called when someone set the Frame property.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual void OnToolWindowCreated()
    {
      VsDebug.Assert(_Frame != null, "Frame should be set before this method is called");

      // --- If any property were set, set them on the frame.
      Caption = _Caption;
      BitmapResourceID = _BitmapResourceID;
      BitmapIndex = _BitmapIndex;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This should be overriden if you want to run code before the window is shown
    /// but after its toolbar is added.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual void OnToolBarAdded()
    {
    }
  }
}