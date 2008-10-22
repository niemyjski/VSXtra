// ================================================================================================
// RdtEvents.cs
//
// Created: 2008.08.02, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  #region RdtEventType

  // ================================================================================================
  /// <summary>
  /// This enumeration represents the RDT event types.
  /// </summary>
  // ================================================================================================
  public enum RdtEventType
  {
    AfterFirstDocumentLock,
    BeforeLastDocumentUnlock,
    AfterSave,
    BeforeDocumentWindowShow,
    AfterDocumentWindowHide,
    AfterAttributeChangeEx,
    BeforeSave,
    AfterSaveAll,
    AfterAttributeChange,
    BeforeFirstDocumentLock,
    AfterLastDocumentUnlock
  }

  #endregion

  #region RdtEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents the event arguments used for simple RDT events
  /// </summary>
  // ================================================================================================
  public class RdtEventArgs : EventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the type of event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public RdtEventType EventType { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the moniker of the event
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual string Moniker { get { return String.Empty; } }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of RDT event</param>
    // --------------------------------------------------------------------------------------------
    protected RdtEventArgs(RdtEventType eventType)
    {
      EventType = eventType;
    }
  }

  #endregion

  #region RdtDocumentEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents the event arguments used for simple RDT events
  /// </summary>
  // ================================================================================================
  public class RdtDocumentEventArgs : RdtEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the document belonging to the event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public RunningDocumentInfo Document { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of RDT event</param>
    // --------------------------------------------------------------------------------------------
    public RdtDocumentEventArgs(RdtEventType eventType)
      : base(eventType)
    {
    }

    /// <summary>
    /// Gets or sets the moniker of the event
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string Moniker
    {
      get { return Document == null ? "<none>" : Document.Moniker; }
    }
  }

  #endregion

  #region RdtLockEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents the event arguments used for locking events
  /// </summary>
  // ================================================================================================
  [CLSCompliant(false)]
  public class RdtLockEventArgs : RdtDocumentEventArgs 
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the document lock type.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public RdtLocks LockType { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the number of remaining read locks.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint ReadLocksRemaining { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the number of remaining edit locks.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint EditLocksRemaining { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of RDT event</param>
    // --------------------------------------------------------------------------------------------
    public RdtLockEventArgs(RdtEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region RdtWindowEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents the event arguments used for RDT events having window frame parameters
  /// </summary>
  // ================================================================================================
  public class RdtWindowEventArgs : RdtDocumentEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the window frame belonging to the event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public WindowFrame Frame { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of RDT event</param>
    // --------------------------------------------------------------------------------------------
    public RdtWindowEventArgs(RdtEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region RdtWindowShowEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents the event arguments used for RDT events having window frame parameters
  /// </summary>
  // ================================================================================================
  public class RdtWindowShowEventArgs : RdtWindowEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the flag indicating if window is shown for the first time.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool FirstTime { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of RDT event</param>
    // --------------------------------------------------------------------------------------------
    public RdtWindowShowEventArgs(RdtEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region RdtDocumentChangedEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents the event arguments used for RDT events having window frame parameters
  /// </summary>
  // ================================================================================================
  public class RdtDocumentChangedEventArgs : RdtDocumentEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the attributes signing which attributes of the document has been changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public RdtAttributeFlags ChangeFlags { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the old moniker of the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string MonikerOld { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the old hierarchy of the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IVsHierarchy HierarchyOld { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the old item ID of the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint ItemIdOld { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of RDT event</param>
    // --------------------------------------------------------------------------------------------
    public RdtDocumentChangedEventArgs(RdtEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region RdtFirstLockEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents the event arguments used for RDT events having window frame parameters
  /// </summary>
  // ================================================================================================
  public class RdtFirstLockEventArgs : RdtEventArgs
  {
    private string _Moniker;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the hierarchy information of the event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IVsHierarchy Hierarchy { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the item ID of the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint ItemId { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the old moniker of the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string Moniker { get { return _Moniker; } }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of RDT event</param>
    // --------------------------------------------------------------------------------------------
    public RdtFirstLockEventArgs(RdtEventType eventType)
      : base(eventType)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the moniker of the event.
    /// </summary>
    /// <param name="moniker">Moniker to set.</param>
    // --------------------------------------------------------------------------------------------
    public void SetMoniker(string moniker)
    {
      _Moniker = moniker;
    }
  }

  #endregion

  #region RdtLastUnockEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents the event arguments used for RDT events having window frame parameters
  /// </summary>
  // ================================================================================================
  public class RdtLastUnlockEventArgs : RdtFirstLockEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the flag indicating if the document has been clsed without saving.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool ClosedWithoutSaving { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of RDT event</param>
    // --------------------------------------------------------------------------------------------
    public RdtLastUnlockEventArgs(RdtEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion
}