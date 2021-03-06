// ================================================================================================
// ComboCommandGroup.cs
//
// Created: 2008.07.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Shell;

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
  public sealed partial class ComboCommandGroup : CommandGroup<ComboboxCommandsPackage>
  {
    // ================================================================================================

    #region Nested type: DropDownComboCommand

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

    #endregion

    // ================================================================================================

    #region Nested type: IndexComboCommand

    /// <summary>
    /// This command handler responds to the IndexCombo events.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidMyIndexCombo)]
    [ListCommandId(CmdIDs.cmdidMyIndexComboGetList)]
    public sealed class IndexComboCommand : IndexComboCommandHandler
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// List of available values.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override IEnumerable<string> GetListValues()
      {
        yield return Resources.Lions;
        yield return Resources.Tigers;
        yield return Resources.Bears;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Displays a message box with the selected value.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        VsMessageBox.Show(SelectedIndex.ToString(), Resources.MyDropDownCombo);
      }
    }

    #endregion

    // ================================================================================================

    #region Nested type: MruComboCommand

    /// <summary>
    /// This command handler responds to the MRUCombo events.
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

    #endregion
  }
}