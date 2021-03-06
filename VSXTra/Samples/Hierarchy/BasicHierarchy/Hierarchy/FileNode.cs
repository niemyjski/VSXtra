// ================================================================================================
// FileNode.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.BasicHierarchy
{
  // ================================================================================================
  /// <summary>
  /// This hierarchy node represents a file.
  /// </summary>
  // ================================================================================================
  [HierarchyBitmap("FileImage")]
  [SortPriority(30)]
  public sealed class FileNode : FileHierarchyNode
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="FileNode"/> class.
    /// </summary>
    /// <param name="fullPath">The full path of the node.</param>
    /// <param name="caption">The caption to display about the node.</param>
    /// <param name="manager">The manager responsible for this node.</param>
    // --------------------------------------------------------------------------------------------
    public FileNode(FileHierarchyManager manager, string fullPath, string caption) : 
      base(manager, fullPath, caption)
    {
    }
  }
}