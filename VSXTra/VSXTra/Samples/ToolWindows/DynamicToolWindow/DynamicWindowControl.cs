// ================================================================================================
// DynamicWindowControl.cs
//
// Created: 2008.07.03, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Drawing;
using System.Windows.Forms;
using VSXtra;

namespace DeepDiver.DynamicToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This class represents the UI of the tool window.
  /// </summary>
  // ================================================================================================
  public partial class DynamicWindowControl : UserControl
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the UI of the tool window
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public DynamicWindowControl()
    {
      InitializeComponent();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Refresh the controls' values in case the window pane's visual status changes.
    /// </summary>
    /// <param name="sender">Frame hosting the window pane.</param>
    /// <param name="arguments">Event arguments.</param>
    // --------------------------------------------------------------------------------------------
    public void RefreshValues(object sender, EventArgs arguments)
    {
      var frame = sender as WindowFrame;
      if (frame == null) return;
      Rectangle rect;
      var framePos = frame.GetWindowPosition(out rect);
      xText.Text = rect.Left.ToString();
      yText.Text = rect.Top.ToString();
      widthText.Text = rect.Width.ToString();
      heightText.Text = rect.Height.ToString();
      dockedCheckBox.Checked = framePos == FramePosition.Docked;
      Invalidate();
    }
  }
}
