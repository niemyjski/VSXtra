// ================================================================================================
// HierarchyAttributes.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra.Hierarchy
{
  // ================================================================================================
  /// <summary>
  /// This attribute is used to set the bitmap used by a hierarchy node.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class HierarchyBitmapAttribute : StringAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyBitmapAttribute"/> class.
    /// </summary>
    /// <param name="value">Initial attribute value.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyBitmapAttribute(string value)
      : base(value)
    {
    }
  }

  // ================================================================================================
  /// <summary>
  /// This attribute is used to set the sort priority used by a hierarchy node
  /// </summary>
  // ================================================================================================
  public sealed class SortPriorityAttribute : Int32Attribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="SortPriorityAttribute"/> class.
    /// </summary>
    /// <param name="value">Initial attribute value.</param>
    // --------------------------------------------------------------------------------------------
    public SortPriorityAttribute(int value)
      : base(value)
    {
    }
  }
}