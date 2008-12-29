// ================================================================================================
// TopLevelNode.cs
//
// Created: 2008.12.10, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.HierarchySort
{
  // ================================================================================================
  /// <summary>
  /// This class represents a simple number node without children.
  /// </summary>
  // ================================================================================================
  [HierarchyBitmap("TopLevelImage")]
  public sealed class TopLevelNode : NumberedNode<FirstLevelNode>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="TopLevelNode"/> class.
    /// </summary>
    /// <param name="manager">The manager responsible for this node.</param>
    // --------------------------------------------------------------------------------------------
    public TopLevelNode(NumberHierarchyManager manager)
      : base(manager)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to create an instance of child node.
    /// </summary>
    /// <returns>Newly created child node</returns>
    // --------------------------------------------------------------------------------------------
    public override FirstLevelNode CreateInstance()
    {
      return new FirstLevelNode(ManagerNode);
    }
  }
}