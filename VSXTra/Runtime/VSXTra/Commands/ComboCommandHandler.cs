using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Properties;

namespace VSXtra.Commands
{
  // ====================================================================================
  /// <summary>
  /// This abstract class is intended to be the base class for combo command handlers.
  /// </summary>
  // ====================================================================================
  public abstract class ComboCommandHandler : MenuCommandHandler
  {
    #region Private and protected fields

    private readonly CommandID _GetListCommandId;
    private OleMenuCommand _GetListMenuCommand;
    protected List<string> _ListValues;

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
    public string SelectedValue { get; protected set; }

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
        mcs.AddCommand(_GetListMenuCommand);
        _GetListMenuCommand.ParametersDescription = ParamsAccepted;
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
        var newChoice = eventArgs.InValue == null ? null : eventArgs.InValue.ToString();
        var vOut = eventArgs.OutValue;
        if (vOut != IntPtr.Zero && newChoice != null)
        {
          throw (new ArgumentException(Resources.BothInOutParamsIllegal));
        }
        if (vOut != IntPtr.Zero)
        {
          // --- When vOut is non-NULL, the IDE is requesting the current value for the combo
          Marshal.GetNativeVariantForObject(DisplayedValue, vOut);
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
          EnsureList();
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
    /// Gets the value to be displayed in the combo box
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual string DisplayedValue
    {
      get { return SelectedValue; }
    }

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
    /// Gets the flag indicating if case sensitive compare is used when checking items or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual bool IsCaseSensitive
    {
      get { return false; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the mode used for string comparison.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual StringComparison StringComparison
    {
      get
      {
        return IsCaseSensitive
                 ? StringComparison.CurrentCultureIgnoreCase
                 : StringComparison.CurrentCulture;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method ensures that the list of combo box is filled up.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected void EnsureList()
    {
      if (_ListValues == null)
      {
        _ListValues = new List<string>(GetListValues());
      }
    }

    #endregion
  }

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
        if (String.Compare(item, input, StringComparison) == 0)
        {
          output = item;
          return true;
        }
      }
      return false;
    }
  }

  /// <summary>
  /// This abstract class implements a Command handler for an IndexCombo.
  /// </summary>
  // ====================================================================================
  public abstract class IndexComboCommandHandler : ComboCommandHandler
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to check the validity of list items.
    /// </summary>
    /// <param name="input">Input item to check for validity.</param>
    /// <param name="output">The selected item in form as it in the list of the combo.</param>
    /// <returns>
    /// True, if the item is valid; otherwise, false.
    /// </returns>
    /// <remarks>
    /// The input is valid, if it represents an integer index in the range between 0 and the 
    /// number of items in the combo - 1; or equals one of the items in the list.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected override bool IsInputValid(string input, out string output)
    {
      int index;
      if (Int32.TryParse(input, out index))
      {
        if (index >= 0 && index < _ListValues.Count)
        {
          output = _ListValues[index];
          SelectedIndex = index;
          return true;
        }
      }
      index = 0;
      output = String.Empty;
      foreach (var item in ListValues)
      {
        if (String.Compare(item, input, StringComparison) == 0)
        {
          output = item;
          SelectedIndex = index;
          return true;
        }
        index++;
      }
      return false;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gtes the selected index of the combo.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int SelectedIndex { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves the currently selected value of the combo box.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override string DisplayedValue
    {
      get
      {
        EnsureList();
        return (SelectedIndex >= 0 && SelectedIndex < _ListValues.Count)
                 ? _ListValues[SelectedIndex]
                 : String.Empty;
      }
    }
  }

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
    protected sealed override bool IsInputValid(string input, out string output)
    {
      output = input;
      return true;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// An MRU Combo manages its list by itself. We do not need to create a list of our own.
    /// </summary>
    /// <returns></returns>
    // --------------------------------------------------------------------------------------------
    protected sealed override IEnumerable<string> GetListValues()
    {
      return null;
    }
  }

  /// <summary>
  /// This abstract class implements a Command handler for an DynamicCombo.
  /// </summary>
  // ====================================================================================
  public abstract class DynamicComboCommandHandler : ComboCommandHandler
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks if the selected item is valid or not.
    /// </summary>
    /// <param name="input">Input item to check for validity.</param>
    /// <param name="output">The selected item in form as it in the list of the combo.</param>
    /// <returns>
    /// True, if the item is valid; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected override bool IsInputValid(string input, out string output)
    {
      output = input;
      return true;
    }
  }
}