// ================================================================================================
// PackageBase.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using VSXtra.Commands;
using VSXtra.Diagnostics;
using VSXtra.Editors;
using VSXtra.Properties;
using VSXtra.Windows;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using System.Globalization;
using System.Linq;

namespace VSXtra.Package
{
  // ================================================================================================
  /// <summary>
  /// This class is alternate implementation of the Microsoft.VisualStudio.Shell.Package type. 
  /// </summary>
  /// <remarks>
  /// 	<para>This alternate implementation targets using new technologies like generics
  ///     and LINQ. Extensions are the followings:</para>
  /// 	<list type="bullet">
  /// 		<item></item>
  /// 		<item>Generic forms of the GetService and GetGlobaleService methods</item>
  /// 	</list>
  /// </remarks>
  // ================================================================================================
  [PackageRegistration]
  [CLSCompliant(false)]
  [ComVisible(true)]
  public abstract class PackageBase:
    // This is the mandatory interface to be implemented in any VSPackage and is used by the shell 
    // to call on the VSPackage for services they might provide. Most environment extensions start 
    // out by loading a VSPackage, which is sited to the environment. Our type is not a VSPackage
    // without implementing this interface.
    IVsPackage,

    // Provides access to a service identified by a globally unique identifier (GUID). We must
    // implement this interface in order other (external) COM objects can access services defined
    // in our package.
    IOleServiceProvider,

    // Enables the dispatching of commands between objects and containers. We need to implement
    // this interface in order to handle commands defined by our package.
    IOleCommandTarget,

    // A service container is, by definition, a service provider. In addition to providing services, 
    // it also provides a mechanism for adding and removing services. Services are a foundation of 
    // VS extensibility architecture.
    // Since we cannot write a functional package without fundamental services, we need to keep
    // track of them, so that is why we implement a service container in our package.
    IServiceContainer,

    // Implementing this interface allows our package providing access to user-specific options in 
    // the user options file associated with the solution.
    IVsPersistSolutionOpts,

    // Our package implements this interface to state information persisted by the Visual Studio 
    // settings mechanism.
    IVsUserSettings,

    // The package provides this interface to provide support for migrating user settings.
    IVsUserSettingsMigration,

    // Provides the ability to create multiple tool windows.
    IVsToolWindowFactory
  {
    #region Private fields

    /// <summary>This flag signs if the package is being disposed.</summary>
    private bool _Zombied;

    /// <summary>Number of packages sited.</summary>
    private static  int _SitedPackageCount;

    /// <summary>Service provider for global services.</summary>
    private static  ServiceProvider _GlobalServiceProvider;

    /// <summary>Service provider for local services.</summary>
    private ServiceProvider _ServiceProvider;

    /// <summary>Dictionary for all sited packages.</summary>
    private static readonly Dictionary<Type, PackageBase> _PackageInstances = 
      new Dictionary<Type, PackageBase>();

    /// <summary>Dictionary for service instances.</summary>
    private Dictionary<Type, object> _Services;

    /// <summary>Automatically registered services.</summary>
    private readonly Dictionary<Type, Type> _AutoRegisteredServices = 
      new Dictionary<Type, Type>();

    /// <summary>Dictionary for tool window instances.</summary>
    private Dictionary<Type, Dictionary<int, IToolWindowPaneBehavior>> _ToolWindows =
      new Dictionary<Type, Dictionary<int, IToolWindowPaneBehavior>>();

    /// <summary>Container for tool windows implementing IComponent</summary>
    private Container _ComponentToolWindows;

    /// <summary>List of option keys used by this package</summary>
    private List<string> _OptionKeys;

    /// <summary>Container of oages and profiles used by this package</summary>
    private Container _PagesAndProfiles;

    /// <summary>
    /// Object responsible to translate command methods to OleMenuCommand instances
    /// </summary>
    private CommandDispatcher<PackageBase> _CommandDispatcher;

    /// <summary>
    /// Gets the editor factories registered with this package
    /// </summary>
    private Dictionary<Type, EditorFactoryInfo> _EditorFactories =
      new Dictionary<Type, EditorFactoryInfo>();

    private Hashtable _ProjectFactories;

    #endregion

    #region Private enums

    private enum ProfileManagerLoadAction
    {
      None,
      LoadPropsFromRegistry,
      ResetSettings
    }

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Instantiates the package.
    /// </summary>
    /// <remarks>
    /// Prepares creation of service instances for IMenuCommandService and IOleCommandService.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected PackageBase()
    {
      _Zombied = false;
      ServiceCreatorCallback callback = OnCreateService;
      this.AddService<IMenuCommandService>(callback);
      this.AddService<IOleCommandTarget>(callback);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Disposes the resources held by this package.
    /// </summary>
    /// <param name="disposing"></param>
    /// <remarks>
    /// This method will be called by Visual Studio in reponse to a package close (disposing will 
    /// be true in this case). The default implementation revokes all services and calls Dispose() 
    /// on any created services that implement IDisposable.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing) return;

      DestroyEditorFactories();
      DestroyPages();
      DestroyComponentToolWindows();
      DestroyServices();
      DestroyServiceProvider();

      // --- Release references to containers
      if (_ToolWindows != null) _ToolWindows = null;
      if (_OptionKeys != null) _OptionKeys = null;

      // --- Disconnect user preference change events
      SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Destroys all editor factories used by this package.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void DestroyEditorFactories()
    {
        var editorFactories = _EditorFactories;
        _EditorFactories = null;
        try
        {
          var registerEditors = GetService<SVsRegisterEditors, IVsRegisterEditors>();
          foreach (var efInfo in editorFactories.Values)
          {
            try
            {
              if (registerEditors != null)
                registerEditors.UnregisterEditor(efInfo.Cookie);
            }
            catch (COMException) { /* do nothing */ }
            finally
            {
              var disposable = efInfo.Factory as IDisposable;
              if (disposable != null)
              {
                disposable.Dispose();
              }
            }
          }
        }
        catch (Exception e)
        {
          VsDebug.Fail(String.Format("Failed to dispose editor factories for package {0}\n{1}", 
            GetType().FullName, e.Message));
        }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Destroys all pages used by this package.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void DestroyPages()
    {
      if (_PagesAndProfiles != null)
      {
        var pagesAndProfiles = _PagesAndProfiles;
        _PagesAndProfiles = null;
        try
        {
          pagesAndProfiles.Dispose();
        }
        catch (Exception e)
        {
          VsDebug.Fail(String.Format("Failed to dispose component toolwindows for package {0}\n{1}", 
                                     GetType().FullName, e.Message));
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Destroys all the IComponent derived tool windows.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void DestroyComponentToolWindows()
    {
      if (_ComponentToolWindows != null)
      {
        Container componentToolWindows = _ComponentToolWindows;
        _ComponentToolWindows = null;
        try
        {
          componentToolWindows.Dispose();
        }
        catch (Exception e)
        {
          VsDebug.Fail(String.Format("Failed to dispose component toolwindows for package {0}\n{1}", 
                                     GetType().FullName, e.Message));
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enumerate the service list and destroy all services. This should always be done last.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void DestroyServices()
    {
      if (_Services != null)
      {
        try
        {
          var ps = GetService<SProfferService, IProfferService>();
          var services = _Services;
          _Services = null;

          foreach (var value in services.Values)
          {
            var service = value;
            var proffer = service as ProfferedService;
            try
            {
              if (null != proffer)
              {
                service = proffer.Instance;
                if (proffer.Cookie != 0 && ps != null)
                {
                  int hr = ps.RevokeService(proffer.Cookie);
                  if (NativeMethods.Failed(hr))
                  {
                    VsDebug.Fail(String.Format(CultureInfo.CurrentUICulture, 
                                               "Failed to unregister service {0}", service.GetType().FullName));
                  }
                }
              }
            }
            finally
            {
              if (service is IDisposable) ((IDisposable)service).Dispose();
            }
          }
        }
        catch (Exception e)
        {
          VsDebug.Fail(String.Format("Failed to dispose proffered service for package {0}\n{1}", 
                                     GetType().FullName, e.Message));
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Disallow any service requests after calling this method by disposing the service 
    /// provider object.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void DestroyServiceProvider()
    {
      if (_ServiceProvider != null)
      {
        // --- If the service provider for the current package is the service provider that we 
        // --- use globally do not dispose yet. Once all packages deriving from PackageBase 
        // --- are disposed we will dispose it.
        if (_ServiceProvider != _GlobalServiceProvider)
        {
          try
          {
            _ServiceProvider.Dispose();
          }
          catch (Exception e)
          {
            VsDebug.Fail(String.Format("Failed to dispose the global service provider for package {0}\n{1}",
                                       GetType().FullName, e.Message));
          }
        }
        _ServiceProvider = null;
      }
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>Gets the root key for the Visual Studio user settings.</summary>
    /// <remarks>
    /// This property returns the registry root for the current user. Typically this is 
    /// HKCU\Software\Microsoft\VisualStudio\[ver] but this can change based on any alternate 
    /// root that the shell is initialized with. This key is read-write.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public RegistryKey UserRegistryRoot
    {
      get
      {
        return VSRegistry.RegistryRoot(_ServiceProvider, 
          __VsLocalRegistryType.RegType_UserSettings, true);
      }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <returns>
    /// A service object of type serviceType or null if there is no service object of 
    /// type serviceType.
    /// </returns>
    /// <param name="serviceType">
    /// An object that specifies the type of service object to get.
    /// </param>
    // --------------------------------------------------------------------------------------------
    protected object GetService(Type serviceType)
    {
      if (_Zombied) return null;
      if (serviceType == null) throw new ArgumentNullException("serviceType");

      // --- Step 1: We return this instance for special service requests
      if (serviceType == typeof(IServiceContainer) || serviceType == typeof(PackageBase) || 
        serviceType == GetType())
      {
        return this;
      }

      // --- Check our service list
      object value = null;
      if (_Services != null)
      {
        lock (serviceType)
        {
          _Services.TryGetValue(serviceType, out value);
          var psValue = value as ProfferedService;
          if (psValue != null) value = psValue.Instance;

          var scValue = value as ServiceCreatorCallback;
          if (scValue != null)
          {
            // --- In case someone recursively requests the same service, null out the service 
            // --- type here.  That way they'll just fail instead of stack fault.
            _Services[serviceType] = null;
            value = scValue(this, serviceType);

            // --- Check if the requested service either implements serviceType or derives from
            // --- VsxService<,servieType>
            if (value != null && !value.GetType().IsCOMObject && 
              !serviceType.IsAssignableFrom(value.GetType()) &&
              value.GetType().GenericParameterOfType(typeof(VsxService<,>), 1) != serviceType)
            {
              // --- Callback passed us a bad service. NULL it, rather than throwing an exception.
              // --- Callers here do not need to be prepared to handle bad callback implemetations.
              VsDebug.Fail("Object " + value.GetType().Name + " was returned from a service creator callback but it does not implement the registered type of " + serviceType.Name);
              value = null;
            }
            _Services[serviceType] = value;
          }
        }
      }

      // --- Delegate to the parent provider, but only if we have verified that _Services doesn't 
      // --- actually contain our key if it does, that means that we're in the middle of trying to 
      // --- resolve this service, and the service resolution has recursed.
      VsDebug.Assert(value != null || _Services == null || !_Services.ContainsKey(serviceType),
        "GetService is recursing on itself while trying to resolve the service " + serviceType.Name + ". This means that someone is asking for this service while the service is trying to create itself.  Breaking the recursion now and aborting this GetService call.");
      if (value == null && _ServiceProvider != null && (_Services == null || 
        !_Services.ContainsKey(serviceType)))
      {
        value = _ServiceProvider.GetService(serviceType);
      }
      return value;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service described by the <typeparamref name="TService"/>
    /// type parameter.
    /// </summary>
    /// <returns>
    /// The service instance requested by the <typeparamref name="TService"/> parameter if found; otherwise null.
    /// </returns>
    /// <typeparam name="TService">The type of the service requested.</typeparam>
    // --------------------------------------------------------------------------------------------
    public TService GetService<TService>()
    {
      return (TService)GetService(typeof(TService));
    }

    // --------------------------------------------------------------------------------------------
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
    // --------------------------------------------------------------------------------------------
    public TInterface GetService<SInterface, TInterface>()
    {
      return (TInterface)GetService(typeof(SInterface));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a service proffered globally by Visual Studio or one of its packages.This is the 
    /// same as calling GetService() on an instance of a package that proffers no services itself. 
    /// </summary>
    /// <param name="serviceType">
    /// Type corresponding to the Service being requested.
    /// </param>
    /// <returns>The service being requested if available, otherwise null</returns>
    // --------------------------------------------------------------------------------------------
    static public object GetGlobalService(Type serviceType)
    {
      object service = null;
      VsDebug.Assert(_GlobalServiceProvider != null, 
        "You are calling GetGlobalService before any package derived from the managed package framework has been sited. This is not supported");

      if (_GlobalServiceProvider != null)
      {
        service = _GlobalServiceProvider.GetService(serviceType);
      }
      return service;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the global service described by the <typeparamref name="TService"/>
    /// type parameter.
    /// </summary>
    /// <returns>
    /// The service instance requested by the <typeparamref name="TService"/> parameter if found; 
    /// otherwise null.
    /// </returns>
    /// <typeparam name="TService">The type of the service requested.</typeparam>
    // --------------------------------------------------------------------------------------------
    public static TService GetGlobalService<TService>()
    {
      return (TService)GetGlobalService(typeof(TService));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the global service described by the <typeparamref name="SInterface"/> type parameter 
    /// and retrieves it as an interface type specified by the <typeparamref name="TInterface"/> 
    /// type parameter.
    /// </summary>
    /// <returns>
    /// The service instance requested by the <see cref="SInterface"/> parameter if found; 
    /// otherwise null.
    /// </returns>
    /// <typeparam name="SInterface">The type of the service requested.</typeparam>
    /// <typeparam name="TInterface">
    /// The type of interface retrieved. The object providing <see cref="SInterface"/> must 
    /// implement <see cref="TInterface"/>.
    /// </typeparam>
    // --------------------------------------------------------------------------------------------
    public static TInterface GetGlobalService<SInterface, TInterface>()
    {
      return (TInterface)GetGlobalService(typeof(SInterface));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the dialog page with the specified type.
    /// </summary>
    /// <returns>
    /// The dialog page of the specified type, if the package has that page; otherwise, null.
    /// </returns>
    /// <remarks>
    /// Dialog pages are cached so they can keep a single instance of their state. This method 
    /// allows a deriving class to get a cached dialog page. The object will be dynamically 
    /// created if it is not in the cache.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected IDialogPageBehavior GetDialogPage(Type dialogPageType)
    {
      // --- Check the current package state and input parameters
      if (_Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);
      if (dialogPageType == null)
        throw new ArgumentNullException("dialogPageType");
      if (!typeof(IDialogPageBehavior).IsAssignableFrom(dialogPageType))
        throw new ArgumentException(string.Format(Resources.Culture, Resources.Package_BadDialogPageType, dialogPageType.FullName));

      // --- Check, if the specified options page has already been created.
      if (_PagesAndProfiles != null)
      {
        foreach (var page in _PagesAndProfiles.Components)
        {
          if (page.GetType() == dialogPageType)
          {
            return (IDialogPageBehavior)page;
          }
        }
      }

      // --- This type of options page has not been created yet, so it's time to do it.
      var ctor = dialogPageType.GetConstructor(new Type[] { });
      if (ctor == null)
        throw new ArgumentException(string.Format(Resources.Culture, Resources.Package_PageCtorMissing, dialogPageType.FullName));
      var p = ctor.Invoke(new object[] { }) as IDialogPageBehavior;

      if (_PagesAndProfiles == null)
      {
        _PagesAndProfiles = new PackageContainer(this);
      }
      _PagesAndProfiles.Add(p);

      return p;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the dialog page with the specified type.
    /// </summary>
    /// <typeparam name="TPage">Type of dialog page to request from this package.</typeparam>
    /// <returns>
    /// The dialog page of the specified type, if the package has that page; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public TPage GetDialogPage<TPage>()
      where TPage : class, IDialogPageBehavior
    {
      return GetDialogPage(typeof (TPage)) as TPage;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the instance of the specified sited package.
    /// </summary>
    /// <typeparam name="TPackage">Type ofpackage instance to obtain.</typeparam>
    /// <returns>
    /// Package instance, if the specified package has already a sited instance; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static TPackage GetPackageInstance<TPackage>()
      where TPackage: PackageBase
    {
      PackageBase package;
      _PackageInstances.TryGetValue(typeof (TPackage), out package);
      return package as TPackage;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the list of VSXtra packages already sited
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static List<PackageBase> SitedVSXtraPackages
    {
      get { return new List<PackageBase>(_PackageInstances.Values); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the command handler instances registered by the specified package type.
    /// </summary>
    /// <param name="type">Type of package to serach for registered command handlers.</param>
    /// <returns>An enumerable collection of registered command handler instances.</returns>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<MenuCommandHandler> GetCommandHandlerInstances(Type type)
    {
      return MenuCommandHandler.GetRegisteredHandlerInstances(type); 
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the command handler instances registered by the specified package type.
    /// </summary>
    /// <typeparam name="TPackage">Type of package to serach for registered command handlers.</typeparam>
    /// <returns>An enumerable collection of registered command handler instances.</returns>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<MenuCommandHandler> GetCommandHandlerInstances<TPackage>()
      where TPackage: PackageBase
    {
      return MenuCommandHandler.GetRegisteredHandlerInstances<TPackage>();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Shows the tool window of the specified type having the given instance ID.
    /// </summary>
    /// <typeparam name="TWindow">Type of tool window to show up.</typeparam>
    /// <param name="instanceID">ID of the tool window toshow up.</param>
    /// <remarks>
    /// If the tool window instance does not exists, this method first creates the instance.
    /// Use this method when you want to manage multiple instances of the same tool window type.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public void ShowToolWindow<TWindow>(int instanceID)
      where TWindow: class, IToolWindowPaneBehavior
    {
      var window = FindToolWindow(typeof(TWindow), instanceID, true);
      if ((null == window) || (null == window.Frame))
      {
        throw new NotSupportedException(Resources.Package_CannotCreateToolWindow);
      }
      var windowFrame = (IVsWindowFrame)window.Frame;
      if (windowFrame != null)
      {
        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
      }
      else
      {
        VsDebug.Fail("Windowframe cannot be obtained.");
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Shows the tool window of the specified type.
    /// </summary>
    /// <typeparam name="TWindow">Type of tool window to show up.</typeparam>
    /// <remarks>
    /// If the tool window instance does not exists, this method first creates the instance. Use 
    /// this method when you want to manage a single instance of the specified tool window type.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public void ShowToolWindow<TWindow>()
      where TWindow : class, IToolWindowPaneBehavior
    {
      ShowToolWindow<TWindow>(0);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Shows the tool window of the specified type having the given instance ID.
    /// </summary>
    /// <param name="type">Type of tool window to create.</param>
    /// <param name="instanceId">ID of the tool window to show up.</param>
    /// <remarks>
    /// If the tool window instance does not exists, this method first creates the instance.
    /// Use this method when you want to manage multiple instances of the same tool window type.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public void ShowToolWindow(Type type, int instanceId)
    {
      var window = FindToolWindow(type, instanceId, true);
      if ((null == window) || (null == window.Frame))
      {
        throw new NotSupportedException(Resources.Package_CannotCreateToolWindow);
      }
      var windowFrame = (IVsWindowFrame)window.Frame;
      if (windowFrame != null)
      {
        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
      }
      else
      {
        VsDebug.Fail("Windowframe cannot be obtained.");
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a tool window of the specified type with the specified ID.
    /// </summary>
    /// <param name="toolWindowType">Type of tool window to create.</param>
    /// <param name="id">Instance ID</param>
    /// <returns>An instance of a class derived from ToolWindowPane</returns>
    // --------------------------------------------------------------------------------------------
    protected IToolWindowPaneBehavior CreateToolWindow(Type toolWindowType, int id)
    {
      if (id < 0)
        throw new ArgumentOutOfRangeException(
          string.Format(Resources.Culture, Resources.Package_InvalidInstanceID, id));

      if (!typeof(IToolWindowPaneBehavior).IsAssignableFrom(toolWindowType))
        throw new ArgumentException(Resources.Package_InvalidToolWindowClass);

      // ---Look in the Attributes of this package and see if this package
      // --- support this type of ToolWindow
      foreach (var tool in GetType().AttributesOfType<XtraProvideToolWindowAttribute>())
      {
        if (tool.ToolType == toolWindowType)
        {
          // --- We found the corresponding attribute on the package,
          // --- so create the toolwindow
          return CreateToolWindow(toolWindowType, id, tool);
        }
      }
      return null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This is the only method that should be calling IVsUiShell.CreateToolWindow()
    /// </summary>
    /// <param name="toolWindowType">Tool window type to create</param>
    /// <param name="id">Instance ID</param>
    /// <param name="tool">Attribute used to create the tool window</param>
    /// <returns>An instance of a class derived from ToolWindowPane</returns>
    // --------------------------------------------------------------------------------------------
    private IToolWindowPaneBehavior CreateToolWindow(Type toolWindowType, int id,
      XtraProvideToolWindowAttribute tool)
    {
      if (id < 0)
        throw new ArgumentOutOfRangeException(
          string.Format(Resources.Culture, Resources.Package_InvalidInstanceID, id));

      if (tool == null)
        throw new ArgumentNullException("tool");

      if (!typeof (IToolWindowPaneBehavior).IsAssignableFrom(toolWindowType))
        throw new ArgumentException(Resources.Package_InvalidToolWindowClass);

      // --- First create an instance of the ToolWindowPane
      var window = (IToolWindowPaneBehavior) Activator.CreateInstance(toolWindowType);
      window.SetInstanceId(id);

      // --- Check if this window has a ToolBar
      bool hasToolBar = (window.ToolBar != null);

      var flags = (uint) __VSCREATETOOLWIN.CTW_fInitNew;
      if (!tool.Transient)
        flags |= (uint) __VSCREATETOOLWIN.CTW_fForceCreate;
      if (hasToolBar)
        flags |= (uint) __VSCREATETOOLWIN.CTW_fToolbarHost;
      if (tool.MultiInstances)
        flags |= (uint) __VSCREATETOOLWIN.CTW_fMultiInstance;
      Guid emptyGuid = Guid.Empty;
      Guid toolClsid = window.ToolClsid;
      IVsWindowPane windowPane = null;
      if (toolClsid.CompareTo(Guid.Empty) == 0)
      {
        // --- If a tool CLSID is not specified, then host the IVsWindowPane
        windowPane = window;
      }
      Guid persistenceGuid = toolWindowType.GUID;
      IVsWindowFrame windowFrame;
      // Use IVsUIShell to create frame.
      var vsUiShell = GetService<SVsUIShell, IVsUIShell>();
      if (vsUiShell == null)
        throw new Exception(string.Format(Resources.Culture, Resources.General_MissingService,
                                          typeof (SVsUIShell).FullName));

      int hr = vsUiShell.CreateToolWindow(flags, // flags
                                          (uint) id, // instance ID
                                          windowPane,
                                          // IVsWindowPane to host in the toolwindow (null if toolClsid is specified)
                                          ref toolClsid,
                                          // toolClsid to host in the toolwindow (Guid.Empty if windowPane is not null)
                                          ref persistenceGuid, // persistence Guid
                                          ref emptyGuid, // auto activate Guid
                                          null, // service provider
                                          window.Caption, // Window title
                                          null,
                                          out windowFrame);
      NativeMethods.ThrowOnFailure(hr);

      // --- If the toolwindow is a component, site it.
      IComponent component = null;
      if (window.Window is IComponent)
        component = (IComponent) window.Window;
      else if (windowPane is IComponent)
        component = (IComponent) windowPane;
      if (component != null)
      {
        if (_ComponentToolWindows == null)
          _ComponentToolWindows = new PackageContainer(this);
        _ComponentToolWindows.Add(component);
      }

      // --- This generates the OnToolWindowCreated event on the ToolWindowPane
      window.Frame = new WindowFrame(windowFrame);

      if (hasToolBar && windowFrame != null)
      {
        // --- Set the toolbar
        object obj;
        NativeMethods.ThrowOnFailure(windowFrame.GetProperty(
                                       (int) __VSFPROPID.VSFPROPID_ToolbarHost, out obj));
        var toolBarHost = obj as IVsToolWindowToolbarHost;
        if (toolBarHost != null)
        {
          Guid toolBarCommandSet = window.ToolBar.Guid;
          NativeMethods.ThrowOnFailure(
            toolBarHost.AddToolbar((VSTWT_LOCATION) window.ToolBarLocation, ref toolBarCommandSet,
                                   (uint) window.ToolBar.ID));
        }
      }

      window.OnToolBarAdded();

      // --- If the ToolWindow was created successfully, keep track of it
      VsDebug.Assert(window != null, "At this point window assumed to be non-null.");
      Dictionary<int, IToolWindowPaneBehavior> toolInstances;
      if (!_ToolWindows.TryGetValue(toolWindowType, out toolInstances))
      {
        toolInstances = new Dictionary<int, IToolWindowPaneBehavior>();
        _ToolWindows.Add(toolWindowType, toolInstances);
      }
      VsDebug.Assert(!toolInstances.ContainsKey(id),
                     "An existing tool window instance has been recreated.");
      toolInstances.Add(id, window);
      return window;
    }


    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Return the tool window corresponding to the specified type and ID. If it does not exist, 
    /// it creates one if create is true, or returns null if create is false.
    /// </summary>
    /// <typeparam name="TWindow">Type of tool window to find.</typeparam>
    /// <param name="id">Instance ID</param>
    /// <param name="create">Create if none exist?</param>
    /// <returns>An instance of the tool window</returns>
    // --------------------------------------------------------------------------------------------
    public TWindow FindToolWindow<TWindow>(int id, bool create)
      where TWindow: class, IToolWindowPaneBehavior
    {
      var window = FindToolWindow(typeof(TWindow), id, create, null);
      var toolWindow = window as TWindow;
      VsDebug.Assert(toolWindow != null, "Tool window is not the expected type.");
      return toolWindow;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Return the tool window corresponding to the specified type and ID. If it does not exist, 
    /// it creates one if create is true, or returns null if create is false.
    /// </summary>
    /// <param name="toolWindowType">Type of tool window to create.</param>
    /// <param name="id">Instance ID</param>
    /// <param name="create">Create if none exist?</param>
    /// <returns>An instance of the tool window</returns>
    // --------------------------------------------------------------------------------------------
    public IToolWindowPaneBehavior FindToolWindow(Type toolWindowType, int id, bool create)
    {
      return FindToolWindow(toolWindowType, id, create, null);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Return the tool window corresponding to the specified type and ID. If it does not exist, 
    /// it creates one if create is true, or returns null if create is false.
    /// </summary>
    /// <param name="toolWindowType">Type of tool window to create.</param>
    /// <param name="id">Instance ID</param>
    /// <param name="create">Create if none exist?</param>
    /// <param name="tool">Optional attribute used for initialization.</param>
    /// <returns>An instance of the tool window</returns>
    // --------------------------------------------------------------------------------------------
    private IToolWindowPaneBehavior FindToolWindow(Type toolWindowType, int id, bool create, XtraProvideToolWindowAttribute tool)
    {
      // --- Check, if we have already created the specified tool window instance
      Dictionary<int, IToolWindowPaneBehavior> toolInstances;
      IToolWindowPaneBehavior window = null;
      if (!_ToolWindows.TryGetValue(toolWindowType, out toolInstances) ||
        !toolInstances.TryGetValue(id, out window))
      {
        if (create)
        {
          window = tool != null
                     ? CreateToolWindow(toolWindowType, id, tool)
                     : CreateToolWindow(toolWindowType, id);
        }
      }
      return window;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method adds a user option key name into the list of option keys that we will load and 
    /// save from the solution file. You should call this early in your constructor. Calling this 
    /// will cause the OnLoadOptions and OnSaveOptions methods to be invoked for each key you add.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected void AddOptionKey(string name)
    {
      if (_Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      if (name == null)
        throw new ArgumentNullException("name");

      // --- The key is the class name of the service interface. While it would be a 
      // --- lot more correct to use the fully-qualified class name, IStorage won't have it and 
      // --- returns STG_E_INVALIDNAME. The doc's don't have any information here; I can only 
      // --- assume it is because of the '.'.
      // --- [clovett]: According to the docs for IStorage::CreateStream, the name cannot be 
      // --- longer than 31 characters.
      if (name.IndexOf('.') != -1 || name.Length > 31)
        throw new ArgumentException(string.Format(Resources.Culture, Resources.Package_BadOptionName, name));
      if (_OptionKeys == null)
      {
        _OptionKeys = new List<string>();
      }
      else
      {
        if (_OptionKeys.Contains(name))
        {
          throw new ArgumentException(string.Format(Resources.Culture, Resources.Package_OptionNameUsed, name));
        }
      }
      _OptionKeys.Add(name);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Return the locale associated with this IServiceProvider.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int GetProviderLocale()
    {
      int lcid = CultureInfo.CurrentCulture.LCID;
      var loc = GetService<IUIHostLocale>();
      VsDebug.Assert(loc != null, "Unable to get IUIHostLocale, defaulting CLR designer to current thread LCID");
      if (loc != null)
      {
        uint locale;
        NativeMethods.ThrowOnFailure(loc.GetUILocale(out locale));
        lcid = (int)locale;
      }
      return lcid;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Registers an editor factory with this package.
    /// </summary>
    /// <remarks>
    /// Registers this editor factory with Visual Studio. There is no need to register or unregister
    /// an editor factory as PackageBase will handle this for you. Also, if your editor factory is 
    /// IDisposable, it will be disposed when it is unregistered.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected void RegisterEditorFactory(IVsEditorFactory factory)
    {
      var registerEditors = GetService<SVsRegisterEditors, IVsRegisterEditors>();
      if (registerEditors == null)
      {
        throw new InvalidOperationException(string.Format(Resources.Culture, Resources.Package_MissingService, 
          typeof(SVsRegisterEditors).FullName));
      }
      uint cookie;
      var riid = factory.GetType().GUID;
      NativeMethods.ThrowOnFailure(registerEditors.RegisterEditor(ref riid, factory, out cookie));

      var factoryType = factory.GetType();
      if (_EditorFactories.ContainsKey(factoryType)) return;
      var factoryInfo = new EditorFactoryInfo {Cookie = cookie, Factory = factory};
      _EditorFactories.Add(factoryType, factoryInfo);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Registers the specified project factory with the package.
    /// </summary>
    /// <devdoc>
    /// Registers this project factory with Visual Studio.
    /// If you are providing an project factory, you should register
    /// it by overriding the Initialize method. Call 
    /// base.Initialize first, and then call RegisterProjectFactory
    /// for each project factory.  There is no need to unregister
    /// an project factory as the Package base class will handle this for you.
    /// Also, if your project factory is IDisposable, it will be
    /// disposed when it is unregistered.
    /// </devdoc>
    // --------------------------------------------------------------------------------------------
    protected void RegisterProjectFactory(IVsProjectFactory factory)
    {
      // TODO: Refactor this method to use dictionaries (like RegisterEditorFactory)
      var registerProjects = GetService<SVsRegisterProjectTypes, IVsRegisterProjectTypes>();
      if (registerProjects == null)
      {
        throw new InvalidOperationException(string.Format(Resources.Culture, Resources.Package_MissingService, typeof(SVsRegisterProjectTypes).FullName));
      }
      uint cookie;
      var riid = factory.GetType().GUID;
      NativeMethods.ThrowOnFailure(registerProjects.RegisterProjectType(ref riid, factory, out cookie));
      if (_ProjectFactories == null)
      {
        _ProjectFactories = new Hashtable();
      }
      _ProjectFactories[factory] = cookie;
    }

    #endregion

    #region Protected methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method binds command handlers in the specified assembly to the corresponding commands.
    /// </summary>
    /// <param name="asm">Assembly to scan for command handlers.</param>
    /// <remarks>
    /// This method is automatically called for the package assembly. If you have command handlers 
    /// in other assemblies, you must manually call this method in the overridden Initialize method
    /// of your package class.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual void BindCommandHandlers(Assembly asm)
    {
      var handlerTypes =
        from commandGroup in asm.GetTypes()
        where
          // --- Derives from CommandGroup<TPackage> where TPackage is this type
          commandGroup.GenericParameterOfType(typeof (CommandGroup<>), 0) == GetType() &&
          !Attribute.IsDefined(commandGroup, typeof (ManualBindAttribute))
        from handler in commandGroup.GetNestedTypes()
        where typeof (MenuCommandHandler).IsAssignableFrom(handler.BaseType) &&
              !handler.IsAbstract &&
              !Attribute.IsDefined(handler, typeof (ManualBindAttribute))
        select handler;
      handlerTypes.ForEach(
        t =>
          {
            var handler = Activator.CreateInstance(t) as MenuCommandHandler;
            if (handler != null) handler.Bind();
          });
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method is executed when options are about to be read.
    /// </summary>
    /// <remarks>
    /// This method can be overridden by the deriving class to load solution options.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnLoadOptions(string key, Stream stream)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method is executed when options are about to be saved.
    /// </summary>
    /// <remarks>
    /// This method can be overridden by the deriving class to save solution options.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnSaveOptions(string key, Stream stream)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Responds to the event when user preferences has been changed.
    /// </summary>
    /// <remarks>
    /// Invoked when a user setting has changed. Here we invalidate the cached locale data so we 
    /// can obtain updated culture information.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
      if (e.Category == UserPreferenceCategory.Locale)
      {
        CultureInfo.CurrentCulture.ClearCachedData();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Scans the types of the specified assembly and binds appropriate service types to this 
    /// package.
    /// </summary>
    /// <param name="assembly">Assembly to scan for service types.</param>
    // --------------------------------------------------------------------------------------------
    protected void BindServiceTypes(Assembly assembly)
    {
      var serviceTypes =
        from type in assembly.GetTypes()
        where type.ImplementsGenericType(typeof(IVsxService<,>)) &&
          !type.IsAbstract &&
          !Attribute.IsDefined(type, typeof(ManualBindAttribute))
        select type;
      serviceTypes.ForEach(
        t =>
          {
            var closedType =
              t.GetImplementorOfGenericInterface(typeof (IVsxService<,>));
            if (closedType.GetGenericArguments()[0] != GetType()) return;

            // --- This service belongs to this package, we register it
            var serviceType = closedType.GetGenericArguments()[1];
            if (_AutoRegisteredServices.ContainsKey(serviceType)) return;

            // --- This is the first service for this type to register
            _AutoRegisteredServices.Add(serviceType, t);
            var container = this as IServiceContainer;
            bool promote = Attribute.IsDefined(t, typeof (PromoteAttribute));
            if (Attribute.IsDefined(t, typeof (AutoCreateServiceAttribute)))
            {
              container.AddService(serviceType, Activator.CreateInstance(t), promote);
            }
            else
            {
              container.AddService(serviceType, CreateServiceInstance, promote);
            }
          }
        );
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to bind services manually.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void RegisterServices()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Scans the types of the specified assembly and binds appropriate service types to this 
    /// package.
    /// </summary>
    /// <param name="assembly">Assembly to scan for service types.</param>
    // --------------------------------------------------------------------------------------------
    protected virtual void BindEditorFactoryTypes(Assembly assembly)
    {
      var serviceTypes =
        from type in assembly.GetTypes()
        where type.DerivesFromGenericType(typeof(EditorFactoryBase<,>)) &&
          type.GenericParameterOfType(typeof (EditorFactoryBase<,>), 0) == GetType() &&
          !type.IsAbstract &&
          !Attribute.IsDefined(type, typeof(ManualBindAttribute))
        select type;
      serviceTypes.ForEach(
        t =>
        {
          var editorFactory = Activator.CreateInstance(t) as IVsEditorFactory;
          RegisterEditorFactory(editorFactory);
        }
        );
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to bind services manually.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void RegisterEditorFactories()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method is used for automatically registered service types as ServiceCreationCallback 
    /// method.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private object CreateServiceInstance(IServiceContainer container, Type serviceType)
    {
      // --- We accept only this package's container
      if (container != this) return null;
      Type implType;
      return _AutoRegisteredServices.TryGetValue(serviceType, out implType) 
        ? Activator.CreateInstance(implType) 
        : null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Proffers the marked services of this package
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void ProfferServices()
    {
      // --- If we have services to proffer, do that now.
      if (_Services != null)
      {
        var ps = GetService<SProfferService, IProfferService>();
        VsDebug.Assert(ps != null,
                       "We have services to proffer but IProfferService is not available.");
        if (ps != null)
        {
          foreach (var de in _Services)
          {
            var service = de.Value as ProfferedService;
            if (service != null)
            {
              var serviceType = de.Key;
              uint cookie;
              var serviceGuid = serviceType.GUID;
              NativeMethods.ThrowOnFailure(
                ps.ProfferService(ref serviceGuid, this, out cookie)
                );
              service.Cookie = cookie;
            }
          }
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Loads package options
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void LoadPackageOptions()
    {
      // --- Be sure to load the package user options from the solution in case
      // --- the package was not already loaded when the solution was opened.
      if (null != _OptionKeys)
      {
        try
        {
          var pPersistance = GetService<SVsSolutionPersistence, IVsSolutionPersistence>();
          if (pPersistance != null)
          {
            foreach (string key in _OptionKeys)
            {
              // --- Don't check for the error code because a failure here is
              // --- expected and not a problem.
              pPersistance.LoadPackageUserOpts(this, key);
            }
          }
        }
        catch (SystemException)
        {
          // --- no settings found, no problem.
        }
      }
    }

    #endregion

    #region IVsPackage Members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a VSPackage with a service provider to the environment.
    /// </summary>
    /// <param name="serviceProvider">
    /// Reference to the IServiceProvider interface through which the VSPackage can query for 
    /// services.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.SetSite(IOleServiceProvider serviceProvider)
    {
      // --- Zombied packages cannot be sited
      if (_Zombied) Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);
      
      // --- Site the package if a service provider is used
      if (serviceProvider != null)
      {
        // --- Prevent siting the package again
        if (_ServiceProvider != null)
        {
          throw new InvalidOperationException(String.Format(Resources.Package_SiteAlreadySet,
            GetType().Name));
        }
        
        // --- Create the service provider and administer siting
        _ServiceProvider = new ServiceProvider(serviceProvider);
        if (!_PackageInstances.ContainsKey(GetType()))
        {
          _PackageInstances.Add(GetType(), this);
        }
        // --- Initialize the global service provider
        if (_GlobalServiceProvider == null)
          _GlobalServiceProvider = _ServiceProvider;
        
        // --- Increment the number of package that have been sited. This allows us to know 
        // --- when to let go of our _GlobalServiceProvider.
        ++_SitedPackageCount;

        // --- Now the package is sited, it's time to initialize it.
        InternalInitialize();
        Initialize();
      }
      // --- Unsite the package if null service provider is used
      else if (_ServiceProvider != null)
      {
        // --- Dispose this package.
        _PackageInstances.Remove(GetType());
        Dispose(true);

        // --- Decrement the number of package that have been sited and dispose of our 
        // --- _GlobalServiceProvider once all packages have been unsited.
        if (--_SitedPackageCount <= 0 && _GlobalServiceProvider != null)
        {
          VsDebug.Assert(_SitedPackageCount == 0, 
            "We should not have unsited more package then we sited");
          _GlobalServiceProvider.Dispose();
          _GlobalServiceProvider = null;
        }
      }
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes internal initialization steps.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void InternalInitialize()
    {
      // --- First service types has to be registered
      var thisAssembly = GetType().Assembly;
      BindServiceTypes(thisAssembly);
      RegisterServices();
      ProfferServices();

      // --- Initialize this thread's culture info with that of the shell's LCID
      int locale = GetProviderLocale();
      Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);

      // --- Begin listening to user preference change events
      SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;

      LoadPackageOptions();

      // --- Set up command handler methods
      _CommandDispatcher = new CommandDispatcher<PackageBase>(this, this);
      var parentService = _GlobalServiceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
      var localService = GetService<IMenuCommandService, OleMenuCommandService>();
      _CommandDispatcher.RegisterCommandHandlers(localService, parentService);

      // --- Set up editor factories
      BindEditorFactoryTypes(thisAssembly);
      RegisterEditorFactories();

      // --- Set up command handler classes
      BindCommandHandlers(GetType().Assembly);
      Console.SetOut(OutputWindow.General);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enables a VSPackage that requires user intervention to abort the shutdown process.
    /// </summary>
    /// <param name="close">
    /// Flag indicating whether the VSPackage can close. Is set to true if the VSPackage can close.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    /// <remarks>
    /// Inheritors must override the QueryClose method to respond to this event.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.QueryClose(out int close)
    {
      bool canClose;
      var hr = QueryClose(out canClose);
      close = canClose ? 1 : 0;
      return hr;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Closes the VSPackage, releases cached interface pointers, and unadvises event sinks.
    /// </summary>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    /// <remarks>
    /// This method will be called by Visual Studio in reponse to a package close (disposing will 
    /// be true in this case).  The default implementation revokes all services and calls 
    /// Dispose() on any created services that implement IDisposable.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.Close()
    {
      if (!_Zombied) Dispose(true);
      _Zombied = true;
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enables a VSPackage to participate in the DTE automation object model.
    /// </summary>
    /// <param name="propName">String containing the prop name used by this package</param>
    /// <param name="auto">Object pointing to an IDispatch interface</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    /// <remarks>
    /// Inheritors must override the GetAutomationObject method to create a reference for the
    /// automation object used.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.GetAutomationObject(string propName, out object auto)
    {
      if (_Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      auto = GetAutomationObject(propName);
      if (auto == null)
      {
        Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
      }
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enables the environment to create on-demand tool windows that are implemented by VSPackages.
    /// </summary>
    /// <param name="rguidPersistenceSlot">Unique identifier of the Tool window.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.CreateTool(ref Guid rguidPersistenceSlot)
    {
      if (_Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      // --- Let the factory do the work
      return ((IVsToolWindowFactory)this).CreateToolWindow(ref rguidPersistenceSlot, 0);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Resets Toolbox defaults.
    /// </summary>
    /// <param name="grfFlags">
    /// Flags whose values are taken from the __VSPKGRESETFLAGS enumeration.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.ResetDefaults(uint grfFlags)
    {
      if (_Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      // TODO: Implement Toolbox support
      //if (grfFlags == (uint)__VSPKGRESETFLAGS.PKGRF_TOOLBOXITEMS)
      //{
      //  if (ToolboxInitialized != null)
      //  {
      //    ToolboxInitialized(this, EventArgs.Empty);
      //  }
      //}
      //else if (grfFlags == (uint)__VSPKGRESETFLAGS.PKGRF_TOOLBOXSETUP)
      //{
      //  if (ToolboxUpgraded != null)
      //  {
      //    ToolboxUpgraded(this, EventArgs.Empty);
      //  }
      //}
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Proffers access to the Tools menu Options and the property pages of the Customize Toolbox 
    /// dialog boxes.
    /// </summary>
    /// <param name="rguidPage">Unique identifier of the requested property page.</param>
    /// <param name="ppage">
    /// Pointer to the property page whose values are taken from the VSPROPSHEETPAGE structure.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.GetPropertyPage(ref Guid rguidPage, VSPROPSHEETPAGE[] ppage)
    {
      if (ppage == null || ppage.Length < 1)
        throw new ArgumentException(Resources.General_ArraySizeShouldBeAtLeast1, "ppage");

      if (_Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      IWin32Window pageWindow = null;

      // --- First, check out the active pages.
      if (_PagesAndProfiles != null)
      {
        foreach (var page in _PagesAndProfiles.Components)
        {
          if (page.GetType().GUID.Equals(rguidPage))
          {
            var w = page as IWin32Window;
            if (w != null)
            {
              if (w is IDialogPageBehavior)
              {
                ((IDialogPageBehavior)w).ResetContainer();
              }
              pageWindow = w;
              break;
            }
          }
        }
      }

      if (pageWindow == null)
      {
        IDialogPageBehavior page = null;

        // --- Didn't find it in our cache. Now look in the metadata attributes for the class. 
        // --- Look at all types at the same time.
        var attributes = TypeDescriptor.GetAttributes(this);
        foreach (Attribute attr in attributes)
        {
          var optionAttr = attr as XtraProvideOptionDialogPageAttribute;
          if (optionAttr != null)
          {
            var pageType = optionAttr.PageType;
            if (pageType.GUID.Equals(rguidPage))
            {
              // --- Found a matching attribute. Now go get the DialogPage with GetDialogPage.
              // --- This has a side-effect of storing the page in _PagesAndProfiles for us.
              //
              page = GetDialogPage(pageType);
              pageWindow = page;
              break;
            }
          }
          if (page != null)
          {
            if (_PagesAndProfiles == null)
            {
              _PagesAndProfiles = new PackageContainer(this);
            }
            _PagesAndProfiles.Add(page);
            break;
          }
        }
      }

      // --- We should now have a page window. If we don't then the requested page doesn't exist.
      if (pageWindow == null)
      {
        Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
      }

      ppage[0].dwSize = (uint)Marshal.SizeOf(typeof(VSPROPSHEETPAGE));
      ppage[0].hwndDlg = pageWindow.Handle;
      // --- Zero-out all the fields we aren't using.
      ppage[0].dwFlags = 0;
      ppage[0].HINSTANCE = 0;
      ppage[0].dwTemplateSize = 0;
      ppage[0].pTemplate = IntPtr.Zero;
      ppage[0].pfnDlgProc = IntPtr.Zero;
      ppage[0].lParam = IntPtr.Zero;
      ppage[0].pfnCallback = IntPtr.Zero;
      ppage[0].pcRefParent = IntPtr.Zero;
      ppage[0].dwReserved = 0;
      ppage[0].wTemplateId = 0;

      return NativeMethods.S_OK;
    }

    #endregion

    #region IOleServiceProvider Members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries for a service object instance.
    /// </summary>
    /// <param name="sid">Service object ID</param>
    /// <param name="iid">Service interface ID</param>
    /// <param name="ppvObj">Service object instance</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IOleServiceProvider.QueryService(ref Guid sid, ref Guid iid, out IntPtr ppvObj)
    {
      ppvObj = (IntPtr)0;
      int hr = NativeMethods.S_OK;

      // ---Let's check if we have the service by its GUID
      object service = null;
      if (_Services != null)
      {
        foreach (Type serviceType in _Services.Keys)
        {
          if (serviceType.GUID.Equals(sid))
          {
            service = GetService(serviceType);
            break;
          }
        }
      }

      if (service == null)
      {
        // --- We do not have the requested service
        hr = NativeMethods.E_NOINTERFACE;
      }
      else
      {
        // --- Now check to see if the user asked for an IID other than IUnknown.
        // --- If so, we must do another QI.
        if (iid.Equals(NativeMethods.IID_IUnknown))
        {
          ppvObj = Marshal.GetIUnknownForObject(service);
        }
        else
        {
          IntPtr pUnk = Marshal.GetIUnknownForObject(service);
          hr = Marshal.QueryInterface(pUnk, ref iid, out ppvObj);
          Marshal.Release(pUnk);
        }
      }
      return hr;
    }

    #endregion

    #region IOleCommandTarget Members

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
    // --------------------------------------------------------------------------------------------
    int IOleCommandTarget.Exec(ref Guid guidGroup, uint nCmdId, uint nCmdExcept, IntPtr pIn, 
      IntPtr vOut)
    {
      var target = GetService<IOleCommandTarget>();
      return target != null 
        ? target.Exec(ref guidGroup, nCmdId, nCmdExcept, pIn, vOut) 
        : NativeMethods.OLECMDERR_E_NOTSUPPORTED;
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
    // --------------------------------------------------------------------------------------------
    int IOleCommandTarget.QueryStatus(ref Guid guidGroup, uint nCmdId, OLECMD[] oleCmd, IntPtr oleText)
    {
      var target = GetService<IOleCommandTarget>();
      return target != null 
        ? target.QueryStatus(ref guidGroup, nCmdId, oleCmd, oleText) 
        : (NativeMethods.OLECMDERR_E_NOTSUPPORTED);
    }

    #endregion

    #region IServiceContainer Members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the specified service to the service container.
    /// </summary>
    /// <param name="serviceType">The type of service to add.</param>
    /// <param name="serviceInstance">
    /// An instance of the service type to add. This object must implement or inherit from the 
    /// type indicated by the serviceType parameter.
    /// </param>
    // --------------------------------------------------------------------------------------------
    void IServiceContainer.AddService(Type serviceType, object serviceInstance)
    {
      ((IServiceContainer)this).AddService(serviceType, serviceInstance, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the specified service to the service container, and optionally promotes the service 
    /// to any parent service containers.
    /// </summary>
    /// <param name="serviceType">The type of service to add.</param>
    /// <param name="serviceInstance">
    /// An instance of the service type to add. This object must implement or inherit from the 
    /// type indicated by the serviceType parameter.
    /// </param>
    /// <param name="promote">
    /// true to promote this request to any parent service containers; otherwise, false.
    /// </param>
    // --------------------------------------------------------------------------------------------
    void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
    {
      if (serviceType == null)
      {
        throw new ArgumentNullException("serviceType");
      }

      if (serviceInstance == null)
      {
        throw new ArgumentNullException("serviceInstance");
      }

      if (_Services == null)
      {
        _Services = new Dictionary<Type, object>();
      }

      // --- Disallow the addition of duplicate services.
      if (_Services.ContainsKey(serviceType))
      {
        throw new InvalidOperationException(String.Format(Resources.Package_DuplicateService, 
          serviceType.Name));
      }

      if (promote)
      {
        // --- If we're promoting, we need to store this guy in a promoted service object because 
        // --- we need to manage additional state.  We attempt to proffer at this time if we have 
        // --- a service provider.  If we don't, we will proffer when we get one.
        var service = new ProfferedService {Instance = serviceInstance};
        if (_ServiceProvider != null)
        {
          var ps = GetService<SProfferService, IProfferService>();
          if (ps != null)
          {
            uint cookie;
            Guid serviceGuid = serviceType.GUID;
            NativeMethods.ThrowOnFailure(ps.ProfferService(ref serviceGuid, this, out cookie));
            service.Cookie = cookie;
            _Services[serviceType] = service;
          }
        }
      }
      else
      {
        _Services[serviceType] = serviceInstance;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the specified service to the service container.
    /// </summary>
    /// <param name="serviceType">The type of service to add.</param>
    /// <param name="callback">
    /// A callback object that is used to create the service. This allows a service to be declared 
    /// as available, but delays the creation of the object until the service is requested.
    /// </param>
    // --------------------------------------------------------------------------------------------
    void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
    {
      ((IServiceContainer)this).AddService(serviceType, callback, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the specified service to the service container, and optionally promotes the service 
    /// to any parent service containers.
    /// </summary>
    /// <param name="serviceType">The type of service to add.</param>
    /// <param name="callback">
    /// A callback object that is used to create the service. This allows a service to be declared 
    /// as available, but delays the creation of the object until the service is requested.
    /// </param>
    /// <param name="promote">
    /// true to promote this request to any parent service containers; otherwise, false.
    /// </param>
    // --------------------------------------------------------------------------------------------
    void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
    {
      if (serviceType == null)
      {
        throw new ArgumentNullException("serviceType");
      }
      if (callback == null)
      {
        throw new ArgumentNullException("callback");
      }
      if (_Services == null)
      {
        _Services = new Dictionary<Type, object>();
      }
      // --- Disallow the addition of duplicate services.
      if (_Services.ContainsKey(serviceType))
      {
        throw new InvalidOperationException(String.Format(Resources.Package_DuplicateService,
          serviceType.Name));
      }
      if (promote)
      {
        // --- If we're promoting, we need to store this guy in a promoted service object because 
        // --- we need to manage additional state.  We attempt to proffer at this time if we have 
        // --- a service provider.  If we don't, we will proffer when we get one.
        var service = new ProfferedService();
        _Services[serviceType] = service;
        service.Instance = callback;

        if (_ServiceProvider != null)
        {
          var ps = GetService<SProfferService, IProfferService>();
          if (ps != null)
          {
            uint cookie;
            var serviceGuid = serviceType.GUID;
            NativeMethods.ThrowOnFailure(ps.ProfferService(ref serviceGuid, this, out cookie));
            service.Cookie = cookie;
          }
        }
      }
      else
      {
        _Services[serviceType] = callback;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes the specified service type from the service container.
    /// </summary>
    /// <param name="serviceType">The type of service to add.</param>
    // --------------------------------------------------------------------------------------------
    void IServiceContainer.RemoveService(Type serviceType)
    {
      ((IServiceContainer)this).RemoveService(serviceType, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes the specified service type from the service container, and optionally promotes 
    /// the service to parent service containers.
    /// </summary>
    /// <param name="serviceType">The type of service to add.</param>
    /// <param name="promote">
    /// true to promote this request to any parent service containers; otherwise, false.
    /// </param>
    // --------------------------------------------------------------------------------------------
    void IServiceContainer.RemoveService(Type serviceType, bool promote)
    {
      if (serviceType == null)
      {
        throw new ArgumentNullException("serviceType");
      }
      if (_Services != null)
      {
        object value;
        if (_Services.TryGetValue(serviceType, out value))
        {
          // --- Remove the service from our dictionary
          _Services.Remove(serviceType);
          try
          {
            // --- If the service is proffered, revoke it from the parent container.
            var service = value as ProfferedService;
            if (null != service)
            {
              value = service.Instance;
              if (service.Cookie != 0)
              {
                var ps = GetService<SProfferService, IProfferService>();
                if (ps != null)
                {
                  NativeMethods.ThrowOnFailure(ps.RevokeService(service.Cookie));
                }
                service.Cookie = 0;
              }
            }
          }
          finally
          {
            if (value is IDisposable)
            {
              ((IDisposable)value).Dispose();
            }
          }
        }
      }
    }

    #endregion

    #region IServiceProvider Members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <returns>
    /// A service object of type serviceType or null if there is no service object of 
    /// type serviceType.
    /// </returns>
    /// <param name="serviceType">
    /// An object that specifies the type of service object to get.
    /// </param>
    // --------------------------------------------------------------------------------------------
    object IServiceProvider.GetService(Type serviceType)
    {
      return GetService(serviceType);
    }

    #endregion

    #region IVsPersistSolutionOpts members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Loads user options for a given solution.
    /// </summary>
    /// <remarks>
    /// Called when a solution is opened, and allows us to inspect our options.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsPersistSolutionOpts.LoadUserOptions(IVsSolutionPersistence pPersistance, uint options)
    {
      int hr = NativeMethods.S_OK;
      if ((options & (uint) __VSLOADUSEROPTS.LUO_OPENEDDSW) != 0)
        return hr;

      if (_OptionKeys != null)
      {
        foreach (string key in _OptionKeys)
        {
          hr = pPersistance.LoadPackageUserOpts(this, key);
          if (NativeMethods.Failed(hr))
            break;
        }
      }

      // --- Shell relies on this being released when we're done with it. If you see strange
      // --- faults in the shell when saving the solution, suspect this!
      Marshal.ReleaseComObject(pPersistance);
      NativeMethods.ThrowOnFailure(hr);
      return hr;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Reads user options for a given solution.
    /// </summary>
    /// <remarks>
    /// Called by the shell to load our solution options.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsPersistSolutionOpts.ReadUserOptions(IStream pStream, string pszKey)
    {
      try
      {
        var stream = new NativeMethods.DataStreamFromComStream(pStream);
        using (stream)
        {
          OnLoadOptions(pszKey, stream);
        }
      }
      finally
      {
        // --- Release the pointer because VS expects it to be released upon function return.
        Marshal.ReleaseComObject(pStream);
      }
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Saves user options for a given solution.
    /// </summary>
    /// <remarks>
    /// Called by the shell when we are to persist our service options
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsPersistSolutionOpts.SaveUserOptions(IVsSolutionPersistence pPersistance)
    {
      if (_OptionKeys != null)
      {
        foreach (string key in _OptionKeys)
        {
          NativeMethods.ThrowOnFailure(pPersistance.SavePackageUserOpts(this, key));
        }
      }

      // --- Shell relies on this being released when we're done with it. If you see strange
      // --- faults in the shell when saving the solution, suspect this!
      Marshal.ReleaseComObject(pPersistance);
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Writes user options for a given solution.
    /// </summary>
    /// <remarks>
    /// Called by the shell to persist our solution options. Here is where the service can persist 
    /// any objects that it cares about.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsPersistSolutionOpts.WriteUserOptions(IStream pStream, string pszKey)
    {
      try
      {
        var stream = new NativeMethods.DataStreamFromComStream(pStream);
        using (stream)
        {
          OnSaveOptions(pszKey, stream);
        }
      }
      finally
      {
        // --- Release the pointer because VS expects it to be released upon function return.
        Marshal.ReleaseComObject(pStream);
      }
      return NativeMethods.S_OK;
    }

    #endregion

    #region IVsUserSettings members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Saves a VSPackage's configuration using the Visual Studio settings mechanism when the 
    /// export option of the Import/Export Settings feature available on the IDE’s Tools menu is 
    /// selected by a user.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    int IVsUserSettings.ExportSettings(string strPageGuid, IVsSettingsWriter writer)
    {
      VsDebug.Assert(!string.IsNullOrEmpty(strPageGuid), "Passed page guid cannot be null");
      VsDebug.Assert(writer != null, "IVsSettingsWriter cannot be null");

      var requestPageGuid = new Guid(strPageGuid ?? String.Empty);
      var profileManager = GetProfileManager(requestPageGuid, ProfileManagerLoadAction.LoadPropsFromRegistry);
      if (profileManager != null)
      {
        profileManager.SaveSettingsToXml(writer);
      }
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves a VSPackage's configuration using the Visual Studio settings mechanism when a 
    /// user selects the import option of the Import/Export Settings feature on the IDE’s 
    /// Tools menu.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    int IVsUserSettings.ImportSettings(string strPageGuid, IVsSettingsReader reader, uint flags, 
      ref int restartRequired)
    {
      // --- Nobody should require a restart...
      if (restartRequired > 0)
        restartRequired = 0;

      VsDebug.Assert(!string.IsNullOrEmpty(strPageGuid), "Passed page guid cannot be null");
      VsDebug.Assert(reader != null, "IVsSettingsReader cannot be null");

      var loadPropsFromRegistry = (flags & (uint)__UserSettingsFlags.USF_ResetOnImport) == 0;
      var requestPageGuid = new Guid(strPageGuid ?? String.Empty);
      var profileManager = GetProfileManager(requestPageGuid, 
        loadPropsFromRegistry 
          ? ProfileManagerLoadAction.LoadPropsFromRegistry 
          : ProfileManagerLoadAction.ResetSettings);
      if (profileManager != null)
      {
        // --- We get the live instance (if any) when we load
        profileManager.LoadSettingsFromXml(reader);
        // --- Update the store
        profileManager.SaveSettingsToStorage();
      }
      return NativeMethods.S_OK;
    }

    #endregion

    #region IVsUserSettingsMigration members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Migrates user settings.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    int IVsUserSettingsMigration.MigrateSettings(IVsSettingsReader reader, IVsSettingsWriter writer, 
      string strPageGuid)
    {
      VsDebug.Assert(!string.IsNullOrEmpty(strPageGuid), "Passed page guid cannot be null");
      VsDebug.Assert(reader != null, "IVsSettingsReader cannot be null");
      VsDebug.Assert(writer != null, "IVsSettingsWriter cannot be null");
      var requestPageGuid = Guid.Empty;

      try
      {
        requestPageGuid = new Guid(strPageGuid ?? String.Empty);
      }
      catch (FormatException)
      {
        // --- If this is thrown, it means strPageGuid is not really a GUID, but rather a
        // --- tools options page name like "Environment.General".
      }
      IProfileMigrator profileMigrator;
      if (requestPageGuid == Guid.Empty)
      {
        profileMigrator = GetAutomationObject(strPageGuid) as IProfileMigrator;
      }
      else
      {
        profileMigrator = GetProfileManager(requestPageGuid, ProfileManagerLoadAction.None) as IProfileMigrator;
      }
      if (profileMigrator != null)
      {
        profileMigrator.MigrateSettings(reader, writer);
      }
      return NativeMethods.S_OK;
    }

    #endregion

    #region IVsToolWindowFactory Members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates the tool window with the specified type and ID.
    /// </summary>
    /// <param name="toolWindowType">Type of the tool window.</param>
    /// <param name="id">The id of the tool window instance.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsToolWindowFactory.CreateToolWindow(ref Guid toolWindowType, uint id)
    {
      if (id > int.MaxValue)
        throw new ArgumentOutOfRangeException(String.Format(CultureInfo.CurrentUICulture, 
          "Instance ID cannot be more then {0}", int.MaxValue));
      var instanceID = (int)id;

      // --- Find the Type for this GUID
      foreach (var tool in GetType().AttributesOfType<XtraProvideToolWindowAttribute>())
      {
        if (tool.ToolType.GUID == toolWindowType)
        {
          // --- We found the corresponding type. If a window gets created this way, 
          // --- FindToolWindow should be used to get a reference to it.
          FindToolWindow(tool.ToolType, instanceID, true, tool);
          break;
        }
      }
      return NativeMethods.S_OK;
    }

    #endregion

    #region Virtual methods definitions

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method is called when the package has been sited in Visual Studio.
    /// </summary>
    /// <remarks>
    /// Override this method to provide your own initialization steps. Here you can access VS Shell
    /// services.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual void Initialize()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the automation object for this package.
    /// </summary>
    /// <remarks>
    /// This method returns the automation object for this package. The default implementation 
    /// will return null if name is null, indicating there is no default automation object. If 
    /// name is non null, this will walk metadata attributes searching for an option page that has 
    /// a name of the format &lt;Category&gt;.&lt;Name&gt;. If the option page has this format and 
    /// indicates that it supports automation, its automation object will be returned.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual object GetAutomationObject(string name)
    {
      if (_Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      if (name == null) return null;
      var nameParts = name.Split(new[] { '.' });
      if (nameParts.Length != 2)
      {
        return null;
      }

      nameParts[0] = nameParts[0].Trim();
      nameParts[1] = nameParts[1].Trim();

      AttributeCollection attributes = TypeDescriptor.GetAttributes(this);
      foreach (Attribute attr in attributes)
      {
        var pa = attr as XtraProvideOptionPageAttribute;
        if (pa != null && pa.SupportsAutomation)
        {
          // --- Check to see if the name matches.
          if (string.Compare(pa.CategoryName, nameParts[0], 
            StringComparison.OrdinalIgnoreCase) != 0) continue;

          if (string.Compare(pa.PageName, nameParts[1],
            StringComparison.OrdinalIgnoreCase) != 0) continue;

          // --- Ok, the name matches. Return this page's automation object.
          var page = GetDialogPage(pa.PageType);
          return page.AutomationObject;
        }
      }
      return null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enables a VSPackage that requires user intervention to abort the shutdown process.
    /// </summary>
    /// <param name="canClose">
    /// Flag indicating whether the VSPackage can close. Is set to true if the VSPackage can close.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    /// <remarks>
    /// Inheritors must override the QueryClose method to respond to this event.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual int QueryClose(out bool canClose)
    {
      canClose = true;
      return NativeMethods.S_OK;
    }

    #endregion

    #region Nested types

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This helper class holds information about a service instance that is proffered to the shell.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private sealed class ProfferedService
    {
      /// <summary>Service instance proffered</summary>
      public object Instance;

      /// <summary>Cookie returned by IProfferService.ProfferService method</summary>
      public uint Cookie;
    }

    // ================================================================================================
    /// <summary>
    /// This class derives from container to provide a service provider connection to the package.
    /// </summary>
    // ================================================================================================
    private sealed class PackageContainer : Container
    {
      private IUIService _UIService;
      private AmbientProperties _AmbientProperties;
      private readonly IServiceProvider _Provider;

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Creates a new container using the given service provider.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      internal PackageContainer(IServiceProvider provider)
      {
        _Provider = provider;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Override to GetService so we can route requests to the package's service provider.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override object GetService(Type serviceType)
      {
        if (serviceType == null) throw new ArgumentNullException("serviceType");
        if (_Provider != null)
        {
          if (serviceType == typeof(AmbientProperties))
          {
            if (_UIService == null)
              _UIService = (IUIService)_Provider.GetService(typeof(IUIService));
            if (_AmbientProperties == null)
              _AmbientProperties = new AmbientProperties();
            if (_UIService != null)
            {
              // --- Update the _AmbientProperties in case the styles have changed
              // --- since last time.
              _AmbientProperties.Font = (Font)_UIService.Styles["DialogFont"];
            }
            return _AmbientProperties;
          }
          object service = _Provider.GetService(serviceType);
          if (service != null) return service;
        }
        return base.GetService(serviceType);
      }
    }

    // ================================================================================================
    /// <summary>
    /// This class describes a registered editor factory
    /// </summary>
    // ================================================================================================
    private sealed class EditorFactoryInfo
    {
      /// <summary>Cookie adressing the registered editor factory</summary>
      public uint Cookie;
      /// <summary>Editor factory instance</summary>
      public IVsEditorFactory Factory;
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an instance of a service on-demand
    /// </summary>
    /// <param name="container">
    /// The service container that requested the creation of the service.
    /// </param>
    /// <param name="serviceType">The type of service to create.</param>
    /// <returns>
    /// The service specified by serviceType, or nullNothingnullptra null reference 
    /// (Nothing in Visual Basic) if the service could not be created.
    /// </returns>
    /// <remarks>
    /// This method creates instances for IMenuCommandService and IOleCommandTarget service
    /// types. IOleCommandTarget is implemented on IMenuCommandService, so we offer both as 
    /// services and delegate the creation of IOleCommandTarget to IMenuCommandService.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    private object OnCreateService(IServiceContainer container, Type serviceType)
    {
      if (serviceType == typeof(IOleCommandTarget))
      {
        object commandService = GetService(typeof(IMenuCommandService));
        if (commandService is IOleCommandTarget)
        {
          return commandService;
        }
        VsDebug.Fail("IMenuCommandService is either unavailable or does not implement IOleCommandTarget");
      }
      else if (serviceType == typeof(IMenuCommandService))
      {
        return new OleMenuCommandService(this);
      }
      VsDebug.Fail("OnCreateService invoked for a service we didn't add");
      return null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified profile manager.
    /// </summary>
    /// <remarks>
    /// This method returns the requested profile manager based on its guid. Profile managers are 
    /// cached so they can keep a single instance of their state. This method allows a deriving 
    /// class to get a cached profile manager. The object will be dynamically created if it is not 
    /// in the cache.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    private IProfileManager GetProfileManager(Guid objectGuid, ProfileManagerLoadAction loadAction)
    {
      IProfileManager result = null;
      if (objectGuid == Guid.Empty)
        throw new ArgumentNullException("objectGuid");
      if (_PagesAndProfiles != null)
      {
        foreach (var profileManager in _PagesAndProfiles.Components)
        {
          if (!profileManager.GetType().GUID.Equals(objectGuid)) continue;
          result = profileManager as IProfileManager;
          if (result != null)
          {
            switch (loadAction)
            {
              case ProfileManagerLoadAction.LoadPropsFromRegistry:
                result.LoadSettingsFromStorage();
                break;
              case ProfileManagerLoadAction.ResetSettings:
                result.ResetSettings();
                break;
            }
          }
          break;
        }
      }
      if (result == null)
      {
        // --- Didn't find it in our cache.  Now look in the metadata attributes for the 
        // --- class. Look at all types at the same time.
        var attributes = TypeDescriptor.GetAttributes(this);
        foreach (Attribute attr in attributes)
        {
          if (attr is ProvideProfileAttribute)
          {
            var objectType = ((ProvideProfileAttribute)attr).ObjectType;
            if (objectType.GUID.Equals(objectGuid))
            {
              // --- Found it... Now instanciate since it was not in the cache
              // --- If not build a constructor for it
              var ctor = objectType.GetConstructor(new Type[] { });
              if (ctor == null)
              {
                throw new ArgumentException(string.Format(Resources.Culture, Resources.Package_PageCtorMissing, objectType.FullName));
              }
              result = (IProfileManager)ctor.Invoke(new object[] { });

              // --- If it's a DialogPage cache it
              if (result != null)
              {
                if (_PagesAndProfiles == null)
                {
                  _PagesAndProfiles = new PackageContainer(this);
                }
                _PagesAndProfiles.Add((IComponent)result);
              }
              break;
            }
          }
        }
      }
      return result;
    }

    #endregion
  }
}
