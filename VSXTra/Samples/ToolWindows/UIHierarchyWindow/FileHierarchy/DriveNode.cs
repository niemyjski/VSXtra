// ================================================================================================
// DriveNode.cs
//
// Created: 2008.12.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.UIHierarchyWindow
{
  // ================================================================================================
  /// <summary>
  /// This node represents a nested hierarchy under a DriveHierarchyRootNode
  /// </summary>
  // ================================================================================================
  [HierarchyBitmap("DriveImage")]
  [UseInnerHierarchyCaption]
  public class DriveNode : HierarchyNode 
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="DriveNode"/> class.
    /// </summary>
    /// <param name="manager">The manager of this node.</param>
    /// <param name="caption">The caption.</param>
    // --------------------------------------------------------------------------------------------
    public DriveNode(IHierarchyManager manager, string caption)
      : base(manager, caption)
    {
    }
  }
}
