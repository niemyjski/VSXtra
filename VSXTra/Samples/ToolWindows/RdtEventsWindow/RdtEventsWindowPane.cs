// ================================================================================================
// RdtEventsWindowPane.cs
//
// Created: 2008.08.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra.Commands;
using VSXtra.Documents;
using VSXtra.Windows;

namespace DeepDiver.RdtEventsWindow
{
  // ================================================================================================
  /// <summary>
  /// This class implements the window pane of the RDT Events Explorer tool window.
  /// </summary>
  // ================================================================================================
  [Guid("4CCE3BDE-60B2-4165-9FC9-8682A19BCB70")]
  [InitialCaption("RDT Event Explorer")]
  [Toolbar(typeof(RdtEventsCommandGroup.RdtEventExplorerToolbar))]
  public class RdtEventsWindowPane: ToolWindowPane<RdtEventsWindowPackage, RdtEventControl>
  {
    #region Private fields

    private RdtEventsOptionsPage _Options;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Subscribes to the RDT events
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void OnToolWindowCreated()
    {
      UIControl.SelectionTracker = SelectionTracker;
      RunningDocumentTable.OnAfterAttributeChange += OnAfterAttributeChange;
      RunningDocumentTable.OnAfterAttributeChangeEx += OnAfterAttributeChangeEx;
      RunningDocumentTable.OnAfterDocumentWindowHide += OnAfterDocumentWindowHide;
      RunningDocumentTable.OnAfterFirstDocumentLock += OnAfterFirstDocumentLock;
      RunningDocumentTable.OnAfterLastDocumentUnlock += OnAfterLastDocumentUnlock;
      RunningDocumentTable.OnAfterSave += OnAfterSave;
      RunningDocumentTable.OnAfterSaveAll += OnAfterSaveAll;
      RunningDocumentTable.OnBeforeDocumentWindowShow += OnBeforeDocumentWindowShow;
      RunningDocumentTable.OnBeforeFirstDocumentLock += OnBeforeFirstDocumentLock;
      RunningDocumentTable.OnBeforeLastDocumentUnlock += OnBeforeLastDocumentUnlock;
      RunningDocumentTable.OnBeforeSave += OnBeforeSave;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Unsubscribes from the RDT events
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void Dispose(bool disposing)
    {
      RunningDocumentTable.OnAfterAttributeChange -= OnAfterAttributeChange;
      RunningDocumentTable.OnAfterAttributeChangeEx -= OnAfterAttributeChangeEx;
      RunningDocumentTable.OnAfterDocumentWindowHide -= OnAfterDocumentWindowHide;
      RunningDocumentTable.OnAfterFirstDocumentLock -= OnAfterFirstDocumentLock;
      RunningDocumentTable.OnAfterLastDocumentUnlock -= OnAfterLastDocumentUnlock;
      RunningDocumentTable.OnAfterSave -= OnAfterSave;
      RunningDocumentTable.OnAfterSaveAll -= OnAfterSaveAll;
      RunningDocumentTable.OnBeforeDocumentWindowShow -= OnBeforeDocumentWindowShow;
      RunningDocumentTable.OnBeforeFirstDocumentLock -= OnBeforeFirstDocumentLock;
      RunningDocumentTable.OnBeforeLastDocumentUnlock -= OnBeforeLastDocumentUnlock;
      RunningDocumentTable.OnBeforeSave -= OnBeforeSave;
      base.Dispose(disposing);
    }

    #endregion

    #region Private properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the options page for the RDT Event Explorer
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private RdtEventsOptionsPage Options
    {
      get
      {
        if (_Options == null)
        {
          _Options = Package.GetDialogPage<RdtEventsOptionsPage>();
        }
        return _Options;
      }
    }

    #endregion

    #region RDT event handler methods

    void OnBeforeSave(object sender, RdtEventArgs e)
    {
      if (Options.OptBeforeSave) LogEvent(e);
    }

    void OnBeforeLastDocumentUnlock(object sender, RdtLockEventArgs e)
    {
      if (Options.OptBeforeLastDocumentUnlock) LogEvent(e);
    }

    void OnBeforeFirstDocumentLock(object sender, RdtFirstLockEventArgs e)
    {
      if (Options.OptBeforeFirstDocumentLock) LogEvent(e);
    }

    void OnBeforeDocumentWindowShow(object sender, RdtWindowShowEventArgs e)
    {
      if (Options.OptBeforeDocumentWindowShow) LogEvent(e);
    }

    void OnAfterSaveAll(object sender, RdtEventArgs e)
    {
      if (Options.OptAfterSaveAll) LogEvent(e);
    }

    void OnAfterSave(object sender, RdtEventArgs e)
    {
      if (Options.OptAfterSave) LogEvent(e);
    }

    void OnAfterLastDocumentUnlock(object sender, RdtLastUnlockEventArgs e)
    {
      if (Options.OptAfterLastDocumentUnlock) LogEvent(e);
    }

    void OnAfterFirstDocumentLock(object sender, RdtLockEventArgs e)
    {
      if (Options.OptAfterFirstDocumentLock) LogEvent(e);
    }

    void OnAfterDocumentWindowHide(object sender, RdtWindowEventArgs e)
    {
      if (Options.OptAfterDocumentWindowHide) LogEvent(e);
    }

    void OnAfterAttributeChangeEx(object sender, RdtDocumentChangedEventArgs e)
    {
      if (Options.OptAfterAttributeChangeEx) LogEvent(e);
    }

    void OnAfterAttributeChange(object sender, RdtDocumentChangedEventArgs e)
    {
      if (Options.OptAfterAttributeChange) LogEvent(e);
    }

    void LogEvent(RdtEventArgs e)
    {
      UIControl.AddEventToGrid(e);
    }

    #endregion

    #region Toolbar command handlers

    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidRefreshList)]
    private void RefreshList()
    {
    }

    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidClearList)]
    private void ClearList()
    {
      UIControl.ClearGrid();
    }

    #endregion
  }
}