// ================================================================================================
// NodeAnalyzerControl.cs
//
// Created: 2008.09.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VSXtra;

namespace DeepDiver.SolutionNodeAnalyzer
{
  /// <summary>
  /// Summary description for NodeAnalyzerControl.
  /// </summary>
  public partial class NodeAnalyzerControl : UserControl
  {
    public NodeAnalyzerControl()
    {
      InitializeComponent();
    }

    public void RefreshList(IEnumerable<HierarchyTraversalInfo> info)
    {
      NodeListView.Items.Clear();
      foreach (var nodeInfo in info)
      {
        var listItem = new ListViewItem();
        if (nodeInfo.HierarchyNode.IsNestedHierachy)
        {
          listItem.Text = String.Format("{0} / {1}", 
            HierarchyItemIdTypeConverter.AsString(nodeInfo.HierarchyNode.ItemId),
            HierarchyItemIdTypeConverter.AsString(nodeInfo.HierarchyNode.IdInOwner));
        }
        else
        {
          listItem.Text = HierarchyItemIdTypeConverter.AsString(nodeInfo.HierarchyNode.ItemId);
        }
        NodeListView.Items.Add(listItem);
        var depthItem = new ListViewItem.ListViewSubItem {Text = nodeInfo.Depth.ToString()};
        listItem.SubItems.Add(depthItem);
        var nameItem = new ListViewItem.ListViewSubItem {Text = nodeInfo.HierarchyNode.Name};
        listItem.SubItems.Add(nameItem);
        var parentItem = new ListViewItem.ListViewSubItem
                           {
                             Text = HierarchyItemIdTypeConverter.AsString(nodeInfo.HierarchyNode.ParentId)
                           };
        listItem.SubItems.Add(parentItem);
        var parentHier = new ListViewItem.ListViewSubItem();
        if (nodeInfo.HierarchyNode.ParentHierarchy != null)
        {
          parentHier.Text = string.Format("{0}:{1}",
                                          nodeInfo.HierarchyNode.ParentHierarchy.Name,
                                          nodeInfo.HierarchyNode.ParentHierarchyItemId);
        }
        listItem.SubItems.Add(parentHier);
      }
    }
  }
}
