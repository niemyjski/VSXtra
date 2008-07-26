// ================================================================================================
// ClearAllPanes.cs
//
// Created: 2008.07.23, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.VSXtraCommands
{
  public partial class VSXtraCommandGroup
  {
    // ================================================================================================
    /// <summary>
    /// This class implements the ClearAllPanes command.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.ClearAllPanesCommand)]
    [DisplayName("Clear All Panes")]
    public sealed class ClearAllPanes: CommandHandlerBase
    {
      protected override void OnExecute(OleMenuCommand command)
      {
        OutputWindow.OutputWindowPanes.ForEach(pane => pane.Clear());
      }
    }
  }
}