// ================================================================================================
// SCommandManagerService.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.23, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;

namespace DeepDiver.VSXtraCommands
{
  // ====================================================================================
  /// <summary>
  /// This interface is a markup interface to provide an "address" for 
  /// <see cref="ICommandManagerService"/>.
  /// </summary>
  // ====================================================================================
  [Guid("357C77BD-7F09-47E6-82E7-2E847D73204C")]
  public interface SCommandManagerService
  {
  }
}