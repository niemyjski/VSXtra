// ================================================================================================
// WpfWindowPane.cs
//
// Revised: 2015.03.25, by Blake Niemyjski
// ================================================================================================

using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Forms.Integration;
using VSXtra.Package;
using WinFormsControl = System.Windows.Forms.Control;

namespace VSXtra.Windows
{
  // ================================================================================================
  /// <summary>
  /// This class implements a WPF hosting tool window pane by specializing the ToolWindowPane.
  /// </summary>
  /// <typeparam name="TPackage">The type of the package owning this window pane.</typeparam>
  /// <typeparam name="TUIElement">The type of <see cref="UIElement"/> represnting the UI.</typeparam>
  // ================================================================================================
  [ComVisible(true)]
  public abstract class WpfWindowPane<TPackage, TUIElement> :
    WindowPane<TPackage, WinFormsControl>
    where TPackage : PackageBase
    where TUIElement : UIElement, new()
  {
    /// <summary>User control representing the UI of the window pane.</summary>
    private readonly ElementHost _UIControlHost;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of WpfToolWindowPane wrapping the UI control into an ElementHost.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected WpfWindowPane()
    {
      _UIControlHost = new ElementHost {Child = new TUIElement()};
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of WpfToolWindowPane using the specified ElementHost.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected WpfWindowPane(ElementHost controlHost)
    {
      _UIControlHost = controlHost;
      _UIControlHost.Child = new TUIElement();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the ElementHost behind this WPF window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override WinFormsControl UIControl
    {
      get { return _UIControlHost; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the UIElement behind this WPF window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public UIElement HostedControl
    {
      get { return _UIControlHost.Child; }
    }

  }
}