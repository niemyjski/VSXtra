// ================================================================================================
// ComboboxCommandsPackage.cs
//
// Created: 2008.07.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.ComboboxCommands
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "ComboboxCommands", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [Guid(GuidList.guidComboboxCommandsPkgString)]
  public sealed class ComboboxCommandsPackage : PackageBase
  {
    /// <summary>
    /// Initialization of the package; this method is called right after the package is sited, so this is the place
    /// where you can put all the initilaization code that rely on services provided by VisualStudio.
    /// </summary>
    protected override void Initialize()
    {
      base.Initialize();

      // Add our command handlers for menu (commands must exist in the .vsct file)
      OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
      if (null != mcs)
      {
        // IndexCombo
        //	 An INDEXCOMBO is the same as a DROPDOWNCOMBO in that it is a "pick from list" only combo.
        //	 The difference is an INDEXCOMBO returns the selected value as an index into the list (0 based).
        //	 For example, this type of combo could be used for the "Solution Configurations" on the "Standard" toolbar.
        //
        //   An IndexCombo box requires two commands:
        //     One command is used to ask for the current value of the combo box and to set the new value when the user
        //     makes a choice in the combo box.
        //
        //     The second command is used to retrieve this list of choices for the combo box.
        CommandID menuMyIndexComboCommandID = new CommandID(GuidList.guidComboboxCommandsCmdSet, (int)CmdIDs.cmdidMyIndexCombo);
        OleMenuCommand menuMyIndexComboCommand = new OleMenuCommand(new EventHandler(OnMenuMyIndexCombo), menuMyIndexComboCommandID);
        menuMyIndexComboCommand.ParametersDescription = "$"; // accept any argument string
        mcs.AddCommand(menuMyIndexComboCommand);

        CommandID menuMyIndexComboGetListCommandID = new CommandID(GuidList.guidComboboxCommandsCmdSet, (int)CmdIDs.cmdidMyIndexComboGetList);
        MenuCommand menuMyIndexComboGetListCommand = new OleMenuCommand(new EventHandler(OnMenuMyIndexComboGetList), menuMyIndexComboGetListCommandID);
        mcs.AddCommand(menuMyIndexComboGetListCommand);

        // DynamicCombo
        //   A DYNAMICCOMBO allows the user to type into the edit box or pick from the list. The 
        //	 list of choices is usually fixed and is managed by the command handler for the command.
        //	 For example, this type of combo is used for the "Zoom" combo on the "Class Designer" toolbar.
        //
        //   A Combo box requires two commands:
        //     One command is used to ask for the current value of the combo box and to set the new value when the user
        //     makes a choice in the combo box.
        //
        //     The second command is used to retrieve this list of choices for the combo box.
        CommandID menuMyDynamicComboCommandID = new CommandID(GuidList.guidComboboxCommandsCmdSet, (int)CmdIDs.cmdidMyDynamicCombo);
        OleMenuCommand menuMyDynamicComboCommand = new OleMenuCommand(new EventHandler(OnMenuMyDynamicCombo), menuMyDynamicComboCommandID);
        menuMyDynamicComboCommand.ParametersDescription = "$"; // accept any argument string
        mcs.AddCommand(menuMyDynamicComboCommand);

        CommandID menuMyDynamicComboGetListCommandID = new CommandID(GuidList.guidComboboxCommandsCmdSet, (int)CmdIDs.cmdidMyDynamicComboGetList);
        MenuCommand menuMyDynamicComboGetListCommand = new OleMenuCommand(new EventHandler(OnMenuMyDynamicComboGetList), menuMyDynamicComboGetListCommandID);
        mcs.AddCommand(menuMyDynamicComboGetListCommand);
      }
    }

    private string[] indexComboChoices = { Resources.Lions, Resources.Tigers, Resources.Bears };
    private int currentIndexComboChoice = 0;

    // IndexCombo
    //	 An INDEXCOMBO is the same as a DROPDOWNCOMBO in that it is a "pick from list" only combo.
    //	 The difference is an INDEXCOMBO returns the selected value as an index into the list (0 based).
    //	 For example, this type of combo could be used for the "Solution Configurations" on the "Standard" toolbar.
    //
    //   An IndexCombo box requires two commands:
    //     One command is used to ask for the current value of the combo box and to set the new value when the user
    //     makes a choice in the combo box.
    //
    //     The second command is used to retrieve this list of choices for the combo box.
    private void OnMenuMyIndexCombo(object sender, EventArgs e)
    {
      if ((null == e) || (e == EventArgs.Empty))
      {
        // We should never get here; EventArgs are required.
        throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
      }

      OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

      if (eventArgs != null)
      {
        object input = eventArgs.InValue;
        IntPtr vOut = eventArgs.OutValue;

        if (vOut != IntPtr.Zero && input != null)
        {
          throw (new ArgumentException(Resources.BothInOutParamsIllegal)); // force an exception to be thrown
        }
        if (vOut != IntPtr.Zero)
        {
          // when vOut is non-NULL, the IDE is requesting the current value for the combo
          Marshal.GetNativeVariantForObject(this.indexComboChoices[this.currentIndexComboChoice], vOut);
        }

        else if (input != null)
        {
          int newChoice = -1;
          try
          {
            // user typed a string argument in command window.
            int index = int.Parse(input.ToString(), CultureInfo.CurrentCulture);
            if (index >= 0 && index < indexComboChoices.Length)
            {
              newChoice = index;
            }
            else
            {
              string errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.InvalidIndex, indexComboChoices.Length);
              throw (new ArgumentOutOfRangeException(errorMessage));
            }
          }
          catch (FormatException)
          {
            // user typed in a non-numeric value, see if it is one of our items
            for (int i = 0; i < indexComboChoices.Length; i++)
            {
              if (String.Compare(indexComboChoices[i], input.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
              {
                newChoice = i;
                break;
              }
            }
          }
          catch (OverflowException)
          {
            // user typed in too large of a number, ignore it
          }

          // new value was selected or typed in
          if (newChoice != -1)
          {
            this.currentIndexComboChoice = newChoice;
            ShowMessage(Resources.MyIndexCombo, this.currentIndexComboChoice.ToString(CultureInfo.CurrentCulture));
          }
          else
          {
            throw (new ArgumentException(Resources.ParamMustBeValidIndexOrStringInList)); // force an exception to be thrown
          }
        }
        else
        {
          // We should never get here; EventArgs are required.
          throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
        }
      }
      else
      {
        // We should never get here; EventArgs are required.
        throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
      }
    }

    // An IndexCombo box requires two commands:
    //    This command is used to retrieve this list of choices for the combo box.
    // 
    // Normally IOleCommandTarget::QueryStatus is used to determine the state of a command, e.g.
    // enable vs. disable, shown vs. hidden, etc. The QueryStatus method does not have any way to 
    // control the statue of a combo box, e.g. what list of items should be shown and what is the 
    // current value. In order to communicate this information actually IOleCommandTarget::Exec
    // is used with a non-NULL varOut parameter. You can think of these Exec calls as extended 
    // QueryStatus calls. There are two pieces of information needed for a combo, thus it takes
    // two commands to retrieve this information. The main command id for the command is used to 
    // retrieve the current value and the second command is used to retrieve the full list of 
    // choices to be displayed as an array of strings.
    private void OnMenuMyIndexComboGetList(object sender, EventArgs e)
    {
      if (e == EventArgs.Empty)
      {
        // We should never get here; EventArgs are required.
        throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
      }

      OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

      if (eventArgs != null)
      {
        object inParam = eventArgs.InValue;
        IntPtr vOut = eventArgs.OutValue;

        if (inParam != null)
        {
          throw (new ArgumentException(Resources.InParamIllegal)); // force an exception to be thrown
        }
        else if (vOut != IntPtr.Zero)
        {
          Marshal.GetNativeVariantForObject(this.indexComboChoices, vOut);
        }
        else
        {
          throw (new ArgumentException(Resources.OutParamRequired)); // force an exception to be thrown
        }
      }
    }

    private double[] numericZoomLevels = { 4.0, 3.0, 2.0, 1.5, 1.25, 1.0, .75, .66, .50, .33, .25, .10 };
    private string zoomToFit = Resources.ZoomToFit;
    private string zoom_to_Fit = Resources.Zoom_to_Fit;
    private string[] zoomLevels = null;
    private NumberFormatInfo numberFormatInfo;
    private double currentZoomFactor = 1.0;

    // DynamicCombo
    //   A DYNAMICCOMBO allows the user to type into the edit box or pick from the list. The 
    //	 list of choices is usually fixed and is managed by the command handler for the command.
    //	 For example, this type of combo is used for the "Zoom" combo on the "Class Designer" toolbar.
    //
    //   A Combo box requires two commands:
    //     One command is used to ask for the current value of the combo box and to set the new value when the user
    //     makes a choice in the combo box.
    //
    //     The second command is used to retrieve this list of choices for the combo box.
    private void OnMenuMyDynamicCombo(object sender, EventArgs e)
    {
      if ((null == e) || (e == EventArgs.Empty))
      {
        // We should never get here; EventArgs are required.
        throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
      }

      OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

      if (eventArgs != null)
      {
        object input = eventArgs.InValue;
        IntPtr vOut = eventArgs.OutValue;

        if (vOut != IntPtr.Zero && input != null)
        {
          throw (new ArgumentException(Resources.BothInOutParamsIllegal)); // force an exception to be thrown
        }
        else if (vOut != IntPtr.Zero)
        {
          // when vOut is non-NULL, the IDE is requesting the current value for the combo
          if (this.currentZoomFactor == 0)
          {
            Marshal.GetNativeVariantForObject(this.zoom_to_Fit, vOut);
          }
          else
          {
            string factorString = currentZoomFactor.ToString("P0", this.numberFormatInfo);
            Marshal.GetNativeVariantForObject(factorString, vOut);
          }

        }
        else if (input != null)
        {
          // new zoom value was selected or typed in
          string inputString = input.ToString();

          if (inputString.Equals(this.zoomToFit) || inputString.Equals(this.zoom_to_Fit))
          {
            currentZoomFactor = 0;
            ShowMessage(Resources.MyDynamicCombo, this.zoom_to_Fit);
          }
          else
          {
            // There doesn't appear to be any percent-parsing routines in the framework (even though you can create
            // a localized percentage in a string!).  So, we need to remove any occurence of the localized Percent 
            // symbol, then parse the value that's left
            try
            {
              float newZoom = Single.Parse(inputString.Replace(NumberFormatInfo.InvariantInfo.PercentSymbol, ""), CultureInfo.CurrentCulture);

              newZoom = (float)Math.Round(newZoom);
              if (newZoom < 0)
              {
                throw (new ArgumentException(Resources.ZoomMustBeGTZero)); // force an exception to be thrown
              }

              currentZoomFactor = newZoom / (float)100.0;

              ShowMessage(Resources.MyDynamicCombo, newZoom.ToString(CultureInfo.CurrentCulture));
            }
            catch (FormatException)
            {
              // user typed in a non-numeric value, ignore it
            }
            catch (OverflowException)
            {
              // user typed in too large of a number, ignore it
            }
          }
        }
        else
        {
          // We should never get here
          throw (new ArgumentException(Resources.InOutParamCantBeNULL)); // force an exception to be thrown
        }
      }
      else
      {
        // We should never get here; EventArgs are required.
        throw (new ArgumentException(Resources.EventArgsRequired)); // force an exception to be thrown
      }
    }

    // A Combo box requires two commands:
    //    This command is used to retrieve this list of choices for the combo box.
    // 
    // Normally IOleCommandTarget::QueryStatus is used to determine the state of a command, e.g.
    // enable vs. disable, shown vs. hidden, etc. The QueryStatus method does not have any way to 
    // control the statue of a combo box, e.g. what list of items should be shown and what is the 
    // current value. In order to communicate this information actually IOleCommandTarget::Exec
    // is used with a non-NULL varOut parameter. You can think of these Exec calls as extended 
    // QueryStatus calls. There are two pieces of information needed for a combo, thus it takes
    // two commands to retrieve this information. The main command id for the command is used to 
    // retrieve the current value and the second command is used to retrieve the full list of 
    // choices to be displayed as an array of strings.
    private void OnMenuMyDynamicComboGetList(object sender, EventArgs e)
    {
      if ((null == e) || (e == EventArgs.Empty))
      {
        // We should never get here; EventArgs are required.
        throw (new ArgumentNullException(Resources.EventArgsRequired)); // force an exception to be thrown
      }

      OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

      if (eventArgs != null)
      {
        object inParam = eventArgs.InValue;
        IntPtr vOut = eventArgs.OutValue;

        if (inParam != null)
        {
          throw (new ArgumentException(Resources.InParamIllegal)); // force an exception to be thrown
        }
        else if (vOut != IntPtr.Zero)
        {
          // initialize the zoom value array if needed
          if (zoomLevels == null)
          {
            this.numberFormatInfo = (NumberFormatInfo)CultureInfo.CurrentUICulture.NumberFormat.Clone();
            if (this.numberFormatInfo.PercentPositivePattern == 0)
              this.numberFormatInfo.PercentPositivePattern = 1;
            if (this.numberFormatInfo.PercentNegativePattern == 0)
              this.numberFormatInfo.PercentNegativePattern = 1;

            zoomLevels = new String[numericZoomLevels.Length + 1];
            for (int i = 0; i < numericZoomLevels.Length; i++)
            {
              zoomLevels[i] = numericZoomLevels[i].ToString("P0", this.numberFormatInfo);
            }

            zoomLevels[zoomLevels.Length - 1] = zoom_to_Fit;
          }

          Marshal.GetNativeVariantForObject(zoomLevels, vOut);
        }
        else
        {
          throw (new ArgumentException(Resources.OutParamRequired)); // force an exception to be thrown
        }
      }
    }

    // Helper method to show a message box using the SVsUiShell/IVsUiShell service
    public void ShowMessage(string title, string message)
    {
      IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
      Guid clsid = Guid.Empty;
      int result = VSConstants.S_OK;
      int hr = uiShell.ShowMessageBox(0,
                          ref clsid,
                          title,
                          message,
                          null,
                          0,
                          OLEMSGBUTTON.OLEMSGBUTTON_OK,
                          OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                          OLEMSGICON.OLEMSGICON_INFO,
                          0,        // false = application modal; true would make it system modal
                          out result);
      ErrorHandler.ThrowOnFailure(hr);
    }
  }
}