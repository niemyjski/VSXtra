// ================================================================================================
// IUndoDocumentInfo.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.08.12, by Istvan Novak (DeepDiver)
// ================================================================================================
namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// Interface that represents a document for the undo close command
  /// </summary>
  // ================================================================================================
  public interface IUndoDocumentInfo
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the document path.
    /// </summary>
    /// <value>The document path.</value>
    // --------------------------------------------------------------------------------------------
    string DocumentPath { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the cursor line.
    /// </summary>
    /// <value>The cursor line.</value>
    // --------------------------------------------------------------------------------------------
    int CursorLine { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the cursor column.
    /// </summary>
    /// <value>The cursor column.</value>
    // --------------------------------------------------------------------------------------------
    int CursorColumn { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the kind of the document view.
    /// </summary>
    /// <value>The kind of the document view.</value>
    // --------------------------------------------------------------------------------------------
    string DocumentViewKind { get; set; }
  }
}