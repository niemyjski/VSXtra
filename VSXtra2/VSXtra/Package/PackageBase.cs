// ================================================================================================
// PackageBase.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2010.07.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;

namespace VSXtra.Package
{
  public class PackageBase:
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
    #region Private members

    /// <summary>This flag signs if the package is being disposed.</summary>
    public bool Zombied { get; private set; }

    /// <summary>Dictionary for service instances.</summary>
    private Dictionary<Type, object> _Services;

    /// <summary>Service provider for local services.</summary>
    private ServiceProvider _ServiceProvider;

    #endregion

    #region Package Members

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
              var message = string.Format(
                "An object was not returned from a service creator callback for the " + 
                "registered type of {0}. This may mean that it failed a type equivalence " + 
                "comparison.  To compare type objects you must use Type.IsEquivalentTo(Type).  " + 
                "Do not use .Equals or the == operator.", 
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
              Debug.Fail(string.Format("Object {0} was returned from a service creator callback " + 
                "but it does not implement the registered type of {1}", 
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
        string.Format("GetService is recursing on itself while trying to resolve the " + 
        "service {0}. This means that someone is asking for this service while the service " + 
        "is trying to create itself.  Breaking the recursion now and aborting this GetService call.", 
        serviceType.Name));
      if (value == null && _ServiceProvider != null && (_Services == null || _Services.Count == 0 || 
        !_Services.ContainsKey(serviceType)))
      {
        value = _ServiceProvider.GetService(serviceType);
      }

      return value;
    }

    #endregion

    #region Implementation of IVsPackage

    int IVsPackage.SetSite(IOleServiceProvider psp)
    {
      throw new NotImplementedException();
    }

    int IVsPackage.QueryClose(out int pfCanClose)
    {
      throw new NotImplementedException();
    }

    int IVsPackage.Close()
    {
      throw new NotImplementedException();
    }

    int IVsPackage.GetAutomationObject(string pszPropName, out object ppDisp)
    {
      throw new NotImplementedException();
    }

    int IVsPackage.CreateTool(ref Guid rguidPersistenceSlot)
    {
      throw new NotImplementedException();
    }

    int IVsPackage.ResetDefaults(uint grfFlags)
    {
      throw new NotImplementedException();
    }

    int IVsPackage.GetPropertyPage(ref Guid rguidPage, VSPROPSHEETPAGE[] ppage)
    {
      throw new NotImplementedException();
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
