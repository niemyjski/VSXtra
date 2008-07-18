// ================================================================================================
// OptionsPageCustom.cs
//
// Created: 2008.07.18, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.OptionsPage
{
  // ================================================================================================
  /// <summary>
  /// Extends a standard dialog functionality for implementing ToolsOptions pages, with support for 
  /// the Visual Studio automation model, Windows Forms, and state persistence through the Visual 
  /// Studio settings mechanism.
  /// </summary>
  // ================================================================================================
  [Guid(GuidList.guidPageCustom)]
  public class OptionsPageCustom : DialogPage<OptionsPagePackage, OptionsCompositeControl>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the options page.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public OptionsPageCustom()
    {
      CustomBitmap = String.Empty;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets up the user control.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void OnPageCreated()
    {
      UIControl.OptionsPage = this;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Refreshes the image after the page data has been loaded.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void OnPageDataLoaded()
    {
      UIControl.RefreshImage();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the path to the image file.
    /// </summary>
    /// <remarks>The property that needs to be persisted.</remarks>
    // --------------------------------------------------------------------------------------------
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string CustomBitmap { get; set; }
  }
}
