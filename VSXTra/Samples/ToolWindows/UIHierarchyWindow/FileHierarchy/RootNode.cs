// ================================================================================================
// RootNode.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.UIHierarchyWindow
{
  // ================================================================================================
  /// <summary>
  /// This hierarchy node represents a file hierarchy root.
  /// </summary>
  // ================================================================================================
  [HierarchyBitmap("HomeImage")]
  public class RootNode : FileHierarchyNode
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="RootNode"/> class.
    /// </summary>
    /// <param name="fullPath">The full path of the node.</param>
    /// <param name="caption">The caption to display about the node.</param>
    /// <param name="manager">The manager responsible for this node.</param>
    // --------------------------------------------------------------------------------------------
    public RootNode(FileHierarchyManager manager, string fullPath, string caption) : 
      base(manager, fullPath, caption)
    {
    }
  }
}