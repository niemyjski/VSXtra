// ================================================================================================
// RunningDocumentTable.cs
//
// Created: 2008.08.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Package;
using VSXtra.Windows;

namespace VSXtra.Documents
{
  // ================================================================================================
  /// <summary>
  /// This static class represent the document table within Visual Studio.
  /// </summary>
  // ================================================================================================
  [CLSCompliant(false)]
  public static class RunningDocumentTable
  {
    #region Private fields

    private static readonly IVsRunningDocumentTable _Rdt;
    private static readonly RdtEventHook _EventHook;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Obtains the running Document Table instance to work on.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    static RunningDocumentTable()
    {
      _Rdt = SiteManager.GetGlobalService<SVsRunningDocumentTable, IVsRunningDocumentTable>();
      if (_Rdt == null)
      {
        throw new NotSupportedException(typeof(SVsRunningDocumentTable).FullName);
      }
      _EventHook = new RdtEventHook();
    }

    #endregion

    #region Public Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the Running Document Table singleton instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static IVsRunningDocumentTable RdtInstance
    {
      get { return _Rdt; }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Finds the document specified with the moniker and retrieve it.
    /// </summary>
    /// <param name="moniker">Moniker to find the document.</param>
    /// <returns>
    /// The document information instance if found by the moniker; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    [CLSCompliant(false)]
    public static RunningDocumentInfo FindDocument(string moniker)
    {
      return FindAndLockDocument(moniker, RdtLocks.NoLock);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Finds the document specified with the moniker and retrieve it. Apply the specified lock 
    /// on the document if found.
    /// </summary>
    /// <param name="moniker">Moniker to find the document.</param>
    /// <param name="locks">Lock flags to apply on the document.</param>
    /// <returns>
    /// The document information instance if found by the moniker; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    [CLSCompliant(false)]
    public static RunningDocumentInfo FindAndLockDocument(string moniker, RdtLocks locks)
    {
      uint itemId;
      IVsHierarchy hierarchy;
      uint docCookie;
      IntPtr docData;
      NativeMethods.ThrowOnFailure(_Rdt.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_NoLock, moniker, out hierarchy, out itemId, out docData, out docCookie));
      return docData == IntPtr.Zero
        ? null :
        GetDocumentInfo(docCookie);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the document information related to the specified cookie.
    /// </summary>
    /// <param name="docCookie">Document cookie.</param>
    /// <returns>
    /// The document information related to the specified cookie; or null, if the document does 
    /// not exist.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    [CLSCompliant(false)]
    public static RunningDocumentInfo GetDocumentInfo(uint docCookie)
    {
      var info = new RunningDocumentInfo();
      IntPtr docData;
      uint flags;
      uint readLocks;
      uint editLocks;
      string moniker;
      IVsHierarchy hierarchy;
      uint itemId;

      int hr = _Rdt.GetDocumentInfo(docCookie, out flags, out readLocks, out editLocks,
        out moniker, out hierarchy, out itemId, out docData);
      if (hr != VSConstants.S_OK) return null;
      {
        try
        {
          if (docData != IntPtr.Zero)
            info.DocumentData = Marshal.GetObjectForIUnknown(docData);
        }
        finally
        {
          Marshal.Release(docData);
        }
      }
      info.DocumentCookie = docCookie;
      info.Flags = flags;
      info.ReadLocks = readLocks;
      info.EditLocks = editLocks;
      info.Moniker = moniker;
      info.Hierarchy = hierarchy;
      info.ItemId = itemId;
      return info;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an entry in the running document table when a document is created or opened.
    /// </summary>
    /// <param name="lockType">Lock to apply for the document.</param>
    /// <param name="mkDocument">Document moniker</param>
    /// <param name="hierarchy">Hierarchy of the document</param>
    /// <param name="itemid">Item iedntifier within the hierarchy.</param>
    /// <param name="docData">Document data</param>
    /// <returns>
    /// The RunningDocumentInfo instance for the newly registered document.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    [CLSCompliant(false)]
    public static RunningDocumentInfo RegisterAndLockDocument(RdtLocks lockType, string mkDocument,
      IVsHierarchy hierarchy, uint itemid, IntPtr docData)
    {
      uint cookie;
      NativeMethods.ThrowOnFailure(_Rdt.RegisterAndLockDocument((uint)lockType, mkDocument, hierarchy,
        itemid, docData, out cookie));
      return GetDocumentInfo(cookie);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enumerator for the documents already in the Running Documents Table.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<RunningDocumentInfo> Documents
    {
      get
      {
        IEnumRunningDocuments ppenum;
        if (NativeMethods.Succeeded(_Rdt.GetRunningDocumentsEnum(out ppenum)))
        {
          var rgelt = new uint[1];
          while (true)
          {
            uint fetched;
            if (NativeMethods.Succeeded(ppenum.Next(1, rgelt, out fetched)) && fetched == 1)
            {
              yield return GetDocumentInfo(rgelt[0]);
            }
            else
            {
              break;
            }
          }
        }
      }
    }

    #endregion

    #region Public events

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called after application of the first lock of the specified type to a document in the 
    /// Running Document Table (RDT).
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtLockEventArgs> OnAfterFirstDocumentLock
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterFirstDocumentLock += value;
      }
      remove
      {
        _EventHook.Events.AfterFirstDocumentLock -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called after application of the first lock of the specified type to a document in the 
    /// Running Document Table (RDT).
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtLockEventArgs> OnBeforeLastDocumentUnlock
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.BeforeLastDocumentUnlock += value;
      }
      remove
      {
        _EventHook.Events.BeforeLastDocumentUnlock -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called before saving a document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtEventArgs> OnBeforeSave
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.BeforeSave += value;
      }
      remove
      {
        _EventHook.Events.BeforeSave -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called after a document in the Running Document Table (RDT) is saved.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtEventArgs> OnAfterSave
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterSave += value;
      }
      remove
      {
        _EventHook.Events.AfterSave -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Fired after a Save All command is executed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtEventArgs> OnAfterSaveAll
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterSaveAll += value;
      }
      remove
      {
        _EventHook.Events.AfterSaveAll -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called before displaying a document window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtWindowShowEventArgs> OnBeforeDocumentWindowShow
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.BeforeDocumentWindowShow += value;
      }
      remove
      {
        _EventHook.Events.BeforeDocumentWindowShow -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called after a document window is hidden.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtWindowEventArgs> OnAfterDocumentWindowHide
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterDocumentWindowHide += value;
      }
      remove
      {
        _EventHook.Events.AfterDocumentWindowHide -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called after a document attribute is changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtDocumentChangedEventArgs> OnAfterAttributeChangeEx
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterAttributeChangeEx += value;
      }
      remove
      {
        _EventHook.Events.AfterAttributeChangeEx -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called after a document attribute is changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtDocumentChangedEventArgs> OnAfterAttributeChange
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterAttributeChange += value;
      }
      remove
      {
        _EventHook.Events.AfterAttributeChange -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Fired after the last document in the Running Document Table (RDT) is unlocked.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtFirstLockEventArgs> OnBeforeFirstDocumentLock
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.BeforeFirstDocumentLock += value;
      }
      remove
      {
        _EventHook.Events.BeforeFirstDocumentLock -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Fired after the last document in the Running Document Table (RDT) is unlocked.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<RdtLastUnlockEventArgs> OnAfterLastDocumentUnlock
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterLastDocumentUnlock += value;
      }
      remove
      {
        _EventHook.Events.AfterLastDocumentUnlock -= value;
        EnsureUnadvise();
      }
    }

    #endregion

    #region RdtEvents nested type

    // ================================================================================================
    /// <summary>
    /// This private class implements the event handler methods for IVsRunningDocTableEvents.
    /// </summary>
    // ================================================================================================
    private sealed class RdtEvents :
      IVsRunningDocTableEvents3,
      IVsRunningDocTableEvents4
    {
      #region Public events

      public EventHandler<RdtLockEventArgs> AfterFirstDocumentLock;
      public EventHandler<RdtLockEventArgs> BeforeLastDocumentUnlock;
      public EventHandler<RdtEventArgs> BeforeSave;
      public EventHandler<RdtEventArgs> AfterSave;
      public EventHandler<RdtEventArgs> AfterSaveAll;
      public EventHandler<RdtWindowEventArgs> AfterDocumentWindowHide;
      public EventHandler<RdtWindowShowEventArgs> BeforeDocumentWindowShow;
      public EventHandler<RdtDocumentChangedEventArgs> AfterAttributeChange;
      public EventHandler<RdtDocumentChangedEventArgs> AfterAttributeChangeEx;
      public EventHandler<RdtFirstLockEventArgs> BeforeFirstDocumentLock;
      public EventHandler<RdtLastUnlockEventArgs> AfterLastDocumentUnlock;

      #endregion

      #region IVsRunningDocTableEvents implementation

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Called after application of the first lock of the specified type to a document in the 
      /// Running Document Table (RDT).
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType,
        uint dwReadLocksRemaining, uint dwEditLocksRemaining)
      {
        if (AfterFirstDocumentLock != null)
        {
          var e = new RdtLockEventArgs(RdtEventType.AfterFirstDocumentLock)
                    {
                      Document = GetDocumentInfo(docCookie),
                      LockType = ((RdtLocks)(dwRDTLockType & (uint)RdtFlagMasks.LockMask)),
                      ReadLocksRemaining = dwReadLocksRemaining,
                      EditLocksRemaining = dwEditLocksRemaining
                    };
          AfterFirstDocumentLock(e.Document, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Called after application of the first lock of the specified type to a document in the 
      /// Running Document Table (RDT).
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType,
        uint dwReadLocksRemaining, uint dwEditLocksRemaining)
      {
        if (BeforeLastDocumentUnlock != null)
        {
          var e = new RdtLockEventArgs(RdtEventType.BeforeLastDocumentUnlock)
          {
            Document = GetDocumentInfo(docCookie),
            LockType = ((RdtLocks)(dwRDTLockType & (uint)RdtFlagMasks.LockMask)),
            ReadLocksRemaining = dwReadLocksRemaining,
            EditLocksRemaining = dwEditLocksRemaining
          };
          BeforeLastDocumentUnlock(e.Document, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Called after a document in the Running Document Table (RDT) is saved.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterSave(uint docCookie)
      {
        if (AfterSave != null)
        {
          var e = new RdtDocumentEventArgs(RdtEventType.AfterSave)
            { Document = GetDocumentInfo(docCookie) };
          AfterSave(e.Document, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// We do not implement this event, we use OnAfterAttributeChangeEx instead.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
      {
        if (AfterAttributeChange != null)
        {
          var e = new RdtDocumentChangedEventArgs(RdtEventType.AfterAttributeChange)
                    {
                      Document = GetDocumentInfo(docCookie),
                      ChangeFlags = (RdtAttributeFlags) grfAttribs,
                      HierarchyOld = null,
                      MonikerOld = string.Empty,
                      ItemIdOld = VSConstants.VSITEMID_NIL
                    };
          AfterAttributeChange(e.Document, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Called before displaying a document window.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
      {
        if (BeforeDocumentWindowShow != null)
        {
          var e = new RdtWindowShowEventArgs(RdtEventType.BeforeDocumentWindowShow)
          {
            Document = GetDocumentInfo(docCookie),
            FirstTime = fFirstShow != 0,
            Frame = new WindowFrame(pFrame)
          };
          BeforeDocumentWindowShow(e.Document, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Called after a document window is hidden.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
      {
        if (AfterDocumentWindowHide != null)
        {
          var e = new RdtWindowEventArgs(RdtEventType.AfterDocumentWindowHide)
                    {
                      Document = GetDocumentInfo(docCookie),
                      Frame = new WindowFrame(pFrame)
                    };
          AfterDocumentWindowHide(e.Document, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Called after a document attribute is changed.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld,
        uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew,
        string pszMkDocumentNew)
      {
        if (AfterAttributeChangeEx != null)
        {
          var e = new RdtDocumentChangedEventArgs(RdtEventType.AfterAttributeChangeEx)
          {
            Document = GetDocumentInfo(docCookie),
            ChangeFlags = (RdtAttributeFlags)grfAttribs,
            HierarchyOld = pHierOld,
            MonikerOld = pszMkDocumentOld,
            ItemIdOld = itemidOld
          };
          AfterAttributeChangeEx(e.Document, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Called before saving a document.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnBeforeSave(uint docCookie)
      {
        if (BeforeSave != null)
        {
          var e = new RdtDocumentEventArgs(RdtEventType.BeforeSave)
            { Document = GetDocumentInfo(docCookie) };
          BeforeSave(e.Document, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// 
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnBeforeFirstDocumentLock(IVsHierarchy pHier, uint itemid, string pszMkDocument)
      {
        if (BeforeFirstDocumentLock != null)
        {
          var e = new RdtFirstLockEventArgs(RdtEventType.BeforeFirstDocumentLock)
                    {
                      Hierarchy = pHier,
                      ItemId = itemid
                    };
          e.SetMoniker(pszMkDocument);
          BeforeFirstDocumentLock(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Fired after a Save All command is executed.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterSaveAll()
      {
        if (AfterSaveAll != null)
        {
          AfterSaveAll(this, new RdtDocumentEventArgs(RdtEventType.AfterSaveAll));
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Fired after the last document in the Running Document Table (RDT) is unlocked.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterLastDocumentUnlock(IVsHierarchy pHier, uint itemid, string pszMkDocument, 
        int fClosedWithoutSaving)
      {
        if (AfterLastDocumentUnlock != null)
        {
          var e = new RdtLastUnlockEventArgs(RdtEventType.AfterLastDocumentUnlock)
          {
            Hierarchy = pHier,
            ItemId = itemid,
            ClosedWithoutSaving = fClosedWithoutSaving != 0
          };
          e.SetMoniker(pszMkDocument);
          AfterLastDocumentUnlock(this, e);
        }
        return VSConstants.S_OK;
      }

      #endregion
    }

    #endregion

    #region EventHook class

    // ================================================================================================
    /// <summary>
    /// This private class implements the event hooker for RDT events.
    /// </summary>
    // ================================================================================================
    private sealed class RdtEventHook : EventHooker<RdtEvents>
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Calls the advise method on the related event provider class.
      /// </summary>
      /// <param name="cookie">The cookie to be used in the Unadvise method.</param>
      // --------------------------------------------------------------------------------------------
      protected override int Advise(out uint cookie)
      {
        return _Rdt.AdviseRunningDocTableEvents(Events, out cookie);
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Calls the unadvise method on the related event provider class.
      /// </summary>
      /// <param name="cookie">The cookie created in the Advise method.</param>
      // --------------------------------------------------------------------------------------------
      protected override int Unadvise(uint cookie)
      {
        return _Rdt.UnadviseRunningDocTableEvents(EventCookie);
      }
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Ensures that IVsRunningDocTableEvents is advised when required.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void EnsureAdvise()
    {
      _EventHook.EnsureAdvise();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Ensures that IVsRunningDocTableEvents is unadvised when required.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void EnsureUnadvise()
    {
      _EventHook.EnsureUnadvise();
    }

    #endregion
  }
}
