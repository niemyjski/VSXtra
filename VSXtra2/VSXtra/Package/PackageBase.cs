// ================================================================================================
// PackageBase.cs
//
// This source code is created by using the source code provided with the VS 2010 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2010.07.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using VSXtra.Core;
using VSXtra.Properties;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using OleMenuCommandService = VSXtra.Core.OleMenuCommandService;
using ServiceProvider = VSXtra.Core.ServiceProvider;
using VSRegistry = VSXtra.Core.VSRegistry;

namespace VSXtra.Package
{
  // ================================================================================================
  /// <summary>
  /// This class is alternate implementation of the Microsoft.VisualStudio.Shell.Package type. 
  /// </summary>
  // ================================================================================================
  public class PackageBase :
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
    IVsToolWindowFactory,

    // Provides statically registered toolbox items.
    IVsToolboxItemProvider,

    // Provides handling for Shell property changes.
    IVsShellPropertyEvents
  {
    #region Private fields

    /// <summary>This flag signs if the package is being disposed.</summary>
    public bool Zombied { get; private set; }

    /// <summary>Dictionary for service instances.</summary>
    private Dictionary<Type, object> _Services;

    /// <summary>Service provider for local services.</summary>
    private ServiceProvider _ServiceProvider;

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
      Zombied = false;
      ServiceCreatorCallback callback = OnCreateService;
      this.AddService<IMenuCommandService>(callback);
      this.AddService<IOleCommandTarget>(callback);
    }

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
      if (disposing)
      {
        //// Unregister any registered editor factories.
        ////
        //if (_editorFactories != null)
        //{
        //  Hashtable editorFactories = _editorFactories;
        //  _editorFactories = null;

        //  try
        //  {
        //    IVsRegisterEditors registerEditors = GetService(typeof(SVsRegisterEditors)) as IVsRegisterEditors;
        //    foreach (DictionaryEntry de in editorFactories)
        //    {
        //      try
        //      {
        //        if (registerEditors != null)
        //        {
        //          // Don't check for the return value because, even if this unregister fails,
        //          // we have anyway to try to unregister the others.
        //          registerEditors.UnregisterEditor((uint)de.Value);
        //        }
        //      }
        //      catch (Exception) { /* do nothing */ }
        //      finally
        //      {
        //        IDisposable disposable = de.Key as IDisposable;
        //        if (disposable != null)
        //        {
        //          disposable.Dispose();
        //        }
        //      }
        //    }
        //  }
        //  catch (Exception e)
        //  {
        //    Debug.Fail(String.Format("Failed to dispose editor factories for package {0}\n{1}", this.GetType().FullName, e.Message));
        //  }
        //}
        //// Unregister any registered project factories.
        ////
        //if (_projectFactories != null)
        //{
        //  Hashtable projectFactories = _projectFactories;
        //  _projectFactories = null;
        //  try
        //  {
        //    IVsRegisterProjectTypes registerProjects = GetService(typeof(SVsRegisterProjectTypes)) as IVsRegisterProjectTypes;

        //    foreach (DictionaryEntry de in projectFactories)
        //    {
        //      try
        //      {
        //        if (registerProjects != null)
        //        {
        //          // As above, don't check for the return value.
        //          registerProjects.UnregisterProjectType((uint)de.Value);
        //        }
        //      }
        //      finally
        //      {
        //        IDisposable disposable = de.Key as IDisposable;
        //        if (disposable != null)
        //        {
        //          disposable.Dispose();
        //        }
        //      }
        //    }
        //  }
        //  catch (Exception e)
        //  {
        //    Debug.Fail(String.Format("Failed to dispose project factories for package {0}\n{1}", this.GetType().FullName, e.Message));
        //  }
        //}

        //// Dispose all IComponent ToolWindows
        ////
        //if (_componentToolWindows != null)
        //{
        //  Container componentToolWindows = _componentToolWindows;
        //  _componentToolWindows = null;
        //  try
        //  {
        //    componentToolWindows.Dispose();
        //  }
        //  catch (Exception e)
        //  {
        //    Debug.Fail(String.Format("Failed to dispose component toolwindows for package {0}\n{1}", this.GetType().FullName, e.Message));
        //  }
        //}

        //// Dispose all pages.
        ////
        //if (_pagesAndProfiles != null)
        //{
        //  Container pagesAndProfiles = _pagesAndProfiles;
        //  _pagesAndProfiles = null;
        //  try
        //  {
        //    pagesAndProfiles.Dispose();
        //  }
        //  catch (Exception e)
        //  {
        //    Debug.Fail(String.Format("Failed to dispose component toolwindows for package {0}\n{1}", this.GetType().FullName, e.Message));
        //  }
        //}

        // Enumerate the service list and destroy all services.  This should
        // always be done last.
        //
        CleanUpServices();
        CleanUpServiceProvider();

        //if (_toolWindows != null)
        //{
        //  _toolWindows.Dispose();
        //  _toolWindows = null;
        //}

        //if (_optionKeys != null)
        //{
        //  _optionKeys = null;
        //}

        // Disconnect user preference change events
        //
        //SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Clean up services registered by the package.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void CleanUpServices()
    {
      if (_Services == null || _Services.Count == 0) return;
      {
        try
        {
          var ps = (IProfferService)GetService(typeof(SProfferService));
          var services = _Services;
          _Services = null;
          foreach (var value in services.Values)
          {
            var service = value;
            var proffer = service as ProfferedService;
            try
            {
              if (proffer != null)
              {
                service = proffer.Instance;
                if (proffer.Cookie != 0 && ps != null)
                {
                  int hr = ps.RevokeService(proffer.Cookie);
                  if (NativeMethods.Failed(hr))
                  {
                    Debug.Fail(String.Format(CultureInfo.CurrentUICulture, "Failed to unregister service {0}", service.GetType().FullName));
                    Trace.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Failed to unregister service {0}", service.GetType().FullName));
                  }
                }
              }
            }
            finally
            {
              if (service is IDisposable)
              {
                ((IDisposable)service).Dispose();
              }
            }
          }
        }
        catch (Exception e)
        {
          Debug.Fail(String.Format("Failed to dispose proffered service for package {0}\n{1}", 
                                   GetType().FullName, e.Message));
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Clean up the service provider.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void CleanUpServiceProvider()
    {
      // --- Disallow any service requests after this.
      if (_ServiceProvider == null) return;
      try
      {
        _ServiceProvider.Dispose();
      }
      catch (Exception e)
      {
        Debug.Fail(String.Format("Failed to dispose the service provider for package {0}\n{1}",
                                 GetType().FullName, e.Message));
      }
      _ServiceProvider = null;
    }

    #endregion

    #region Service registration and instantiation members

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
      // --- IOleCommandTarget is implemented on IMenuCommandService, so we offer both as 
      // --- services and delegate the creation of IOleCommandTarget to IMenuCommandService.
      if (serviceType.IsEquivalentTo(typeof(IOleCommandTarget)))
      {
        var commandService = GetService(typeof(IMenuCommandService));
        if (commandService is IOleCommandTarget)
        {
          return commandService;
        }
        Debug.Fail("IMenuCommandService is either unavailable or does not implement IOleCommandTarget");
      }
      else if (serviceType.IsEquivalentTo(typeof(IMenuCommandService)))
      {
        return new OleMenuCommandService(this);
      }
      Debug.Fail("OnCreateService invoked for a service we didn't add");
      return null;
    }

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
      // --- No services can be retrieved when the package is about to be disposed.
      if (Zombied) return null;

      // --- Must specify a concrete service type
      if (serviceType == null) throw new ArgumentNullException("serviceType");

      // --- Check for the special services that must return this service instance
      if (serviceType.IsEquivalentTo(typeof(IServiceContainer)) || 
        serviceType.IsEquivalentTo(typeof(PackageBase)) 
        || serviceType.IsEquivalentTo(GetType()))
      {
        return this;
      }

      object value = null;

      // --- Check the list of services
      if (_Services != null && _Services.Count > 0)
      {
        lock (serviceType)
        {
          if (_Services.ContainsKey(serviceType))
            value = _Services[serviceType];
          if (value is ProfferedService)
          {
            value = ((ProfferedService)value).Instance;
          }

          if (value is ServiceCreatorCallback)
          {
            // --- In case someone recursively requests the same service, null out the service 
            // --- type here. That way they'll just fail instead of stack fault.
            //
            _Services[serviceType] = null;
            value = ((ServiceCreatorCallback)value)(this, serviceType);
            if (value == null)
            {
              var message = string.Format(Resources.PackageBase_GetService_TypeEquivalence, 
                serviceType.Name);

              // --- If this fails, it will likely indicate that the pacakge did a .Equals 
              // --- or == on the Service type which may fail type equivalence.
              var appCmdLine = GetService(typeof(SVsAppCommandLine)) as IVsAppCommandLine;
              if (appCmdLine != null)
              {
                // --- If we are running under the experimental hive, notify the VSIP developer 
                // --- that this is incorrect.
                string suffix;
                int fPresent;
                appCmdLine.GetOption("RootSuffix", out fPresent, out suffix);
                if (fPresent == 1 && 
                  string.Compare(suffix, "Exp", StringComparison.OrdinalIgnoreCase) == 0)
                {
                  System.Windows.Forms.MessageBox.Show(message);
                }
              }
              Debug.Fail(message);
            }
            else if (!value.GetType().IsCOMObject && !serviceType.IsAssignableFrom(value.GetType()))
            {
              // Callback passed us a bad service.  NULL it, rather than throwing an exception.
              // Callers here do not need to be prepared to handle bad callback implementations.
              Debug.Fail(string.Format(Resources.PackageBase_GetService_BadService, 
                value.GetType().Name, serviceType.Name));
              // ReSharper disable HeuristicUnreachableCode
              value = null;
              // ReSharper restore HeuristicUnreachableCode
            }
            _Services[serviceType] = value;
          }
        }
      }

      // --- Delegate to the parent provider, but only if we have verified that _services 
      // --- doesn't actually contain our key if it does, that means that we're in the 
      // --- middle of trying to resolve this service, and the service resolution
      // --- has recursed.
      Debug.Assert(value != null || _Services == null || _Services.Count == 0 || 
        !_Services.ContainsKey(serviceType), 
        string.Format(Resources.PackageBase_GetService_Recursion, serviceType.Name));
      if (value == null && _ServiceProvider != null && (_Services == null || _Services.Count == 0 || 
        !_Services.ContainsKey(serviceType)))
      {
        value = _ServiceProvider.GetService(serviceType);
      }

      return value;
    }

    #endregion

    #region Registry-related members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This property returns the registry root for the application.
    /// </summary>
    /// <remarks>
    /// Typically this is HKLM\Software\Microsoft\VisualStudio\[ver] but this can change based on 
    /// any alternate root that the shell was initialized with. This key is read-only.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public RegistryKey ApplicationRegistryRoot
    {
      get { return VSRegistry.RegistryRoot(_ServiceProvider, LocalRegistryType.Configuration, false); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This property returns the path to user data storage for Visual Studio.
    /// </summary>
    /// <remarks>
    /// Typically this is %USERPROFILE%\Application Data\Visual Studio\[ver] but this can change 
    /// based on any alternate root that the shell was initialized with.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public string UserDataPath
    {
      get
      {
        // --- Get the registry root and remove the SOFTWARE\ part
        var registryRoot = GetRegistryRoot();
        registryRoot = registryRoot.Substring("SOFTWARE\\".Length);
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, registryRoot);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This property returns the path to user data storage for Visual Studio.
    /// </summary>
    /// <remarks>
    /// Typically this is %USERPROFILE%\Local Settings\Application Data\Visual Studio\[ver] but 
    /// this can change based on any alternate root that the shell was initialized with.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public string UserLocalDataPath
    {
      get
      {
        // --- Get the registry root and remove the SOFTWARE\ part
        var registryRoot = GetRegistryRoot();
        registryRoot = registryRoot.Substring("SOFTWARE\\".Length);
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, registryRoot);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This property returns the registry root for the current user.
    /// </summary>
    /// <remarks>
    /// Typically this is HKCU\Software\Microsoft\VisualStudio\[ver] but this can change based 
    /// on any alternate root that the shell is initialized with. This key is read-write.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public RegistryKey UserRegistryRoot
    {
      get { return VSRegistry.RegistryRoot(_ServiceProvider, LocalRegistryType.UserSettings, true); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the shell's root key for VS options, or uses the value of the 
    /// DefaultRegistryRootAttribute if we coundn't get the shell service.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private string GetRegistryRoot()
    {
      string regisrtyRoot;
      var vsh = (IVsShell)GetService(typeof(SVsShell));
      if (vsh == null)
      {
        // --- Search our custom attributes for an instance of DefaultRegistryRoot
        //
        var regRootAttr = (DefaultRegistryRootAttribute)TypeDescriptor.
          GetAttributes(GetType())[typeof(DefaultRegistryRootAttribute)];
        if (regRootAttr == null)
        {
          Debug.Fail("Package should have a registry root attribute");
          throw new NotSupportedException();
        }
        regisrtyRoot = @"SOFTWARE\Microsoft\VisualStudio\" + regRootAttr.Root;
      }
      else
      {
        object obj;
        NativeMethods.ThrowOnFailure(vsh.GetProperty((int)__VSSPROPID.VSSPROPID_VirtualRegistryRoot, out obj));
        regisrtyRoot = obj.ToString();
      }
      return regisrtyRoot;
    }

    #endregion

    #region Package members

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
      //if (Zombied)
      //  Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      //if (name == null) return null;
      //var nameParts = name.Split(new[] { '.' });
      //if (nameParts.Length != 2)
      //{
      //  return null;
      //}

      //nameParts[0] = nameParts[0].Trim();
      //nameParts[1] = nameParts[1].Trim();

      //AttributeCollection attributes = TypeDescriptor.GetAttributes(this);
      //foreach (Attribute attr in attributes)
      //{
      //  var pa = attr as XtraProvideOptionPageAttribute;
      //  if (pa != null && pa.SupportsAutomation)
      //  {
      //    // --- Check to see if the name matches.
      //    if (string.Compare(pa.CategoryName, nameParts[0],
      //      StringComparison.OrdinalIgnoreCase) != 0) continue;

      //    if (string.Compare(pa.PageName, nameParts[1],
      //      StringComparison.OrdinalIgnoreCase) != 0) continue;

      //    // --- Ok, the name matches. Return this page's automation object.
      //    var page = GetDialogPage(pa.PageType);
      //    return page.AutomationObject;
      //  }
      //}
      return null;
    }


    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Return the locale associated with this IServiceProvider.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int GetProviderLocale()
    {
      var ci = CultureInfo.CurrentCulture;
      var lcid = ci.LCID;
      var loc = (IUIHostLocale)GetService(typeof(IUIHostLocale));
      Debug.Assert(loc != null, 
        "Unable to get IUIHostLocale, defaulting CLR designer to current thread LCID");
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

    #region Implementation of IVsPackage

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a VSPackage with a service provider to the environment.
    /// </summary>
    /// <param name="psp">
    /// Reference to the IServiceProvider interface through which the VSPackage can query for 
    /// services.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.SetSite(IOleServiceProvider psp)
    {
      if (Zombied) Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      if (psp != null)
      {
        if (_ServiceProvider != null)
        {
          throw new InvalidOperationException(string.Format(Resources.Culture, 
            Resources.Package_SiteAlreadySet, GetType().FullName));
        }
        _ServiceProvider = ServiceProvider.CreateFromSetSite(psp);
        InternalInitialize();
        Initialize();
      }
      else if (_ServiceProvider != null)
      {
        // --- No SP, dispose us.
        Dispose(true);
      }
      return NativeMethods.S_OK;
    }

    private void InternalInitialize()
    {
      // --- If we have services to proffer, do that now.
      if (_Services.Count > 0)
      {
        var ps = (IProfferService)GetService(typeof(SProfferService));
        Debug.Assert(ps != null, "We have services to proffer but IProfferService is not available.");
        if (ps != null)
        {
          foreach (var pair in _Services)
          {
            var service = pair.Value as ProfferedService;
            if (service == null) continue;
            uint cookie;
            var serviceGuid = pair.Key.GUID;
            NativeMethods.ThrowOnFailure(ps.ProfferService(ref serviceGuid, this, out cookie));
            service.Cookie = cookie;
          }
        }
      }

      // Initialize this thread's culture info with that of the shell's LCID
      //
      var locale = GetProviderLocale();
      Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);

      // Begin listening to user preference change events
      //
      //SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);

      // Be sure to load the package user options from the solution in case
      // the package was not already loaded when the solution was opened.
      //if (null != _optionKeys)
      //{
      //  try
      //  {
      //    IVsSolutionPersistence pPersistance = (IVsSolutionPersistence)this.GetService(typeof(SVsSolutionPersistence));
      //    if (pPersistance != null)
      //    {
      //      foreach (string key in _optionKeys)
      //      {
      //        // NOTE: don't check for the error code because a failure here is
      //        // expected and not a problem.
      //        pPersistance.LoadPackageUserOpts(this, key);
      //      }
      //    }
      //  }
      //  catch (Exception)
      //  {
      //    // no settings found, no problem.
      //  }
      //}
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enables a VSPackage that requires user intervention to abort the shutdown process.
    /// </summary>
    /// <param name="pfCanClose">
    /// Flag indicating whether the VSPackage can close. Is set to true if the VSPackage can close.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    /// <remarks>
    /// Inheritors must override the QueryClose method to respond to this event.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.QueryClose(out int pfCanClose)
    {
      // --- Default to true as we don't want an error to prevent the shell from closing
      pfCanClose = 1;
      bool canClose;
      var hr = QueryClose(out canClose);
      if (!canClose)
        pfCanClose = 0;
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
      if (!Zombied)
      {
        Dispose(true);
      }
      Zombied = true;
      return NativeMethods.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enables a VSPackage to participate in the DTE automation object model.
    /// </summary>
    /// <param name="pszPropName">String containing the prop name used by this package</param>
    /// <param name="ppDisp">Object pointing to an IDispatch interface</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    /// <remarks>
    /// Inheritors must override the GetAutomationObject method to create a reference for the
    /// automation object used.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IVsPackage.GetAutomationObject(string pszPropName, out object ppDisp)
    {
      if (Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);
      ppDisp = GetAutomationObject(pszPropName);
      if (ppDisp == null)
      {
        Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
      }
      return NativeMethods.S_OK;
    }

    int IVsPackage.CreateTool(ref Guid rguidPersistenceSlot)
    {
      throw new NotImplementedException();
    }

    int IVsPackage.ResetDefaults(uint grfFlags)
    {

      if (Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      if (grfFlags == (uint)__VSPKGRESETFLAGS.PKGRF_TOOLBOXITEMS)
      {
        //if (ToolboxInitialized != null)
        //{
        //  ToolboxInitialized(this, EventArgs.Empty);
        //}
      }
      else if (grfFlags == (uint)__VSPKGRESETFLAGS.PKGRF_TOOLBOXSETUP)
      {
        //if (ToolboxUpgraded != null)
        //{
        //  ToolboxUpgraded(this, EventArgs.Empty);
        //}
      }
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
      //if (ppage == null || ppage.Length < 1)
      //  throw new ArgumentException(string.Format(Resources.Culture, 
      //    Resources.General_ArraySizeShouldBeAtLeast1), "ppage");

      if (Zombied)
        Marshal.ThrowExceptionForHR(NativeMethods.E_UNEXPECTED);

      IWin32Window pageWindow = null;

      //// First, check out the active pages.
      ////
      //if (_pagesAndProfiles != null)
      //{
      //  foreach (object page in _pagesAndProfiles.Components)
      //  {
      //    if (page.GetType().GUID.Equals(rguidPage))
      //    {

      //      // Found a match.
      //      //
      //      IWin32Window w = page as IWin32Window;
      //      if (w != null)
      //      {
      //        if (w is DialogPage)
      //        {
      //          ((DialogPage)w).ResetContainer();
      //        }
      //        pageWindow = w;
      //        break;
      //      }
      //    }
      //  }
      //}

      //if (pageWindow == null)
      //{

      //  DialogPage page = null;

      //  // Didn't find it in our cache.  Now look in the metadata attributes
      //  // for the class.  Look at all types at the same time.
      //  //
      //  AttributeCollection attributes = TypeDescriptor.GetAttributes(this);
      //  foreach (Attribute attr in attributes)
      //  {
      //    if (attr is ProvideOptionDialogPageAttribute)
      //    {
      //      Type pageType = ((ProvideOptionDialogPageAttribute)attr).PageType;
      //      if (pageType.GUID.Equals(rguidPage))
      //      {

      //        // Found a matching attribute.  Now go get the DialogPage with GetDialogPage.
      //        // This has a side-effect of storing the page in
      //        // _pagesAndProfiles for us.
      //        //
      //        page = GetDialogPage(pageType);
      //        pageWindow = page;
      //        break;
      //      }
      //    }

      //    if (page != null)
      //    {
      //      if (_pagesAndProfiles == null)
      //      {
      //        _pagesAndProfiles = new PackageContainer(this);
      //      }
      //      _pagesAndProfiles.Add(page);

      //      // No need to continue looking in the attributes, 
      //      // we've already found the one we're looking for
      //      break;
      //    }
      //  }
      //}

      //// We should now have a page window. If we don't then the requested page
      //// doesn't exist.
      ////
      //if (pageWindow == null)
      //{
      //  Marshal.ThrowExceptionForHR(NativeMethods.E_NOTIMPL);
      //}

      //ppage[0].dwSize = (uint)Marshal.SizeOf(typeof(VSPROPSHEETPAGE));
      //ppage[0].hwndDlg = pageWindow.Handle;
      //// zero-out all the fields we aren't using.
      //ppage[0].dwFlags = 0;
      //ppage[0].HINSTANCE = 0;
      //ppage[0].dwTemplateSize = 0;
      //ppage[0].pTemplate = IntPtr.Zero;
      //ppage[0].pfnDlgProc = IntPtr.Zero;
      //ppage[0].lParam = IntPtr.Zero;
      //ppage[0].pfnCallback = IntPtr.Zero;
      //ppage[0].pcRefParent = IntPtr.Zero;
      //ppage[0].dwReserved = 0;
      //ppage[0].wTemplateId = (ushort)0;

      return NativeMethods.S_OK;
    }

    #endregion

    #region Implementation of IServiceProvider

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries for a service object instance.
    /// </summary>
    /// <param name="guidService">Service object ID</param>
    /// <param name="riid">Service interface ID</param>
    /// <param name="ppvObject">Service object instance</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IOleServiceProvider.QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
    {
      ppvObject = (IntPtr)0;
      int hr = NativeMethods.S_OK;

      // ---Let's check if we have the service by its GUID
      object service = null;
      if (_Services != null)
      {
        foreach (Type serviceType in _Services.Keys)
        {
          if (serviceType.GUID.Equals(guidService))
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
        if (riid.Equals(NativeMethods.IID_IUnknown))
        {
          ppvObject = Marshal.GetIUnknownForObject(service);
        }
        else
        {
          IntPtr pUnk = Marshal.GetIUnknownForObject(service);
          hr = Marshal.QueryInterface(pUnk, ref riid, out ppvObject);
          Marshal.Release(pUnk);
        }
      }
      return hr;
    }

    #endregion

    #region Implementation of IOleCommandTarget

    /// <param name="pguidCmdGroup"/><param name="cCmds"/><param name="prgCmds"/><param name="pCmdText"/>
    int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      throw new NotImplementedException();
    }

    /// <param name="pguidCmdGroup"/><param name="nCmdID"/><param name="nCmdexecopt"/><param name="pvaIn"/><param name="pvaOut"/>
    int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IServiceProvider

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

    #region Implementation of IServiceContainer

    /// <summary>
    /// Adds the specified service to the service container.
    /// </summary>
    /// <param name="serviceType">The type of service to add. </param><param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the <paramref name="serviceType"/> parameter. </param>
    void IServiceContainer.AddService(Type serviceType, object serviceInstance)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds the specified service to the service container, and optionally promotes the service to any parent service containers.
    /// </summary>
    /// <param name="serviceType">The type of service to add. </param><param name="serviceInstance">An instance of the service type to add. This object must implement or inherit from the type indicated by the <paramref name="serviceType"/> parameter. </param><param name="promote">true to promote this request to any parent service containers; otherwise, false. </param>
    void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds the specified service to the service container.
    /// </summary>
    /// <param name="serviceType">The type of service to add. </param><param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested. </param>
    void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds the specified service to the service container, and optionally promotes the service to parent service containers.
    /// </summary>
    /// <param name="serviceType">The type of service to add. </param><param name="callback">A callback object that is used to create the service. This allows a service to be declared as available, but delays the creation of the object until the service is requested. </param><param name="promote">true to promote this request to any parent service containers; otherwise, false. </param>
    void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Removes the specified service type from the service container.
    /// </summary>
    /// <param name="serviceType">The type of service to remove. </param>
    void IServiceContainer.RemoveService(Type serviceType)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Removes the specified service type from the service container, and optionally promotes the service to parent service containers.
    /// </summary>
    /// <param name="serviceType">The type of service to remove. </param><param name="promote">true to promote this request to any parent service containers; otherwise, false. </param>
    void IServiceContainer.RemoveService(Type serviceType, bool promote)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IVsPersistSolutionOpts

    int IVsPersistSolutionOpts.SaveUserOptions(IVsSolutionPersistence pPersistence)
    {
      throw new NotImplementedException();
    }

    int IVsPersistSolutionOpts.LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts)
    {
      throw new NotImplementedException();
    }

    int IVsPersistSolutionOpts.WriteUserOptions(IStream pOptionsStream, string pszKey)
    {
      throw new NotImplementedException();
    }

    int IVsPersistSolutionOpts.ReadUserOptions(IStream pOptionsStream, string pszKey)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IVsUserSettings

    /// <summary>
    /// Saves a VSPackage's configuration using the Visual Studio settings mechanism when the export option of the Import/Export Settings feature available on the IDE’s Tools menu is selected by a user.
    /// </summary>
    /// <returns>
    /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
    /// </returns>
    /// <param name="pszCategoryGuid">[in] GUID identifying the group of settings to be exported. This is the identifying GUID for the Custom Settings Point. For more information on Custom Settings Points, see Persisting Settings</param><param name="pSettings">[in] An <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter"/> interface provided by the environment to the VSPackage providing write access to the Visual Studio settings file.
    ///                 </param>
    int IVsUserSettings.ExportSettings(string pszCategoryGuid, IVsSettingsWriter pSettings)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves a VSPackage's configuration using the Visual Studio settings mechanism when a user selects the import option of the Import/Export Settings feature on the IDE’s Tools menu.
    /// </summary>
    /// <returns>
    /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
    /// </returns>
    /// <param name="pszCategoryGuid">[in] GUID identifying the group of settings to be imported. This is the identify GUID of the Custom Settings Point. For more information on Custom Settings Points see Persisting Settings.
    ///                 </param><param name="pSettings">[in]An <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter"/> interface provided by the environment to the VSPackage providing read access to the Visual Studio settings file.
    ///                 </param><param name="flags">[in] Flag from the system indicating how an implementation of <see cref="M:Microsoft.VisualStudio.Shell.Interop.IVsUserSettings.ImportSettings(System.String,Microsoft.VisualStudio.Shell.Interop.IVsSettingsReader,System.UInt32,System.Int32@)"/> is supposed to process retrieved settings.
    ///                     The supported values of that are members of the <see cref="T:Microsoft.VisualStudio.Shell.Interop.__UserSettingsFlags"/> enumeration. 
    ///                 </param><param name="pfRestartRequired">[out] Flag returned to the environment indicating if a restart of the IDE is required to complete environment reconfiguration based on retrieved data. If the value returned by <paramref name="pfRestartRequired"/> is true, the environment should be restarted. 
    ///                 </param>
    int IVsUserSettings.ImportSettings(string pszCategoryGuid, IVsSettingsReader pSettings, uint flags, ref int pfRestartRequired)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IVsUserSettingsMigration

    /// <returns>
    /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK"/>. If it fails, it returns an error code.
    /// </returns>
    /// <param name="pSettingsReader">[in] <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsSettingsReader"/> to access configuration information.
    ///                 </param><param name="pSettingsWriter">[in] <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsSettingsWriter"/> to write configuration information.
    ///                 </param><param name="pszGuidCategory">Guid representing settings category.
    ///                 </param>
    int IVsUserSettingsMigration.MigrateSettings(IVsSettingsReader pSettingsReader, IVsSettingsWriter pSettingsWriter, string pszGuidCategory)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IVsToolWindowFactory

    int IVsToolWindowFactory.CreateToolWindow(ref Guid rguidPersistenceSlot, uint dwToolWindowId)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IVsShellPropertyEvents

    int IVsShellPropertyEvents.OnShellPropertyChange(int propid, object var)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IVsToolboxItemProvider

    public int GetItemContent(string szItemID, ushort format, out IntPtr pGlobal)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region ProfferedService definition

    // ================================================================================================
    /// <summary>
    /// This class contains information about a service that is being promoted to the Shell.  
    /// </summary>
    // ================================================================================================
    private sealed class ProfferedService
    {
      public object Instance;
      public uint Cookie;
    }

    #endregion
  }
}
