// ================================================================================================
// HierarchyNodeEventArgs.cs
//
// Created: 2008.09.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// Event argument for hierarchy nodes.
  /// </summary>
  // ================================================================================================
  public class HierarchyNodeEventArgs : EventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the child node.
    /// </summary>
    /// <value>The child node.</value>
    // --------------------------------------------------------------------------------------------
    public IHierarchyBehavior ChildNode { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyNodeEventArgs"/> class.
    /// </summary>
    /// <param name="childNode">The child node.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyNodeEventArgs(IHierarchyBehavior childNode)
    {
      ChildNode = childNode;
    }
  }
}