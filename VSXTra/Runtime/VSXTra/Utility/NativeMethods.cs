// ================================================================================================
// NativeMethods.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using IServiceProvider=System.IServiceProvider;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// Static class for native P/Invoke methods and types
  /// </summary>
  // ================================================================================================
  internal static class NativeMethods
  {
    public const int E_ABORT = -2147467260;
    public const int E_ACCESSDENIED = -2147024891;
    public const int E_FAIL = -2147467259;
    public const int E_HANDLE = -2147024890;
    public const int E_INVALIDARG = -2147024809;
    public const int E_NOINTERFACE = -2147467262;
    public const int E_NOTIMPL = -2147467263;
    public const int E_OUTOFMEMORY = -2147024882;
    public const int E_PENDING = -2147483638;
    public const int E_POINTER = -2147467261;
    public const int E_UNEXPECTED = -2147418113;
    public const int GWL_EXSTYLE = -20;
    public const int GWL_STYLE = -16;
    public const int GWL_WNDPROC = -4;
    public const int OLECMDERR_E_NOTSUPPORTED = -2147221248;
    public const int OLECMDERR_E_UNKNOWNGROUP = -2147221244;
    public const int S_FALSE = 1;
    public const int S_OK = 0;
    public const int SW_SHOWNORMAL = 1;
    public const int SWP_FRAMECHANGED = 0x20;
    public const int SWP_NOACTIVATE = 0x10;
    public const int SWP_NOMOVE = 2;
    public const int SWP_NOSIZE = 1;
    public const int SWP_NOZORDER = 4;
    public const int TRANSPARENT = 1;
    public const int TVM_GETEDITCONTROL = 0x110f;
    public const int TVM_SETINSERTMARK = 0x111a;
    public const int WS_BORDER = 0x800000;
    public const int WS_CAPTION = 0xc00000;
    public const int WS_CHILD = 0x40000000;
    public const int WS_CLIPCHILDREN = 0x2000000;
    public const int WS_CLIPSIBLINGS = 0x4000000;
    public const int WS_DISABLED = 0x8000000;
    public const int WS_DLGFRAME = 0x400000;
    public const int WS_EX_APPWINDOW = 0x40000;
    public const int WS_EX_CLIENTEDGE = 0x200;
    public const int WS_EX_CONTEXTHELP = 0x400;
    public const int WS_EX_CONTROLPARENT = 0x10000;
    public const int WS_EX_DLGMODALFRAME = 1;
    public const int WS_EX_LAYERED = 0x80000;
    public const int WS_EX_LEFT = 0;
    public const int WS_EX_LEFTSCROLLBAR = 0x4000;
    public const int WS_EX_MDICHILD = 0x40;
    public const int WS_EX_NOPARENTNOTIFY = 4;
    public const int WS_EX_RIGHT = 0x1000;
    public const int WS_EX_RTLREADING = 0x2000;
    public const int WS_EX_STATICEDGE = 0x20000;
    public const int WS_EX_TOOLWINDOW = 0x80;
    public const int WS_EX_TOPMOST = 8;
    public const int WS_HSCROLL = 0x100000;
    public const int WS_MAXIMIZE = 0x1000000;
    public const int WS_MAXIMIZEBOX = 0x10000;
    public const int WS_MINIMIZE = 0x20000000;
    public const int WS_MINIMIZEBOX = 0x20000;
    public const int WS_OVERLAPPED = 0;
    public const int WS_POPUP = -2147483648;
    public const int WS_SYSMENU = 0x80000;
    public const int WS_TABSTOP = 0x10000;
    public const int WS_THICKFRAME = 0x40000;
    public const int WS_VISIBLE = 0x10000000;
    public const int WS_VSCROLL = 0x200000;
    public const int WSF_VISIBLE = 1;

    public static readonly Guid IID_IObjectWithSite = typeof(IObjectWithSite).GUID;
    public static readonly Guid IID_IServiceProvider = typeof(IServiceProvider).GUID;
    public static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");

    public static bool Succeeded(int hr)
    {
      return (hr >= 0);
    }

    public static bool Failed(int hr)
    {
      return (hr < 0);
    }

    public static int ThrowOnFailure(int hr)
    {
      return ThrowOnFailure(hr, null);
    }

    public static int ThrowOnFailure(int hr, params int[] expectedHRFailure)
    {
      if (Failed(hr) && ((expectedHRFailure == null) || (Array.IndexOf(expectedHRFailure, hr) < 0)))
      {
        Marshal.ThrowExceptionForHR(hr);
      }
      return hr;
    }
  }
}
