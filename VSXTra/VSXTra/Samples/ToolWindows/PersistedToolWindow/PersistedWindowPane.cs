// ================================================================================================
// PersistedWindowPane.cs
//
// Created: 2008.07.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.PersistedToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This class defines a persisted window pane.
  /// </summary>
  // ================================================================================================
  [Guid("0A6F8EDC-5DDB-4aaa-A6B3-2AC1E319693E")]
  [InitialCaption("Persisted Tool Window")]
  [BitmapResourceId(301)]
  [Toolbar(typeof(PersistedToolWindowCommandGroup.PersistedWindowToolbar))]
  class PersistedWindowPane : ToolWindowPane<PersistedToolWindowPackage, PersistedWindowControl>
  {
    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Our tool window has been sited, refresh the list of tool windows.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void OnToolWindowCreated()
    {
      UIControl.SelectionTracker = SelectionTracker;
      RefreshList();
    }

    #endregion

    #region Command handler methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the "Refresh List" toolbar command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [Promote]
    [CommandId(CmdIDs.cmdidRefreshWindowsList)]
    private void RefreshList()
    {
      UIControl.RefreshData();
    }

    #endregion
  }
}
