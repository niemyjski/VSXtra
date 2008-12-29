// ================================================================================================
// SecondLevelNode.cs
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
  [HierarchyBitmap("SecondLevelImage")]
  public sealed class SecondLevelNode : NumberedNode<SingleNumberNode>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="SecondLevelNode"/> class.
    /// </summary>
    /// <param name="manager">The manager responsible for this node.</param>
    // --------------------------------------------------------------------------------------------
    public SecondLevelNode(IHierarchyManager manager) : base(manager)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to create an instance of child node.
    /// </summary>
    /// <returns>Newly created child node</returns>
    // --------------------------------------------------------------------------------------------
    public override SingleNumberNode CreateInstance()
    {
      return new SingleNumberNode(ManagerNode);
    }
  }
}