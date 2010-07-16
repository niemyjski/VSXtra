// ================================================================================================
// ServiceProvider.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2010.07.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using IServiceProvider = System.IServiceProvider;

namespace VSXtra.Core
{
  // ================================================================================================
  /// <summary>
  /// This class acts as a bridge between Microsoft.VisualStudio.OLE.Interop.IServiceProvider 
  /// and System.IServiceProvider.
  /// </summary>  
  /// <remarks>
  /// It implements System.IServiceProvider and takes as a constructor argument an instance of 
  /// Microsoft.VisualStudio.OLE.Interop.IServiceProvider. It supports both GUID and type based 
  /// lookups and also has debug code to assert for common native implementation pitfalls, like 
  /// not implementing IUnknown on an object or requiring a specific IID along with a matching SID.
  /// </remarks>
  // ================================================================================================
  [CLSCompliant(false)]
  [ComVisible(true)]
  public sealed class ServiceProvider : IServiceProvider, IDisposable, IObjectWithSite
  {
    private static readonly TraceSwitch _TraceService = 
      new TraceSwitch("TRACESERVICE", "ServiceProvider: Trace service provider requests.");

    /// <summary>The global service provider.</summary>
    static ServiceProvider _GlobalProvider;

    /// <summary>The thread which set globalProvider</summary>
    static Thread _ThreadOwningGlobalProvider;

    private Microsoft.VisualStudio.OLE.Interop.IServiceProvider _ServiceProvider;
    private readonly bool _DefaultServices;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new ServiceProvider object and uses the given interface to resolve services.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ServiceProvider(Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp)
      : this(sp, true)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new ServiceProvider object and uses the given interface to resolve services.
    /// </summary>
    /// <remarks>
    /// If defaultServices is true (the default) this service  provider will
    ///     respond to Microsoft.VisualStudio.OLE.Interop.IServiceProvider and IObjectWithSite
    ///     as services.  A query for Microsoft.VisualStudio.OLE.Interop.IServiceProvider will
    ///     return the underlying COM service provider and a query for IObjectWithSite will
    ///     return this object.  If false is passed into defaultServices these two services
    ///     will not be provided and the service provider will be "transparent".
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public ServiceProvider(Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp, bool defaultServices)
    {
      if (sp == null)
      {
        throw new ArgumentNullException("sp");
      }
      _ServiceProvider = sp;
      _DefaultServices = defaultServices;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Private, default constructor used to create a dummy ServiceProvider with no underlying 
    /// services.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private ServiceProvider()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Releases resources held by this instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Dispose()
    {
      _ServiceProvider = null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the requested service.
    /// </summary>
    /// <param name="serviceType">Type of the service.</param>
    // --------------------------------------------------------------------------------------------
    public object GetService(Type serviceType)
    {
      // --- If we have already been disposed, disallow all service requests.
      if (_ServiceProvider == null) return null;

      if (serviceType == null) throw new ArgumentNullException("serviceType");

      // --- First, can we resolve this service class into a GUID?  If not, then 
      // ---we have nothing to pass.
      Debug.WriteLineIf(_TraceService.TraceVerbose, 
        string.Format("Resolving service '{0} through the service provider {1}.", 
        serviceType.FullName, _ServiceProvider));
      return GetService(serviceType.GUID, serviceType);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the requested service.
    /// </summary>
    /// <param name="guid">GUID identifying the service</param>
    // --------------------------------------------------------------------------------------------
    public object GetService(Guid guid)
    {
      return _ServiceProvider == null ? null : GetService(guid, null);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the requested service.  The guid must be specified; the class is only used when 
    /// debugging and it may be null.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private object GetService(Guid guid, Type serviceType)
    {
      object service;

      // --- No valid guid on the passed in class, so there is no service for it.
      if (guid.Equals(Guid.Empty))
      {
        Debug.WriteLineIf(_TraceService.TraceVerbose, "\tNo SID -- Guid is empty");
        return null;
      }

      // --- We provide a couple of services of our own.
      if (_DefaultServices)
      {
        if (guid.Equals(NativeMethods.IID_IServiceProvider))
        {
          return _ServiceProvider;
        }
        if (guid.Equals(NativeMethods.IID_IObjectWithSite))
        {
          return this;
        }
      }

      IntPtr pUnk;
      var guidUnk = NativeMethods.IID_IUnknown;
      var hr = _ServiceProvider.QueryService(ref guid, ref guidUnk, out pUnk);

      if ((NativeMethods.Succeeded(hr)) && (IntPtr.Zero != pUnk))
      {
        try
        {
          service = Marshal.GetObjectForIUnknown(pUnk);
        }
        finally
        {
          Marshal.Release(pUnk);
        }
      }
      else
      {
        service = null;
        // --- These may be interesting to log.
        //
        Debug.WriteLineIf(_TraceService.TraceVerbose, "\tQueryService failed");

#if DEBUG
        // --- Ensure that this service failure was not the result of a bad QI implementation.
        // --- In C++, 99% of a service query uses SID == IID, but for us, we always use IID = IUnknown
        // --- first.  If the service didn't implement IUnknown correctly, we'll fail the service request
        // --- and it's very difficult to track this down. 
        hr = _ServiceProvider.QueryService(ref guid, ref guid, out pUnk);
        if ((NativeMethods.Succeeded(hr)) && (IntPtr.Zero != pUnk))
        {
          object obj;
          try
          {
            obj = Marshal.GetObjectForIUnknown(pUnk);
          }
          finally
          {
            Marshal.Release(pUnk);
          }

          // --- This service is not returned if we succeed -- I don't want to make debug work correctly 
          // --- when retail doesn't!
          Debug.Assert(!Marshal.IsComObject(obj),
            string.Format("The service {0} implements it's own interface, but does not implement IUnknown!" + 
            "\r\nThis is a bad service implementation, not a problem in the CLR service provider mechanism.{1}", 
            (serviceType != null ? serviceType.Name : guid.ToString()), obj));
        }

#endif
      }

      return service;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the current site object we're using to resolve services.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void IObjectWithSite.GetSite(ref Guid riid, out IntPtr ppv)
    {
      var o = GetService(riid);
      if (o == null)
      {
        Marshal.ThrowExceptionForHR(NativeMethods.E_NOINTERFACE);
      }

      var punk = Marshal.GetIUnknownForObject(o);
      var hr = Marshal.QueryInterface(punk, ref riid, out ppv);
      Marshal.Release(punk);
      if (NativeMethods.Failed(hr))
      {
        Marshal.ThrowExceptionForHR(hr);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the site object we will be using to resolve services.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void IObjectWithSite.SetSite(object pUnkSite)
    {
      if (pUnkSite is Microsoft.VisualStudio.OLE.Interop.IServiceProvider)
      {
        _ServiceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)pUnkSite;
      }
    }

    static void SetGlobalProvider(ServiceProvider sp)
    {
      _GlobalProvider = sp;
      _ThreadOwningGlobalProvider = Thread.CurrentThread;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks if the calling thread is the same as the thread which set the global provider.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    internal static bool CheckServiceProviderThreadAccess()
    {
      return _ThreadOwningGlobalProvider == Thread.CurrentThread;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Determines if the given ServiceProvider is uninitialized.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    static bool IsNullOrUnsited(ServiceProvider sp)
    {
      return sp == null || sp._ServiceProvider == null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Get the global service provider for the calling thread.
    /// </summary>
    /// <remarks>
    /// The global service provider is set by calling ServiceProvider.CreateFromSetSite. If 
    /// ServiceProvider.CreateFromSetSite has not been called, an attempt is made to retrieve the 
    /// service provider from the OLE message filter. If no suitable service provider can be 
    /// found on the calling thread, then a new, empty ServiceProvider is returned.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public static ServiceProvider GlobalProvider
    {
      get
      {
        if (IsNullOrUnsited(_GlobalProvider))
        {
          var oleProvider = OleServiceProvider.GlobalProvider;
          if (oleProvider != null)
          {
            Debug.WriteLineIf(_TraceService.TraceVerbose, "Initializing ServiceProvider.GlobalProvider from OLE message filter.");
            SetGlobalProvider(new ServiceProvider(oleProvider));
          }
          else if (_GlobalProvider == null)
          {
            Debug.WriteLineIf(_TraceService.TraceVerbose, "Creating a dummy global ServiceProvider because OleServiceProvider.GlobalProvider is unavailable for this thread.");
            SetGlobalProvider(new ServiceProvider());
          }
        }
        return _GlobalProvider;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Create a new ServiceProvider for the given site.
    /// </summary>
    /// <remarks>
    /// Should be called from an object that implements SetSite (IObjectWithSite or IVsPackage). 
    /// Automatically sets the global service provider if it hasn't already been set. This method 
    /// is typically called from the SetSite method of a Visual Studio package.
    /// By calling this method, the caller declares that it knows the global OLE service provider 
    /// for the calling thread and that the ServiceProvider instance returned may be used by other,
    /// unrelated components, accessed via the ServiceProvider.GlobalProvider static property.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public static ServiceProvider CreateFromSetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp)
    {
      var provider = new ServiceProvider(sp);

      // --- If there is no current global service provider or the existing one is unsited, 
      // --- then set it here.
      if (IsNullOrUnsited(_GlobalProvider))
      {
        Debug.WriteLineIf(_TraceService.TraceVerbose, 
          "Initializing ServiceProvider.GlobalProvider from SetSite.");
        SetGlobalProvider(provider);
      }
      return provider;
    }
  }
}
