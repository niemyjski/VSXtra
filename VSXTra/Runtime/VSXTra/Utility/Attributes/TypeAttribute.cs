// ================================================================================================
// TypeAttribute.cs
//
// Created: 2008.07.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  // ==================================================================================
  /// <summary>
  /// This abstract class defines an attribute with a simple System.Type value.
  /// </summary>
  /// <remarks>
  /// The class is intended to derive new attributes having a simple Syetem.Type value. 
  /// Do not use this class to add other properties to the attribute!
  /// </remarks>
  // ==================================================================================
  public abstract class TypeAttribute: Attribute
  {
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of this attribute and sets its initial value.
    /// </summary>
    /// <param name="value">Initial attribute value.</param>
    // --------------------------------------------------------------------------------
    protected TypeAttribute(Type value)
    {
      Value = value;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of this attribute.
    /// </summary>
    // --------------------------------------------------------------------------------
    public Type Value { get; private set; }
  }
}