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

  // ================================================================================================
  /// <summary>
  /// Cancellable event argument for hierarchy nodes.
  /// </summary>
  // ================================================================================================
  public class HierarchyNodeCancelEventArgs : HierarchyNodeEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this event is cancelled.
    /// </summary>
    /// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
    // --------------------------------------------------------------------------------------------
    public bool Cancel { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyNodeCancelEventArgs"/> class.
    /// </summary>
    /// <param name="childNode">The child node.</param>
    // --------------------------------------------------------------------------------------------
    public HierarchyNodeCancelEventArgs(IHierarchyBehavior childNode)
      : base(childNode)
    {
    }
  }
}