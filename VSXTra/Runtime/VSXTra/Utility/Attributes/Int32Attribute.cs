// ================================================================================================
// Int32Attribute.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  // ==================================================================================
  /// <summary>
  /// This abstract class defines an attribute with a simple Int32 value.
  /// </summary>
  /// <remarks>
  /// The class is intended to derive new attributes having a simple string value. 
  /// Do not use this class to add other properties to the attribute!
  /// </remarks>
  // ==================================================================================
  public abstract class Int32Attribute : Attribute
  {
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of this attribute and sets its initial value.
    /// </summary>
    /// <param name="value">Initial attribute value.</param>
    // --------------------------------------------------------------------------------
    protected Int32Attribute(int value)
    {
      Value = value;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of this attribute.
    /// </summary>
    // --------------------------------------------------------------------------------
    public int Value { get; private set; }
  }
}