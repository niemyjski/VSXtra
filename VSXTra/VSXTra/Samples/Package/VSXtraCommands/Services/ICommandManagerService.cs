// ================================================================================================
// ICommandManagerService.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.23, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.VSXtraCommands
{
  // ====================================================================================
  /// <summary>
  /// Service for managing registered commands
  /// </summary>
  // ====================================================================================
  [Guid("B8BEFDF0-DFB9-48cc-8726-E89C9EF24F20")]
  [ComVisible(true)]
  public interface ICommandManagerService
  {
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Registers the command.
    /// </summary>
    /// <param name="command">The command.</param>
    // --------------------------------------------------------------------------------
    void RegisterCommand(MenuCommandHandler command);

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Unregisters a registered command.
    /// </summary>
    /// <param name="command">The command.</param>
    // --------------------------------------------------------------------------------
    void UnRegisterCommand(MenuCommandHandler command);

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves registered commands.
    /// </summary>
    /// <returns></returns>
    // --------------------------------------------------------------------------------
    IEnumerable<MenuCommandHandler> GetRegisteredCommands();
  }
}