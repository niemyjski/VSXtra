// ================================================================================================
// NumberStackWindowPane.cs
//
// Created: 2008.07.11, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.MultiToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This class defines the Number Stack tool window pane.
  /// </summary>
  // ================================================================================================
  [Guid("8DA957B1-85D3-4cb3-8FDA-A4CD30CF9A39")]
  [BitmapResourceId(300, 2)]
  [ToolbarLocation(ToolbarLocation.Right)]
  [Toolbar(typeof(CommandGroup.StackWindowToolbar))]
  public sealed class NumberStackWindowPane: ToolWindowPane<MultiToolWindowPackage, NumberStackControl>
  {
    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      Caption = "Number Stack Window #" + InstanceID;
      VsUIShell.UpdateCommandUI();
    }

    #region Command handler methods

    [CommandStatusMethod]
    [PromoteCommand]
    [CommandId(CmdIDs.cmdidAdd1)]
    [CommandId(CmdIDs.cmdidSubtract1)]
    [CommandId(CmdIDs.cmdidMultiply1)]
    [CommandId(CmdIDs.cmdidDivide1)]
    [CommandId(CmdIDs.cmdidAdd2)]
    [CommandId(CmdIDs.cmdidSubtract2)]
    [CommandId(CmdIDs.cmdidMultiply2)]
    [CommandId(CmdIDs.cmdidDivide2)]
    private void OperationStatus(OleMenuCommand command)
    {
      var id = command.CommandID.ID;
      command.Visible =
        (InstanceID == 1 && id >= CmdIDs.cmdidAdd1 && id <= CmdIDs.cmdidDivide1) ||
        (InstanceID == 2 && id >= CmdIDs.cmdidAdd2 && id <= CmdIDs.cmdidDivide2);
      OutputWindow.Debug.WriteLine("{0}:{1}", id, command.Visible);
      command.Enabled = UIControl.HasAtLeastTwoOperands;
    }

    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidAdd1)]
    [CommandId(CmdIDs.cmdidAdd2)]
    private void AddExec(OleMenuCommand command)
    {
      UIControl.Operation((x, y) => { checked { return x + y; } });
    }

    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidSubtract1)]
    [CommandId(CmdIDs.cmdidSubtract2)]
    private void SubtractExec(OleMenuCommand command)
    {
      UIControl.Operation((x, y) => { checked { return x - y; } });
    }

    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidMultiply1)]
    [CommandId(CmdIDs.cmdidMultiply2)]
    private void MultiplyExec(OleMenuCommand command)
    {
      UIControl.Operation((x, y) => { checked { return x * y; } });
    }

    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidDivide1)]
    [CommandId(CmdIDs.cmdidDivide2)]
    private void DivideExec(OleMenuCommand command)
    {
      UIControl.Operation((x, y) => { checked { return x / y; } });
    }

    #endregion
  }
}