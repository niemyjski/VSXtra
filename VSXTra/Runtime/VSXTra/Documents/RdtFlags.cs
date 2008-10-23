// ================================================================================================
// RdtFlags.cs
//
// Created: 2008.08.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra.Documents
{
  // ================================================================================================
  /// <summary>
  /// This enumeration specifies the lock options for a document in the Running Document Table.
  /// </summary>
  // ================================================================================================
  [Flags]
  public enum RdtLocks
  {
    /// <summary>Indicates that no lock is placed on the document.</summary>
    NoLock = _VSRDTFLAGS.RDT_NoLock,

    /// <summary>Places a read lock on the document.</summary>
    ReadLock = _VSRDTFLAGS.RDT_ReadLock,

    /// <summary>Places an edit lock on the document.</summary>
    EditLock = _VSRDTFLAGS.RDT_EditLock,

    /// <summary>Requests an unlock of the document.</summary>
    RequestUnlock = _VSRDTFLAGS.RDT_RequestUnlock,
  }

  // ================================================================================================
  /// <summary>
  /// This enumeration specifies the save options for a document in the Running Document Table.
  /// </summary>
  // ================================================================================================
  [Flags]
  public enum RdtSaveFlags
  {
    /// <summary>Indicates that the SaveAs command should not be made available for this document.</summary>
    DontSaveAs = _VSRDTFLAGS.RDT_DontSaveAs,

    /// <summary>
    /// Indicates that the document is created through some special programmatic means. For example, 
    /// using a wizard. If you specify the RDT_NonCreatable flag, then the RDT_DontAutoOpen flag 
    /// automatically applies to your document.
    /// </summary>
    NonCreatable = _VSRDTFLAGS.RDT_NonCreatable,

    /// <summary>
    /// Any document marked with this value is not included in the list of documents shown in the 
    /// SaveChanges dialog box. The Save Changes dialog box is displayed when the user selects 
    /// Exit from the File menu.
    /// </summary>
    DontSave = _VSRDTFLAGS.RDT_DontSave,

    /// <summary>
    /// Indicates that the document is not persisted in the list of documents that can be opened 
    /// when the solution is re-opened. Such a document would not be opened using an editor 
    /// factory, but might be opened using a wizard or special programmatic code.</summary>
    DontAutoOpen = _VSRDTFLAGS.RDT_DontAutoOpen,

    /// <summary>When comparing MkDocument strings, perform a case-sensitive comparison of the strings.</summary>
    CaseSensitive = _VSRDTFLAGS.RDT_CaseSensitive,

    /// <summary>Combination of DontSave and DontSaveAs flags.</summary>
    CantSave = _VSRDTFLAGS.RDT_CantSave,

    /// <summary>Exclude this document from being considered in the documents collection for the automation model.</summary>
    VirtualDocument = _VSRDTFLAGS.RDT_VirtualDocument,

    /// <summary>
    /// Set automatically by the environment when a solution or project is opened. Used to flag 
    /// solution and project files in the running document table. Clients are required to set this 
    /// flag in the case of nested projects.
    /// </summary>
    ProjectOrSolution = _VSRDTFLAGS.RDT_ProjSlnDocument,

    /// <summary>
    /// Used in the implementation of miscellaneous files. Prevents the Miscellaneous Files project 
    /// from calling the CreateDocumentWindow method on the document added to the project.
    /// </summary>
    PlaceHolderDoc = _VSRDTFLAGS.RDT_PlaceHolderDoc,

    /// <summary>Indicates that a save of the document is not forced on a build.</summary>
    CanBuildFromMemory = _VSRDTFLAGS.RDT_CanBuildFromMemory,

    /// <summary>Do not add to the list of most recently used files.</summary>
    DontAddToMRU = _VSRDTFLAGS.RDT_DontAddToMRU,
  }

  // ================================================================================================
  /// <summary>
  /// This enumeration specifies the unlock options for a document in the Running Document Table.
  /// </summary>
  // ================================================================================================
  [Flags]
  public enum RdtUnLockFlags
  {
    /// <summary>Used by UnlockDocument. Release the edit lock and do not save.</summary>
    NoSave = _VSRDTFLAGS.RDT_Unlock_NoSave,

    /// <summary>Used by the UnlockDocument method. Release the edit lock and save the file if it is dirty.</summary>
    SaveIfDirty = _VSRDTFLAGS.RDT_Unlock_SaveIfDirty,

    /// <summary>Used by the UnlockDocument method. Release the edit lock and prompt the user to save the file.</summary>
    PromptSave = _VSRDTFLAGS.RDT_Unlock_PromptSave
  }
  
  // ================================================================================================
  /// <summary>
  /// This enumeration specifies the unlock options for a document in the Running Document Table.
  /// </summary>
  // ================================================================================================
  [Flags]
  public enum RdtFlagMasks
  {
    /// <summary>Mask of the RDT_NoLock, RDT_ReadLock, RDT_EditLock, and RDT_RequestUnlock flags.</summary>
    LockMask = _VSRDTFLAGS.RDT_LOCKMASK,

    /// <summary>
    /// Mask of the flags from RDT_DontSaveAs through RDT_DontAddToMRU. Allow __VSCREATEDOCWIN 
    /// flags in doc mask.
    /// </summary>
    DocMask = _VSRDTFLAGS.RDT_DOCMASK,

    /// <summary>Mask of the RDT_NoLock, RDT_ReadLock, RDT_EditLock, and RDT_RequestUnlock flags.</summary>
    SaveMask = _VSRDTFLAGS.RDT_LOCKMASK
  }

  // ================================================================================================
  /// <summary>
  /// This enum contains flags for signing RDT document attributer changes.
  /// </summary>
  // ================================================================================================
  [Flags]
  public enum RdtAttributeFlags
  {
    /// <summary>Hierarchical position of the document in the RDT.</summary>
    Hierarchy = __VSRDTATTRIB.RDTA_Hierarchy,

    /// <summary>Item identifier of the document in the RDT.</summary>
    ItemID = __VSRDTATTRIB.RDTA_ItemID,

    /// <summary>Full path to the document in the RDT.</summary>
    DocumentMoniker = __VSRDTATTRIB.RDTA_MkDocument,

    /// <summary>Flag indicates that the data of the document in the RDT has changed.</summary>
    DocumentDataIsDirty = __VSRDTATTRIB.RDTA_DocDataIsDirty,

    /// <summary>Flag indicates that the data of the document in the RDT has not changed.</summary>
    DocumentDataIsNotDirty = __VSRDTATTRIB.RDTA_DocDataIsNotDirty,

    /// <summary>This attribute event is fired by calling the NotifyDocumentChanged method.</summary>
    DocDataReloaded = __VSRDTATTRIB.RDTA_DocDataReloaded,

    /// <summary>This attribute event is fired by calling the NotifyDocumentChanged method.</summary>
    AlterHierarchyItemID = __VSRDTATTRIB.RDTA_AltHierarchyItemID,

    /// <summary>A mask for the flags passed to the NotifyDocumentChanged method.</summary>
    NotifyDocumentChangedMask = __VSRDTATTRIB.RDTA_NOTIFYDOCCHANGEDMASK
  }
}