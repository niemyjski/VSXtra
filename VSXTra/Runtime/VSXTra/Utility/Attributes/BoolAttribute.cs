// ================================================================================================
// BoolAttribute.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================

using System;

namespace VSXtra
{
  // ==================================================================================
  /// <summary>
  /// This abstract class defines an attribute with a simple boolean value.
  /// </summary>
  /// <remarks>
  /// The class is intended to derive new attributes having a simple bool value. 
  /// Do not use this class to add other properties to the attribute!
  /// </remarks>
  // ==================================================================================
  public abstract class BoolAttribute: Attribute
  {
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of this attribute and sets its initial value.
    /// </summary>
    /// <param name="value">Initial attribute value.</param>
    // --------------------------------------------------------------------------------
    protected BoolAttribute(bool value)
    {
      Value = value;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of this attribute.
    /// </summary>
// --------------------------------------------------------------------------------
    public bool Value { get; private set; }
  }
}
