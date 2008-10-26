// ================================================================================================
// UndoCloseManagerService.cs
//
// Created: 2008.08.12, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using VSXtra;

namespace DeepDiver.VSXtraCommands
{
  internal class UndoCloseManagerService : IUndoCloseManagerService, SUndoCloseManagerService
  {
    #region Fields

    private const int capacity = 20;

    private FixedCapacityStack<IUndoDocumentInfo> documents;

    #endregion

    #region Constructors

    public UndoCloseManagerService()
    {
      documents = new FixedCapacityStack<IUndoDocumentInfo>(capacity);
    }

    #endregion

    #region Public Implementation

    public void PushDocument(IUndoDocumentInfo undoDocument)
    {
      IUndoDocumentInfo docInfo =
        documents.SingleOrDefault(
          info => info.DocumentPath.Equals(undoDocument.DocumentPath));

      if (docInfo != null)
      {
        var temp =
          new FixedCapacityStack<IUndoDocumentInfo>(capacity);

        documents.Reverse().ForEach(
          info =>
            {
              if (info.DocumentPath != undoDocument.DocumentPath)
              {
                temp.Push(info);
              }
            });

        temp.Push(undoDocument);
        documents = temp;
      }
      else
      {
        documents.Push(undoDocument);
      }
    }

    public IUndoDocumentInfo PopDocument()
    {
      try
      {
        return documents.Pop();
      }
      catch (InvalidOperationException)
      {
        return null;
      }
    }

    public IUndoDocumentInfo PopDocument(IUndoDocumentInfo undoDocument)
    {
      IUndoDocumentInfo docInfo =
        documents.SingleOrDefault(
          info => info.DocumentPath.Equals(undoDocument.DocumentPath));

      if (docInfo != null)
      {
        var temp =
          new FixedCapacityStack<IUndoDocumentInfo>(capacity);

        documents.Reverse().ForEach(
          info =>
            {
              if (info.DocumentPath != undoDocument.DocumentPath)
              {
                temp.Push(info);
              }
            });

        documents = temp;
      }

      return docInfo;
    }

    public IUndoDocumentInfo CurrentUndoDocument
    {
      get
      {
        try
        {
          return documents.Peek();
        }
        catch (InvalidOperationException)
        {
          return null;
        }
      }
    }

    public void ClearDocuments()
    {
      documents.Clear();
    }

    public IEnumerable<IUndoDocumentInfo> GetDocuments()
    {
      return documents;
    }

    #endregion
  }
}