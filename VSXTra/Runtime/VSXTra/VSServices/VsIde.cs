// ================================================================================================
// VsIde.cs
//
// Created: 2008.07.26, by Istvan Novak (DeepDiver)
// ================================================================================================
using EnvDTE;
using EnvDTE80;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This static class represents behavior of the DTE2 object belonging to the current VS IDE 
  /// instance.
  /// </summary>
  // ================================================================================================
  public static class VsIde
  {
    #region Private fields

    private static DTE2 _DteInstance;

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the DTE2 object representing the VS IDE instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static DTE2 DteInstance
    {
      get
      {
        if (_DteInstance == null)
          _DteInstance = PackageBase.GetGlobalService<DTE>() as DTE2;
        return _DteInstance;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a ToolWindows object used as a shortcut for finding tool windows.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static ToolWindows ToolWindows
    {
      get { return DteInstance.ToolWindows; }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the specified command.
    /// </summary>
    /// <param name="commandName">Required. The name of the command to invoke.</param>
    /// <param name="commandArgs">
    /// Optional. A string containing the same arguments you would supply if you were invoking 
    /// the command from the Command window. If a string is supplied, it is passed to the command 
    /// line as the command's first argument and is parsed to form the various arguments for the 
    /// command. This is similar to how commands are invoked in the Command window.
    /// </param>
    /// <remarks>
    /// ExecuteCommand runs commands or macros listed in the Keyboard section of the Environment 
    /// panel of the Options dialog box on the Tools menu.
    /// You can also invoke commands or macros by running them from the command line, in the 
    /// Command window, or by pressing toolbar buttons or keystrokes associated with them.
    /// ExecuteCommand cannot execute commands that are currently disabled in the environment. 
    /// The Build method, for example, will not execute while a build is currently in progress.
    /// ExecuteCommand implicitly pauses macro recording so that the executing command does not 
    /// emit macro code. This prevents double code emission when recording and invoking macros as 
    /// part of what you are recording.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public static void ExecuteMethod(string commandName, string commandArgs)
    {
      DteInstance.ExecuteCommand(commandName, commandArgs);
    }

    #endregion
  }
}