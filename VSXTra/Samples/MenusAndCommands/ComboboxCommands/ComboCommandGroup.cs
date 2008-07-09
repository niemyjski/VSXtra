// ================================================================================================
// ComboCommandGroup.cs
//
// Created: 2008.07.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.ComboboxCommands
{
  // ================================================================================================
  /// <summary>
  /// This type represents a command group owned by the ComboboxCommandsPackage.
  /// </summary>
  /// <remarks>
  /// This type is a logical container, its only role is to group the commands having
  /// the same GUID in their comman ID. It inherits from CommandGroup signing the owner package type.
  /// </remarks>
  // ================================================================================================
  [Guid(GuidList.guidComboboxCommandsCmdSetString)]
  public sealed class ComboCommandGroup : CommandGroup<ComboboxCommandsPackage>
  {
    // ================================================================================================
    /// <summary>
    /// This command handler responds to the DropDownCombo events.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidMyDropDownCombo)]
    [ListCommandId(CmdIDs.cmdidMyDropDownComboGetList)]
    public sealed class DropDownComboCommand : DropDownComboCommandHandler
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// List of available values.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override IEnumerable<string> GetListValues()
      {
        yield return Resources.Apples;
        yield return Resources.Oranges;
        yield return Resources.Pears;
        yield return Resources.Bananas;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Displays a message box with the selected value.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        VsMessageBox.Show(SelectedValue, Resources.MyDropDownCombo);
      }
    }

    // ================================================================================================
    /// <summary>
    /// This command handler responds to the DropDownCombo events.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidMyMRUCombo)]
    public sealed class MruComboCommand : MruComboCommandHandler
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Displays a message box with the selected value.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        VsMessageBox.Show(SelectedValue, Resources.MyMRUCombo);
      }
    }
  }
}