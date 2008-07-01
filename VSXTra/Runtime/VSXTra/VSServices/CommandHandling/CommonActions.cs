// ================================================================================================
// CommonActions.cs
//
// Created: 2008.07.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace VSXtra
{
  #region WriteMessageAction

  // ================================================================================================
  /// <summary>
  /// This class implements a menu action that puts a message to the output window.
  /// </summary>
  // ================================================================================================
  public sealed class WriteMessageActionAttribute : ActionAttribute
  {
    private readonly string _Message;

    public WriteMessageActionAttribute(string message)
    {
      _Message = message;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Puts the message to the current pane of the output window.
    /// </summary>
    /// <param name="command">Menu command instance.</param>
    // --------------------------------------------------------------------------------------------
    public override void ExecuteAction(OleMenuCommand command)
    {
      Console.WriteLine(_Message);
    }
  }

  #endregion

  #region ShowMessageAction

  // ================================================================================================
  /// <summary>
  /// This class implements a menu action that shows a message in a dialog box.
  /// </summary>
  // ================================================================================================
  public sealed class ShowMessageActionAttribute : ActionAttribute
  {
    private string _Text;
    private string _Title = String.Empty;
    private MessageBoxButtons _Buttons = MessageBoxButtons.OK;
    private MessageBoxIcon _Icon = MessageBoxIcon.Information;
    private MessageBoxDefaultButton _DefaultButton = MessageBoxDefaultButton.Button1;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an attribute instance with the specified message attributes.
    /// </summary>
    /// <param name="text">Message text</param>
    // --------------------------------------------------------------------------------------------
    public ShowMessageActionAttribute(string text)
    {
      _Text = text;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an attribute instance with the specified message attributes.
    /// </summary>
    /// <param name="text">Message text</param>
    /// <param name="title">Title of the message</param>
    // --------------------------------------------------------------------------------------------
    public ShowMessageActionAttribute(string text, string title)
    {
      _Text = text;
      _Title = title;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an attribute instance with the specified message attributes.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="text">Message text</param>
    /// <param name="buttons">Buttons to show on the message box</param>
    // --------------------------------------------------------------------------------------------
    public ShowMessageActionAttribute(string text, string title, MessageBoxButtons buttons)
    {
      _Text = text;
      _Title = title;
      _Buttons = buttons;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an attribute instance with the specified message attributes.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="text">Message text</param>
    /// <param name="buttons">Buttons to show on the message box</param>
    /// <param name="icon">Icon to display in the message box.</param>
    // --------------------------------------------------------------------------------------------
    public ShowMessageActionAttribute(string text, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
      _Text = text;
      _Title = title;
      _Buttons = buttons;
      _Icon = icon;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an attribute instance with the specified message attributes.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="text">Message text</param>
    /// <param name="buttons">Buttons to show on the message box</param>
    /// <param name="icon">Icon to display in the message box.</param>
    /// <param name="defaultButton">Default message box button.</param>
    // --------------------------------------------------------------------------------------------
    public ShowMessageActionAttribute(string text, string title, MessageBoxButtons buttons, 
      MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
    {
      _Text = text;
      _Title = title;
      _Buttons = buttons;
      _Icon = icon;
      _DefaultButton = defaultButton;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays the message in a Visual Studio message box.
    /// </summary>
    /// <param name="command">Menu command instance.</param>
    // --------------------------------------------------------------------------------------------
    public override void ExecuteAction(OleMenuCommand command)
    {
      VsMessageBox.Show(_Text, _Title, _Buttons, _Icon, _DefaultButton);
    }
  }

  #endregion
}
