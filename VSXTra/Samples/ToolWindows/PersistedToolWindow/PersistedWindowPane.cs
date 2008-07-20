// ================================================================================================
// PersistedWindowPane.cs
//
// Created: 2008.07.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
  [Toolbar(typeof(DynamicToolWindowCommandGroup.PersistedWindowToolbar))]
  class PersistedWindowPane : ToolWindowPane<PersistedToolWindowPackage, PersistedWindowControl>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Our tool window has been sited, refresh the list of tool windows.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      UIControl.TrackSelection = GetService<STrackSelection, ITrackSelection>();
      RefreshList(null);
    }

    #region Command handler methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the "Refresh List" toolbar command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [Promote]
    [CommandId(CmdIDs.cmdidRefreshWindowsList)]
    private void RefreshList(OleMenuCommand command)
    {
      UIControl.RefreshData();
    }

    #endregion
  }
}
