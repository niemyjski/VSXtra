// ================================================================================================
// CommonActions.cs
//
// Created: 2008.07.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Windows.Forms;

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
    /// <param name="command">Menu command handler instance.</param>
    // --------------------------------------------------------------------------------------------
    public override void ExecuteAction(MenuCommandHandler command)
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
    private readonly MessageBoxButtons _Buttons = MessageBoxButtons.OK;
    private readonly MessageBoxDefaultButton _DefaultButton = MessageBoxDefaultButton.Button1;
    private readonly MessageBoxIcon _Icon = MessageBoxIcon.Information;
    private readonly string _Text;
    private readonly string _Title = String.Empty;

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
    public override void ExecuteAction(MenuCommandHandler command)
    {
      VsMessageBox.Show(_Text, _Title, _Buttons, _Icon, _DefaultButton);
    }
  }

  #endregion

  #region ShowToolWindowAction

  // ================================================================================================
  /// <summary>
  /// This class implements a menu action that puts a message to the output window.
  /// </summary>
  // ================================================================================================
  public sealed class ShowToolWindowActionAttribute : ActionAttribute
  {
    private readonly int _InstanceId;
    private readonly Type _Type;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Cretaes an action to show up a singleton instance of a tool window with the specified type.
    /// </summary>
    /// <param name="type">Typeof the tool window.</param>
    // --------------------------------------------------------------------------------------------
    public ShowToolWindowActionAttribute(Type type)
    {
      _Type = type;
      _InstanceId = 0;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an action to show up the specified instance of a window with the specified type.
    /// </summary>
    /// <param name="type">Typeof the tool window.</param>
    /// <param name="instanceId">Tool window instance ID.</param>
    // --------------------------------------------------------------------------------------------
    public ShowToolWindowActionAttribute(Type type, int instanceId)
    {
      _Type = type;
      _InstanceId = instanceId;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Puts the message to the current pane of the output window.
    /// </summary>
    /// <param name="command">Menu command handler instance.</param>
    // --------------------------------------------------------------------------------------------
    public override void ExecuteAction(MenuCommandHandler command)
    {
      command.Package.ShowToolWindow(_Type, _InstanceId);
    }
  }

  #endregion

  #region ExecuteCommandAction

  // ================================================================================================
  /// <summary>
  /// This class implements a menu action that puts a message to the output window.
  /// </summary>
  // ================================================================================================
  public sealed class ExecuteCommandActionAttribute : ActionAttribute
  {
    private readonly string _CommandName;
    private readonly string _Args;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an action with the specified command name and arguments.
    /// </summary>
    /// <param name="commandName">Name of command to execute.</param>
    /// <param name="args">Optional command arguments</param>
    // --------------------------------------------------------------------------------------------
    public ExecuteCommandActionAttribute(string commandName, string args)
    {
      _CommandName = commandName;
      _Args = args;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an action with the specified command name and empty arguments.
    /// </summary>
    /// <param name="commandName">Name of command to execute.</param>
    // --------------------------------------------------------------------------------------------
    public ExecuteCommandActionAttribute(string commandName)
    {
      _CommandName = commandName;
      _Args = string.Empty;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Puts the message to the current pane of the output window.
    /// </summary>
    /// <param name="command">Menu command handler instance.</param>
    // --------------------------------------------------------------------------------------------
    public override void ExecuteAction(MenuCommandHandler command)
    {
      VsIde.ExecuteCommand(_CommandName, _Args);
    }
  }

  #endregion
}