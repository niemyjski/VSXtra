// ================================================================================================
// UnsafeNativeMethods.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
namespace VSXtra
{
  using Microsoft.VisualStudio.OLE.Interop;
  using System;
  using System.Runtime.InteropServices;
  using System.Security;
  using System.Text;

  // ================================================================================================
  /// <summary>
  /// Static class for unsafe native P/Invoke methods and types
  /// </summary>
  // ================================================================================================
  [SuppressUnmanagedCodeSecurity]
  internal class UnsafeNativeMethods
  {
    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int CloseClipboard();
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern bool CloseHandle(HandleRef handle);
    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", ExactSpelling = true)]
    internal static extern void CopyMemory(IntPtr pdst, HandleRef psrc, int cb);
    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", ExactSpelling = true)]
    internal static extern void CopyMemory(IntPtr pdst, string psrc, int cb);
    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", ExactSpelling = true)]
    internal static extern void CopyMemory(byte[] pdst, HandleRef psrc, int cb);
    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", ExactSpelling = true)]
    internal static extern void CopyMemory(IntPtr pdst, byte[] psrc, int cb);
    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern void CopyMemoryW(IntPtr pdst, char[] psrc, int cb);
    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern void CopyMemoryW(IntPtr pdst, string psrc, int cb);
    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern void CopyMemoryW(char[] pdst, HandleRef psrc, int cb);
    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern void CopyMemoryW(StringBuilder pdst, HandleRef psrc, int cb);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern void DebugBreak();
    [DllImport("shell32.dll", EntryPoint = "DragQueryFileW", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, char[] lpszFile, uint cch);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int EmptyClipboard();
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetFileAttributes(string name);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern void GetTempFileName(string tempDirName, string prefixName, int unique, StringBuilder sb);
    public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
    {
      if (IntPtr.Size == 4)
      {
        return GetWindowLong32(hWnd, nIndex);
      }
      return GetWindowLongPtr64(hWnd, nIndex);
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern IntPtr GlobalFree(HandleRef handle);
    [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern IntPtr GlobalLock(IntPtr h);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern IntPtr GlobalLock(HandleRef handle);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern IntPtr GlobalReAlloc(HandleRef handle, int bytes, int flags);
    [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern int GlobalSize(IntPtr h);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern int GlobalSize(HandleRef handle);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern bool GlobalUnlock(HandleRef handle);
    [DllImport("kernel32.dll", EntryPoint = "GlobalUnlock", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern bool GlobalUnLock(IntPtr h);
    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    public static extern bool ImageList_Draw(HandleRef himl, int i, HandleRef hdcDst, int x, int y, int fStyle);
    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    public static extern int ImageList_GetImageCount(HandleRef himl);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool LoadString(HandleRef hInstance, int uID, StringBuilder lpBuffer, int nBufferMax);
    [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int OleFlushClipboard();
    [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int OleGetClipboard(out IDataObject dataObject);
    [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int OleSetClipboard(IDataObject dataObject);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int OpenClipboard(IntPtr newOwner);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
    [DllImport("user32.dll", EntryPoint = "RegisterClipboardFormatW", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern ushort RegisterClipboardFormat(string format);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hwnd, int msg, bool wparam, int lparam);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);
    public static IntPtr SetWindowLong(IntPtr hWnd, short nIndex, IntPtr dwNewLong)
    {
      if (IntPtr.Size == 4)
      {
        return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
      }
      return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
      if (IntPtr.Size == 4)
      {
        return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
      }
      return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, short nIndex, IntPtr dwNewLong);
    [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int WideCharToMultiByte(int codePage, int flags, [MarshalAs(UnmanagedType.LPWStr)] string wideStr, int chars, [In, Out] byte[] pOutBytes, int bufferBytes, IntPtr defaultChar, IntPtr pDefaultUsed);
  }
}