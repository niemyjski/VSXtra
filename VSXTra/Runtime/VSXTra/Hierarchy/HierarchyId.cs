// ================================================================================================
// HierarchyId.cs
//
// Created: 2008.10.02, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.VisualStudio;

namespace VSXtra.Hierarchy
{
  // ================================================================================================
  /// <summary>
  /// This structure represents an ID used to identify the VS hierarchy items.
  /// </summary>
  // ================================================================================================
  [TypeConverter(typeof(HierarchyIdTypeConverter))]
  public struct HierarchyId
  {
    #region Private fields

    private readonly uint _Id;

    #endregion

    #region Static Fields

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This field represents the VSITEMID_ROOT identifier
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static HierarchyId Root = new HierarchyId(VSConstants.VSITEMID_ROOT);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This field represents the VSITEMID_NIL identifier
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static HierarchyId Nil = new HierarchyId(VSConstants.VSITEMID_NIL);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This field represents the VSITEMID_SELECTION identifier
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static HierarchyId Selection = new HierarchyId(VSConstants.VSITEMID_SELECTION);

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyId"/> struct.
    /// </summary>
    /// <param name="id">The id used to initialize the value of the instance.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyId(uint id)
    {
      _Id = id;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyId"/> struct.
    /// </summary>
    /// <param name="id">The id used to initialize the value of the instance.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyId(int id)
    {
      _Id = (uint)id;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the 32-bit unsigned value for this hierarchy ID.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint Value
    {
      get { return _Id; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating whether the ID represents the *Nil* value.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool IsNil
    {
      get { return _Id == Nil._Id; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating whether the ID represents the *Root* value.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool IsRoot
    {
      get { return _Id == Root._Id; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating whether the ID represents the *Selection* value.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool IsSelection
    {
      get { return _Id == Selection._Id; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating whether the ID represents a concrete ID.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool IsConcrete
    {
      get { return !IsSelection && !IsNil; }
    }

    #endregion

    #region Cast operators

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Performs an explicit conversion from <see cref="VSXtra.HierarchyId"/> to <see cref="System.Int32"/>.
    /// </summary>
    /// <param name="id">The value to convert.</param>
    /// <returns>The result of the conversion.</returns>
    // --------------------------------------------------------------------------------------------
    public static explicit operator int(HierarchyId id)
    {
      return (int) id._Id;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Performs an explicit conversion from <see cref="VSXtra.HierarchyId"/> to <see cref="System.UInt32"/>.
    /// </summary>
    /// <param name="id">The value to convert.</param>
    /// <returns>The result of the conversion.</returns>
    // --------------------------------------------------------------------------------------------
    public static explicit operator uint(HierarchyId id)
    {
      return id._Id;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="VSXtra.HierarchyId"/>.
    /// </summary>
    /// <param name="id">The value to convert.</param>
    /// <returns>The result of the conversion.</returns>
    // --------------------------------------------------------------------------------------------
    public static implicit operator HierarchyId(int id)
    {
      return new HierarchyId(id);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Performs an implicit conversion from <see cref="System.UInt32"/> to <see cref="VSXtra.HierarchyId"/>.
    /// </summary>
    /// <param name="id">The value to convert.</param>
    /// <returns>The result of the conversion.</returns>
    // --------------------------------------------------------------------------------------------
    public static implicit operator HierarchyId(uint id)
    {
      return new HierarchyId(id);
    }

    #endregion

    #region Conversion methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the string representation of this hierarchy item.
    /// </summary>
    /// <returns>
    /// A string represnting this hierarchy item.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public override string ToString()
    {
      if (_Id == VSConstants.VSITEMID_ROOT) return "ROOT";
      if (_Id == VSConstants.VSITEMID_NIL) return "NIL";
      if (_Id == VSConstants.VSITEMID_NIL) return "SELECTION";
      return _Id.ToString();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts the string representation of a hierarchy ID to HierarchyID equivalent. A return 
    /// value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="value">A string containing a hierarchy ID to convert.</param>
    /// <param name="id">
    /// When this method returns, contains the hierarchy ID value equivalent to the id contained 
    /// in *value*, if the conversion succeeded, or Nil if the conversion failed.</param>
    /// <returns>
    /// true if *value* was converted successfully; otherwise, false. 
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static bool TryParse(string value, out HierarchyId id)
    {
      if (String.Compare("root", value, true) == 0)
      {
        id = Root;
        return true;
      }
      if (String.Compare("nil", value, true) == 0)
      {
        id = Nil;
        return true;
      }
      if (String.Compare("selection", value, true) == 0)
      {
        id = Selection;
        return true;
      }
      int intId;
      if (Int32.TryParse(value, out intId))
      {
        id = new HierarchyId(intId);
        return true;
      }
      uint uintId;
      if (UInt32.TryParse(value, out uintId))
      {
        id = new HierarchyId(uintId);
        return true;
      }
      id = Nil;
      return false;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts the string representation of a hierarchy ID to HierarchyID equivalent.
    /// </summary>
    /// <param name="value">A string containing a hierarchy ID to convert.</param>
    /// <returns>
    /// The hierarchy ID value equivalent to the id contained in *value*.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static HierarchyId Parse(string value)
    {
      HierarchyId result;
      if (TryParse(value, out result)) return result;
      throw new InvalidCastException();
    }

    #endregion
  }

  #region HierarchyItemIdTypeConverter

  // ================================================================================================
  /// <summary>
  /// Helper class to convert hierachy item ID information to string.
  /// </summary>
  // ================================================================================================
  public class HierarchyIdTypeConverter : TypeConverter
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns whether this converter can convert an object of the given type to the type of this 
    /// converter, using the specified context.
    /// </summary>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="sourceType">
    /// A <see cref="Type"/> that represents the type you want to convert from.
    /// </param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string) || sourceType == typeof(int) || 
        sourceType == typeof(uint)) return true;
      return base.CanConvertFrom(context, sourceType);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns whether this converter can convert the object to the specified type, using 
    /// the specified context.
    /// </summary>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="destinationType">
    /// A <see cref="Type"/> that represents the type you want to convert to.
    /// </param>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(string) || destinationType == typeof(int) ||
        destinationType == typeof(uint)) return true;
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
      if (value is int) return new HierarchyId((int) value);
      if (value is uint) return new HierarchyId((uint) value);
      var strValue = value as string;
      if (strValue != null) return HierarchyId.Parse(strValue);
      return base.ConvertFrom(context, culture, value);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts the given value object to the specified type, using the specified context and 
    /// culture information.
    /// </summary>
    /// <param name="context">
    /// An <see cref="ITypeDescriptorContext"/> that provides a format context.
    /// </param>
    /// <param name="culture">
    /// A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed.
    /// </param>
    /// <param name="value">The <see cref="Object"/> to convert.</param>
    /// <param name="destinationType">
    /// The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.
    /// </param>
    /// <returns>
    /// An <see cref="Object"/> that represents the converted value.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <paramref name="destinationType"/> parameter is null.
    /// </exception>
    /// <exception cref="T:System.NotSupportedException">
    /// The conversion cannot be performed.
    /// </exception>
    // --------------------------------------------------------------------------------------------
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
      object value, Type destinationType)
    {
      if (!(value is HierarchyId)) return null;
      var hierValue = (HierarchyId) value;
      if (destinationType == typeof(int)) return (int)hierValue;
      if (destinationType == typeof(uint)) return (uint)hierValue;
      if (destinationType == typeof(string)) return hierValue.ToString();
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }

  #endregion
}