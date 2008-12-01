// ================================================================================================
// NotLoadedNode.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.UIHierarchyWindow
{
  // ================================================================================================
  /// <summary>
  /// This node represents that its parent's content has not been loaded.
  /// </summary>
  // ================================================================================================
  [HierarchyBitmap("NotLoadedImage")]
  [SortPriority(10)]
  public class NotLoadedNode : FileHierarchyNode
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="NotLoadedNode"/> class.
    /// </summary>
    /// <param name="manager">The manager responsible for this node.</param>
    // --------------------------------------------------------------------------------------------
    public NotLoadedNode(FileHierarchyManager manager)
      : base(manager, "The content of the node has not been loaded yet")
    {
    }
  }
}