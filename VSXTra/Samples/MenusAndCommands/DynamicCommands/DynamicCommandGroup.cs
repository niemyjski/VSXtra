// ================================================================================================
// DynamicCommandGroup.cs
//
// Created: 2008.06.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.DynamicCommands
{
  // ================================================================================================
  /// <summary>
  /// This type represents a command group owned by the DynamicCommandsPackage.
  /// </summary>
  /// <remarks>
  /// This type is a logical container, its only role is to group the commands having
  /// the same GUID in their comman ID. It inherits from CommandGroup signing the owner package type.
  /// </remarks>
  // ================================================================================================
  [Guid(GuidList.guidDynamicCommandsCmdSetString)]
  public sealed class DynamicCommandGroup : CommandGroup<DynamicCommandsPackage>
  {
    // ================================================================================================
    /// <summary>
    /// This class implements the "VSXtra Sample Command".
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidMyCommand)]
    public sealed class MyCommand : MenuCommandHandler
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Executes the command: outputs a simple message to the output window.
      /// </summary>
      /// <param name="command">Command to be executed.</param>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        Console.WriteLine("Sample Command executed.");
      }
    }

    // ================================================================================================
    /// <summary>
    /// This class implements the "VSXtra Graph Command".
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidMyGraph)]
    public sealed class MyGraph : MenuCommandHandler
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Executes the command: outputs a simple message to the output window.
      /// </summary>
      /// <param name="command">Command to be executed.</param>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        Console.WriteLine("Graph Command executed.");
      }
    }

    // ================================================================================================
    /// <summary>
    /// This class implements the "VSXtra Graph Command".
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidMyZoom)]
    public sealed class MyZoom : MenuCommandHandler
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Executes the command: outputs a simple message to the output window.
      /// </summary>
      /// <param name="command">Command to be executed.</param>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        Console.WriteLine("Zoom Command executed.");
      }
    }

    // ================================================================================================
    /// <summary>
    /// This abstract class implements the command handlers for dynamic visibility commands.
    /// </summary>
    // ================================================================================================
    public abstract class DynamicVisibility : MenuCommandHandler
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Executes the command: outputs a simple message to the output window.
      /// </summary>
      /// <param name="command">Command to be executed.</param>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        var dynCommand1 = GetHandler<DynamicVisibility1>();
        var dynCommand2 = GetHandler<DynamicVisibility2>();
        if (dynCommand1 == null || dynCommand2 == null) return;
        dynCommand1.MenuCommand.Visible = command.CommandID.ID == CmdIDs.cmdidDynVisibility2;
        dynCommand2.MenuCommand.Visible = !dynCommand1.MenuCommand.Visible;
      }
    }

    // ================================================================================================
    /// <summary>
    /// This class implements the command handlers for DynamicVisibility1 command.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidDynVisibility1)]
    public sealed class DynamicVisibility1 : DynamicVisibility { }

    // ================================================================================================
    /// <summary>
    /// This class implements the command handlers for DynamicVisibility2 command.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidDynVisibility2)]
    public sealed class DynamicVisibility2 : DynamicVisibility
    {
      protected override void BeforeBind(OleMenuCommand command)
      {
        command.Visible = false;
      }
    }

    // ================================================================================================
    /// <summary>
    /// This class implements the "VSXtra Graph Command".
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidDynamicTxt)]
    public sealed class DynamicText : MenuCommandHandler
    {
      private int _ClickCount;

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Executes the command: outputs a simple message to the output window.
      /// </summary>
      /// <param name="command">Command to be executed.</param>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        command.Text = "VSXtra Text Changed: " + ++_ClickCount;
      }
    }
  }
}
