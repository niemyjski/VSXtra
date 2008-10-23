// ================================================================================================
// WindowPane.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new Managed Package Framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created by: Istvan Novak (DiveDeeper), 05/05/2008
// ================================================================================================
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Commands;
using VSXtra.Diagnostics;
using VSXtra.Package;
using VSXtra.Selection;
using IServiceProvider=System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra.Windows
{
  // ================================================================================================
  /// <summary>
  /// This interface represents the behavior of a window pane.
  /// </summary>
  // ================================================================================================
  public interface IWindowPaneBehavior:
    // Provides methods that fill the content of a window pane into a window frame and handle 
    // commands for the pane.
    IVsWindowPane,

    // Enables dispatching of commands between objects and containers. We need to implement
    // this interface in order to handle commands related to the window pane.
    IOleCommandTarget,

    // Broadcasts messages to clients that registered to be notified of events within the 
    // environment. We implement this interface since we want to receive broadcast messages
    // from VS Shell.
    IVsBroadcastMessageEvents,

    // Defines a mechanism for retrieving a service object; that is, an object that provides custom 
    // support to other objects. Since window pane provides services for external objects, this
    // interface must be implemented.
    IServiceProvider
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the window handle for the UI of this window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    IWin32Window Window { get; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package owning this window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    PackageBase GetPackageInstance();
  }

  // ================================================================================================
  /// <summary>
  /// This is a quick way to implement a window pane. This class implements IVsWindowPane; you
  /// must provide an implementation of an object that returns an IWin32Window, however. In addition
  /// to IVsWindowPane this object implements IOleCommandTarget, mapping it to IMenuCommandService
  /// and IObjectWithSite, mapping the site to services that can be querried through its protected
  /// GetService method.
  /// </summary>
  /// <typeparam name="TPackage">The type of the package owning this window pane.</typeparam>
  /// <typeparam name="TUIControl">The type of the user control represnting the UI.</typeparam>
  // ================================================================================================
  [ComVisible(true)]
  public abstract class WindowPane<TPackage, TUIControl>:
    IWindowPaneBehavior,

    // --- Defines a method to release allocated resources. Since we allocate unmanaged resources we
    // --- must implement this interface.
    IDisposable
    where TPackage: PackageBase
    where TUIControl: Control, new()
  {
    #region Private fields

    /// <summary>Package owning this window pane.</summary>
    private readonly TPackage _Package;

    /// <summary>Service provider obtained when the pane instance has been created.</summary>
    private IServiceProvider _ParentServiceProvider;
    
    /// <summary>Service provider obtained when the pane has been sited.</summary>
    private ServiceProvider _ServiceProvider;

    /// <summary>Object to access IVsShell event setup services.</summary>
    private IVsShell _VsShell;

    /// <summary>Cookie from the call of IVsShell.AdviseBroadcastMessages</summary>
    private uint _BroadcastEventCookie;

    /// <summary>Object providing command services.</summary>
    private IMenuCommandService _CommandService;

    /// <summary>HelpService instance belonging to this window pane.</summary>
    private HelpService _HelpService;

    /// <summary>Signs if this window pane is being disposed.</summary>
    private bool _Zombied;

    /// <summary>User control representing the UI of the window pane.</summary>
    private readonly TUIControl _UIControl;

    /// <summary>
    /// Object responsible to translate command methods to OleMenuCommand instances
    /// </summary>
    private CommandDispatcher<TPackage> _CommandDispatcher;

    /// <summary>Object responsible for the selection tracking.</summary>
    private SelectionTracker _SelectionTracker;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowPane&lt;TPackage, TUIControl&gt;"/> class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected WindowPane()
    {
      _Package = PackageBase.GetPackageInstance<TPackage>();
      _ParentServiceProvider = _Package;
      _UIControl = new TUIControl();
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package owning this window pane.
    /// </summary>
    /// <value>The package owning this window pane.</value>
    // --------------------------------------------------------------------------------------------
    public TPackage Package
    {
      get { return _Package; }
    } 
    
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the UI instance of this window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public TUIControl UIControl
    {
      get { return _UIControl; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the window handle for the UI of this window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IWin32Window Window
    {
      get
      {
        return typeof(TUIControl) == typeof(WindowPanePlaceHolderControl)
                 ? null 
                 : _UIControl;
      }
    }

    public SelectionTracker SelectionTracker
    {
      get { return _SelectionTracker; }
    }

    #endregion

    #region IWindowPaneBehavior implementetion

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package owning this window pane.
    /// </summary>
    /// <value>The package owning this window pane.</value>
    // --------------------------------------------------------------------------------------------
    PackageBase IWindowPaneBehavior.GetPackageInstance()
    {
      return _Package;
    }

    #endregion

    #region IVsWindowPane implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Closes a window pane.
    /// </summary>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    /// <remarks>
    /// Inheritors must override the OnClose method to add custom behavior to the window pane.
    /// The Dispose method is called independently of the result of OnClose.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsWindowPane.ClosePane()
    {
      if (_VsShell != null)
      {
        NativeMethods.ThrowOnFailure(_VsShell.UnadviseBroadcastMessages(_BroadcastEventCookie));
        _VsShell = null;
        _BroadcastEventCookie = 0;
      }
      try
      {
        OnClose();
      }
      finally
      {
        Dispose();
      }
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates the pane window.
    /// </summary>
    /// <param name="hwndParent">Handle to the parent window.</param>
    /// <param name="x">Absolute x ordinate.</param>
    /// <param name="y">Absolute y ordinate.</param>
    /// <param name="cx">x ordinate relative to x.</param>
    /// <param name="cy">y ordinate relative to y.</param>
    /// <param name="pane">Pointer to a handle to the new window pane.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowPane.CreatePaneWindow(IntPtr hwndParent, int x, int y, int cx, int cy, out IntPtr pane)
    {
      OnCreate();
      IntPtr hwnd = Window.Handle;
      var style = (int)UnsafeNativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_STYLE);

      // --- Set up the required styles of an IVsWindowPane
      style |= (NativeMethods.WS_CLIPSIBLINGS | NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE);
      style &= ~(NativeMethods.WS_POPUP |
                 NativeMethods.WS_MINIMIZE |
                 NativeMethods.WS_MAXIMIZE |
                 NativeMethods.WS_DLGFRAME |
                 NativeMethods.WS_SYSMENU |
                 NativeMethods.WS_THICKFRAME |
                 NativeMethods.WS_MINIMIZEBOX |
                 NativeMethods.WS_MAXIMIZEBOX);

      UnsafeNativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_STYLE, (IntPtr)style);
      style = (int)UnsafeNativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
      style &= ~(NativeMethods.WS_EX_DLGMODALFRAME |
                 NativeMethods.WS_EX_NOPARENTNOTIFY |
                 NativeMethods.WS_EX_TOPMOST |
                 NativeMethods.WS_EX_MDICHILD |
                 NativeMethods.WS_EX_TOOLWINDOW |
                 NativeMethods.WS_EX_CONTEXTHELP |
                 NativeMethods.WS_EX_APPWINDOW);

      UnsafeNativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, (IntPtr)style);
      UnsafeNativeMethods.SetParent(hwnd, hwndParent);
      UnsafeNativeMethods.SetWindowPos(hwnd, IntPtr.Zero, x, y, cx, cy, 
                                       NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
      UnsafeNativeMethods.ShowWindow(hwnd, NativeMethods.SW_SHOWNORMAL);

      // --- Sync broadcast events so we update our UI when colors/fonts change.
      if (_VsShell == null)
      {
        _VsShell = GetService<SVsShell, IVsShell>();
        if (_VsShell != null)
        {
          NativeMethods.ThrowOnFailure(_VsShell.AdviseBroadcastMessages(this, 
                                                                        out _BroadcastEventCookie));
        }
      }
      pane = hwnd;
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the default size of a given window pane.
    /// </summary>
    /// <param name="size">Pointer to the size of a given window pane</param>
    /// <returns>
    /// We do not implement this method, so return E_NOTIMPL.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowPane.GetDefaultSize(SIZE[] size)
    {
      return NativeMethods.E_NOTIMPL;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Loads the state of the view.
    /// </summary>
    /// <param name="pstream">Pointer to the IStream interface of the view state to load.</param>
    /// <returns>
    /// We do not implement this method, so return E_NOTIMPL.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowPane.LoadViewState(IStream pstream)
    {
      // --- We release the stream object in order it can be disposed.
      Marshal.ReleaseComObject(pstream);
      return NativeMethods.E_NOTIMPL;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Saves the state of the view.
    /// </summary>
    /// <param name="pstream">Pointer to the IStream interface of the view state to load.</param>
    /// <returns>
    /// We do not implement this method, so return E_NOTIMPL.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowPane.SaveViewState(IStream pstream)
    {
      // --- We release the stream object in order it can be disposed.
      Marshal.ReleaseComObject(pstream);
      return NativeMethods.E_NOTIMPL;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the site.
    /// </summary>
    /// <param name="serviceProvider">The p.</param>
    /// <remarks>
    /// The siting mechanism works as follows:  If the parent provider provides 
    /// ServiceProviderHierarchy as a service we will insert our service provider in the 
    /// WindowPaneSite slot of the hierarchy. If, however, it does not provide this service, we
    /// will create a new ServiceProvider that will be used to resolve services through this site.  
    /// </remarks>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowPane.SetSite(IOleServiceProvider serviceProvider)
    {
      if (_ServiceProvider != null)
      {
        _ServiceProvider.Dispose();
        _ServiceProvider = null;
      }

      var ows = GetService<IObjectWithSite>();
      var serviceHierarchy = GetService<ServiceProviderHierarchy>();
      if (serviceHierarchy != null)
      {
        ServiceProvider sp = (serviceProvider == null ? null : new ServiceProvider(serviceProvider));
        serviceHierarchy[ServiceProviderHierarchyOrder.WindowPaneSite] = sp;
      }
      else if (ows != null)
      {
        ows.SetSite(serviceProvider);
      }
      else
      {
        if (serviceProvider != null)
        {
          _ServiceProvider = new ServiceProvider(serviceProvider);
        }
      }

      // --- We can initialize the window pane as soon as it gets sited.
      if (serviceProvider != null)
      {
        InternalInit();
        Initialize();
      }
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the internal initialization steps.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void InternalInit()
    {
      // --- Set up command dispatching
      _CommandDispatcher = new CommandDispatcher<TPackage>(this);

      // --- Register command handlers
      var parentService = Package.GetService<IMenuCommandService, OleMenuCommandService>();
      var localService = GetService<IMenuCommandService, OleMenuCommandService>();
      _CommandDispatcher.RegisterCommandHandlers(localService, parentService);
      
      // --- Create the selection tracker for the window
      _SelectionTracker = new SelectionTracker(this);
      var container = CreateSelectionContainer();
      if (container != null)
      {
        _SelectionTracker.Container = container;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the translation of special navigation keys.
    /// </summary>
    /// <param name="msg">Keyboard character or character combination to be handled.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsWindowPane.TranslateAccelerator(MSG[] msg)
    {
      var m = Message.Create(msg[0].hwnd, (int)msg[0].message, msg[0].wParam, msg[0].lParam);
      bool eat = PreProcessMessage(ref m);

      msg[0].message = (uint)m.Msg;
      msg[0].wParam = m.WParam;
      msg[0].lParam = m.LParam;

      return eat ? NativeMethods.S_OK : NativeMethods.E_FAIL;
    }

    #endregion

    #region IDisposable implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting 
    /// unmanaged resources.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Dispose()
    {
      Dispose(true);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only 
    /// unmanaged resources.
    /// </param>
    // --------------------------------------------------------------------------------------------
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_VsShell != null)
        {
          try
          {
            // --- Don't check for return code because here we can't do anything in case of failure.
            _VsShell.UnadviseBroadcastMessages(_BroadcastEventCookie);
          }
          catch (SystemException)
          {
            // --- This exception is intentionally caught
          }
          _VsShell = null;
          _BroadcastEventCookie = 0;
        }
        IWin32Window window = Window;
        if (window is IDisposable)
        {
          try
          {
            ((IDisposable)window).Dispose();
          }
          catch (Exception)
          {
            VsDebug.Fail("Failed to dispose window");
          }
        }
        if (_CommandService != null && _CommandService is IDisposable)
        {
          try
          {
            ((IDisposable)_CommandService).Dispose();
          }
          catch (Exception)
          {
            VsDebug.Fail("Failed to dispose command service");
          }
        }
        _CommandService = null;

        if (_ParentServiceProvider != null)
          _ParentServiceProvider = null;

        if (_HelpService != null)
          _HelpService = null;

        // --- Do not clear _ServiceProvider. SetSite will do it for us.
        _Zombied = true;
      }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">
    /// An object that specifies the type of service object to get.
    /// </param>
    /// <returns>
    /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service 
    /// object of type <paramref name="serviceType"/>.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected virtual object GetService(Type serviceType)
    {
      if (_Zombied)
      {
        VsDebug.Fail("GetService called after WindowPane was zombied");
        return null;
      }

      if (serviceType == null)
      {
        throw new ArgumentNullException("serviceType");
      }

      // --- We provide IMenuCommandService, so we will demand create it. MenuCommandService also
      // --- implements IOleCommandTarget, but unless someone requested IMenuCommandService no 
      // --- commands will exist, so we don't demand create for IOleCommandTarget
      if (serviceType == typeof(IMenuCommandService))
      {
        EnsureCommandService();
        return _CommandService;
      }
      if (serviceType == typeof (IOleCommandTarget)) return _CommandService;
      if (serviceType == typeof (IHelpService))
      {
        if (_HelpService == null)
        {
          _HelpService = new HelpService(this);
        }
        return _HelpService;
      }

      // --- Ask the 
      if (_ServiceProvider != null)
      {
        object service = _ServiceProvider.GetService(serviceType);
        if (service != null)
        {
          return service;
        }
      }

      if (_ParentServiceProvider != null)
      {
        return _ParentServiceProvider.GetService(serviceType);
      }

      return null;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service described by the <typeparamref name="TService"/>
    /// type parameter.
    /// </summary>
    /// <returns>
    /// The service instance requested by the <typeparamref name="TService"/> parameter 
    /// if found; otherwise null.
    /// </returns>
    /// <typeparam name="TService">The type of the service requested.</typeparam>
    // --------------------------------------------------------------------------------
    protected TService GetService<TService>()
    {
      return (TService)GetService(typeof(TService));
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service described by the <typeparamref name="SInterface"/>
    /// type parameter and retrieves it as an interface type specified by the
    /// <typeparamref name="TInterface"/> type parameter.
    /// </summary>
    /// <returns>
    /// The service instance requested by the <see cref="SInterface"/> parameter if
    /// found; otherwise null.
    /// </returns>
    /// <typeparam name="SInterface">The type of the service requested.</typeparam>
    /// <typeparam name="TInterface">
    /// The type of interface retrieved. The object providing <see cref="SInterface"/>
    /// must implement <see cref="TInterface"/>.
    /// </typeparam>
    // --------------------------------------------------------------------------------
    protected TInterface GetService<SInterface, TInterface>()
    {
      return (TInterface)GetService(typeof(SInterface));
    }

    #endregion

    #region IOleCommandTarget implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes a specified command or displays help for a command.
    /// </summary>
    /// <param name="guidGroup">
    /// Unique identifier of the command group; can be NULL to specify the standard group.
    /// </param>
    /// <param name="nCmdId">
    /// The command to be executed. This command must belong to the group specified with pguidCmdGroup.
    /// </param>
    /// <param name="nCmdExcept">
    /// Values taken from the OLECMDEXECOPT enumeration, which describe how the object should 
    /// execute the command.
    /// </param>
    /// <param name="pIn">
    /// Pointer to a VARIANTARG structure containing input arguments. Can be NULL.
    /// </param>
    /// <param name="vOut">
    /// Pointer to a VARIANTARG structure to receive command output. Can be NULL.
    /// </param>
    /// <returns>
    /// This method supports the standard return values E_FAIL and E_UNEXPECTED, 
    /// as well as the following: 
    /// <para>S_OK: The command was executed successfully.</para>
    /// <para>
    /// OLECMDERR_E_UNKNOWNGROUP: The pguidCmdGroup parameter is not NULL but does not specify 
    /// a recognized command group.
    /// </para>
    /// <para>
    /// OLECMDERR_E_NOTSUPPORTED: The nCmdID parameter is not a valid command in the group 
    /// identified by pguidCmdGroup.
    /// </para>
    /// <para>
    /// OLECMDERR_E_DISABLED: The command identified by nCmdID is currently disabled and 
    /// cannot be executed.
    /// </para>
    /// <para>
    /// OLECMDERR_E_NOHELP: The caller has asked for help on the command identified by 
    /// nCmdID, but no help is available.
    /// </para>
    /// <para>OLECMDERR_E_CANCELED: The user canceled the execution of the command.</para>
    /// </returns>
    /// <remarks>
    /// This is called by Visual Studio when the user has requested to execute a particular
    /// command.  There is no need to override this method.  If you need access to menu
    /// commands use IMenuCommandService.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IOleCommandTarget.Exec(ref Guid guidGroup, uint nCmdId, uint nCmdExcept, IntPtr pIn, IntPtr vOut)
    {
      var cmdTarget = GetService<IOleCommandTarget>();
      return cmdTarget == null 
               ? NativeMethods.OLECMDERR_E_NOTSUPPORTED 
               : cmdTarget.Exec(ref guidGroup, nCmdId, nCmdExcept, pIn, vOut);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the object for the status of one or more commands generated by user interface events.
    /// </summary>
    /// <param name="guidGroup">
    /// Unique identifier of the command group; can be NULL to specify the standard group. All the 
    /// commands that are passed in the prgCmds array must belong to the group specified by 
    /// pguidCmdGroup.
    /// </param>
    /// <param name="nCmdId">The number of commands in the prgCmds array.</param>
    /// <param name="oleCmd">
    /// A caller-allocated array of OLECMD structures that indicate the commands for which the 
    /// caller needs status information. This method fills the cmdf member of each structure with 
    /// values taken from the OLECMDF enumeration.
    /// </param>
    /// <param name="oleText">
    /// Pointer to an OLECMDTEXT structure in which to return name and/or status information of a 
    /// single command. Can be NULL to indicate that the caller does not need this information.
    /// </param>
    /// <returns>
    /// This method supports the standard return values E_FAIL and E_UNEXPECTED, 
    /// as well as the following: 
    /// <para>S_OK: The command was executed successfully.</para>
    /// <para>E_POINTER: The prgCmds argument is NULL.</para>
    /// <para>
    /// OLECMDERR_E_UNKNOWNGROUP: The pguidCmdGroup parameter is not NULL but does not specify 
    /// a recognized command group.
    /// </para>
    /// </returns>
    /// <remarks>
    /// This is called by Visual Studio when it needs the status of our menu commands.  There
    /// is no need to override this method.  If you need access to menu commands use
    /// IMenuCommandService.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IOleCommandTarget.QueryStatus(ref Guid guidGroup, uint nCmdId, OLECMD[] oleCmd, 
                                      IntPtr oleText)
    {
      var cmdTarget = GetService<IOleCommandTarget>();
      return cmdTarget != null 
               ? cmdTarget.QueryStatus(ref guidGroup, nCmdId, oleCmd, oleText) 
               : NativeMethods.OLECMDERR_E_NOTSUPPORTED;
    }

    #endregion

    #region IServiceProvider implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">
    /// An object that specifies the type of service object to get.
    /// </param>
    /// <returns>
    /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service 
    /// object of type <paramref name="serviceType"/>.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    object IServiceProvider.GetService(Type serviceType)
    {
      return GetService(serviceType);
    }

    #endregion

    #region IVsBroadcastMessageEvents implementation

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Receives broadcast messages from the shell.
    /// </summary>
    /// <param name="msg">Specifies the notification message.</param>
    /// <param name="wParam">
    /// Word value parameter for the Windows message, as received by the environment.
    /// </param>
    /// <param name="lParam">
    /// Long integer parameter for the Windows message, as received by the environment.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------
    int IVsBroadcastMessageEvents.OnBroadcastMessage(uint msg, IntPtr wParam, IntPtr lParam)
    {
      int hr = NativeMethods.S_OK;
      IntPtr hwnd = Window.Handle;
      bool result = UnsafeNativeMethods.PostMessage(hwnd, (int)msg, wParam, wParam);
      if (!result)
        hr = NativeMethods.E_FAIL;
      return hr;
    }

    #endregion

    #region Virtual methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the window pane.
    /// </summary>
    /// <remarks>
    /// This method is called after the window pane has been sited. Any initialization that 
    /// requires window frame services from VS can be done by overriding this method.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual void Initialize()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Responds to the event when the window pane is closed.
    /// </summary>
    /// <devdoc>
    /// The OnClose method is called in response to the ClosePane method on IVsWindowPane. The 
    /// default implementation does nothing. Even if this method raises an exception the window
    /// pane is disposed.
    /// </devdoc>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnClose()
    {
      Dispose();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Responds to the event when the window pane is created.
    /// </summary>
    /// <remarks>
    /// The OnCreate method is called during the CreatePaneWindow method of IVsWindowPane. This 
    /// provides a handy hook for knowing when VS wants to create the window. The default 
    /// implementation does nothing.
    /// </remarks>
    /// </devdoc>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnCreate()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Preprocesses the specified message.
    /// </summary>
    /// <remarks>
    /// This method will be called to pre-process keyboard messages before VS handles them. It is 
    /// directly attached to IVsWindowPane.TranslateAccellerator method. The default 
    /// implementation calls the PreProcessMessage method on a Windows Forms control. Inheritors
    /// may override this if the window pane is not based on Windows Forms.
    /// Arguments and return values are the same as for Windows Forms: return true if you handled
    /// the message, false if you want the default processing to occur.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual bool PreProcessMessage(ref Message m)
    {
      var c = Control.FromChildHandle(m.HWnd);
      return c != null && 
             c.PreProcessControlMessage(ref m) == PreProcessControlState.MessageProcessed;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to change how the selection container of the window is created.
    /// </summary>
    /// <returns>
    /// The selection container if the method creates it; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected virtual SelectionContainer CreateSelectionContainer()
    {
      return null;
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This is a separate method so the jitter doesn't see MenuCommandService (from 
    /// System.Design.dll) in the GetService call and load the assembly.
    /// </summary> 
    // --------------------------------------------------------------------------------------------
    private void EnsureCommandService()
    {
      if (_CommandService == null)
      {
        _CommandService = new OleMenuCommandService(this);
      }
    }

    #endregion
  }
}