// ================================================================================================
// DynamicWindowPane.cs
//
// Created: 2008.07.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.ComponentModel;
using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.DynamicToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This class represents the dynamic tool window.
  /// </summary>
  // ================================================================================================
  [Guid("F0E1E9A1-9860-484d-AD5D-367D79AABF55")]
  [InitialCaption("Dynamic Tool Window")]
  class DynamicWindowPane : ToolWindowPane<DynamicToolWindowPackage, DynamicWindowControl>
  {
    //private OutputWindowPane _OutputPane;

    //// --------------------------------------------------------------------------------------------
    ///// <summary>
    ///// This is called after our control has been created and sited.
    ///// This is a good place to initialize the control with data gathered
    ///// from Visual Studio services.
    ///// </summary>
    //// --------------------------------------------------------------------------------------------
    //public override void OnToolWindowCreated()
    //{
    //  base.OnToolWindowCreated();
    //  _OutputPane = OutputWindow.GetPane<EventsPane>();
    //  // Register to the window events
    //  // WindowStatus windowFrameEventsHandler = new WindowStatus(this.OutputWindowPane);
    //  // ErrorHandler.ThrowOnFailure(((IVsWindowFrame)this.Frame).SetProperty((int)__VSFPROPID.VSFPROPID_ViewHelper, (IVsWindowFrameNotify3)windowFrameEventsHandler));
    //  // Let our control have access to the window state
    //  // control.CurrentState = windowFrameEventsHandler;
    //}
  }

  //// ================================================================================================
  ///// <summary>
  ///// This class represents the output window pane to be used to output window events.
  ///// </summary>
  //// ================================================================================================
  //[AutoActivate(true)]
  //[DisplayName("Dynamic window events")]
  //class EventsPane: OutputPaneDefinition
  //{
  //}
}
