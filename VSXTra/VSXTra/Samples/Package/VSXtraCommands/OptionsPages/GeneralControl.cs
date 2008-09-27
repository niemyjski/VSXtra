// ================================================================================================
// GeneralControl.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.19, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Windows.Forms;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// UserControl representing the UI of the General options page
  /// </summary>
  // ================================================================================================
  public partial class GeneralControl : UserControl
  {
    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the option page.
    /// </summary>
    /// <value>The option page.</value>
    // --------------------------------------------------------------------------------------------
    public GeneralPage OptionPage { get; set; }

    #endregion

    #region Constructors

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralControl"/> class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public GeneralControl()
    {
      InitializeComponent();
    }

    #endregion

    #region Event Handlers

    private void chkFormatOnSave_CheckedChanged(object sender, EventArgs e)
    {
      OptionPage.FormatOnSave = chkFormatOnSave.Checked;
    }

    private void RemoveAndSortUsingsOnSave_CheckedChanged(object sender, EventArgs e)
    {
      OptionPage.RemoveAndSortUsingsOnSave = chkRemoveAndSortUsingsOnSave.Checked;
    }

    private void GeneralControl_Load(object sender, EventArgs e)
    {
      chkFormatOnSave.Checked = OptionPage.FormatOnSave;
      chkRemoveAndSortUsingsOnSave.Checked = OptionPage.RemoveAndSortUsingsOnSave;
    }

    #endregion
  }
}