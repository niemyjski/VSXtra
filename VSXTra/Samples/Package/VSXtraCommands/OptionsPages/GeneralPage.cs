// ================================================================================================
// GeneralPage.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.19, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra.Windows;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// This Options page represents the general settings of VSXtraCommands
  /// </summary>
  // ================================================================================================
  [ComVisible(true)]
  [Guid("DF0D89F1-C9A3-47BF-B277-42E0C178F1A0")]
  public class GeneralPage : DialogPage<VSXtraCommandsPackage, GeneralControl>
  {
    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether [format on save].
    /// </summary>
    /// <value><c>true</c> if [format on save]; otherwise, <c>false</c>.</value>
    // --------------------------------------------------------------------------------------------
    public bool FormatOnSave { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether [remove and sort usings on save].
    /// </summary>
    /// <value>
    /// 	<c>true</c> if [remove and sort usings on save]; otherwise, <c>false</c>.
    /// </value>
    // --------------------------------------------------------------------------------------------
    public bool RemoveAndSortUsingsOnSave { get; set; }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets up the options page property of the user control.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void OnPageCreated()
    {
      UIControl.OptionPage = this;
    }

    #endregion
  }
}