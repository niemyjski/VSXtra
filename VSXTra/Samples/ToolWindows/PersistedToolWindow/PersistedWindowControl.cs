// ================================================================================================
// PersistedWindowControl.cs
//
// Created: 2008.07.07, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using VSXtra;

namespace DeepDiver.PersistedToolWindow
{
  // ================================================================================================
  /// <summary>
  /// PersistedWindowControl is the control that will be hosted in the PersistedWindowPane. It 
  /// consists of a list view that will display the tool windows that have already been 
  /// created.
  /// </summary>
  // ================================================================================================
  public partial class PersistedWindowControl : UserControl
  {
    private List<WindowFrame> _ToolWindowList;
    private bool _IgnoreSelectedObjectsChanges;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This constructor is the default for a user control
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public PersistedWindowControl()
    {
      InitializeComponent();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the selection tracker object.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public SelectionTracker SelectionTracker { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Repopulate the listview with the latest data.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    internal void RefreshData()
    {
      _ToolWindowList = new List<WindowFrame>(WindowFrame.ToolWindowFrames);
      PopulateListView();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Repopulate the listview with the data provided.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void PopulateListView()
    {
      listView1.Items.Clear();
      foreach (var windowFrame in _ToolWindowList)
      {
        listView1.Items.Add(windowFrame.Caption);
      }
      listView1.SelectedItems.Clear();
      listView1_SelectedIndexChanged(this, null);
      listView1.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      listView1.Invalidate();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Push properties for the selected item to the properties window.
    /// 
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Arguments</param>
    /// <remarks>
    /// Throwing an exception from a Windows Forms event handler would cause Visual Studio to 
    /// crash. So if you expect your code to throw you should make sure to catch the exceptions 
    /// you expect.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (_IgnoreSelectedObjectsChanges) return;
      var selectedObjects = new List<SelectionProperties>();
      if (listView1.SelectedItems.Count > 0)
      {
        int index = listView1.SelectedItems[0].Index;
        var frame = _ToolWindowList[index];
        var properties = new SelectionProperties(frame.Caption, frame.Guid) { Index = index };
        selectedObjects.Add(properties);
      }
      SelectionTracker.SelectObjects(selectedObjects, WindowsProperties);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Changes the current selection to the specified object.
    /// </summary>
    /// <param name="selection">Current selection.</param>
    // --------------------------------------------------------------------------------------------
    public void ChangeSelection(SelectionProperties selection)
    {
      _IgnoreSelectedObjectsChanges = true;
      try
      {
        if (selection != null) listView1.Items[selection.Index].Selected = true;
      }
      finally
      {
        _IgnoreSelectedObjectsChanges = false;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the list of available tool windows' properties.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private ArrayList WindowsProperties
    {
      get
      {
        int index = 0;
        var properties = new ArrayList();
        foreach (WindowFrame frame in _ToolWindowList)
        {
          var property = new SelectionProperties(frame.Caption, frame.Guid) { Index = index };
          properties.Add(property);
          ++index;
        }
        return properties;
      }
    }
  }
}
