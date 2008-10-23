// ================================================================================================
// DynamicWindowPane.cs
//
// Created: 2008.07.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using VSXtra.Diagnostics;
using VSXtra.Windows;

namespace DeepDiver.DynamicToolWindow
{
  // ================================================================================================
  /// <summary>
  /// This class represents the dynamic tool window.
  /// </summary>
  // ================================================================================================
  [Guid("F0E1E9A1-9860-484d-AD5D-367D79AABF55")]
  [InitialCaption("Dynamic Tool Window")]
  [BitmapResourceId(301)]
  class DynamicWindowPane : ToolWindowPane<DynamicToolWindowPackage, DynamicWindowControl>
  {
    private OutputWindowPane _OutputPane;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Here we set up events related to the tool window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void OnToolWindowCreated()
    {
      base.OnToolWindowCreated();
      
      // --- Set up the window pane
      _OutputPane = OutputWindow.GetPane<EventsPane>();
      VsDebug.Assert(_OutputPane != null, "Output pane creation failed.");
      
      // --- Set up window frame events
      Frame.OnShow += OnFrameShow;
      Frame.OnClose += OnFrameClose;
      Frame.OnResize += OnFrameResize;
      Frame.OnMove += OnFrameMove;
      Frame.OnDockChange += OnFrameDockChange;
      Frame.OnStatusChange += UIControl.RefreshValues;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The dock position has been chnaged.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void OnFrameDockChange(object sender, WindowFrameDockChangedEventArgs e)
    {
      _OutputPane.WriteLine("Dock state changed.");
      _OutputPane.WriteLine("  Docked: {0}", e.Docked);
      DisplayPosition(e.Position);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The tool window has been moved.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void OnFrameMove(object sender, WindowFramePositionChangedEventArgs e)
    {
      _OutputPane.WriteLine("Frame moved.");
      DisplayPosition(e.Position);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The tool window has been resized.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void OnFrameResize(object sender, WindowFramePositionChangedEventArgs e)
    {
      _OutputPane.WriteLine("Frame sized.");
      DisplayPosition(e.Position);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The tool window has been closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void OnFrameClose(object sender, WindowFrameCloseEventArgs e)
    {
      _OutputPane.WriteLine("Frame closed.");
      _OutputPane.WriteLine("  Reason: {0}", e.CloseOption);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The show state of the tool window has been changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void OnFrameShow(object sender, WindowFrameShowEventArgs e)
    {
      _OutputPane.WriteLine("Frame show state changed.");
      _OutputPane.WriteLine("  Reason: {0}", e.Reason);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The show state of the tool window has been changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void DisplayPosition(Rectangle rect)
    {
      _OutputPane.WriteLine("  New position: {0}", rect);
    }
  }

  // ================================================================================================
  /// <summary>
  /// This class represents the output window pane to be used to output window events.
  /// </summary>
  // ================================================================================================
  [AutoActivate(true)]
  [DisplayName("Dynamic window events")]
  class EventsPane : OutputPaneDefinition
  {
  }
}
