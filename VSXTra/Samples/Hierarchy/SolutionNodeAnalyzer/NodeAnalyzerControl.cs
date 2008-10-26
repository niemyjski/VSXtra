// ================================================================================================
// NodeAnalyzerControl.cs
//
// Created: 2008.09.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VSXtra;
using VSXtra.Hierarchy;
using VSXtra.Selection;
using VSXtra.Shell;

namespace DeepDiver.SolutionNodeAnalyzer
{
  /// <summary>
  /// Summary description for NodeAnalyzerControl.
  /// </summary>
  public partial class NodeAnalyzerControl : UserControl
  {
    private List<HierarchyItem> _HierarchyNodes;

    public NodeAnalyzerControl()
    {
      InitializeComponent();
    }

    public SelectionTracker SelectionTracker { get; set; }

    public void RefreshList(IEnumerable<HierarchyTraversalInfo> info)
    {
      _HierarchyNodes = new List<HierarchyItem>();
      info.ForEach(item => _HierarchyNodes.Add(item.HierarchyNode));
      NodeListView.Items.Clear();
      foreach (HierarchyTraversalInfo nodeInfo in info)
      {
        HierarchyItem node = nodeInfo.HierarchyNode;
        var listItem = new ListViewItem {Tag = node};
        if (node.Id.IsRoot)
        {
          listItem.ImageKey = "Root";
        }
        else
          listItem.ImageKey = node.FirstChild.IsNil
                                ? "Item"
                                : "Folder";
        listItem.Text = node.IsNestedHierachy
                          ? String.Format("{0} / {1}", node.Id, node.ParentHierarchyItemId)
                          : node.Id.ToString();
        NodeListView.Items.Add(listItem);
        var depthItem = new ListViewItem.ListViewSubItem {Text = nodeInfo.Depth.ToString()};
        listItem.SubItems.Add(depthItem);
        var nameItem = new ListViewItem.ListViewSubItem {Text = node.Name};
        listItem.SubItems.Add(nameItem);
        var parentItem = new ListViewItem.ListViewSubItem {Text = node.ParentId.ToString()};
        listItem.SubItems.Add(parentItem);
        var parentHItem = new ListViewItem.ListViewSubItem
                            {
                              Text = node.ParentHierarchyItemId.ToString()
                            };
        listItem.SubItems.Add(parentHItem);
      }
      if (_HierarchyNodes.Count > 0)
        SelectionTracker.SelectObject(_HierarchyNodes[0], _HierarchyNodes);
    }

    private void NodeListView_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (NodeListView.SelectedItems.Count == 0) return;
      var selection = NodeListView.SelectedItems[0].Tag as HierarchyItem;
      if (selection != null)
      {
        SelectionTracker.SelectObject(selection, _HierarchyNodes);
      }
    }
  }
}