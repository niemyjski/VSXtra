// ================================================================================================
// NumberStackWindowPane.cs
//
// Created: 2008.07.11, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.MultiToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This class defines the abstract base class of Number Stack tool window panes.
  /// </summary>
  // ================================================================================================
  public abstract class NumberStackWindowPane: ToolWindowPane<MultiToolWindowPackage, NumberStackControl>
  {
    #region Command handler methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the command status of binary operators.
    /// </summary>
    /// <param name="command">Command object to set the status for.</param>
    // --------------------------------------------------------------------------------------------
    [CommandStatusMethod]
    [PromoteCommand]
    [CommandId(CmdIDs.cmdidAdd1)]
    [CommandId(CmdIDs.cmdidSubtract1)]
    [CommandId(CmdIDs.cmdidMultiply1)]
    [CommandId(CmdIDs.cmdidDivide1)]
    protected void OperationStatus(OleMenuCommand command)
    {
      command.Enabled = UIControl.HasAtLeastTwoOperands;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Add operator.
    /// </summary>
    /// <param name="command">Command object.</param>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidAdd1)]
    protected void AddExec(OleMenuCommand command)
    {
      UIControl.Operation((x, y) => { checked { return x + y; } });
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Subtract operator.
    /// </summary>
    /// <param name="command">Command object.</param>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidSubtract1)]
    protected void SubtractExec(OleMenuCommand command)
    {
      UIControl.Operation((x, y) => { checked { return x - y; } });
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Multiply operator.
    /// </summary>
    /// <param name="command">Command object.</param>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidMultiply1)]
    protected void MultiplyExec(OleMenuCommand command)
    {
      UIControl.Operation((x, y) => { checked { return x * y; } });
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Divide operator.
    /// </summary>
    /// <param name="command">Command object.</param>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [CommandId(CmdIDs.cmdidDivide1)]
    protected void DivideExec(OleMenuCommand command)
    {
      UIControl.Operation((x, y) => { checked { return x / y; } });
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the command status of Cut and Copy commands.
    /// </summary>
    /// <param name="command">Command object to set the status for.</param>
    // --------------------------------------------------------------------------------------------
    [CommandStatusMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Cut)]
    [VsCommandId(VSConstants.VSStd97CmdID.Copy)]
    protected void CutCopyStatus(OleMenuCommand command)
    {
      command.Enabled = UIControl.HasAtLeastOneOperand;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the command status of Paste commands.
    /// </summary>
    /// <param name="command">Command object to set the status for.</param>
    // --------------------------------------------------------------------------------------------
    [CommandStatusMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Paste)]
    protected void PasteStatus(OleMenuCommand command)
    {
      command.Enabled = Clipboard.ContainsText();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Cut command.
    /// </summary>
    /// <param name="command">Command object.</param>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Cut)]
    protected void CutExec(OleMenuCommand command)
    {
      int value = UIControl.PopValue();
      Clipboard.SetText(value.ToString());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Copy command.
    /// </summary>
    /// <param name="command">Command object.</param>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Copy)]
    protected void CopyExec(OleMenuCommand command)
    {
      int value = UIControl.PeekValue();
      Clipboard.SetText(value.ToString());
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the Paste command.
    /// </summary>
    /// <param name="command">Command object.</param>
    // --------------------------------------------------------------------------------------------
    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Paste)]
    protected void PasteExec(OleMenuCommand command)
    {
      UIControl.PushText(Clipboard.GetText());
    }

    #endregion
  }

  // ================================================================================================
  /// <summary>
  /// This class defines the first instance of Number stack tool window pane.
  /// </summary>
  // ================================================================================================
  [Guid("B097B9C4-D98C-45fe-B355-DE4865E77DCF")]
  [BitmapResourceId(300, 1)]
  [ToolbarLocation(ToolbarLocation.Right)]
  [Toolbar(typeof(CommandGroup.StackWindowToolbar1))]
  public sealed class NumberStackWindowPane1 : NumberStackWindowPane
  {
  }

  // ================================================================================================
  /// <summary>
  /// This class defines the first instance of Number stack tool window pane.
  /// </summary>
  // ================================================================================================
  [Guid("7B5DC947-D280-4255-B525-3AA62AAA52D4")]
  [BitmapResourceId(300, 2)]
  [ToolbarLocation(ToolbarLocation.Right)]
  [Toolbar(typeof(CommandGroup.StackWindowToolbar2))]
  [CommandMap(CmdIDs.cmdidAdd1, CmdIDs.cmdidDivide1, (int)(CmdIDs.cmdidAdd2 - CmdIDs.cmdidAdd1))]
  public sealed class NumberStackWindowPane2 : NumberStackWindowPane
  {
  }
}