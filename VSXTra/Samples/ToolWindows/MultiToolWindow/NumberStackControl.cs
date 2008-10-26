// ================================================================================================
// NumberStackControl.cs
//
// Created: 2008.07.11, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Windows.Forms;
using VSXtra.Shell;

namespace DeepDiver.MultiToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This class represents the user interface for the Number stack window.
  /// </summary>
  // ================================================================================================
  public partial class NumberStackControl : UserControl
  {
    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates and initializes the control instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public NumberStackControl()
    {
      InitializeComponent();
    }

    #endregion

    #region User control events

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Enables/Disables the push button according to the text typed in.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void TypedTextEdit_TextChanged(object sender, EventArgs e)
    {
      PushToStack.Enabled = TypedTextEdit.Text.Trim().Length > 0;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Pushes a text or number to the stack.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void PushToStack_Click(object sender, EventArgs e)
    {
      PushText(TypedTextEdit.Text);
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------

    #region Delegates

    /// <summary>
    /// Delegate representing a binary operator.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public delegate int BinaryOperator(int x, int y);

    #endregion

    /// <summary>
    /// Gets the flag indicating if a binary operation can be executed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool HasAtLeastTwoOperands
    {
      get { return NumberStack.Items.Count > 1; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if there is any number on the stack.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool HasAtLeastOneOperand
    {
      get { return NumberStack.Items.Count > 0; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Pushes the specified text to the appropriate stack.
    /// </summary>
    /// <param name="text">Text to push to the stack.</param>
    /// <remarks>
    /// If the text can be parsed as an Int32 value, it goes to the number stack; otehrwise to the
    /// text stack.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public void PushText(string text)
    {
      string toPush = text.Trim();
      int number;
      if (Int32.TryParse(toPush, out number))
      {
        NumberStack.Items.Insert(0, number);
      }
      else
      {
        TextStack.Items.Insert(0, toPush);
      }
      VsUIShell.UpdateCommandUI();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Pops and retrieves the value on the number stack.
    /// </summary>
    /// <returns>Top item on the number stack.</returns>
    // --------------------------------------------------------------------------------------------
    public int PopValue()
    {
      if (NumberStack.Items.Count == 0)
        throw new InvalidOperationException("Number stack is empty.");
      var result = (int) NumberStack.Items[0];
      NumberStack.Items.RemoveAt(0);
      return result;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Peeks and retrieves the value on the number stack without removing it.
    /// </summary>
    /// <returns>Top item on the number stack.</returns>
    // --------------------------------------------------------------------------------------------
    public int PeekValue()
    {
      if (NumberStack.Items.Count == 0)
        throw new InvalidOperationException("Number stack is empty.");
      return (int) NumberStack.Items[0];
    }

    // --------------------------------------------------------------------------------------------

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes the specified binary operation.
    /// </summary>
    /// <param name="oper">Operation to execute on the top two number stack items.</param>
    /// <remarks>The result is pushed back to the stack.</remarks>
    // --------------------------------------------------------------------------------------------
    public void Operation(BinaryOperator oper)
    {
      int y = PopValue();
      int x = PopValue();
      try
      {
        PushText(oper(x, y).ToString());
      }
      catch (SystemException ex)
      {
        PushText(String.Format("*** {0} ***", ex.Message));
      }
    }

    #endregion
  }
}