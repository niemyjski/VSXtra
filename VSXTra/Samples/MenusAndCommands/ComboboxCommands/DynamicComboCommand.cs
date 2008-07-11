// ================================================================================================
// DynamicComboCommand.cs
//
// Created: 2008.07.10, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.ComboboxCommands
{
  // ================================================================================================
  /// <summary>
  /// This type represents a command group owned by the ComboboxCommandsPackage.
  /// </summary>
  // ================================================================================================
  public partial class ComboCommandGroup
  {
    // ================================================================================================
    /// <summary>
    /// This command handler responds to the DropDownCombo events.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidMyDynamicCombo)]
    [ListCommandId(CmdIDs.cmdidMyDynamicComboGetList)]
    public sealed class DynamicComboCommand : DynamicComboCommandHandler
    {
      private readonly double[] _NumericZoomLevels = 
        { 4.0, 3.0, 2.0, 1.5, 1.25, 1.0, .75, .66, .50, .33, .25, .10 };
      private readonly string _ZoomToFit = Resources.ZoomToFit;
      private readonly string _Zoom_to_Fit = Resources.Zoom_to_Fit;
      private readonly NumberFormatInfo _NumberFormatInfo;


      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Sets up the initial values of the combo box
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public DynamicComboCommand()
      {
        CurrentZoomFactor = 1.0;
        SelectedValue = "100 %";
        _NumberFormatInfo = (NumberFormatInfo)CultureInfo.CurrentUICulture.NumberFormat.Clone();
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Displays a message box with the selected value.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        VsMessageBox.Show(CurrentZoomFactor.ToString(), Resources.MyDynamicCombo);
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Gets the list used in the DynamicCombo.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override IEnumerable<string> GetListValues()
      {
        for (int i = 0; i < _NumericZoomLevels.Length; i++)
          yield return _NumericZoomLevels[i].ToString("P0", _NumberFormatInfo);
        yield return _Zoom_to_Fit;
      }

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
        if (input.Equals(_ZoomToFit) || input.Equals(_Zoom_to_Fit))
        {
          CurrentZoomFactor = 0;
          output = _Zoom_to_Fit;
          return true;
        }
        // --- There doesn't appear to be any percent-parsing routines in the framework (even though you can create
        // --- a localized percentage in a string!).  So, we need to remove any occurence of the localized Percent 
        // --- symbol, then parse the value that's left
        output = input;
        CurrentZoomFactor = 0;
        try
        {
          float newZoom = Single.Parse(input.Replace(NumberFormatInfo.InvariantInfo.PercentSymbol, ""), CultureInfo.CurrentCulture);
          newZoom = (float)Math.Round(newZoom);
          if (newZoom < 0)
            throw (new ArgumentException(Resources.ZoomMustBeGTZero));
          CurrentZoomFactor = newZoom / (float)100.0;
          output = CurrentZoomFactor.ToString("P0", _NumberFormatInfo);
        }
        catch (FormatException)
        {
          // --- user typed in a non-numeric value, ignore it
        }
        catch (OverflowException)
        {
          // --- user typed in too large of a number, ignore it
        }
        return true;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Gets the currently selected zoom factor
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public double CurrentZoomFactor { get; private set; }
    }
  }
}