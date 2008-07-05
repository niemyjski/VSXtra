// ================================================================================================
// ToolWindowAttributes.cs
//
// Created by: Istvan Novak (DiveDeeper), 05/05/2008
// ================================================================================================
using System;
using VSXtra.Properties;

namespace VSXtra
{
  #region InitialCaptionAttribute

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

  #endregion

  #region BitmapResourceIdAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute can set the bitmap resource information of a tool window pane.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class BitmapResourceIdAttribute: Attribute
  {
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
      ResourceId = resourceId;
      BitmapIndex = bitmapIndex;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the resource id.
    /// </summary>
    /// <value>The resource id.</value>
    // --------------------------------------------------------------------------------------------
    public int ResourceId { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the index of the bitmap.
    /// </summary>
    /// <value>The index of the bitmap.</value>
    // --------------------------------------------------------------------------------------------
    public int BitmapIndex { get; private set; }
  }

  #endregion

  #region ToolbarLoacationAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute can set the toolbar location of a tool window pane.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ToolbarLocationAttribute : Attribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the toolbar location of the tool window pane.
    /// </summary>
    /// <param name="location">Location of the toolbar.</param>
    // --------------------------------------------------------------------------------------------
    public ToolbarLocationAttribute(ToolbarLocation location)
    {
      Location = location;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the toolbar location of a tool window pane.
    /// </summary>
    /// <value>Toolbar location of a tool window pane.</value>
    // --------------------------------------------------------------------------------------------
    public ToolbarLocation Location { get; private set; }
  }

  #endregion

  #region ToolbarAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute can set the toolbar of a tool window pane.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ToolbarAttribute : TypeAttribute
  {
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of this attribute and sets its initial value.
    /// </summary>
    /// <param name="value">Toolbar type belonging to the tool window.</param>
    // --------------------------------------------------------------------------------
    public ToolbarAttribute(Type value)
      : base(value)
    {
      if (!typeof(IToolbarProvider).IsAssignableFrom(value))
      {
        throw new ArgumentException(Resources.IToolbarProvider_Expected);
      }
    }
  }

  #endregion
}
