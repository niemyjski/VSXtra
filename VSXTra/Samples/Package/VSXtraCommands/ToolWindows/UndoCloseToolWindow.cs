// ================================================================================================
// UndoCloseToolWindow.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.08.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace DeepDiver.VSXtraCommands
{
  /// <summary>
  /// Undo Close Toolwindow
  /// </summary>
  [Guid("ECCC9E97-FD3B-4C15-AF76-EF71A71D8B17")]
  public class UndoCloseToolWindow : ToolWindowPane
  {
    #region Fields

    private readonly UndoCloseControl _Control;

    #endregion

    #region Properties
    /// <summary>
    /// Gets the control.
    /// </summary>
    /// <value>The control.</value>
    public UndoCloseControl Control
    {
      get
      {
        return _Control;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="UndoCloseToolWindow"/> class.
    /// </summary>
    public UndoCloseToolWindow() :
      base(null)
    {
      Caption = Resources.UndoCloseCaption;
      BitmapResourceID = 10969;
      BitmapIndex = 0;

      _Control = new UndoCloseControl(this);
    }
    #endregion

    #region Public Implementation
    /// <summary>
    /// </summary>
    /// <value></value>
    /// The window this dialog page will use for its UI.
    /// This window handle must be constant, so if you are
    /// returning a Windows Forms control you must make sure
    /// it does not recreate its handle.  If the window object
    /// implements IComponent it will be sited by the
    /// dialog page so it can get access to global services.
    public override IWin32Window Window
    {
      get
      {
        return _Control;
      }
    }

    #endregion
  }
}