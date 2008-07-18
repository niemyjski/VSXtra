// ================================================================================================
// OptionsPageGeneral.cs
//
// Created: 2008.07.16, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VSXtra;

namespace DeepDiver.OptionsPage
{
  // ================================================================================================
  /// <summary>
  /// This class represents the General options page.
  /// </summary>
  // ================================================================================================
  [Guid(GuidList.guidPageGeneral)]
  public class OptionsPageGeneral : DialogPage<OptionsPagePackage>
  {
    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the options page.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public OptionsPageGeneral()
    {
      OptionString = "Hello World";
      OptionInteger = 567;
      CustomSize = new Size(50, 50);
    }

    #endregion

    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the String type custom option value.
    /// </summary>
    /// <remarks>
    /// The property that you want to be show in the options page.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    [Category("String Options"), Description("My string option")]
    public string OptionString { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the integer type custom option value.
    /// </summary>
    /// <remarks>
    /// The property that you want to be show in the options page.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    [Category("Integer Options")] 
    [Description("My integer option")]
    public int OptionInteger { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Size type custom option value.
    /// </summary>
    /// <remarks>
    /// The property that you want to be show in the options page.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    [Category("Expandable Options")]
    [Description("My Expandable option")]
    public Size CustomSize { get; set; }

    #endregion Properties

    #region Event Handlers

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles "Activate" messages from the Visual Studio environment.
    /// </summary>
    /// <remarks>
    /// This method is called when Visual Studio wants to activate this page. If the Cancel 
    /// property of the event is set to true, the page is not activated.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected override void OnActivate(CancelEventArgs e)
    {
      var result = VsMessageBox.Show(Resources.MessageOnActivateEntered, "OnActivate",
        MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
      if (result == DialogResult.Cancel)
      {
        Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, 
          "Cancelled the OnActivate event"));
        e.Cancel = true;
      }
      base.OnActivate(e);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles "Close" messages from the Visual Studio environment.
    /// </summary>
    /// <remarks>
    /// This event is raised when the page is closed.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected override void OnClosed(EventArgs e)
    {
      VsMessageBox.Show(Resources.MessageOnClosed);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles "Deactive" messages from the Visual Studio environment.
    /// </summary>
    /// <remarks>
    /// This method is called when VS wants to deactivate this page. If true is set for the Cancel
    /// property of the event, the page is not deactivated. A "Deactive" message is sent when a 
    /// dialog page's user interface window loses focus or is minimized but is not closed.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected override void OnDeactivate(CancelEventArgs e)
    {
      var result = VsMessageBox.Show(Resources.MessageOnDeactivateEntered, 
        "OnDeactivate", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
      if (result == DialogResult.Cancel)
      {
        Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, 
          "Cancelled the OnDeactivate event"));
        e.Cancel = true;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles Apply messages from the Visual Studio environment.
    /// </summary>
    /// <devdoc>
    /// This method is called when VS wants to save the user's 
    /// changes then the dialog is dismissed.
    /// </devdoc>
    // --------------------------------------------------------------------------------------------
    protected override void OnApply(PageApplyEventArgs e)
    {
      var result = VsMessageBox.Show(Resources.MessageOnApplyEntered);
      if (result == DialogResult.Cancel)
      {
        Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, 
          "Cancelled the OnApply event"));
        e.ApplyBehavior = PageApplyKind.Cancel;
      }
      else
      {
        base.OnApply(e);
      }
      VsMessageBox.Show(Resources.MessageOnApply);
    }

    #endregion Event Handlers
  }
}