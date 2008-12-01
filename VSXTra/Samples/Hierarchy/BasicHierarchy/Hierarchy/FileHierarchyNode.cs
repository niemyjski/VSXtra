// ================================================================================================
// FileHierarchyNode.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.BasicHierarchy
{
  // ================================================================================================
  /// <summary>
  /// This class represents an abstract hierarchy node.
  /// </summary>
  // ================================================================================================
  public abstract class FileHierarchyNode : HierarchyNode
  {
    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="FileHierarchyNode"/> class.
    /// </summary>
    /// <param name="manager">The manager responsible for this node.</param>
    /// <param name="caption">The caption.</param>
    // --------------------------------------------------------------------------------------------
    protected FileHierarchyNode(FileHierarchyManager manager, string caption)
      : base(manager, caption)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="FileHierarchyNode"/> class.
    /// </summary>
    /// <param name="manager">The manager responsible for this node.</param>
    /// <param name="fullPath">The full path of the node.</param>
    /// <param name="caption">The caption to display about the node.</param>
    // --------------------------------------------------------------------------------------------
    public FileHierarchyNode(FileHierarchyManager manager, string fullPath, string caption) : 
      base(manager, caption)
    {
      FullPath = fullPath;
    }

    #endregion

    #region Public Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the full path representing the node.
    /// </summary>
    /// <value>The full path.</value>
    // --------------------------------------------------------------------------------------------
    public string FullPath { get; private set; }

    #endregion
  }
}