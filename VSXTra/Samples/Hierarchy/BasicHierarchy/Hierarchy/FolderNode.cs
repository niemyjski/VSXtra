// ================================================================================================
// FolderNode.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.BasicHierarchy
{
  // ================================================================================================
  /// <summary>
  /// This hierarchy node represents a folder.
  /// </summary>
  // ================================================================================================
  [HierarchyBitmap("FolderImage")]
  [SortPriority(20)]
  public sealed class FolderNode : FileHierarchyNode
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="FolderNode"/> class.
    /// </summary>
    /// <param name="manager">The manager responsible for this node.</param>
    /// <param name="fullPath">The full path of the node.</param>
    /// <param name="caption">The caption to display about the node.</param>
    // --------------------------------------------------------------------------------------------
    public FolderNode(FileHierarchyManager manager, string fullPath, string caption) : 
      base(manager, fullPath, caption)
    {
    }
  }
}