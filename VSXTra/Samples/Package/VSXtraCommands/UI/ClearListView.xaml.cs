// ================================================================================================
// ClearListView.xaml.cs
//
// Created: 2008.07.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VSXtra.Package;

namespace DeepDiver.VSXtraCommands
{
  /// <summary>
  /// View for the extract to Clear List command
  /// </summary>
  public partial class ClearListView
  {
    #region Constructors

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="ClearListView"/> class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ClearListView()
    {
      InitializeComponent();
      DataContext = new ClearListModel();
      CommandBindings.Add(new CommandBinding(DoClearAndRestart, OnDoClearAndRestart,
                                             OnCanDoClearAndRestart));
      CommandBindings.Add(new CommandBinding(DoCancel, OnDoCancel));
    }

    #endregion

    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the model.
    /// </summary>
    /// <value>The model.</value>
    // --------------------------------------------------------------------------------------------
    public ClearListModel Model
    {
      get { return (ClearListModel) DataContext; }
    }

    #endregion

    #region Routed commands

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Fired when the Cancel button is clicked
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static readonly RoutedCommand DoCancel =
      new RoutedCommand("DoCancel", typeof (ClearListView));

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Fired when the ClearAndRestart button is clicked
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static readonly RoutedCommand DoClearAndRestart =
      new RoutedCommand("DoClearAndRestart", typeof (ClearListView));

    #endregion

    #region Private Implementation

    private void OnDoClearAndRestart(object sender, ExecutedRoutedEventArgs e)
    {
      Model.SelectedListEntries.Clear();
      Model.SelectedListEntries.AddRange(lstEntries.SelectedItems.Cast<FileEntry>());
      DialogResult = true;
      Close();
    }

    private void OnCanDoClearAndRestart(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = lstEntries.SelectedItems.Count > 0;
    }

    private void OnDoCancel(object sender, ExecutedRoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }

    private void chkSelectAll_Click(object sender, RoutedEventArgs e)
    {
      if ((bool) chkSelectAll.IsChecked)
      {
        lstEntries.SelectAll();
      }
      else
      {
        lstEntries.UnselectAll();
      }
    }

    private void chkEntry_Click(object sender, RoutedEventArgs e)
    {
      bool areAllItemChecked = true;

      foreach (FileEntry dataItem in Model.ListEntries)
      {
        var lbitem = (ListBoxItem) lstEntries.ItemContainerGenerator.ContainerFromItem(dataItem);
        if (lbitem == null) continue;
        var chkEntry = (CheckBox) GetChildHelper(lbitem, "chkEntry");
        if ((bool) chkEntry.IsChecked) continue;
        areAllItemChecked = false;
        break;
      }
      chkSelectAll.IsChecked = areAllItemChecked;
    }

    private object GetChildHelper(ListBoxItem lbitem, string name)
    {
      var border = VisualTreeHelper.GetChild(lbitem, 0) as Border;
      if (border == null) return null;
      var contentPresenter = VisualTreeHelper.GetChild(border, 0) as ContentPresenter;
      return contentPresenter == null
               ? null
               : lstEntries.ItemTemplate.FindName(name, contentPresenter);
    }

    #endregion
  }
}