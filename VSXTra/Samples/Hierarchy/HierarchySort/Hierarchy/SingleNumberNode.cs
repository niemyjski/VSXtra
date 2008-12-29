// ================================================================================================
// SingleNumberNode.cs
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
  [HierarchyBitmap("NumberImage")]
  public sealed class SingleNumberNode : NumberedNode<SingleNumberNode>
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleNumberNode"/> class.
    /// </summary>
    /// <param name="manager">The manager responsible for this node.</param>
    // --------------------------------------------------------------------------------------------
    public SingleNumberNode(IHierarchyManager manager) : base(manager)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if Child nodes should be created at all.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override bool ShouldCreateChildren
    {
      get { return false; }
    }
  }
}
