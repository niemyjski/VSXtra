// ================================================================================================
// RdtEventControl.cs
//
// Created: 2008.08.03, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Drawing;
using System.Windows.Forms;
using VSXtra.Documents;
using VSXtra.Selection;

namespace DeepDiver.RdtEventsWindow
{
  // ================================================================================================
  /// <summary>
  /// This user control implements the user interface for the RDT Event Explorer tool window.
  /// </summary>
  /// <remarks>
  /// Displays a log of events in a data grid.When an event is selected, its details appear in the 
  /// Properties window.
  /// </remarks>
  // ================================================================================================
  public partial class RdtEventControl : UserControl
  {
    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates and initializes the RDT Event Explorer UI.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public RdtEventControl()
    {
      InitializeComponent();

      // --- Prepare the event grid.
      eventGrid.AutoGenerateColumns = false;
      eventGrid.AllowUserToAddRows = false;
      eventGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

      eventGrid.Columns.Add("Event", Resources.EventHeader);
      eventGrid.Columns.Add("Moniker", Resources.MonikerHeader);
      eventGrid.Columns["Event"].ReadOnly = true;
      eventGrid.Columns["Moniker"].ReadOnly = true;
      eventGrid.AllowUserToResizeRows = false;
      eventGrid.AllowUserToResizeColumns = true;
      eventGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

      int x = Screen.PrimaryScreen.Bounds.Size.Width;
      int y = Screen.PrimaryScreen.Bounds.Size.Height;
      Size = new Size(x/3, y/3);
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the options page describing RDT event Explorer options
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public SelectionTracker SelectionTracker { get; set; }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary> 
    /// Let this control process the mnemonics.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override bool ProcessDialogChar(char charCode)
    {
      // --- If we're the top-level form or control, we need to do the mnemonic handling
      if (charCode != ' ' && ProcessMnemonic(charCode))
      {
        return true;
      }
      return base.ProcessDialogChar(charCode);
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds an RDT event wrapper to the grid.
    /// </summary>
    /// <param name="ev">Event to add to the grid.</param>
    // --------------------------------------------------------------------------------------------
    public void AddEventToGrid(RdtEventArgs ev)
    {
      if (ev == null) return;
      int n = eventGrid.Rows.Add();
      DataGridViewRow row = eventGrid.Rows[n];
      row.Cells["Event"].Value = ev.EventType.ToString();
      row.Cells["Moniker"].Value = ev.Moniker;
      row.Tag = ev;
    }

    #endregion

    #region Control events

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Clear event lines from grid and refresh display to show empty grid.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void ClearGrid()
    {
      eventGrid.Rows.Clear();
      eventGrid.Refresh();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Track the event associated with selected grid row in the Properties window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void eventGrid_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      // --- Ignore click on header row.
      if (e.RowIndex < 0) return;

      // --- Find the selected row.
      DataGridViewRow row = eventGrid.Rows[e.RowIndex];
      // --- Recover the associated event object.
      var ev = (RdtEventArgs) row.Tag;

      // --- Create an array of one event object and track it in the Properties window.
      SelectionTracker.SelectObject(ev);
    }

    #endregion
  }
}