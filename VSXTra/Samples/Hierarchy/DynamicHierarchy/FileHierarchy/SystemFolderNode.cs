// ================================================================================================
// SystemFolderNode.cs
//
// Created: 2008.11.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.DynamicHierarchy
{
  // ================================================================================================
  /// <summary>
  /// This hierarchy node represents a folder.
  /// </summary>
  // ================================================================================================
  [HierarchyBitmap("SystemFolderImage")]
  public sealed class SystemFolderNode : FolderNode
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="SystemFolderNode"/> class.
    /// </summary>
    /// <param name="manager">The manager responsible for this node.</param>
    /// <param name="fullPath">The full path of the node.</param>
    /// <param name="caption">The caption to display about the node.</param>
    // --------------------------------------------------------------------------------------------
    public SystemFolderNode(FileHierarchyManager manager, string fullPath, string caption) : 
      base(manager, fullPath, caption)
    {
    }
  }
}