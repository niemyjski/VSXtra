/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace DeepDiver.DynamicToolWindow
{
  public partial class DynamicWindowControl : UserControl
  {
    private WindowStatus currentState = null;
    /// <summary>
    /// This is the object that will keep track of the state of the IVsWindowFrame
    /// that is hosting this control. The pane should set this property once
    /// the frame is created to enable us to stay up to date.
    /// </summary>
    public WindowStatus CurrentState
    {
      get { return currentState; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        currentState = value;
        // Subscribe to the change notification so we can update our UI
        currentState.StatusChange += new EventHandler<EventArgs>(this.RefreshValues);
        // Update the display now
        this.RefreshValues(this, null);
      }
    }

    public DynamicWindowControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// This method is the call back for state changes events
    /// </summary>
    /// <param name="sender">Event senders</param>
    /// <param name="arguments">Event arguments</param>
    private void RefreshValues(object sender, EventArgs arguments)
    {
      this.xText.Text = currentState.X.ToString(CultureInfo.CurrentCulture);
      this.yText.Text = currentState.Y.ToString(CultureInfo.CurrentCulture);
      this.widthText.Text = currentState.Width.ToString(CultureInfo.CurrentCulture);
      this.heightText.Text = currentState.Height.ToString(CultureInfo.CurrentCulture);
      this.dockedCheckBox.Checked = currentState.IsDockable;
      this.Invalidate();
    }
  }
}
