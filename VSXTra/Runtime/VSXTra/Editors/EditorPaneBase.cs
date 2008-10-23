using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Commands;
using VSXtra.Package;
using VSXtra.Properties;
using VSXtra.Shell;
using VSXtra.Windows;

namespace VSXtra.Editors
{
  // ==================================================================================
  /// <summary>
  /// This class provides a base class for simple text or dialog based custom editors.
  /// </summary>
  /// <typeparam name="TPackage">Package owning this editor pane.</typeparam>
  /// <typeparam name="TFactory">Editor factory for this pane.</typeparam>
  /// <typeparam name="TUIControl">UI control representing the editor.</typeparam>
  // ==================================================================================
  public abstract class EditorPaneBase<TPackage, TFactory, TUIControl> : 
    WindowPane<TPackage, TUIControl>,
    ICommonEditorCommand,
    IVsPersistDocData,
    IPersistFileFormat
    where TPackage: PackageBase
    where TFactory: IVsEditorFactory
    where TUIControl: Control, new()
  {
    #region Constant values

    // --- Our editor will support only one file format, this is its index.
    private const uint FileFormatIndex = 0;

    // --- Character separating file format lines.
    private const char EndLineChar = (char)10;

    #endregion

    #region Private fields

    // --- Common commands if supported by the UI.
    private readonly ICommonEditorCommand _CommonCommands;
    
    // --- File extension used for this editor.
    private readonly string _FileExtensionUsed;

    // --- Full path to the file.
    private string _FileName;

    // --- Determines whether an object has changed since being saved to its current file.
    private bool _IsDirty;

    // --- Flag true when we are loading the file. It is used to avoid to change the 
    // --- _IsDirty flag when the changes are related to the load operation.
    private bool _Loading;

    // --- This flag is true when we are asking the QueryEditQuerySave service if we can 
    // --- edit the file. It is used to avoid to have more than one request queued.
    private bool _GettingCheckoutStatus;

    // --- Indicate that object is in NoScribble mode or in Normal mode. Object enter 
    // --- into the NoScribble mode when IPersistFileFormat.Save() call is occurred.
    // --- This flag using to indicate SaveCompleted state (entering into the Normal mode).
    private bool _NoScribbleMode;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of this editor pane.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of the UI control associated with the editor pane.
    /// </remarks>
    // --------------------------------------------------------------------------------
    protected EditorPaneBase()
    {
      // --- Set up command dispatching
      _FileExtensionUsed = GetFileExtensionValue();
      _CommonCommands = UIControl as ICommonEditorCommand;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Avoids using virtual method in constructor
    /// </summary>
    // --------------------------------------------------------------------------------
    private string GetFileExtensionValue()
    {
      return GetFileExtension();
    }

    #endregion

    #region Abstract and virtual members to override

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the file extension used by this editor.
    /// </summary>
    /// <returns>
    /// File extension used by the editor.
    /// </returns>
    /// <remarks>
    /// This method is called only once at construction time. The returned file 
    /// extension must contain the leading "." character.
    /// </remarks>
    // --------------------------------------------------------------------------------
    protected abstract string GetFileExtension();

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Override this abstract method to define how to load the editor content from a
    /// file.
    /// </summary>
    /// <param name="fileName">Name of the file to load the content from.</param>
    // --------------------------------------------------------------------------------
    protected abstract void LoadFile(string fileName);

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Override this abstract method to define how to save the editor content into a
    /// file.
    /// </summary>
    /// <param name="fileName">Name of the file to save the content to.</param>
    // --------------------------------------------------------------------------------
    protected abstract void SaveFile(string fileName);

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Use this method to sign that the content of the editor has been changed.
    /// </summary>
    // --------------------------------------------------------------------------------
    protected virtual void OnContentChanged()
    {
      // --- During the load operation the text of the control will change, but
      // --- this change must not be stored in the status of the document.
      if (!_Loading)
      {
        // --- The only interesting case is when we are changing the document
        // --- for the first time
        if (!_IsDirty)
        {
          // Check if the QueryEditQuerySave service allow us to change the file
          if (!CanEditFile())
          {
            // --- We can not change the file (e.g. a checkout operation failed),
            // --- so undo the change and exit.
            if (SupportsUndo) DoUndo();
            return;
          }

          // --- It is possible to change the file, so update the status.
          _IsDirty = true;
        }
      }
    }

    #endregion

    #region Advanced virtual members (rarely to override)

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Returns the path to the object's current working file.
    /// </summary>
    /// <param name="ppszFilename">Pointer to the file name.</param>
    /// <param name="pnFormatIndex">
    /// Value that indicates the current format of the file as a zero based index into 
    /// the list of formats. Since we support only a single format, we need to 
    /// return zero. Subsequently, we will return a single element in the format list 
    /// through a call to GetFormatList.
    /// </param>
    /// <returns>S_OK if the function succeeds.</returns>
    // --------------------------------------------------------------------------------
    protected virtual int OnGetCurFile(out string ppszFilename, out uint pnFormatIndex)
    {
      // --- We only support 1 format so return its index
      pnFormatIndex = FileFormatIndex;
      ppszFilename = _FileName;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Provides the caller with the information necessary to open the standard 
    /// common "Save As" dialog box. This returns an enumeration of supported formats, 
    /// from which the caller selects the appropriate format.
    /// Each string for the format is terminated with a newline (\n) character. 
    /// The last string in the buffer must be terminated with the newline character 
    /// as well. The first string in each pair is a display string that describes the 
    /// filter, such as "Text Only (*.txt)". The second string specifies the filter 
    /// pattern, such as "*.txt". To specify multiple filter patterns for a single 
    /// display string, use a semicolon to separate the patterns: "*.htm;*.html;*.asp". 
    /// A pattern string can be a combination of valid file name characters and the 
    /// asterisk (*) wildcard character. Do not include spaces in the pattern string. 
    /// The following string is an example of a file pattern string: 
    /// "HTML File (*.htm; *.html; *.asp)\n*.htm;*.html;*.asp\nText File (*.txt)\n*.txt\n."
    /// </summary>
    /// <param name="ppszFormatList">
    /// Pointer to a string that contains pairs of format filter strings.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    protected virtual int OnGetFormatList(out string ppszFormatList)
    {
      string formatList =
        string.Format(CultureInfo.CurrentCulture,
        "Editor Files (*{0}){1}*{0}{1}{1}",
        FileExtensionUsed, EndLineChar);
      ppszFormatList = formatList;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Notifies the object that it has concluded the Save transaction.
    /// </summary>
    /// <param name="pszFilename">Pointer to the file name.</param>
    /// <returns>S_OK if the function succeeds.</returns>
    // --------------------------------------------------------------------------------
    protected virtual int OnSaveCompleted(string pszFilename)
    {
      return _NoScribbleMode ? VSConstants.S_FALSE : VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Initialization for the object.
    /// </summary>
    /// <param name="nFormatIndex">
    /// Zero based index into the list of formats that indicates the current format
    /// of the file.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    protected virtual int OnInitNew(uint nFormatIndex)
    {
      if (nFormatIndex != FileFormatIndex)
      {
        throw new ArgumentException(Resources.ExceptionMessageFormat);
      }
      // --- Until someone change the file, we can consider it not dirty as
      // --- the user would be annoyed if we prompt him to save an empty file
      _IsDirty = false;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Determines whether an object has changed since being saved to its current file.
    /// </summary>
    /// <param name="pfIsDirty">true if the document has changed.</param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    protected virtual int OnIsDirty(out int pfIsDirty)
    {
      pfIsDirty = _IsDirty ? 1 : 0;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Close the IVsPersistDocData object.
    /// </summary>
    /// <returns>S_OK if the function succeeds.</returns>
    // --------------------------------------------------------------------------------
    protected virtual int OnCloseEditor()
    {
      return VSConstants.S_OK;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the ID of the editor factory creating instances of this editor pane.
    /// </summary>
    // --------------------------------------------------------------------------------
    public Guid FactoryGuid
    {
      get { return typeof (TFactory).GUID; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the file extension used by the editor.
    /// </summary>
    // --------------------------------------------------------------------------------
    public string FileExtensionUsed
    {
      get { return _FileExtensionUsed; }
    }

    #endregion

    #region Command Handler Methods for common editor commands

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the command status of the SelectAll command.
    /// </summary>
    /// <param name="command">Command object to set the status for.</param>
    // --------------------------------------------------------------------------------------------
    [CommandStatusMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.SelectAll)]
    protected virtual void QuerySelectAllStatus(OleMenuCommand command)
    {
      command.Enabled = SupportsSelectAll;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the command status of the Cut command.
    /// </summary>
    /// <param name="command">Command object to set the status for.</param>
    // --------------------------------------------------------------------------------------------
    [CommandStatusMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Cut)]
    protected virtual void QueryCutStatus(OleMenuCommand command)
    {
      command.Enabled = SupportsCut;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the command status of the Copy command.
    /// </summary>
    /// <param name="command">Command object to set the status for.</param>
    // --------------------------------------------------------------------------------------------
    [CommandStatusMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Copy)]
    protected virtual void QueryCopyStatus(OleMenuCommand command)
    {
      command.Enabled = SupportsCopy;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the command status of the Paste command.
    /// </summary>
    /// <param name="command">Command object to set the status for.</param>
    // --------------------------------------------------------------------------------------------
    [CommandStatusMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Paste)]
    protected virtual void QueryPasteStatus(OleMenuCommand command)
    {
      command.Enabled = SupportsPaste;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the command status of the Redo command.
    /// </summary>
    /// <param name="command">Command object to set the status for.</param>
    // --------------------------------------------------------------------------------------------
    [CommandStatusMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Redo)]
    protected virtual void QueryRedoStatus(OleMenuCommand command)
    {
      command.Enabled = SupportsRedo;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the command status of the Redo command.
    /// </summary>
    /// <param name="command">Command object to set the status for.</param>
    // --------------------------------------------------------------------------------------------
    [CommandStatusMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Undo)]
    protected virtual void QueryUndoStatus(OleMenuCommand command)
    {
      command.Enabled = SupportsUndo;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the SelectAll command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.SelectAll)]
    protected void ExecuteSelectAll()
    {
      DoSelectAll();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Cut command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Cut)]
    protected void ExecuteCut()
    {
      DoCut();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Copy command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Copy)]
    protected void ExecuteCopy()
    {
      DoCopy();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Paste command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Paste)]
    protected void ExecutePaste()
    {
      DoPaste();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Redo command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Redo)]
    protected void ExecuteRedo()
    {
      DoRedo();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the SelectAll command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Undo)]
    protected void ExecuteUndo()
    {
      DoUndo();
    }

    #endregion

    #region ICommonEditorCommand Implementation

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "SelectAll" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    public bool SupportsSelectAll
    {
      get { return _CommonCommands != null && _CommonCommands.SupportsSelectAll; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Copy" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    public bool SupportsCopy
    {
      get { return _CommonCommands != null && _CommonCommands.SupportsCopy; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Cut" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    public bool SupportsCut
    {
      get { return _CommonCommands != null && _CommonCommands.SupportsCut; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Paste" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    public bool SupportsPaste
    {
      get { return _CommonCommands != null && _CommonCommands.SupportsPaste; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Redo" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    public bool SupportsRedo
    {
      get { return _CommonCommands != null && _CommonCommands.SupportsRedo; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Undo" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    public bool SupportsUndo
    {
      get { return _CommonCommands != null && _CommonCommands.SupportsUndo; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "SelectAll" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    public void DoSelectAll()
    {
      if (_CommonCommands != null) _CommonCommands.DoSelectAll();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Copy" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    public void DoCopy()
    {
      if (_CommonCommands != null) _CommonCommands.DoCopy();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Cut" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    public void DoCut()
    {
      if (_CommonCommands != null) _CommonCommands.DoCut();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Paste" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    public void DoPaste()
    {
      if (_CommonCommands != null) _CommonCommands.DoPaste();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Redo" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    public void DoRedo()
    {
      if (_CommonCommands != null) _CommonCommands.DoRedo();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Undo" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    public void DoUndo()
    {
      if (_CommonCommands != null) _CommonCommands.DoUndo();
    }

    #endregion

    #region IDisposable Pattern implementation

    // --------------------------------------------------------------------------------
    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    // --------------------------------------------------------------------------------
    protected override void Dispose(bool disposing)
    {
      try
      {
        if (disposing)
        {
          GC.SuppressFinalize(this);
        }
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    #endregion

    #region IPersist Members

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the class identifier (CLSID) of an object.
    /// </summary>
    /// <param name="pClassID">
    /// Class identifier of the object.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IPersist.GetClassID(out Guid pClassID)
    {
      pClassID = FactoryGuid;
      return VSConstants.S_OK;
    }

    #endregion

    #region IPersistFileFormat Members

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the class identifier (CLSID) of an object.
    /// </summary>
    /// <param name="pClassID">
    /// Class identifier of the object.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IPersistFileFormat.GetClassID(out Guid pClassID)
    {
      pClassID = FactoryGuid;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Determines whether an object has changed since being saved to its current file.
    /// </summary>
    /// <param name="pfIsDirty">true if the document has changed.</param>
    /// <returns>S_OK if the method succeeds.</returns>
    /// <remarks>
    /// Override the <see cref="OnIsDirty"/> method to change the behaviour.
    /// </remarks>
    // --------------------------------------------------------------------------------
    int IPersistFileFormat.IsDirty(out int pfIsDirty)
    {
      return OnIsDirty(out pfIsDirty);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Initialization for the object.
    /// </summary>
    /// <param name="nFormatIndex">
    /// Zero based index into the list of formats that indicates the current format
    /// of the file.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    /// <remarks>
    /// Override the <see cref="OnInitNew"/> method to change the behaviour.
    /// </remarks>
    // --------------------------------------------------------------------------------
    int IPersistFileFormat.InitNew(uint nFormatIndex)
    {
      return OnInitNew(nFormatIndex);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Loads the file content into the editor (into the controls representing the UI).
    /// </summary>
    /// <param name="pszFilename">
    /// Pointer to the full path name of the file to load.
    /// </param>
    /// <param name="grfMode">File format mode.</param>
    /// <param name="fReadOnly">
    /// determines if the file should be opened as read only.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
    {
      // --- A valid file name is required.
      if ((pszFilename == null) && (string.IsNullOrEmpty(_FileName)))
        throw new ArgumentNullException("pszFilename");

      _Loading = true;
      try
      {
        // --- If the new file name is null, then this operation is a reload
        bool isReload = false;
        if (pszFilename == null)
        {
          isReload = true;
        }

        // --- Show the wait cursor while loading the file
        VsUIShell.SetWaitCursor();

        // --- Set the new file name
        if (!isReload)
        {
          // --- Unsubscribe from the notification of the changes in the previous file.
          _FileName = pszFilename;
        }
        // --- Load the file
        LoadFile(_FileName);
        _IsDirty = false;

        // --- Notify the load or reload
        NotifyDocChanged();
      }
      finally
      {
        _Loading = false;
      }
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Save the contents of the editor into the specified file. If doing the save 
    /// on the same file, we need to suspend notifications for file changes during 
    /// the save operation.
    /// </summary>
    /// <param name="pszFilename">
    /// Pointer to the file name. If the pszFilename parameter is a null reference 
    /// we need to save using the current file.
    /// </param>
    /// <param name="fRemember">
    /// Boolean value that indicates whether the pszFileName parameter is to be used 
    /// as the current working file.
    /// If remember != 0, pszFileName needs to be made the current file and the 
    /// dirty flag needs to be cleared after the save. Also, file notifications need 
    /// to be enabled for the new file and disabled for the old file.
    /// If remember == 0, this save operation is a Save a Copy As operation. In this 
    /// case, the current file is unchanged and dirty flag is not cleared.
    /// </param>
    /// <param name="nFormatIndex">
    /// Zero based index into the list of formats that indicates the format in which 
    /// the file will be saved.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IPersistFileFormat.Save(string pszFilename, int fRemember, uint nFormatIndex)
    {
      // --- switch into the NoScribble mode
      _NoScribbleMode = true;
      try
      {
        // --- If file is null or same --> SAVE
        if (pszFilename == null || pszFilename == _FileName)
        {
          SaveFile(_FileName);
          _IsDirty = false;
        }
        else
        {
          // --- If remember --> SaveAs 
          if (fRemember != 0)
          {
            _FileName = pszFilename;
            SaveFile(_FileName);
            _IsDirty = false;
          }
          else // --- Else, Save a Copy As
          {
            SaveFile(pszFilename);
          }
        }
      }
      finally
      {
        // --- Switch into the Normal mode
        _NoScribbleMode = false;
      }
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Notifies the object that it has concluded the Save transaction.
    /// </summary>
    /// <param name="pszFilename">Pointer to the file name.</param>
    /// <returns>S_OK if the function succeeds.</returns>
    /// <remarks>
    /// Override the <see cref="OnSaveCompleted"/> method to change the behaviour.
    /// </remarks>
    // --------------------------------------------------------------------------------
    int IPersistFileFormat.SaveCompleted(string pszFilename)
    {
      return OnSaveCompleted(pszFilename);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Returns the path to the object's current working file.
    /// </summary>
    /// <param name="ppszFilename">Pointer to the file name.</param>
    /// <param name="pnFormatIndex">
    /// Value that indicates the current format of the file as a zero based index into 
    /// the list of formats. Since we support only a single format, we need to 
    /// return zero. Subsequently, we will return a single element in the format list 
    /// through a call to GetFormatList.
    /// </param>
    /// <returns>S_OK if the function succeeds.</returns>
    /// <remarks>
    /// Override the <see cref="OnGetCurFile"/> method to change the behaviour.
    /// </remarks>
    // --------------------------------------------------------------------------------
    int IPersistFileFormat.GetCurFile(out string ppszFilename, out uint pnFormatIndex)
    {
      return OnGetCurFile(out ppszFilename, out pnFormatIndex);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Provides the caller with the information necessary to open the standard 
    /// common "Save As" dialog box. 
    /// </summary>
    /// <param name="ppszFormatList">
    /// Pointer to a string that contains pairs of format filter strings.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    /// <remarks>
    /// Override the <see cref="OnGetFormatList"/> method to change the behaviour.
    /// </remarks>
    // --------------------------------------------------------------------------------
    int IPersistFileFormat.GetFormatList(out string ppszFormatList)
    {
      return OnGetFormatList(out ppszFormatList);
    }

    #endregion

    #region IVsPersistDocData Members

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Returns the Guid of the editor factory that created the IVsPersistDocData 
    /// object.
    /// </summary>
    /// <param name="pClassID">
    /// Pointer to the class identifier of the editor type.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
    {
      pClassID = FactoryGuid;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Used to determine if the document data has changed since the last time it was 
    /// saved.
    /// </summary>
    /// <param name="pfDirty">Will be set to 1 if the data has changed.</param>
    /// <returns>S_OK if the function succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IVsPersistDocData.IsDocDataDirty(out int pfDirty)
    {
      return ((IPersistFileFormat)this).IsDirty(out pfDirty);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Used to set the initial name for unsaved, newly created document data.
    /// </summary>
    /// <param name="pszDocDataPath">String containing the path to the document.
    /// We need to ignore this parameter.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
    {
      return ((IPersistFileFormat)this).InitNew(FileFormatIndex);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Loads the document data from the file specified.
    /// </summary>
    /// <param name="pszMkDocument">
    /// Path to the document file which needs to be loaded.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IVsPersistDocData.LoadDocData(string pszMkDocument)
    {
      return ((IPersistFileFormat)this).Load(pszMkDocument, 0, 0);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Saves the document data. Before actually saving the file, we first need to 
    /// indicate to the environment that a file is about to be saved. This is done 
    /// through the "SVsQueryEditQuerySave" service. We call the "QuerySaveFile" 
    /// function on the service instance and then proceed depending on the result 
    /// returned as follows:
    /// 
    /// If result is QSR_SaveOK - We go ahead and save the file and the file is not 
    /// read only at this point.
    /// 
    /// If result is QSR_ForceSaveAs - We invoke the "Save As" functionality which will 
    /// bring up the Save file name dialog.
    /// 
    /// If result is QSR_NoSave_Cancel - We cancel the save operation and indicate that 
    /// the document could not be saved by setting the "pfSaveCanceled" flag.
    /// 
    /// If result is QSR_NoSave_Continue - Nothing to do here as the file need not be 
    /// saved.
    /// </summary>
    /// <param name="dwSave">Flags which specify the file save options:
    /// VSSAVE_Save        - Saves the current file to itself.
    /// VSSAVE_SaveAs      - Prompts the User for a filename and saves the file to 
    ///                      the file specified.
    /// VSSAVE_SaveCopyAs  - Prompts the user for a filename and saves a copy of the 
    ///                      file with a name specified.
    /// VSSAVE_SilentSave  - Saves the file without prompting for a name or confirmation.  
    /// </param>
    /// <param name="pbstrMkDocumentNew">Pointer to the path to the new document.</param>
    /// <param name="pfSaveCanceled">Value 1 if the document could not be saved.</param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
    int IVsPersistDocData.SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
    {
      pbstrMkDocumentNew = null;
      pfSaveCanceled = 0;
      int hr;

      switch (dwSave)
      {
        case VSSAVEFLAGS.VSSAVE_Save:
        case VSSAVEFLAGS.VSSAVE_SilentSave:
          {
            var queryEditQuerySave = GetService<SVsQueryEditQuerySave, IVsQueryEditQuerySave2>();
            // --- Call QueryEditQuerySave
            uint result;
            hr = queryEditQuerySave.QuerySaveFile(
              _FileName, // filename
              0, // flags
              null, // file attributes
              out result); // result

            if (ErrorHandler.Failed(hr))
            {
              return hr;
            }

            // Process according to result from QuerySave
            switch ((tagVSQuerySaveResult)result)
            {
              case tagVSQuerySaveResult.QSR_NoSave_Cancel:
                // --- This is also case tagVSQuerySaveResult.QSR_NoSave_UserCanceled because these
                // --- two tags have the same value.
                pfSaveCanceled = ~0;
                break;

              case tagVSQuerySaveResult.QSR_SaveOK:
                {
                  // Call the shell to do the save for us
                  hr = VsUIShell.SaveDocDataToFile(dwSave, this, _FileName,
                    out pbstrMkDocumentNew, out pfSaveCanceled);
                  if (ErrorHandler.Failed(hr)) return hr;
                }
                break;

              case tagVSQuerySaveResult.QSR_ForceSaveAs:
                {
                  // Call the shell to do the SaveAS for us
                  hr = VsUIShell.SaveDocDataToFile(VSSAVEFLAGS.VSSAVE_SaveAs, this, _FileName,
                    out pbstrMkDocumentNew, out pfSaveCanceled);
                  if (ErrorHandler.Failed(hr)) return hr;
                }
                break;

              case tagVSQuerySaveResult.QSR_NoSave_Continue:
                // In this case there is nothing to do.
                break;

              default:
                throw new COMException(Resources.ExceptionMessageQEQS);
            }
            break;
          }
        case VSSAVEFLAGS.VSSAVE_SaveAs:
        case VSSAVEFLAGS.VSSAVE_SaveCopyAs:
          {
            // Make sure the file name as the right extension
            if (String.Compare(FileExtensionUsed, Path.GetExtension(_FileName), true, CultureInfo.CurrentCulture) != 0)
            {
              _FileName += FileExtensionUsed;
            }
            // Call the shell to do the save for us
            hr = VsUIShell.SaveDocDataToFile(dwSave, this, _FileName, out pbstrMkDocumentNew,
              out pfSaveCanceled);
            if (ErrorHandler.Failed(hr)) return hr;
            break;
          }
        default:
          throw new ArgumentException(Resources.ExceptionMessageSaveFlag);
      }
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Close the IVsPersistDocData object.
    /// </summary>
    /// <returns>S_OK if the function succeeds.</returns>
    /// <remarks>
    /// Override the <see cref="OnCloseEditor"/> method to change the behaviour.
    /// </remarks>
    // --------------------------------------------------------------------------------
    int IVsPersistDocData.Close()
    {
      return OnCloseEditor();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Called by the Running Document Table when it registers the document data. 
    /// </summary>
    /// <param name="docCookie">Handle for the document to be registered.</param>
    /// <param name="pHierNew">Pointer to the IVsHierarchy interface.</param>
    /// <param name="itemidNew">
    /// Item identifier of the document to be registered from VSITEM.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew,
      uint itemidNew)
    {
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Renames the document data.
    /// </summary>
    /// <param name="grfAttribs">
    /// File attribute of the document data to be renamed. See the data type 
    /// __VSRDTATTRIB.
    /// </param>
    /// <param name="pHierNew">
    /// Pointer to the IVsHierarchy interface of the document being renamed.
    /// </param>
    /// <param name="itemidNew">
    /// Item identifier of the document being renamed. See the data type VSITEMID.
    /// </param>
    /// <param name="pszMkDocumentNew">Path to the document being renamed.</param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IVsPersistDocData.RenameDocData(uint grfAttribs, IVsHierarchy pHierNew,
      uint itemidNew, string pszMkDocumentNew)
    {
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Determines if it is possible to reload the document data.
    /// </summary>
    /// <param name="pfReloadable">set to 1 if the document can be reloaded.</param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
    {
      // --- Allow file to be reloaded
      pfReloadable = 1;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Reloads the document data.
    /// </summary>
    /// <param name="grfFlags">
    /// Flag indicating whether to ignore the next file change when reloading the 
    /// document data. This flag should not be set for us since we implement the 
    /// "IVsDocDataFileChangeControl" interface in order to indicate ignoring of file 
    /// changes.
    /// </param>
    /// <returns>S_OK if the method succeeds.</returns>
    // --------------------------------------------------------------------------------
    int IVsPersistDocData.ReloadDocData(uint grfFlags)
    {
      return ((IPersistFileFormat)this).Load(null, grfFlags, 0);
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// This function asks to the QueryEditQuerySave service if it is possible to
    /// edit the file.
    /// </summary>
    /// <returns>
    /// True if the editing of the file are enabled, otherwise returns false.
    /// </returns>
    // --------------------------------------------------------------------------------
    private bool CanEditFile()
    {
      // --- Check the status of the recursion guard
      if (_GettingCheckoutStatus)
      {
        return false;
      }

      try
      {
        // Set the recursion guard
        _GettingCheckoutStatus = true;

        // Get the QueryEditQuerySave service
        var queryEditQuerySave = GetService<SVsQueryEditQuerySave, IVsQueryEditQuerySave2>();

        // Now call the QueryEdit method to find the edit status of this file
        string[] documents = { _FileName };
        uint result;
        uint outFlags;

        // This function can pop up a dialog to ask the user to checkout the file.
        // When this dialog is visible, it is possible to receive other request to change
        // the file and this is the reason for the recursion guard.
        int hr = queryEditQuerySave.QueryEditFiles(
          0, // Flags
          1, // Number of elements in the array
          documents, // Files to edit
          null, // Input flags
          null, // Input array of VSQEQS_FILE_ATTRIBUTE_DATA
          out result, // result of the checkout
          out outFlags // Additional flags
          );
        if (ErrorHandler.Succeeded(hr) && (result == (uint)tagVSQueryEditResult.QER_EditOK))
        {
          // In this case (and only in this case) we can return true from this function.
          return true;
        }
      }
      finally
      {
        _GettingCheckoutStatus = false;
      }
      return false;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets an instance of the RunningDocumentTable (RDT) service which manages the 
    /// set of currently open documents in the environment and then notifies the 
    /// client that an open document has changed.
    /// </summary>
    // --------------------------------------------------------------------------------
    private void NotifyDocChanged()
    {
      // --- Make sure that we have a file name
      if (_FileName.Length == 0)
      {
        return;
      }

      // --- Get a reference to the Running Document Table
      var runningDocTable = (IVsRunningDocumentTable)GetService(typeof(SVsRunningDocumentTable));

      // --- Lock the document
      uint docCookie;
      IVsHierarchy hierarchy;
      uint itemID;
      IntPtr docData;
      int hr = runningDocTable.FindAndLockDocument(
        (uint)_VSRDTFLAGS.RDT_ReadLock,
        _FileName,
        out hierarchy,
        out itemID,
        out docData,
        out docCookie
        );
      ErrorHandler.ThrowOnFailure(hr);

      // --- Send the notification
      hr = runningDocTable.NotifyDocumentChanged(docCookie, (uint)__VSRDTATTRIB.RDTA_DocDataReloaded);

      // --- Unlock the document.
      // --- We have to unlock the document even if the previous call failed.
      runningDocTable.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, docCookie);

      // --- Check Off the call to NotifyDocChanged failed.
      ErrorHandler.ThrowOnFailure(hr);
    }

    #endregion Other methods
  }
}