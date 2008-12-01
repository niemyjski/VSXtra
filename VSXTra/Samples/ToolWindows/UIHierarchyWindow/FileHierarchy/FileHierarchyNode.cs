// ================================================================================================
// FileHierarchyNode.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.UIHierarchyWindow
{
  // ================================================================================================
  /// <summary>
  /// This class represents an abstract hierarchy node.
  /// </summary>
  // ================================================================================================
  public class FileHierarchyNode : HierarchyNode
  {
    #region Private fields

    /// <summary>Object responsible for managing the node</summary>
    private readonly FileHierarchyManager _Manager;

    #endregion

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
      Loaded = false;
      _Manager = manager;
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
      _Manager = manager;
    }

    #endregion

    #region Public Properties

    public bool Loaded { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the full path representing the node.
    /// </summary>
    /// <value>The full path.</value>
    // --------------------------------------------------------------------------------------------
    public string FullPath { get; private set; }

    #endregion

    #region Overridden Properties and methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Responds to the event when the node is about to be expanded.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void OnBeforeExpanded()
    {
      if (!Loaded && FirstChild is NotLoadedNode)
      {
        RemoveChild(FirstChild);
        if (_Manager != null) _Manager.ScanContent(this);
        InvalidateItem();
      }
      Loaded = true;
    }

    #endregion
  }
}