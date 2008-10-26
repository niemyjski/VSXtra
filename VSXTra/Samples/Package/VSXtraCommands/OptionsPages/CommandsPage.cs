// ================================================================================================
// CommandsPage.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.24, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using VSXtra.Windows;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// Option page for enabling/disabling commands
  /// </summary>
  // ================================================================================================
  [ComVisible(true)]
  [Guid("B5E0BF81-7319-49d8-BA6C-071B51D0F06C")]
  public class CommandsPage : DialogPage<VSXtraCommandsPackage, CommandsControl>
  {
    #region Fields

    private IList<CommandID> _DisabledCommands = new List<CommandID>();

    #endregion

    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the disabled commands.
    /// </summary>
    /// <value>The disabled commands.</value>
    // --------------------------------------------------------------------------------------------
    [TypeConverter(typeof (DisabledCommandsDictionaryConverter))]
    public IList<CommandID> DisabledCommands
    {
      get { return _DisabledCommands; }
      set { _DisabledCommands = value; }
    }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Assigns this options page with the related UI.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void OnPageCreated()
    {
      UIControl.OptionPage = this;
    }

    #endregion
  }
}