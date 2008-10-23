// ================================================================================================
// WindowFrame.cs
//
// Created: 2008.07.02, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Package;

namespace VSXtra.Windows
{
  // ================================================================================================
  /// <summary>
  /// This class encapsulates an IVsWindowFrame instance and build functionality around.
  /// </summary>
  // ================================================================================================
  public class WindowFrame : 
    // --- Our class behaves like a native Visual Studio window frame.
    IVsWindowFrame, 
    // --- Our window frame handles events.
    IVsWindowFrameNotify3
  {
    #region Private fields

    private readonly IVsWindowFrame _Frame;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the class 
    /// </summary>
    /// <param name="frame"></param>
    // --------------------------------------------------------------------------------------------
    public WindowFrame(IVsWindowFrame frame)
    {
      if (frame == null)
      {
        throw new ArgumentNullException("frame");
      }
      _Frame = frame;

      // --- Set up event handlers
      ErrorHandler.ThrowOnFailure(_Frame.SetProperty((int)__VSFPROPID.VSFPROPID_ViewHelper, this));
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the caption of the window frame.
    /// </summary>
    /// <value>
    /// Caption of the window frame.
    /// </value>
    // --------------------------------------------------------------------------------------------
    public string Caption
    {
      get
      {
        object result;
        ErrorHandler.ThrowOnFailure(_Frame.GetProperty(
                                      (int)__VSFPROPID.VSFPROPID_Caption, out result));
        return result.ToString();
      }
      set
      {
        ErrorHandler.ThrowOnFailure(_Frame.SetProperty(
                                      (int) __VSFPROPID.VSFPROPID_Caption, value));
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the bitmap resource id of the window frame.
    /// </summary>
    /// <value>
    /// Bitmap resource id of the window frame.
    /// </value>
    // --------------------------------------------------------------------------------------------
    public int BitmapResourceID
    {
      get
      {
        object result;
        ErrorHandler.ThrowOnFailure(_Frame.GetProperty(
                                      (int)__VSFPROPID.VSFPROPID_BitmapResource, out result));
        return (int)result;
      }
      set
      {
        ErrorHandler.ThrowOnFailure(_Frame.SetProperty(
                                      (int)__VSFPROPID.VSFPROPID_BitmapResource, value));
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the bitmap index of the window frame.
    /// </summary>
    /// <value>
    /// Bitmap index of the window frame.
    /// </value>
    // --------------------------------------------------------------------------------------------
    public int BitmapIndex
    {
      get
      {
        object result;
        ErrorHandler.ThrowOnFailure(_Frame.GetProperty(
                                      (int)__VSFPROPID.VSFPROPID_BitmapIndex, out result));
        return (int)result;
      }
      set
      {
        ErrorHandler.ThrowOnFailure(_Frame.SetProperty(
                                      (int)__VSFPROPID.VSFPROPID_BitmapIndex, value));
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the GUID of this window frame.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public Guid Guid
    {
      get
      {
        Guid guid;
        _Frame.GetGuidProperty((int)__VSFPROPID.VSFPROPID_GuidPersistenceSlot, out guid);
        return guid;
      }
    }

    #endregion

    #region Public Events

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when the show state of the window frame changes.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public event EventHandler<WindowFrameShowEventArgs> OnShow;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when the window frame is being closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public event EventHandler<WindowFrameCloseEventArgs> OnClose;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when the window frame is being resized.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public event EventHandler<WindowFramePositionChangedEventArgs> OnResize;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when the window frame is being moved.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public event EventHandler<WindowFramePositionChangedEventArgs> OnMove;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when the window frame's dock state is being changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public event EventHandler<WindowFrameDockChangedEventArgs> OnDockChange;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when the window frame's state is being changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public event EventHandler OnStatusChange;

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Renders this window visible, brings the window to the top, and activates the window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Show()
    {
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame) this).Show());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Hides a window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Hide()
    {
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame)this).Hide());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines whether or not the window is visible.
    /// </summary>
    /// <returns>
    /// Returns S_OK if the window is visible, otherwise returns S_FALSE.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public bool IsVisible()
    {
      int hresult = ((IVsWindowFrame) this).Hide();
      if (hresult == VSConstants.S_OK) return true;
      if (hresult == VSConstants.S_FALSE) return false;
      ErrorHandler.ThrowOnFailure(hresult);
      return false;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Shows or makes a window visible and brings it to the top, but does not make it the 
    /// active window.
    /// </summary>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public void ShowNoActivate()
    {
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame)this).ShowNoActivate());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Closes a window.
    /// </summary>
    /// <param name="option">Save options</param>
    // --------------------------------------------------------------------------------------------
    public void CloseFrame(FrameCloseOption option)
    {
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame)this).CloseFrame((uint)option));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the size of the window frame
    /// </summary>
    /// <param name="size">Size of the frame.</param>
    // --------------------------------------------------------------------------------------------
    public void SetFrameSize(Size size)
    {
      Guid guid = Guid.Empty;
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame)this).
                                    SetFramePos(VSSETFRAMEPOS.SFP_fSize, ref guid, 0, 0, size.Width, size.Height));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the position of the window frame
    /// </summary>
    /// <param name="rec">Rectangle defining the size of the frame.</param>
    // --------------------------------------------------------------------------------------------
    public void SetWindowPosition(Rectangle rec)
    {
      Guid guid = Guid.Empty;
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame)this).
                                    SetFramePos(VSSETFRAMEPOS.SFP_fMove, ref guid, 
                                                rec.Left, rec.Top, rec.Width, rec.Height));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the position of the window frame.
    /// </summary>
    /// <param name="position">Window frame coordinates.</param>
    /// <returns>
    /// General position of the frame (docked, tabbed, floating, etc.)
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public FramePosition GetWindowPosition(out Rectangle position)
    {
      int left;
      int top;
      int width;
      int height;
      var pdwSFP = new VSSETFRAMEPOS[1];
      Guid guid;
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame)this).
                                    GetFramePos(pdwSFP, out guid,
                                                out left, out top, out width, out height));
      position = new Rectangle(left, top, width, height);
      switch (pdwSFP[0])
      {
        case VSSETFRAMEPOS.SFP_fDock:
          return FramePosition.Docked;
        case VSSETFRAMEPOS.SFP_fTab:
          return FramePosition.Tabbed;
        case VSSETFRAMEPOS.SFP_fFloat:
          return FramePosition.Float;
        case VSSETFRAMEPOS.SFP_fMdiChild:
          return FramePosition.MdiChild;
      }
      return FramePosition.Unknown;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks if the window frame is on the screen.
    /// </summary>
    /// <returns>True, if the frame is on the screen; otherwise, false.</returns>
    // --------------------------------------------------------------------------------------------
    public bool IsOnScreen()
    {
      int onScreen;
      ErrorHandler.ThrowOnFailure(((IVsWindowFrame) this).IsOnScreen(out onScreen));
      return onScreen != 0;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Obtains all tool window frames.
    /// </summary>
    /// <value>All available tool window frames.</value>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<WindowFrame> ToolWindowFrames
    {
      get
      {
        var uiShell = PackageBase.GetGlobalService<SVsUIShell, IVsUIShell>();
        IEnumWindowFrames windowEnumerator;
        ErrorHandler.ThrowOnFailure(uiShell.GetToolWindowEnum(out windowEnumerator));
        var frame = new IVsWindowFrame[1];
        uint fetched;
        int hr = VSConstants.S_OK;
        while (hr == VSConstants.S_OK)
        {
          hr = windowEnumerator.Next(1, frame, out fetched);
          ErrorHandler.ThrowOnFailure(hr);
          if (fetched == 1)
          {
            yield return new WindowFrame(frame[0]);
          }
        }
      }
    }

    #endregion
    
    #region Implementation of IVsWindowFrame

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Renders this window visible, brings the window to the top, and activates the window.
    /// </summary>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.Show()
    {
      return _Frame.Show();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Hides a window.
    /// </summary>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.Hide()
    {
      return _Frame.Hide();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines whether or not the window is visible.
    /// </summary>
    /// <returns>
    /// Returns S_OK if the window is visible, otherwise returns S_FALSE.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.IsVisible()
    {
      return _Frame.IsVisible();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Shows or makes a window visible and brings it to the top, but does not make it the 
    /// active window.
    /// </summary>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.ShowNoActivate()
    {
      return _Frame.ShowNoActivate();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Closes a window.
    /// </summary>
    /// <param name="grfSaveOptions">
    /// Save options whose values are taken from the __FRAMECLOSE.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.CloseFrame(uint grfSaveOptions)
    {
      return _Frame.CloseFrame(grfSaveOptions);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the position of the window.
    /// </summary>
    /// <param name="dwSFP">
    /// Frame position whose values are taken from the VSSETFRAMEPOS enumeration.
    /// </param>
    /// <param name="rguidRelativeTo">Not used.</param>
    /// <param name="x">Absolute x ordinate.</param>
    /// <param name="y">Absolute y ordinate.</param>
    /// <param name="cx">x ordinate relative to x.</param>
    /// <param name="cy">y ordinate relative to y.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.SetFramePos(VSSETFRAMEPOS dwSFP, ref Guid rguidRelativeTo, int x, int y, int cx, int cy)
    {
      return _Frame.SetFramePos(dwSFP, ref rguidRelativeTo, x, y, cx, cy);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the position of the window.
    /// </summary>
    /// <param name="pdwSFP">Pointer to the frame position.</param>
    /// <param name="pguidRelativeTo">Not used.</param>
    /// <param name="px">Pointer to the absolute x ordinate.</param>
    /// <param name="py">Pointer to the absolute y ordinate.</param>
    /// <param name="pcx">Pointer to the x ordinate relative to px.</param>
    /// <param name="pcy">Pointer to the y ordinate relative to py.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.GetFramePos(VSSETFRAMEPOS[] pdwSFP, out Guid pguidRelativeTo, out int px, 
                                   out int py, out int pcx, out int pcy)
    {
      return _Frame.GetFramePos(pdwSFP, out pguidRelativeTo, out px, out py, out pcx, out pcy);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns a window property.
    /// </summary>
    /// <param name="propid">
    /// Identifier of the property whose values are taken from the __VSFPROPID enumeration.
    /// </param>
    /// <param name="pvar">Pointer to a VARIANT.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.GetProperty(int propid, out object pvar)
    {
      return _Frame.GetProperty(propid, out pvar);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets a window frame property.
    /// </summary>
    /// <param name="propid">
    /// Identifier of the property whose values are taken from the __VSFPROPID enumeration.
    /// </param>
    /// <param name="var">The value depends on the property set (see __VSFPROPID ).</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.SetProperty(int propid, object var)
    {
      return _Frame.SetProperty(propid, var);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns a window frame property based on a supplied GUID.
    /// </summary>
    /// <param name="propid">
    /// Identifier of the property whose values are taken from the __VSFPROPID enumeration.
    /// </param>
    /// <param name="pguid">Pointer to the unique identifier of the property.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.GetGuidProperty(int propid, out Guid pguid)
    {
      return _Frame.GetGuidProperty(propid, out pguid);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets a window frame property based on a supplied GUID.
    /// </summary>
    /// <param name="propid">
    /// Identifier of the property whose values are taken from the __VSFPROPID enumeration.
    /// </param>
    /// <param name="rguid">Unique identifier of the property to set.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.SetGuidProperty(int propid, ref Guid rguid)
    {
      return _Frame.SetGuidProperty(propid, ref rguid);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Provides IVsWindowFrame with a view helper (VSFPROPID_ViewHelper) inserted into its list 
    /// of event notifications.
    /// </summary>
    /// <param name="riid">Identifier of the window frame being requested.</param>
    /// <param name="ppv">
    /// Address of pointer variable that receives the window frame pointer requested in riid.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.QueryViewInterface(ref Guid riid, out IntPtr ppv)
    {
      return _Frame.QueryViewInterface(ref riid, out ppv);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the window frame is on the screen.
    /// </summary>
    /// <param name="pfOnScreen">true if the window frame is visible on the screen.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    /// <remarks>
    /// IVsWindowFrame.IsOnScreen checks to see if a window hosted by the Visual Studio IDE has 
    /// been autohidden, or if the window is part of a tabbed display and currently obscured by 
    /// another tab. IsOnScreen also checks to see whether the instance of the Visual Studio IDE 
    /// is minimized or obscured. IsOnScreen differs from the behavior of IsWindowVisible a 
    /// method that may return true even if the window is completely obscured or minimized. 
    /// IsOnScreen also differs from IsVisible which does not check to see if the Visual Studio 
    /// IDE has autohidden the window, or if the window is tabbed and currently obscured by 
    /// another window.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrame.IsOnScreen(out int pfOnScreen)
    {
      return _Frame.IsOnScreen(out pfOnScreen);
    }

    #endregion

    #region Implementation of IVsWindowFrameNotify3

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies the VSPackage of a change in the window's display state.
    /// </summary>
    /// <param name="fShow">
    /// Specifies the reason for the display state change. Value taken from the __FRAMESHOW.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrameNotify3.OnShow(int fShow)
    {
      if (OnShow != null)
      {
        var e = new WindowFrameShowEventArgs((FrameShow) fShow);
        OnShow(this, e);
      }
      InvokeStatusChanged();
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies the VSPackage that a window is being moved.
    /// </summary>
    /// <param name="x">New horizontal position.</param>
    /// <param name="y">New vertical position.</param>
    /// <param name="w">New window width.</param>
    /// <param name="h">New window height.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrameNotify3.OnMove(int x, int y, int w, int h)
    {
      if (OnMove != null)
      {
        var e = new WindowFramePositionChangedEventArgs(new Rectangle(x, y, w, h));
        OnMove(this, e);
      }
      InvokeStatusChanged();
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies the VSPackage that a window is being resized.
    /// </summary>
    /// <param name="x">New horizontal position.</param>
    /// <param name="y">New vertical position.</param>
    /// <param name="w">New window width.</param>
    /// <param name="h">New window height.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrameNotify3.OnSize(int x, int y, int w, int h)
    {
      if (OnResize != null)
      {
        var e = new WindowFramePositionChangedEventArgs(new Rectangle(x, y, w, h));
        OnResize(this, e);
      }
      InvokeStatusChanged();
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies the VSPackage that a window's docked state is being altered.
    /// </summary>
    /// <param name="fDockable">true if the window frame is being docked.</param>
    /// <param name="x">Horizontal position of undocked window.</param>
    /// <param name="y">Vertical position of undocked window.</param>
    /// <param name="w">Width of undocked window.</param>
    /// <param name="h">Height of undocked window.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrameNotify3.OnDockableChange(int fDockable, int x, int y, int w, int h)
    {
      if (OnDockChange != null)
      {
        var e = new WindowFrameDockChangedEventArgs(new Rectangle(x, y, w, h), fDockable != 0);
        OnDockChange(this, e);
      }
      InvokeStatusChanged();
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies the VSPackage that a window frame is closing and tells the environment what 
    /// action to take.
    /// </summary>
    /// <param name="pgrfSaveOptions">
    /// Specifies options for saving window content. Values are taken from the __FRAMECLOSE 
    /// enumeration.
    /// </param>
    // --------------------------------------------------------------------------------------------
    int IVsWindowFrameNotify3.OnClose(ref uint pgrfSaveOptions)
    {
      if (OnClose != null)
      {
        var e = new WindowFrameCloseEventArgs((FrameCloseOption)pgrfSaveOptions);
        OnClose(this, e);
      }
      InvokeStatusChanged();
      return VSConstants.S_OK;
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Invokes the event handler for the status changed event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void InvokeStatusChanged()
    {
      if (OnStatusChange != null)
      {
        var e = new EventArgs();
        OnStatusChange(this, e);
      }
    }

    #endregion
  }

  /// <summary>
  /// Specifies close options when closing a window frame.
  /// </summary>
  // ================================================================================================
  public enum FrameCloseOption
  {
    /// <summary>Do not save the document.</summary>
    NoSave = __FRAMECLOSE.FRAMECLOSE_NoSave,

    /// <summary>Save the document if it is dirty.</summary>
    SaveIfDirty = __FRAMECLOSE.FRAMECLOSE_SaveIfDirty,

    /// <summary>Prompt for document save.</summary>
    PromptSave = __FRAMECLOSE.FRAMECLOSE_PromptSave
  }

  /// <summary>
  /// Specifies the window frame positions.
  /// </summary>
  // ================================================================================================
  public enum FramePosition
  {
    /// <summary>Window frame has unknown position.</summary>
    Unknown = 0,
    /// <summary>Window frame is docked.</summary>
    Docked = VSSETFRAMEPOS.SFP_fDock,
    /// <summary>Window frame is tabbed.</summary>
    Tabbed = VSSETFRAMEPOS.SFP_fTab,
    /// <summary>Window frame floats.</summary>
    Float = VSSETFRAMEPOS.SFP_fFloat,
    /// <summary>Window frame is currently within the MDI space.</summary>
    MdiChild = VSSETFRAMEPOS.SFP_fMdiChild
  }

  /// <summary>
  /// Specifies options when the show state of a window frame changes.
  /// </summary>
  // ================================================================================================
  [Flags]
  public enum FrameShow
  {
    /// <summary>Reason unknown</summary>
    Unknown = 0,
    /// <summary>Obsolete; use WinHidden.</summary>
    Hidden = __FRAMESHOW.FRAMESHOW_Hidden,
    /// <summary>Window (tabbed or otherwise) is hidden.</summary>
    WinHidden = __FRAMESHOW.FRAMESHOW_WinHidden,
    /// <summary>A nontabbed window is made visible.</summary>
    Shown = __FRAMESHOW.FRAMESHOW_WinShown,
    /// <summary>A tabbed window is activated (made visible).</summary>
    TabActivated = __FRAMESHOW.FRAMESHOW_TabActivated,
    /// <summary>A tabbed window is deactivated.</summary>
    TabDeactivated = __FRAMESHOW.FRAMESHOW_TabDeactivated,
    /// <summary>Window is restored to normal state.</summary>
    Restored = __FRAMESHOW.FRAMESHOW_WinRestored,
    /// <summary>Window is minimized.</summary>
    Minimized = __FRAMESHOW.FRAMESHOW_WinMinimized,
    /// <summary>Window is maximized.</summary>
    Maximized = __FRAMESHOW.FRAMESHOW_WinMaximized,
    /// <summary>Multi-instance tool window destroyed.</summary>
    DestroyMultipleInstance = __FRAMESHOW.FRAMESHOW_DestroyMultInst,
    /// <summary>Autohidden window is about to slide into view.</summary>
    AutoHideSlideBegin = __FRAMESHOW.FRAMESHOW_AutoHideSlideBegin
  }

  /// <summary>
  /// Event arguments for the event raised when the show state of a window frame changes.
  /// </summary>
  // ================================================================================================
  public class WindowFrameShowEventArgs : EventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Reason of the event (why the show state is changed)?
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public FrameShow Reason { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an event argument instance with the initial reason.
    /// </summary>
    /// <param name="reason">Event reason.</param>
    // --------------------------------------------------------------------------------------------
    public WindowFrameShowEventArgs(FrameShow reason)
    {
      Reason = reason;
    }
  }

  /// <summary>
  /// Event arguments for the event raised when the window frame is closed.
  /// </summary>
  // ================================================================================================
  public class WindowFrameCloseEventArgs : EventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Options used to close the window frame.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public FrameCloseOption CloseOption { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an event argument instance with the initial close option.
    /// </summary>
    /// <param name="closeOption">Close option.</param>
    // --------------------------------------------------------------------------------------------
    public WindowFrameCloseEventArgs(FrameCloseOption closeOption)
    {
      CloseOption = closeOption;
    }
  }

  /// <summary>
  /// Event arguments for the events raised when the window frame position is changed.
  /// </summary>
  // ================================================================================================
  public class WindowFramePositionChangedEventArgs : EventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// New window frame position.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public Rectangle Position { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an event argument instance with the new frame position.
    /// </summary>
    /// <param name="position">New frame position.</param>
    // --------------------------------------------------------------------------------------------
    public WindowFramePositionChangedEventArgs(Rectangle position)
    {
      Position = position;
    }
  }

  /// <summary>
  /// Event arguments for the event raised when the dock state of the window frame is changed.
  /// </summary>
  // ================================================================================================
  public class WindowFrameDockChangedEventArgs : WindowFramePositionChangedEventArgs
  {
    public bool Docked { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an event argument instance with the new position and dock state..
    /// </summary>
    /// <param name="position">New position of the window frame.</param>
    /// <param name="docked">True, if the frame is docked; otherwise, false.</param>
    // --------------------------------------------------------------------------------------------
    public WindowFrameDockChangedEventArgs(Rectangle position, bool docked)
      : base(position)
    {
      Docked = docked;
    }
  }
}