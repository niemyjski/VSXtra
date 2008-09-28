// ================================================================================================
// HierarchyPropertyWrapper.cs
//
// Created: 2008.09.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.ComponentModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to wrap properties of an IVsHierarchy into a class in order to display
  /// in the Properties window
  /// </summary>
  // ================================================================================================
  public class HierarchyPropertyWrapper
  {
    #region Private fields

    private readonly IVsHierarchy _Hierarchy;
    private readonly uint _ItemId;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of this class wrapping around the specified hierarchy item.
    /// </summary>
    /// <param name="hierarchy">The hierarchy to wrap.</param>
    /// <param name="itemId">The item id in the hierarchy.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyPropertyWrapper(IVsHierarchy hierarchy, uint itemId)
    {
      _Hierarchy = hierarchy;
      _ItemId = itemId;
    }

    #endregion

    #region Properties

    [DisplayName("Parent Node ID")]
    [Description("Item ID of the parent node; VSITEMID_NIL, if no parent.")]
    public int ParentId
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_Parent); }
    }

    [DisplayName("Name")]
    [Description("Name of the item.")]
    public string Name
    {
      get { return GetProperty<string>(__VSHPROPID.VSHPROPID_Name); }
    }

    #endregion

    #region Helper methods

    private T GetProperty<T>(__VSHPROPID propId)
    {
      return (T) GetProperty(propId);
    }

    private T GetProperty<T>(int propId)
    {
      return (T) GetProperty(propId);
    }

    private object GetProperty(__VSHPROPID propId)
    {
      return GetProperty((int) propId);
    }

    private object GetProperty(int propId)
    {
      if (propId == (int)__VSHPROPID.VSHPROPID_NIL) return null;
      object propValue;
      _Hierarchy.GetProperty(_ItemId, propId, out propValue);
      return propValue;
    }

    #endregion
  }
}