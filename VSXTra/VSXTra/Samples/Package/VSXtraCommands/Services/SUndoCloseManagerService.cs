// ================================================================================================
// SUndoCloseManagerService.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.08.12, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;

namespace DeepDiver.VSXtraCommands
{
  // ====================================================================================
  /// <summary>
  /// This interface is a markup interface to provide an "address" for 
  /// <see cref="IUndoCloseManagerService"/>.
  /// </summary>
  // ====================================================================================
  [Guid("69CE37EC-74F7-44E8-B915-51D0CE0459A5")]
  public interface SUndoCloseManagerService
  {
  }
}