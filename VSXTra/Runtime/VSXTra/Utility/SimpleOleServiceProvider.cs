// ================================================================================================
// SimpleOleServiceProvider.cs
//
// Created: 2008.09.14, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class represents a simple ole service provider used by hierarchy nodes.
  /// </summary>
  // ================================================================================================
  public class SimpleOleServiceProvider : IOleServiceProvider, IDisposable
  {
    #region Private fields

    /// <summary>Service instances</summary>
    private Dictionary<Guid, ServiceData> _Services;

    /// <summary>Is this instance disposed?</summary>
    private bool _IsDisposed;

    /// <summary>
    /// Defines an object that will be a mutex for this object for synchronizing thread calls.
    /// </summary>
    private static volatile object Mutex = new object();

    #endregion

    #region Nested Types

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Represents a callback method used to create a service instance.
    /// </summary>
    /// <param name="serviceType">Type of service to create.</param>
    /// <returns>Service object created.</returns>
    // --------------------------------------------------------------------------------------------
    public delegate object ServiceCreatorCallback(Type serviceType);

    // ================================================================================================
    /// <summary>
    /// Holds information about a service.
    /// </summary>
    // ================================================================================================
    private class ServiceData : IDisposable
    {
      private readonly Type _ServiceType;
      private object _Instance;
      private ServiceCreatorCallback _Creator;
      private readonly bool _ShouldDispose;

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Initializes a new instance of the <see cref="ServiceData"/> class.
      /// </summary>
      /// <param name="serviceType">Type of the service.</param>
      /// <param name="instance">The instance.</param>
      /// <param name="callback">The callback.</param>
      /// <param name="shouldDispose">
      /// If set to <c>true</c> the service instance should be disposed.
      /// </param>
      // --------------------------------------------------------------------------------------------
      public ServiceData(Type serviceType, object instance, ServiceCreatorCallback callback, 
        bool shouldDispose)
      {
        if (null == serviceType) 
          throw new ArgumentNullException("serviceType");
        if ((null == instance) && (null == callback))
          throw new ArgumentNullException("instance");
        _ServiceType = serviceType;
        _Instance = instance;
        _Creator = callback;
        _ShouldDispose = shouldDispose;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Retrieves the current service instance.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public object ServiceInstance
      {
        get
        {
          if (null == _Instance)
          {
            _Instance = _Creator(_ServiceType);
          }
          return _Instance;
        }
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Retrieves the GUID identifying the service.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public Guid Guid
      {
        get { return _ServiceType.GUID; }
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Disposes the service instance
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public void Dispose()
      {
        if ((_ShouldDispose) && (null != _Instance))
        {
          var disp = _Instance as IDisposable;
          if (null != disp)
          {
            disp.Dispose();
          }
          _Instance = null;
        }
        _Creator = null;
        GC.SuppressFinalize(this);
      }
    }
    #endregion

    #region IOleServiceProvider Members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// </summary>
    /// <param name="guidService">Guid representing the service type.</param>
    /// <param name="riid">Guid representing the service interfase queried.</param>
    /// <param name="ppvObject">The object representing the service.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
    {
      ppvObject = (IntPtr)0;
      int hr = VSConstants.S_OK;

      ServiceData serviceInstance = null;
      if (_Services != null && _Services.ContainsKey(guidService))
        serviceInstance = _Services[guidService];
      if (serviceInstance == null)
        return VSConstants.E_NOINTERFACE;

      // --- Now check to see if the user asked for an IID other than IUnknown.  
      // --- If so, we must do another QI.
      if (riid.Equals(NativeMethods.IID_IUnknown))
      {
        ppvObject = Marshal.GetIUnknownForObject(serviceInstance.ServiceInstance);
      }
      else
      {
        IntPtr pUnk = IntPtr.Zero;
        try
        {
          pUnk = Marshal.GetIUnknownForObject(serviceInstance.ServiceInstance);
          hr = Marshal.QueryInterface(pUnk, ref riid, out ppvObject);
        }
        finally
        {
          if (pUnk != IntPtr.Zero)
          {
            Marshal.Release(pUnk);
          }
        }
      }
      return hr;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The IDispose interface Dispose method for disposing the object determinastically.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the given service to the service container.
    /// </summary>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="serviceInstance">An instance of the service.</param>
    /// <param name="shouldDisposeServiceInstance">
    /// true if the Dipose of the service provider is allowed to dispose the sevice instance.
    /// </param>
    // --------------------------------------------------------------------------------------------
    public void AddService(Type serviceType, object serviceInstance, bool shouldDisposeServiceInstance)
    {
      AddService(new ServiceData(
        serviceType, serviceInstance, null, shouldDisposeServiceInstance));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the given service to the service container using on-demand instantiation.
    /// </summary>
    /// <param name="serviceType">The type of the service to add.</param>
    /// <param name="callback">Callback method instantiating the service.</param>
    /// <param name="shouldDisposeServiceInstance">
    /// true if the Dipose of the service provider is allowed to dispose the sevice instance.
    /// </param>
    // --------------------------------------------------------------------------------------------
    public void AddService(Type serviceType, ServiceCreatorCallback callback, bool shouldDisposeServiceInstance)
    {
      AddService(new ServiceData(
        serviceType, null, callback, shouldDisposeServiceInstance));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes the given service type from the service container.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void RemoveService(Type serviceType)
    {
      if (serviceType == null)
      {
        throw new ArgumentNullException("serviceType");
      }
      if (_Services.ContainsKey(serviceType.GUID))
      {
        _Services.Remove(serviceType.GUID);
      }
    }

    #endregion

    #region Helper methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds the given service to the service container using the specified service information.
    /// </summary>
    /// <param name="data">Service infomration.</param>
    // --------------------------------------------------------------------------------------------
    private void AddService(ServiceData data)
    {
      // --- Make sure that the collection of services is created.
      if (null == _Services)
      {
        _Services = new Dictionary<Guid, ServiceData>();
      }

      // --- Disallow the addition of duplicate services.
      if (_Services.ContainsKey(data.Guid))
      {
        throw new InvalidOperationException();
      }
      _Services.Add(data.Guid, data);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The method that does the cleanup.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void Dispose(bool disposing)
    {
      // --- Everybody can go here.
      if (_IsDisposed) return;

      // --- Synchronize calls to the Dispose simulteniously.
      lock (Mutex)
      {
        if (disposing)
        {
          // --- Remove all our services
          if (_Services != null)
          {
            _Services.Values.ForEach(data => data.Dispose());
            _Services.Clear();
            _Services = null;
          }
        }
        _IsDisposed = true;
      }
    }

    #endregion

  }
}