// ================================================================================================
// NodeAnalyzerControl.cs
//
// Created: 2008.09.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using VSXtra;

namespace DeepDiver.SolutionNodeAnalyzer
{
  /// <summary>
  /// Summary description for NodeAnalyzerControl.
  /// </summary>
  public partial class NodeAnalyzerControl : UserControl
  {
    private IEnumerable<HierarchyTraversalInfo> _HierarchyNodes;

    public SelectionTracker SelectionTracker { get; set; }

    public NodeAnalyzerControl()
    {
      InitializeComponent();
    }

    public void RefreshList(IEnumerable<HierarchyTraversalInfo> info)
    {
      _HierarchyNodes = info;
      NodeListView.Items.Clear();
      foreach (var nodeInfo in info)
      {
        var node = nodeInfo.HierarchyNode;
        var listItem = new ListViewItem {Tag = node};
        if (node.ItemId == VSConstants.VSITEMID_ROOT)
        {
          listItem.ImageKey = "Root";
        }
        else listItem.ImageKey = (uint)node.FirstChild != VSConstants.VSITEMID_NIL 
          ? "Folder" 
          : "Item";
        listItem.Text = node.IsNestedHierachy 
          ? String.Format("{0} / {1}", 
            HierarchyItemIdTypeConverter.AsString(node.ItemId),
            HierarchyItemIdTypeConverter.AsString(node.ParentHierarchyItemId)) 
          : HierarchyItemIdTypeConverter.AsString(node.ItemId);
        NodeListView.Items.Add(listItem);
        var depthItem = new ListViewItem.ListViewSubItem {Text = nodeInfo.Depth.ToString()};
        listItem.SubItems.Add(depthItem);
        var nameItem = new ListViewItem.ListViewSubItem {Text = node.Name};
        listItem.SubItems.Add(nameItem);
        var parentItem = new ListViewItem.ListViewSubItem
                           {
                             Text = HierarchyItemIdTypeConverter.AsString(node.ParentId)
                           };
        listItem.SubItems.Add(parentItem);
        var fiChildItem = new ListViewItem.ListViewSubItem
        {
          Text = HierarchyItemIdTypeConverter.AsString(node.FirstChild)
        };
        listItem.SubItems.Add(fiChildItem);
      }
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
