// ================================================================================================
// CommandsControl.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.07.24, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using VSXtra;
using VSXtra.Commands;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// This control represents the UI of the Commands options page.
  /// </summary>
  // ================================================================================================
  public partial class CommandsControl : UserControl
  {
    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the option page.
    /// </summary>
    /// <value>The option page.</value>
    // --------------------------------------------------------------------------------------------
    public CommandsPage OptionPage { get; set; }

    #endregion

    #region Constructors

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandsControl"/> class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public CommandsControl()
    {
      InitializeComponent();
    }

    #endregion

    #region Event Handlers

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the control page grid according to the commands registered.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void CommandsControl_Load(object sender, EventArgs e)
    {
      var items =
        from command in MenuCommandHandler.GetRegisteredHandlerInstances<VSXtraCommandsPackage>()
        orderby command.GetType().Name
        select
          new CommandRowItem()
            {
              Command = command.CommandId,
              CommandText = GetDisplayName(command.GetType()),
              Enabled = OptionPage.DisabledCommands.SingleOrDefault(
                          cmd => cmd.Guid.Equals(command.Guid) &&
                                 cmd.ID.Equals(command.Id)) == null
            };
      gridVisibility.DataSource = items.ToList();
      gridVisibility.Columns[0].Width = 200;
      gridVisibility.Columns[0].ReadOnly = true;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Responds to the event when a cell value changes, stores the changes
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void gridVisibility_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      // --- Do checks before processing the command
      if (e.ColumnIndex != 1) return;
      if (gridVisibility.CurrentRow == null) return;
      var item = gridVisibility.CurrentRow.DataBoundItem as CommandRowItem;
      if (item == null) return;

      // --- OK, the Enabled flag is about to modify
      OptionPage.DisabledCommands.Remove(item.Command);
      if (!item.Enabled)
      {
        OptionPage.DisabledCommands.Add(item.Command);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Finishes the edit operation.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void gridVisibility_MouseLeave(object sender, EventArgs e)
    {
      gridVisibility.EndEdit();
    }

    #endregion

    #region Private Implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the display name associated with the specified command hanlder type.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static string GetDisplayName(Type command)
    {
      var attr = command.GetAttribute<DisplayNameAttribute>();
      return attr == null ? command.Name : attr.DisplayName;
    }

    #endregion
  }
}