// ================================================================================================
// HierarchyProperty.cs
//
// Created: 2008.10.03, by Istvan Novak (DeepDiver)
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
  /// This class represents a property of a hierarchy item that can be undefined for a certain 
  /// hierarchy instance.
  /// </summary>
  /// <typeparam name="T">Storage type</typeparam>
  // ================================================================================================
  public class HierarchyProperty<T>
  {
    #region Protected fields

    protected readonly HierarchyItem _Item;
    protected readonly int _PropId;
    protected bool _Defined;
    protected T _Value;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyProperty{T}"/> class.
    /// </summary>
    /// <param name="item">The item this property belongs to.</param>
    /// <param name="propId">The id of the property.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyProperty(HierarchyItem item, int propId)
    {
      _Item = item;
      _PropId = propId;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyProperty{T}"/> class.
    /// </summary>
    /// <param name="item">The item this property belongs to.</param>
    /// <param name="propId">The id of the property.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyProperty(HierarchyItem item, __VSHPROPID propId)
    {
      _Item = item;
      _PropId = (int)propId;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a flag indicating whether this <see cref="HierarchyProperty&lt;T&gt;"/> is defined.
    /// </summary>
    /// <value><c>true</c> if defined; otherwise, <c>false</c>.</value>
    // --------------------------------------------------------------------------------------------
    public bool Defined
    {
      get
      {
        GetValue();
        return _Defined;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of this property.
    /// </summary>
    /// <value>The value of this property.</value>
    // --------------------------------------------------------------------------------------------
    public T Value
    {
      get
      {
        GetValue();
        return _Value;
      }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Flag indicating if the property value is defined or not.</returns>
    // --------------------------------------------------------------------------------------------
    public bool QueryValue(out T value)
    {
      GetValue();
      value = _Value;
      return _Defined;
    }

    #endregion 

    #region Helper methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the property and checks if that is defined or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void GetValue()
    {
      object propValue;
      if (_Item.Hierarchy.GetProperty(_Item.Id.Value, _PropId, out propValue)
          == VSConstants.DISP_E_MEMBERNOTFOUND || propValue == null)
      {
        _Defined = false;
        _Value = default(T);
      }
      else
      {
        _Defined = true;
        _Value = (T) propValue;
      }
    }

    #endregion
  }

  #region HandleProperty and its type converter

  // ================================================================================================
  /// <summary>
  /// Defines a type representing an optional handle property.
  /// </summary>
  // ================================================================================================
  [TypeConverter(typeof(HandlePropertyTypeConverter))]
  public sealed class HandleProperty: HierarchyProperty<int>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HandleProperty"/> class.
    /// </summary>
    /// <param name="item">The item this property belongs to.</param>
    /// <param name="propId">The id of the property.</param>
    // --------------------------------------------------------------------------------------------
    public HandleProperty(HierarchyItem item, int propId)
      : base(item, propId)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HandleProperty"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="propId">The prop id.</param>
    // --------------------------------------------------------------------------------------------
    public HandleProperty(HierarchyItem item, __VSHPROPID propId)
      : base(item, propId)
    {
    }
  }

  // ================================================================================================
  /// <summary>
  /// Type converter for the handle property.
  /// </summary>
  // ================================================================================================
  public sealed class HandlePropertyTypeConverter : HierarchyPropertyTypeConverter<int>
  {
  }

  #endregion

  #region GuidProperty and its type converter

  // ================================================================================================
  /// <summary>
  /// Defines a type representing an optional Guid property.
  /// </summary>
  // ================================================================================================
  [TypeConverter(typeof(GuidPropertyTypeConverter))]
  public sealed class GuidProperty : HierarchyProperty<Guid>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="GuidProperty"/> class.
    /// </summary>
    /// <param name="item">The item this property belongs to.</param>
    /// <param name="propId">The id of the property.</param>
    // --------------------------------------------------------------------------------------------
    public GuidProperty(HierarchyItem item, int propId)
      : base(item, propId)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="GuidProperty"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="propId">The prop id.</param>
    // --------------------------------------------------------------------------------------------
    public GuidProperty(HierarchyItem item, __VSHPROPID propId)
      : base(item, propId)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the property and checks if that is defined or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void GetValue()
    {
      Guid propValue;
      if (_Item.Hierarchy.GetGuidProperty(_Item.Id.Value, _PropId, out propValue)
          == VSConstants.DISP_E_MEMBERNOTFOUND || propValue == Guid.Empty)
      {
        _Defined = false;
        _Value = Guid.Empty;
      }
      else
      {
        _Defined = true;
        _Value = propValue;
      }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Type converter for the Guid property.
  /// </summary>
  // ================================================================================================
  public sealed class GuidPropertyTypeConverter : HierarchyPropertyTypeConverter<Guid>
  {
  }

  #endregion

  #region HierarchyPropertyTypeConverter

  // ================================================================================================
  /// <summary>
  /// Abstract type converter class for hierarchy properties.
  /// </summary>
  /// <typeparam name="T">Storage type</typeparam>
  // ================================================================================================
  public abstract class HierarchyPropertyTypeConverter<T> : TypeConverter
  {
    /// <summary>String used to describe the "undefined" value</summary>
    public const string UndefinedValue = "Undefined";

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns whether this converter can convert an object of the given type to the type of 
    /// this converter, using the specified context.
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
      if (sourceType == typeof(string)) return true;
      return base.CanConvertFrom(context, sourceType);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns whether this converter can convert the object to the specified type, using the 
    /// specified context.
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
      if (destinationType == typeof(string)) return true;
      return base.CanConvertTo(context, destinationType);
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
    /// An <see cref="T:System.Object"/> that represents the converted value.
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
      if (destinationType == typeof(string))
      {
        var propValue = value as HierarchyProperty<T>;
        if (propValue != null)
        {
          return propValue.Defined
                   ? propValue.Value.ToString()
                   : UndefinedValue;
        }
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }

  #endregion
}