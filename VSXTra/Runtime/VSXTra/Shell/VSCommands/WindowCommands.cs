// ================================================================================================
// WindowCommands.cs
//
// Created: 2008.07.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra.Shell
{
  // ================================================================================================
  /// <summary>
  /// This static class is a container for all commands in Visual Studio.
  /// </summary>
  // ================================================================================================
  public static partial class VsIde
  {
    // ================================================================================================
    /// <summary>
    /// This class is a container for commands with "Window" prefix.
    /// </summary>
    // ================================================================================================
    public static class Window
    {
      private static string Prefix { get { return "Window"; }}

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Executes the "Window.CloseAllDocuments" command.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public static void CloseAllDocuments()
      {
        ExecuteCommand(CreateCommandName(Prefix, "CloseAllDocuments"), String.Empty);
      }
    }
  }
}