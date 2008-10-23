// ================================================================================================
// RunningDocumentInfo.cs
//
// Created: 2008.08.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace VSXtra.Documents
{
  // ================================================================================================
  /// <summary>
  /// This class holds information about a document in the Running Document Table.
  /// </summary>
  // ================================================================================================
  public sealed class RunningDocumentInfo
  {
    #region Properties 

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the cookie value identifying the document in the RDT.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint DocumentCookie { get; internal set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the option flags belonging to the document.
    /// </summary>
    /// <remarks>Use the other properties to obtain flag values.</remarks>
    // --------------------------------------------------------------------------------------------
    public uint Flags { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the number of read locks held by the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint ReadLocks { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the number of edit locks held by the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint EditLocks { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the hierarchy owning the document
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IVsHierarchy Hierarchy { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the hierarchy item identifier of the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint ItemId { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the Moniker of the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string Moniker { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the data representing the document.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public object DocumentData { get; set; }

    #endregion

    #region Flag properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the ReadLock flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool ReadLockFlag
    {
      get { return (Flags & (uint)RdtLocks.ReadLock) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the EditLock flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool EditLockFlag
    {
      get { return (Flags & (uint)RdtLocks.EditLock) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the RequestUnlock flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool RequestUnlockFlag
    {
      get { return (Flags & (uint)RdtLocks.RequestUnlock) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the ReadLock flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool HasAnyLock
    {
      get { return (Flags & (uint)RdtFlagMasks.LockMask) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the DontSaveAs flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool DontSaveAsFlag
    {
      get { return (Flags & (uint)RdtLocks.RequestUnlock) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the NonCreatable flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool NonCreatableFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.NonCreatable) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the DontSave flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool DontSaveFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.DontSave) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the DontAutoOpen flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool DontAutoOpenFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.DontAutoOpen) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the CaseSensitive flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool CaseSensitiveFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.CaseSensitive) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the CantSave flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool CantSaveFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.CantSave) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the VirtualDocument flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool VirtualDocumentFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.VirtualDocument) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the ProjectOrSolution flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool ProjectOrSolutionFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.ProjectOrSolution) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the PlaceHolderDoc flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool PlaceHolderDocFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.PlaceHolderDoc) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the CanBuildFromMemory flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool CanBuildFromMemoryFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.CanBuildFromMemory) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the DontAddToMRU flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool DontAddToMRUFlag
    {
      get { return (Flags & (uint)RdtSaveFlags.DontAddToMRU) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the NoSave flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool NoSaveFlag
    {
      get { return (Flags & (uint)RdtUnLockFlags.NoSave) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the SaveIfDirty flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool SaveIfDirtyFlag
    {
      get { return (Flags & (uint)RdtUnLockFlags.SaveIfDirty) != 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the PromptSave flag.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool PromptSaveFlag
    {
      get { return (Flags & (uint)RdtUnLockFlags.PromptSave) != 0; }
    }

    #endregion

    #region Public methods and properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the textual content of the object data
    /// </summary>
    /// <returns>
    /// Textual content of the document if found and the document implements either the 
    /// IVsTextLines or the IVsTextBufferProvider interface; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public string BufferContents
    {
      get
      {
        string text = null;
        var buffer = DocumentData as IVsTextLines;
        if (buffer == null)
        {
          var tp = DocumentData as IVsTextBufferProvider;
          if (tp != null && tp.GetTextBuffer(out buffer) != NativeMethods.S_OK)
            buffer = null;
        }
        if (buffer != null)
        {
          int endLine;
          int endIndex;
          NativeMethods.ThrowOnFailure(buffer.GetLastLineIndex(out endLine, out endIndex));
          NativeMethods.ThrowOnFailure(buffer.GetLineText(0, 0, endLine, endIndex, out text));
        }
        return text;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Saves the specified file if that file is dirty.
    /// </summary>
    /// <returns>
    /// The path or moniker of the saved file.
    /// </returns>
    /// <remarks>
    /// During the save operation user might select a new name for the document.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public bool SaveIfDirty()
    {
      var cancelled = false;
      var persistDocData = DocumentData as IVsPersistDocData2;
      if (persistDocData != null)
      {
        int dirty;
        var hr = persistDocData.IsDocDataDirty(out dirty);
        if (NativeMethods.Succeeded(hr) && dirty != 0)
        {
          string newdoc;
          int userCancelled;
          NativeMethods.ThrowOnFailure(persistDocData.SaveDocData(VSSAVEFLAGS.VSSAVE_Save,
            out newdoc, out userCancelled));
          cancelled = userCancelled != 0;
          Moniker = newdoc;
        }
      }
      return cancelled;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Renames the to the specified new name.
    /// </summary>
    /// <param name="newName">New name of the document.</param>
    // --------------------------------------------------------------------------------------------
    [CLSCompliant(false)]
    public void RenameDocument(string newName)
    {
      var pUnk = Marshal.GetIUnknownForObject(Hierarchy);
      if (pUnk == IntPtr.Zero) return;
      try
      {
        IntPtr pHier;
        var guid = typeof(IVsHierarchy).GUID;
        NativeMethods.ThrowOnFailure(Marshal.QueryInterface(pUnk, ref guid, out pHier));
        try
        {
          NativeMethods.ThrowOnFailure(RunningDocumentTable.RdtInstance.
            RenameDocument(Moniker, newName, pHier, ItemId));
        }
        finally
        {
          Marshal.Release(pHier);
        }
      }
      finally
      {
        Marshal.Release(pUnk);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Locks the document with the given lock type.
    /// </summary>
    /// <param name="lockType">Lock type to apply on the document.</param>
    // --------------------------------------------------------------------------------------------
    [CLSCompliant(false)]
    public void LockDocument(RdtLocks lockType)
    {
      NativeMethods.ThrowOnFailure(RunningDocumentTable.RdtInstance.
        LockDocument((uint)lockType, DocumentCookie));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes the specified lock type from the document.
    /// </summary>
    /// <param name="lockType">Lock type to remove from the document.</param>
    // --------------------------------------------------------------------------------------------
    [CLSCompliant(false)]
    public void UnlockDocument(_VSRDTFLAGS lockType)
    {
      NativeMethods.ThrowOnFailure(RunningDocumentTable.RdtInstance.
        UnlockDocument((uint)lockType, DocumentCookie));
    }

    #endregion
  }
}