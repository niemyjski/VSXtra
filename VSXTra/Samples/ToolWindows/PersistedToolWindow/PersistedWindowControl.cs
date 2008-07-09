// ================================================================================================
// PersistedWindowControl.cs
//
// Created: 2008.07.07, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
    private ITrackSelection _TrackSelection;
    private readonly SelectionContainer _SelectionContainer = new SelectionContainer();
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
    /// Track selection service for the tool window. This should be set by the tool window pane 
    /// as soon as the tool window is created.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    internal ITrackSelection TrackSelection
    {
      get { return _TrackSelection; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        _TrackSelection = value;
        // --- Inititalize with an empty selection. Failure to do this would result in our 
        // --- later calls to OnSelectChange to be ignored (unless focus is lost and regained).
        _SelectionContainer.SelectableObjects = null;
        _SelectionContainer.SelectedObjects = null;
        _TrackSelection.OnSelectChange(_SelectionContainer);
        _SelectionContainer.SelectedObjectsChanged += SelectedObjectsChanged;
      }
    }

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
      var selectedObjects = new ArrayList();
      if (listView1.SelectedItems.Count > 0)
      {
        int index = listView1.SelectedItems[0].Index;
        var frame = _ToolWindowList[index];
        var properties = new SelectionProperties(frame.Caption, frame.Guid) { Index = index };
        selectedObjects.Add(properties);
      }
      _SelectionContainer.SelectedObjects = selectedObjects;
      _SelectionContainer.SelectableObjects = WindowsProperties;
      TrackSelection.OnSelectChange(_SelectionContainer);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handle change to the current selection is done throught the properties window drop 
    /// down list.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    // --------------------------------------------------------------------------------------------
    private void SelectedObjectsChanged(object sender, EventArgs e)
    {
      _IgnoreSelectedObjectsChanges = true;
      try
      {
        listView1.SelectedItems.Clear();
        if (_SelectionContainer.SelectedObjects.Count > 0)
        {
          IEnumerator enumerator = _SelectionContainer.SelectedObjects.GetEnumerator();
          if (enumerator.MoveNext())
          {
            var newSelection = (SelectionProperties)enumerator.Current;
            int index = newSelection.Index;
            listView1.Items[index].Selected = true;
          }
        }
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
