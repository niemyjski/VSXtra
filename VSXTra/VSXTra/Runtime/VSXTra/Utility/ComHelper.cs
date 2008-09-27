// ================================================================================================
// ComHelper.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================

using System;
using System.Runtime.InteropServices;

namespace VSXtra
{
  // ==============================================================================================
  /// <summary>
  /// This static class contains methods to help managing COM interop tasks in an easy and 
  /// consistent way.
  /// </summary>
  // ==============================================================================================
  public static class ComHelper
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a typed object for the specified base object.
    /// </summary>
    /// <typeparam name="T">Type of object to obtain for the base object.</typeparam>
    /// <param name="baseObject">Base object instance.</param>
    /// <returns>
    /// Object with the specified type, if typed object can be obtained; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static T GetTypedObject<T>(object baseObject)
    {
      T typedObject;
      var typeUnknown = Marshal.GetIUnknownForObject(baseObject);
      try
      {
        typedObject = (T)Marshal.GetTypedObjectForIUnknown(typeUnknown, typeof(T));
      }
      finally
      {
        if (typeUnknown != IntPtr.Zero)
        {
          Marshal.Release(typeUnknown);
        }
      }
      return typedObject;
    }
  }
}
