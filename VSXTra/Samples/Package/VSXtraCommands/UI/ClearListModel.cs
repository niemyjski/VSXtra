// ================================================================================================
// ClearListModel.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using VSXtra;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// Class that represents the model for the ClearListView
  /// </summary>
  // ================================================================================================
  public class ClearListModel
  {
    #region Constructors

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="ClearListModel"/> class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ClearListModel()
    {
      ListEntries = new List<FileEntry>();
      SelectedListEntries = new List<FileEntry>();
    }
    
    #endregion

    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the list entries.
    /// </summary>
    /// <value>The list entries.</value>
    // --------------------------------------------------------------------------------------------
    public List<FileEntry> ListEntries { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the selected list entries.
    /// </summary>
    /// <value>The selected list entries.</value>
    // --------------------------------------------------------------------------------------------
    public List<FileEntry> SelectedListEntries { get; set; }
    
    #endregion
  }
}