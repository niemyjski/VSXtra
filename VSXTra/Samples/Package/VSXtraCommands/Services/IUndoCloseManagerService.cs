// ================================================================================================
// IUndoCloseManagerService.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.08.12, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DeepDiver.VSXtraCommands
{
  // ====================================================================================
  /// <summary>
  /// Service for managing closed documents
  /// </summary>
  // ====================================================================================
  [Guid("AF0C3D86-775F-4BEF-AB72-87D18E36873D")]
  [ComVisible(true)]
  public interface IUndoCloseManagerService
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the current document.
    /// </summary>
    /// <value>The current document.</value>
    // --------------------------------------------------------------------------------------------
    IUndoDocumentInfo CurrentUndoDocument { get; }

    /// <summary>
    /// Pushes an UndoDocument to the stack.
    /// </summary>
    /// <param name="undoDocument">The document.</param>
    // --------------------------------------------------------------------------------------------
    void PushDocument(IUndoDocumentInfo undoDocument);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Pops the topmost document from the stack.
    /// </summary>
    /// <returns>
    /// Document popped
    /// </returns>
    // --------------------------------------------------------------------------------------------
    IUndoDocumentInfo PopDocument();

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Pops a particular document from the stack.
    /// </summary>
    /// <returns>
    /// Document popped
    /// </returns>
    // --------------------------------------------------------------------------------------------
    IUndoDocumentInfo PopDocument(IUndoDocumentInfo undoDocument);

    // --------------------------------------------------------------------------------------------

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets all the documents.
    /// </summary>
    /// <returns></returns>
    // --------------------------------------------------------------------------------------------
    IEnumerable<IUndoDocumentInfo> GetDocuments();

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Clears the documents.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void ClearDocuments();
  }
}