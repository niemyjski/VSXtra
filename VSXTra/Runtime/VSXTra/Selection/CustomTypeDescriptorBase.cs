// ================================================================================================
// CustomTypeDescriptorBase.cs
//
// Created: 2008.07.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;

namespace VSXtra.Selection
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to be a base class for type descriptors related to selection tracking
  /// in the Properties window.
  /// </summary>
  /// <remarks>
  /// This base class forwards all the ICustomTypeDescriptor calls to the default TypeDescriptor, 
  /// except for GetComponentName. This allows for a class to specify the name that will be 
  /// displayed in the combo box of the Properties window.
  /// </remarks>
  // ================================================================================================
  public abstract class CustomTypeDescriptorBase : ICustomTypeDescriptor
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Name of the component.
    /// </summary>
    /// <remarks>
    /// When this class is used to expose property in the Properties window, this should be the 
    /// name associated with this instance.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected abstract string ComponentName { get; }

    #region ICustomTypeDescriptor Members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns a collection of custom attributes for this instance of a component.
    /// </summary>
    /// <returns>
    /// An AttributeCollection containing the attributes for this object.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return TypeDescriptor.GetAttributes(GetType());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the class name of this instance of a component.
    /// </summary>
    /// <returns>
    /// The class name of the object, or null if the class does not have a name.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    string ICustomTypeDescriptor.GetClassName()
    {
      return TypeDescriptor.GetClassName(GetType());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of this instance of a component.
    /// </summary>
    /// <returns>
    /// The name of the object, or null if the object does not have a name.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    string ICustomTypeDescriptor.GetComponentName()
    {
      return ComponentName;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns a type converter for this instance of a component.
    /// </summary>
    /// <returns>
    /// A TypeConverter that is the converter for this object, or null if there is no 
    /// TypeConverter for this object.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
      return TypeDescriptor.GetConverter(GetType());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the default event for this instance of a component.
    /// </summary>
    /// <returns>
    /// An EventDescriptor that represents the default event for this object, or null if this 
    /// object does not have events.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent(GetType());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the default property for this instance of a component.
    /// </summary>
    /// <returns>
    /// A PropertyDescriptor that represents the default property for this object, or null if this 
    /// object does not have properties.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty(GetType());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns an editor of the specified type for this instance of a component.
    /// </summary>
    /// <returns>
    /// An Object of the specified type that is the editor for this object, or null if the editor 
    /// cannot be found.
    /// </returns>
    /// <param name="editorBaseType">
    /// A Type that represents the editor for this object.
    /// </param>
    // --------------------------------------------------------------------------------------------
    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor(GetType(), editorBaseType);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the events for this instance of a component.
    /// </summary>
    /// <returns>
    /// An EventDescriptorCollection that represents the events for this component instance.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
      return TypeDescriptor.GetEvents(GetType());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the events for this instance of a component using the specified attribute array 
    /// as a filter.
    /// </summary>
    /// <returns>
    /// An EventDescriptorCollection that represents the filtered events for this component instance.
    /// </returns>
    /// <param name="attributes">
    /// An array of type Attribute that is used as a filter.
    /// </param>
    // --------------------------------------------------------------------------------------------
    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents(GetType(), attributes);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the properties for this instance of a component.
    /// </summary>
    /// <returns>
    /// A PropertyDescriptorCollection that represents the properties for this component instance.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      return TypeDescriptor.GetProperties(GetType());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the properties for this instance of a component using the attribute array as a filter.
    /// </summary>
    /// <returns>
    /// A PropertyDescriptorCollection that represents the filtered properties for this component 
    /// instance.
    /// </returns>
    /// <param name="attributes">
    /// An array of type Attribute that is used as a filter.
    /// </param>
    // --------------------------------------------------------------------------------------------
    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
    {
      return TypeDescriptor.GetProperties(GetType(), attributes);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns an object that contains the property described by the specified property 
    /// descriptor.
    /// </summary>
    /// <returns>
    /// An Object that represents the owner of the specified property.
    /// </returns>
    /// <param name="pd">
    /// A PropertyDescriptor that represents the property whose owner is to be found.
    /// </param>
    // --------------------------------------------------------------------------------------------
    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return this;
    }

    #endregion
  }
}