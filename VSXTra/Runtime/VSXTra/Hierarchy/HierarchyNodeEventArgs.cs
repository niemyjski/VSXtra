// ================================================================================================
// HierarchyNodeEventArgs.cs
//
// Created: 2008.09.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using VSXtra.Package;

namespace VSXtra.Hierarchy
{
  // ================================================================================================
  /// <summary>
  /// Event argument for hierarchy nodes.
  /// </summary>
  // ================================================================================================
  public class HierarchyNodeEventArgs<TPackage> : EventArgs
    where TPackage : PackageBase
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the child node.
    /// </summary>
    /// <value>The child node.</value>
    // --------------------------------------------------------------------------------------------
    public HierarchyNode<TPackage> ChildNode { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyNodeEventArgs{TPackage}"/> class.
    /// </summary>
    /// <param name="childNode">The child node.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyNodeEventArgs(HierarchyNode<TPackage> childNode)
    {
      ChildNode = childNode;
    }
  }
}