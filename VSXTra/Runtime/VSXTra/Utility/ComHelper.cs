// ================================================================================================
// ComHelper.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;

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

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a typed object supporting the input interface for the specified base object.
    /// </summary>
    /// <param name="baseObject">Base object instance.</param>
    /// <param name="guidInterface">Interface requested for the base object.</param>
    /// <returns>
    /// Object with the specified type, if typed object can be obtained; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static IntPtr GetComObject(object baseObject, Guid guidInterface)
    {
      IntPtr outObject;
      var typeUnknown = Marshal.GetIUnknownForObject(baseObject);
      try
      {
        ErrorHandler.ThrowOnFailure(
          Marshal.QueryInterface(typeUnknown, ref guidInterface, out outObject));
      }
      finally
      {
        if (typeUnknown != IntPtr.Zero)
        {
          Marshal.Release(typeUnknown);
        }
      }
      return outObject;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Verifies that two objects represent the same instance of a COM object.
    /// This essentially compares the IUnkown pointers of the 2 objects.
    /// This is needed in scenario where aggregation is involved.
    /// </summary>
    /// <param name="obj1">Can be an object, interface or IntPtr</param>
    /// <param name="obj2">Can be an object, interface or IntPtr</param>
    /// <returns>True if the 2 items represent the same thing</returns>
    // --------------------------------------------------------------------------------------------
    public static bool IsSameComObject(object obj1, object obj2)
    {
      var isSame = false;
      var unknown1 = IntPtr.Zero;
      var unknown2 = IntPtr.Zero;
      try
      {
        // --- If we have 2 null, then they are not COM objects and as such 
        // --- "it's not the same COM object"
        if (obj1 != null && obj2 != null)
        {
          unknown1 = QueryInterfaceIUnknown(obj1);
          unknown2 = QueryInterfaceIUnknown(obj2);
          isSame = Equals(unknown1, unknown2);
        }
      }
      finally
      {
        if (unknown1 != IntPtr.Zero) Marshal.Release(unknown1);
        if (unknown2 != IntPtr.Zero) Marshal.Release(unknown2);
      }
      return isSame;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieve the IUnknown for the managed or COM object passed in.
    /// </summary>
    /// <param name="objToQuery">Managed or COM object.</param>
    /// <returns>Pointer to the IUnknown interface of the object.</returns>
    // --------------------------------------------------------------------------------------------
    internal static IntPtr QueryInterfaceIUnknown(object objToQuery)
    {
      var releaseIt = false;
      var unknown = IntPtr.Zero;
      IntPtr result;
      try
      {
        if (objToQuery is IntPtr)
        {
          unknown = (IntPtr)objToQuery;
        }
        else
        {
          // --- This is a managed object (or RCW)
          unknown = Marshal.GetIUnknownForObject(objToQuery);
          releaseIt = true;
        }

        // --- We might already have an IUnknown, but if this is an aggregated
        // --- object, it may not be THE IUnknown until we QI for it.				
        var IID_IUnknown = VSConstants.IID_IUnknown;
        ErrorHandler.ThrowOnFailure(Marshal.QueryInterface(unknown, ref IID_IUnknown, out result));
      }
      finally
      {
        if (releaseIt && unknown != IntPtr.Zero) Marshal.Release(unknown);
      }
      return result;
    }
  }
}
