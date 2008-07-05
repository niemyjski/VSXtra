// ================================================================================================
// PersistedWindowPane.cs
//
// Created: 2008.07.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra;

namespace DeepDiver.PersistedToolWindow
{
  [Guid("0A6F8EDC-5DDB-4aaa-A6B3-2AC1E319693E")]
  [InitialCaption("Persisted Tool Window")]
  [BitmapResourceId(301)]
  [Toolbar(typeof(DynamicToolWindowCommandGroup.PersistedWindowToolbar))]
  class PersistedWindowPane : ToolWindowPane<PersistedToolWindowPackage, PersistedWindowControl>
  {
    /// <summary>
    /// This is called after our control has been created and sited.
    /// This is a good place to initialize the control with data gathered
    /// from Visual Studio services.
    /// </summary>
    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      UIControl.TrackSelection = (ITrackSelection)this.GetService(typeof(STrackSelection));
      RefreshList(this, null);
    }

    private void RefreshList(object sender, EventArgs arguments)
    {
      UIControl.RefreshData();
    }
  }
}
