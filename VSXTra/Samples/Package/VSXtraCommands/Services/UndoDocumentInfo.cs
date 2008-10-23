// ================================================================================================
// UndoDocumentInfo.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.08.12, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.IO;
using EnvDTE;
using VSXtra.Shell;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// Represents a document for the undo close command
  /// </summary>
  // ================================================================================================
  internal class UndoDocumentInfo : IUndoDocumentInfo
  {
    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the document path.
    /// </summary>
    /// <value>The document path.</value>
    // --------------------------------------------------------------------------------------------
    public string DocumentPath { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the cursor line.
    /// </summary>
    /// <value>The cursor line.</value>
    // --------------------------------------------------------------------------------------------
    public int CursorLine { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the cursor column.
    /// </summary>
    /// <value>The cursor column.</value>
    // --------------------------------------------------------------------------------------------
    public int CursorColumn { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the kind of the document view.
    /// </summary>
    /// <value>The kind of the document view.</value>
    // --------------------------------------------------------------------------------------------
    public string DocumentViewKind { get; set; }

    #endregion

    #region Constructors

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance with the specified property values.
    /// </summary>
    /// <param name="document">Document path</param>
    /// <param name="cursorLine">Cursor line number</param>
    /// <param name="cursorColumn">Cursor column number</param>
    /// <param name="viewKind">View kind of the document</param>
    // --------------------------------------------------------------------------------------------
    public UndoDocumentInfo(string document, int cursorLine, int cursorColumn, string viewKind)
    {
      DocumentPath = document;
      CursorLine = cursorLine;
      CursorColumn = cursorColumn;
      DocumentViewKind = viewKind;
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Opens the document covered by this instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void OpenDocument()
    {
      if (File.Exists(DocumentPath))
      {
        var window = VsIde.DteInstance.OpenFile(DocumentViewKind, DocumentPath);
        if (window != null)
        {
          window.Visible = true;
          window.Activate();

          if (CursorLine > 1 || CursorColumn > 1)
          {
            var selection = window.Document.Selection as TextSelection;

            if (selection != null)
            {
              //Move cursor
              selection.MoveTo(CursorLine, CursorColumn, true);
              //Clear selection
              selection.Cancel();
            }
          }
        }
      }
    }

    #endregion
  }
}