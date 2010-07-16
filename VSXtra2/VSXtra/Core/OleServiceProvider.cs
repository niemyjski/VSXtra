// ================================================================================================
// OleServiceProvider.cs
//
// This source code is created by using the source code provided with the VS 2010 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2010.07.06, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra.Core
{
  internal static class OleServiceProvider
  {
    private static readonly HandleRef _NullHandleRef = new HandleRef(null, IntPtr.Zero);

    private static IServiceProvider GetGlobalProviderFromMessageFilter()
    {
      var oleMessageFilterForCallingThread = GetOleMessageFilterForCallingThread();
      if (oleMessageFilterForCallingThread == null)
      {
        return null;
      }
      return (oleMessageFilterForCallingThread as IServiceProvider);
    }

    private static object GetOleMessageFilterForCallingThread()
    {
      if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
      {
        return null;
      }
      var zero = IntPtr.Zero;
      if (ErrorHandler.Failed(NativeMethods.CoRegisterMessageFilter(_NullHandleRef, ref zero)))
      {
        return null;
      }
      if (zero == IntPtr.Zero)
      {
        return null;
      }
      var oldMsgFilter = IntPtr.Zero;
      NativeMethods.CoRegisterMessageFilter(new HandleRef(null, zero), ref oldMsgFilter);
      var objectForIUnknown = Marshal.GetObjectForIUnknown(zero);
      Marshal.Release(zero);
      return objectForIUnknown;
    }

    public static IServiceProvider GlobalProvider
    {
      get { return (GetGlobalProviderFromMessageFilter()); }
    }
  }

}
