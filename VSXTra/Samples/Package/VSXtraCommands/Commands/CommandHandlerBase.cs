// ================================================================================================
// CommandHandlerBase.cs
//
// Created: 2008.07.26, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Linq;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to be the base class of all VSXtraCommands handler.
  /// </summary>
  /// <remarks>
  /// This class automatically checks if the command is enabled on the Commands options page. If it 
  /// is the CanExecute method is called. If you want to set a specific command state you must 
  /// override the CanExecute method.
  /// </remarks>
  // ================================================================================================
  public abstract class CommandHandlerBase : MenuCommandHandler 
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method defines how the command status should be queried.
    /// </summary>
    /// <param name="command">OleMenuCommand instance</param>
    /// <remarks>
    /// Generally you do not need to override this method. For command specific checks, override 
    /// the CanExecute method.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected override void OnQueryStatus(OleMenuCommand command)
    {
      // --- Check if the command is enabled on the Commands options page or not
      var commandsPage = Package.GetDialogPage<CommandsPage>();
      bool canExecute = 
        commandsPage.DisabledCommands.SingleOrDefault(
          cmd => cmd.Guid.Equals(command.CommandID.Guid) &&
              cmd.ID.Equals(command.CommandID.ID)) == null;
      if (canExecute)
      {
        // --- Command is enabled, so we examine the command specific status
        canExecute = CanExecute(command);
      }
      command.Enabled = command.Visible = command.Supported = canExecute;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method if you want to add command specific check on the command status.
    /// </summary>
    /// <param name="command">Command to check</param>
    /// <remarks>
    /// You do not need to call the base implementation, just simply define your own check.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual bool CanExecute(OleMenuCommand command)
    {
      return true;
    }
  }
}