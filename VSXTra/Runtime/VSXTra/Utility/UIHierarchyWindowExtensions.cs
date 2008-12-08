// ================================================================================================
// UIHierarchyWindowExtensions.cs
//
// Created: 2008.11.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class provides useful extension methods to UIHierarchyWindow
  /// </summary>
  // ================================================================================================
  public static class UIHierarchyWindowExtensions
  {
    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Expands the specified hierarchy item.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    /// <param name="recursive">Set true to expand the node recursively</param>
    // --------------------------------------------------------------------------------------------
    public static void ExpandNode(this IVsUIHierarchyWindow window, IVsUIHierarchy node, uint id,
      bool recursive)
    {
      var flags = EXPANDFLAGS.EXPF_ExpandFolder;
      if (recursive) flags |= EXPANDFLAGS.EXPF_ExpandFolderRecursively;
      ExpandItem(window, node, id, flags);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Collapses the specified hierarchy item.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void CollapseNode(this IVsUIHierarchyWindow window, IVsUIHierarchy node, 
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_CollapseFolder);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Expands the parents of the specified node to show the node
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void ExpandParents(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_ExpandParentsToShowItem);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Selects the specified node.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void SelectItem(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_SelectItem);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a new node to the current selection.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void AddSelectNode(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_SelectItem | 
        EXPANDFLAGS.EXPF_AddSelectItem);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Extends the current selection into a range with the specified node.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void ExtendSelectNode(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_SelectItem | 
        EXPANDFLAGS.EXPF_ExtendSelectItem);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Higlights the specified node with bold font.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void BoldNode(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_BoldItem);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes the bold higlights from the specified node with.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void UnBoldNode(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_UnBoldItem);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets Cut highlighting to the specified node.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void CutHighlightNode(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_CutHighlightItem);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds Cut highlighting to the specified node.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void AddCutHighlightNode(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_AddCutHighlightItem);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes Cut highlighting from the specified node.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void UnCutHighlightNode(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_UnCutHighlightItem);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Starts editing of the specified node label.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node.</param>
    /// <param name="id">The id of the node.</param>
    // --------------------------------------------------------------------------------------------
    public static void EditNodeLabel(this IVsUIHierarchyWindow window, IVsUIHierarchy node,
      uint id)
    {
      ExpandItem(window, node, id, EXPANDFLAGS.EXPF_EditItemLabel);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the state or appearance of the hierarchy.
    /// </summary>
    /// <param name="window">The hierarchy window.</param>
    /// <param name="hier">The hierarchy instance.</param>
    /// <param name="id">The id of the hierarchy node.</param>
    /// <param name="statemask">The mask to check the state.</param>
    /// <param name="result">The state flags of the specified item.</param>
    // --------------------------------------------------------------------------------------------
    public static void GetNodeState(this IVsUIHierarchyWindow window,
      IVsUIHierarchy hier, uint id, uint statemask, out uint result)
    {
      window.GetItemState(hier, id, statemask, out result);
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Expands the specified hierarchy item.
    /// </summary>
    /// <param name="window">The hierarchy window hosting the item.</param>
    /// <param name="node">The hierarchy of the node to expand.</param>
    /// <param name="id">The id of the node.</param>
    /// <param name="flags">Flags specifying how to expand the node.</param>
    // --------------------------------------------------------------------------------------------
    private static void ExpandItem(IVsUIHierarchyWindow window, IVsUIHierarchy node, uint id, 
      EXPANDFLAGS flags)
    {
      if (window == null) return;
      if (node == null) throw new ArgumentNullException("node");
      ErrorHandler.ThrowOnFailure(window.ExpandItem(node, id, flags));
    }

    #endregion
  }
}