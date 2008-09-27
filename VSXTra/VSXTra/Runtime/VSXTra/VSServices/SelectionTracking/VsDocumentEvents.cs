// ================================================================================================
// VsDocumentEvents.cs
//
// Created: 2008.08.12, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using EnvDTE;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class provides access to document events.
  /// </summary>
  // ================================================================================================
  public static class VsDocumentEvents
  {
    #region Private fields

    private static readonly DocumentEvents _Events;
    private static int _ClosingEventCounter;
    private static EventHandler<DocumentEventArgs> _Closing;
    private static int _OpenedEventCounter;
    private static EventHandler<DocumentEventArgs> _Opened;
    private static int _OpeningEventCounter;
    private static EventHandler<DocumentOpeningEventArgs> _Opening;
    private static int _SavedEventCounter;
    private static EventHandler<DocumentEventArgs> _Saved;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the event listener.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    static VsDocumentEvents()
    {
      _Events = VsIde.GetObject<DocumentEvents>("DocumentEvents");
      _ClosingEventCounter = 0;
      _OpenedEventCounter = 0;
      _OpeningEventCounter = 0;
      _SavedEventCounter = 0;
    }

    #endregion

    #region Event properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when a document is about to be closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<DocumentEventArgs> OnDocumentClosing
    {
      add 
      { 
        ProvideClosingEvent();
        _Closing += value;
      }
      remove
      {
        _Closing -= value;
        RemoveClosingEvent();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when a document is opened.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<DocumentEventArgs> OnDocumentOpened
    {
      add
      {
        ProvideOpenedEvent();
        _Opened += value;
      }
      remove
      {
        _Opened -= value;
        RemoveOpenedEvent();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when a document is about to be opened.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<DocumentOpeningEventArgs> OnDocumentOpening
    {
      add
      {
        ProvideOpeningEvent();
        _Opening += value;
      }
      remove
      {
        _Opening -= value;
        RemoveOpeningEvent();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when a document is saved.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<DocumentEventArgs> OnDocumentSaved
    {
      add
      {
        ProvideSavedEvent();
        _Saved += value;
      }
      remove
      {
        _Saved -= value;
        RemoveSavedEvent();
      }
    }

    #endregion

    #region Event utility methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Provides a closing event handler
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void ProvideClosingEvent()
    {
      if (_ClosingEventCounter == 0)
      {
        _Events.DocumentClosing += HandleClosingEvent;
      }
      _ClosingEventCounter++;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Safely removes the closing event handler
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void RemoveClosingEvent()
    {
      _ClosingEventCounter--;
      if (_ClosingEventCounter == 0)
      {
        _Events.DocumentClosing -= HandleClosingEvent;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the closing event
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void HandleClosingEvent(Document Document)
    {
      if (_Closing != null)
      {
        _Closing.Invoke(VsIde.DteInstance, new DocumentEventArgs(Document));
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Provides an opened event handler
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void ProvideOpenedEvent()
    {
      if (_OpenedEventCounter == 0)
      {
        _Events.DocumentOpened += HandleOpenedEvent;
      }
      _OpenedEventCounter++;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Safely removes the opened event handler
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void RemoveOpenedEvent()
    {
      _OpenedEventCounter--;
      if (_OpenedEventCounter == 0)
      {
        _Events.DocumentOpened -= HandleOpenedEvent;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the opened event
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void HandleOpenedEvent(Document Document)
    {
      if (_Opened != null)
      {
        _Opened.Invoke(VsIde.DteInstance, new DocumentEventArgs(Document));
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Provides an opening event handler
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void ProvideOpeningEvent()
    {
      if (_OpeningEventCounter == 0)
      {
        _Events.DocumentOpening += HandleOpeningEvent;
      }
      _OpeningEventCounter++;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Safely removes the opening event handler
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void RemoveOpeningEvent()
    {
      _OpeningEventCounter--;
      if (_OpeningEventCounter == 0)
      {
        _Events.DocumentOpening -= HandleOpeningEvent;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the opening event
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void HandleOpeningEvent(string DocumentPath, bool ReadOnly)
    {
      if (_Opening != null)
      {
        _Opening.Invoke(VsIde.DteInstance, 
          new DocumentOpeningEventArgs(DocumentPath, ReadOnly));
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Provides a saved event handler
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void ProvideSavedEvent()
    {
      if (_SavedEventCounter == 0)
      {
        _Events.DocumentSaved += HandleSavedEvent;
      }
      _SavedEventCounter++;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Safely removes the saved event handler
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void RemoveSavedEvent()
    {
      _SavedEventCounter--;
      if (_SavedEventCounter == 0)
      {
        _Events.DocumentSaved -= HandleSavedEvent;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the saved event
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void HandleSavedEvent(Document Document)
    {
      if (_Saved != null)
      {
        _Saved.Invoke(VsIde.DteInstance, new DocumentEventArgs(Document));
      }
    }

    #endregion
  }

  #region DocumentEventArgs class

  // ================================================================================================
  /// <summary>
  /// Event argument holding a document instance.
  /// </summary>
  // ================================================================================================
  public class DocumentEventArgs : EventArgs
  {
    public Document Document { get; set; }

    public DocumentEventArgs(Document document)
    {
      Document = document;
    }
  }

  #endregion

  #region DocumentOpeningEventArgs

  // ================================================================================================
  /// <summary>
  /// Event argument holding a document path and a readonly flag.
  /// </summary>
  // ================================================================================================
  public class DocumentOpeningEventArgs : EventArgs
  {
    public string DocumentPath { get; private set; }
    public bool ReadOnly { get; private set; }

    public DocumentOpeningEventArgs(string documentPath, bool readOnly)
    {
      DocumentPath = documentPath;
      ReadOnly = readOnly;
    }
  }

  #endregion
}