// ================================================================================================
// NumberStackControl.cs
//
// Created: 2008.07.11, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Windows.Forms;
using VSXtra;

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

    public NumberStackControl()
    {
      InitializeComponent();
    }

    #endregion

    #region User control events

    private void TypedTextEdit_TextChanged(object sender, EventArgs e)
    {
      PushToStack.Enabled = TypedTextEdit.Text.Trim().Length > 0;
    }

    private void PushToStack_Click(object sender, EventArgs e)
    {
      PushText(TypedTextEdit.Text);
    }

    #endregion

    #region Public methods

    public bool HasAtLeastTwoOperands
    {
      get { return NumberStack.Items.Count > 1; }
    }

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

    public int PopValue()
    {
      if (NumberStack.Items.Count == 0)
        throw new InvalidOperationException("Number stack is empty.");
      int result = (int)NumberStack.Items[0];
      NumberStack.Items.RemoveAt(0);
      return result;
    }

    public delegate int BinaryOperator(int x, int y);

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
