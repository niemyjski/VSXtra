// ================================================================================================
// CommandRowItem.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.19, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.ComponentModel;
using System.ComponentModel.Design;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// This class that represents a row in a grid containing Command items on the Commands options 
  /// page.
  /// </summary>
  // ================================================================================================
  public class CommandRowItem
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the command text.
    /// </summary>
    /// <value>The command text.</value>
    // --------------------------------------------------------------------------------------------
    [DisplayName("Command")]
    public string CommandText { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="CommandRowItem"/> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    // --------------------------------------------------------------------------------------------
    public bool Enabled { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>The command.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public CommandID Command { get; set; }
  }
}