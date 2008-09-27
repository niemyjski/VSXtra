using System;

namespace VSXtra
{
  // ==================================================================================
  /// <summary>
  /// This abstract class defines an attribute with a simple string value.
  /// </summary>
  /// <remarks>
  /// The class is intended to derive new attributes having a simple string value. 
  /// Do not use this class to add other properties to the attribute!
  /// </remarks>
  // ==================================================================================
  public abstract class StringAttribute : Attribute
  {
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of this attribute and sets its initial value.
    /// </summary>
    /// <param name="value">Initial attribute value.</param>
    // --------------------------------------------------------------------------------
    protected StringAttribute(string value)
    {
      Value = value;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of this attribute.
    /// </summary>
    // --------------------------------------------------------------------------------
    public string Value { get; private set; }
  }
}