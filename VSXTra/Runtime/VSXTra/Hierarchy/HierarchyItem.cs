// ================================================================================================
// HierarchyItem.cs
//
// Created: 2008.09.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to wrap properties of an IVsHierarchy into a class in order to display
  /// in the Properties window
  /// </summary>
  // ================================================================================================
  public class HierarchyItem
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
    public HierarchyItem(IVsHierarchy hierarchy, uint itemId)
    {
      _Hierarchy = hierarchy;
      _ItemId = itemId;
    }

    #endregion

    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the hierarchy object.
    /// </summary>
    /// <value>The hierarchy object.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public IVsHierarchy Hierarchy
    {
      get { return _Hierarchy; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the item id of this node.
    /// </summary>
    /// <value>The item id of this node.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("ID")]
    [TypeConverter(typeof(HierarchyItemIdTypeConverter))]
    [Description("Item ID within the hierarchy.")]
    public uint ItemId
    {
      get { return _ItemId; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent id of this item within the hierarchy.
    /// </summary>
    /// <value>The parent id within the hierarchy.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Parent Node ID")]
    [TypeConverter(typeof(HierarchyItemIdTypeConverter))]
    [Description("Item ID of the parent node; VSITEMID_NIL, if no parent.")]
    public int ParentId
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_Parent); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the name of this item.
    /// </summary>
    /// <value>The name of this item.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Name")]
    [Description("Name of the item.")]
    public string Name
    {
      get { return GetProperty<string>(__VSHPROPID.VSHPROPID_Name); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy of this item.
    /// </summary>
    /// <value>The parent hierarchy.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public HierarchyItem ParentHierarchy
    {
      get
      {
        var parentHierarchy = GetProperty(__VSHPROPID.VSHPROPID_ParentHierarchy) as IVsHierarchy;
        return parentHierarchy == null 
          ? null
          : new HierarchyItem(parentHierarchy, VSConstants.VSITEMID_ROOT);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy item id.
    /// </summary>
    /// <value>The parent hierarchy item id.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Item ID in Parent Hierarchy")]
    [Description("If the current hierachy is nested into a perant hierarchy, this value tells the ID used in the parent hierarchy.")]
    public uint ParentHierarchyItemId
    {
      get
      {
        object id = GetProperty(__VSHPROPID.VSHPROPID_ParentHierarchyItemid);
        if (id is int) return (uint) (int) id;
        if (id is uint) return (uint) id;
        return 0;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a value indicating whether this instance is nested hierachy.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is nested hierachy; otherwise, <c>false</c>.
    /// </value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Is Nested Hierarchy?")]
    [Description("This flag tells if this hierarchy item is nested into an owner hierarchy or not.")]
    public bool IsNestedHierachy
    {
      get { return ItemId == VSConstants.VSITEMID_ROOT && ParentHierarchyItemId != 0; }
    }

    [DisplayName("First Child ID")]
    [TypeConverter(typeof(HierarchyItemIdTypeConverter))]
    [Description("Item ID of the parent node; VSITEMID_NIL, if no parent.")]
    public int FirstChild
    {
      get { return GetProperty<int>(__VSHPROPID.VSHPROPID_FirstChild); }
    }

    #endregion

    #region Helper methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    private T GetProperty<T>(__VSHPROPID propId)
    {
      return (T) GetProperty(propId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    private T GetProperty<T>(int propId)
    {
      return (T) GetProperty(propId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    private object GetProperty(__VSHPROPID propId)
    {
      return GetProperty((int) propId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    private object GetProperty(int propId)
    {
      if (propId == (int)__VSHPROPID.VSHPROPID_NIL) return null;
      object propValue;
      _Hierarchy.GetProperty(_ItemId, propId, out propValue);
      return propValue;
    }

    #endregion
  }

  #region HierarchyItemIdTypeConverter

  // ================================================================================================
  /// <summary>
  /// Helper class to convert hierachy item ID information to string.
  /// </summary>
  // ================================================================================================
  public class HierarchyItemIdTypeConverter : TypeConverter
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts the specified id to its string representation.
    /// </summary>
    /// <param name="id">The id to convert.</param>
    /// <returns>String representation of the ID.</returns>
    // --------------------------------------------------------------------------------------------
    public static string AsString(uint id)
    {
      if (id == VSConstants.VSITEMID_ROOT) return "ROOT";
      if (id == VSConstants.VSITEMID_NIL) return "NIL";
      if (id == VSConstants.VSITEMID_NIL) return "SELECTION";
      return id.ToString();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts the specified id to its string representation.
    /// </summary>
    /// <param name="id">The id to convert.</param>
    /// <returns>String representation of the ID.</returns>
    // --------------------------------------------------------------------------------------------
    public static string AsString(int id)
    {
      return AsString((uint) id);
    }

    /// <summary>
    /// Gets a value indicating whether this converter can convert an object in the given source type to a string using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you wish to convert from.</param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string)) return true;
      return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(string)) return true;
      return base.CanConvertTo(context, destinationType);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts the specified value object to a <see cref="T:System.String"/> object.
    /// </summary>
    /// <param name="context">
    /// An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="culture">
    /// The <see cref="T:System.Globalization.CultureInfo"/> to use.
    /// </param>
    /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
    /// <returns>
    /// An <see cref="T:System.Object"/> that represents the converted value.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">
    /// The conversion could not be performed.
    /// </exception>
    // --------------------------------------------------------------------------------------------
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value is int || value is uint)
      {
        return AsString((uint) value);
      }
      return base.ConvertFrom(context, culture, value);
    }
  }

  #endregion
}