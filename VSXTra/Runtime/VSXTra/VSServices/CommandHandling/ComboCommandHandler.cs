using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Properties;

namespace VSXtra
{
  #region ComboCommandHandler

  // ====================================================================================
  /// <summary>
  /// This abstract class is intended to be the base class for combo command handlers.
  /// </summary>
  // ====================================================================================
  public abstract class ComboCommandHandler : MenuCommandHandler
  {
    #region Private fields

    private readonly CommandID _GetListCommandId;
    private OleMenuCommand _GetListMenuCommand;
    private List<string> _ListValues = new List<string>();

    #endregion
    
    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the combo command handler.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected ComboCommandHandler()
    {
      // --- Obtain command GUID
      var commandGuid = Attribute.IsDefined(GetType(), typeof(GuidAttribute))
        ? GetType().GUID
        : CommandId.Guid;

      // --- Check for attributes
      foreach (object attr in GetType().GetCustomAttributes(false))
      {
        var idAttr = attr as ListCommandIdAttribute;
        if (idAttr != null && attr.GetType() == typeof (ListCommandIdAttribute))
        {
          if (idAttr.Guid != Guid.Empty) commandGuid = idAttr.Guid;
          _GetListCommandId = new CommandID(commandGuid, (int) idAttr.Id);
        }
      }
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the CommandID related to the command responsible for obtaining the combo list value.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public CommandID GetListCommandId
    {
      get { return _GetListCommandId; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the menu command related to getting the list.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public OleMenuCommand GetListMenuCommand
    {
      get { return _GetListMenuCommand; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the currently selected text.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string SelectedValue { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value of the currently selected text.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IEnumerable<string> ListValues
    {
      get { return new ReadOnlyCollection<string>(_ListValues); }
    }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Overrides the binding method to add the GetList command to the handler.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void Bind()
    {
      // --- Bind standard commands
      base.Bind();
      if (Package == null) return;
      var mcs = ServiceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
      if (mcs == null) return;

      // --- Bind the GetList command
      if (_GetListCommandId != null)
      {
        _GetListMenuCommand = new OleMenuCommand(GetListCommandCallback, _GetListCommandId);
        _GetListMenuCommand.ParametersDescription = ParamsAccepted;
        mcs.AddCommand(_GetListMenuCommand);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Overrides how the menu command should be executed.
    /// </summary>
    /// <param name="sender">Object originating the event.</param>
    /// <param name="e">Event arguments.</param>
    // --------------------------------------------------------------------------------------------
    protected sealed override void ExecuteMenuCommandCallback(object sender, EventArgs e)
    {
      if (e == EventArgs.Empty)
      {
        throw (new ArgumentException(Resources.EventArgsRequired));
      }
      var eventArgs = e as OleMenuCmdEventArgs;
      if (eventArgs != null)
      {
        var newChoice = eventArgs.InValue as string;
        var vOut = eventArgs.OutValue;
        if (vOut != IntPtr.Zero && newChoice != null)
        {
          throw (new ArgumentException(Resources.BothInOutParamsIllegal));
        }
        if (vOut != IntPtr.Zero)
        {
          // --- When vOut is non-NULL, the IDE is requesting the current value for the combo
          Marshal.GetNativeVariantForObject(SelectedValue, vOut);
        }

        else if (newChoice != null)
        {
          string selection;
          if (IsInputValid(newChoice, out selection))
          {
            SelectedValue = selection;
            OnExecute(MenuCommand);
          }
          else
          {
            throw (new ArgumentException(Resources.ParamNotValidStringInList));
          }
        }
        else
        {
          throw (new ArgumentException(Resources.InOutParamCantBeNULL));
        }
      }
      else
      {
        throw (new ArgumentException(Resources.EventArgsRequired));
      }
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Callback method called when the command is to be executed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void GetListCommandCallback(object sender, EventArgs e)
    {
      if ((null == e) || (e == EventArgs.Empty))
      {
        throw (new ArgumentNullException(Resources.EventArgsRequired));
      }
      var eventArgs = e as OleMenuCmdEventArgs;
      if (eventArgs != null)
      {
        object inParam = eventArgs.InValue;
        IntPtr vOut = eventArgs.OutValue;
        if (inParam != null)
        {
          throw (new ArgumentException(Resources.InParamIllegal)); // force an exception to be thrown
        }
        if (vOut != IntPtr.Zero)
        {
          _ListValues = new List<string>(GetListValues());
          var stringArray = new string[_ListValues.Count];
          _ListValues.CopyTo(stringArray);
          Marshal.GetNativeVariantForObject(stringArray, vOut);
        }
        else
        {
          throw (new ArgumentException(Resources.OutParamRequired)); // force an exception to be thrown
        }
      }
    }

    #endregion

    #region Virtual methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to provide a list of strings to be shown in the combo box.
    /// </summary>
    /// <returns>Collection of strings to be shown in the combo box.</returns>
    // --------------------------------------------------------------------------------------------
    protected abstract IEnumerable<string> GetListValues();

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to check the validity of list items.
    /// </summary>
    /// <param name="input">Input item to check for validity.</param>
    /// <param name="output">The selected item in form as it in the list of the combo.</param>
    /// <returns>
    /// True, if the item is valid; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected abstract bool IsInputValid(string input, out string output);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the strings accepted as input parameter.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual string ParamsAccepted
    {
      get { return "$"; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method if you want to parse (and store) the selected value in a non-string 
    /// form.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void ParseSelectedValue()
    {
    }

    #endregion
  }

  #endregion

  #region DropDownComboCommandHandler

  // ====================================================================================
  /// <summary>
  /// This abstract class implements a Command handler for a DropDownCombo.
  /// </summary>
  // ====================================================================================
  public abstract class DropDownComboCommandHandler : ComboCommandHandler
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method checks if the specified item is in the current combo item list or not.
    /// </summary>
    /// <param name="input">Input item to check for validity.</param>
    /// <param name="output">The selected item in form as it in the list of the combo.</param>
    /// <returns>
    /// True, if the item is on the list; otherwise false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected override bool IsInputValid(string input, out string output)
    {
      output = String.Empty;
      foreach (string item in ListValues)
      {
        if (String.Compare(item, input, StringComparison.CurrentCultureIgnoreCase) == 0)
        {
          output = item;
          return true;
        }
      }
      return false;
    }
  }

  #endregion

  #region MruComboCommandHandler

  // ====================================================================================
  /// <summary>
  /// This abstract class implements a Command handler for an MRUCombo.
  /// </summary>
  // ====================================================================================
  public abstract class MruComboCommandHandler : ComboCommandHandler
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method checks if the specified item is in the current combo item list or not.
    /// </summary>
    /// <param name="input">Input item to check for validity.</param>
    /// <param name="output">The selected item in form as it in the list of the combo.</param>
    /// <returns>
    /// This implementation always returns true.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected override bool IsInputValid(string input, out string output)
    {
      output = input;
      return true;
    }

    protected override IEnumerable<string> GetListValues()
    {
      return new List<string>();
    }
  }

  #endregion
}