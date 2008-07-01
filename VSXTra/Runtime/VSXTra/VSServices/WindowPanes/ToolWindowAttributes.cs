// ================================================================================================
// ToolWindowAttributes.cs
//
// Created by: Istvan Novak (DiveDeeper), 05/05/2008
// ================================================================================================
using System;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This attribute can set the initial caption of a tool window pane.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class InitialCaptionAttribute: StringAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="InitialCaptionAttribute"/> class.
    /// </summary>
    /// <param name="value">Initial attribute value.</param>
    // --------------------------------------------------------------------------------------------
    public InitialCaptionAttribute(string value)
      : base(value)
    {
    }
  }

  // ================================================================================================
  /// <summary>
  /// This attribute can set the bitmap resource information of a tool window pane.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class BitmapResourceIdAttribute: Attribute
  {
    private readonly int _ResourceId;
    private readonly int _BitmapIndex;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapResourceIdAttribute"/> class.
    /// </summary>
    /// <param name="resourceId">The resource id used by the bitmap.</param>
    // --------------------------------------------------------------------------------------------
    public BitmapResourceIdAttribute(int resourceId) :
      this (resourceId, 1)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapResourceIdAttribute"/> class.
    /// </summary>
    /// <param name="resourceId">The resource id.</param>
    /// <param name="bitmapIndex">Index of the bitmap.</param>
    // --------------------------------------------------------------------------------------------
    public BitmapResourceIdAttribute(int resourceId, int bitmapIndex)
    {
      _ResourceId = resourceId;
      _BitmapIndex = bitmapIndex;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the resource id.
    /// </summary>
    /// <value>The resource id.</value>
    // --------------------------------------------------------------------------------------------
    public int ResourceId
    {
      get { return _ResourceId; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the index of the bitmap.
    /// </summary>
    /// <value>The index of the bitmap.</value>
    // --------------------------------------------------------------------------------------------
    public int BitmapIndex
    {
      get { return _BitmapIndex; }
    }
  }
}
