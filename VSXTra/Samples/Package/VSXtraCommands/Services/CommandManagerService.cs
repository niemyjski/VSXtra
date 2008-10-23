// ================================================================================================
// CommandManagerService.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.23, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VSXtra;
using VSXtra.Commands;
using VSXtra.Package;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// This class stores registered commands.
  /// </summary>
  /// <remarks>
  /// This class is proffered as a service for other VS packages. Provides operations for
  /// registering, unregistering commands and retrieving the list of registered commands.
  /// </remarks>
  // ================================================================================================
  [Promote]
  internal class CommandManagerService : 
    VsxService<VSXtraCommandsPackage, SCommandManagerService>,
    ICommandManagerService
  {
    #region Fields

    private readonly IList<MenuCommandHandler> _RegisteredCommands = 
      new List<MenuCommandHandler>();
    
    #endregion

    #region Public Implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Registers a command with the command manager.
    /// </summary>
    /// <param name="command">The command to register.</param>
    // --------------------------------------------------------------------------------------------
    public void RegisterCommand(MenuCommandHandler command)
    {
      if (_RegisteredCommands.SingleOrDefault(
          cmd => cmd.Guid.Equals(command.Guid) && cmd.Id.Equals(command.Id)) == null)
      {
        _RegisteredCommands.Add(command);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Unregisters a previously registered command.
    /// </summary>
    /// <param name="command">Command to unregister.</param>
    // --------------------------------------------------------------------------------------------
    public void UnRegisterCommand(MenuCommandHandler command)
    {
      if (_RegisteredCommands.SingleOrDefault(
          cmd => cmd.Guid.Equals(command.Guid) && cmd.Id.Equals(command.Id)) != null)
      {
        _RegisteredCommands.Remove(command);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a read-only list of the commands registered with the command manager.
    /// </summary>
    /// <returns>Read-only list of the commands registered with the command manager.</returns>
    // --------------------------------------------------------------------------------------------
    public IEnumerable<MenuCommandHandler> GetRegisteredCommands()
    {
      return new ReadOnlyCollection<MenuCommandHandler>(_RegisteredCommands);
    }

    #endregion
  }
}