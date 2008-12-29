// ================================================================================================
// FileHierarchyNode.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.HierarchySort
{
  // ================================================================================================
  /// <summary>
  /// Responsibilities of a numbered node.
  /// </summary>
  // ================================================================================================
  public interface INumberedNode
  {
    int Number { get; }
    bool ShouldCreateChildren { get; }
    void CreateChildren();
  }

  // ================================================================================================
  /// <summary>
  /// Responsibilities of a numbered node with a specific child type.
  /// </summary>
  /// <typeparam name="TChild">The type of the child.</typeparam>
  // ================================================================================================
  public interface INumberedNode<TChild>: INumberedNode
    where TChild: HierarchyNode
  {
    TChild CreateInstance();
  }

  // ================================================================================================
  /// <summary>
  /// This class represents an abstract hierarchy node.
  /// </summary>
  // ================================================================================================
  public abstract class NumberedNode<TChild> : HierarchyNode,
    INumberedNode<TChild>
    where TChild: HierarchyNode
  {
    #region Private fields

    /// <summary>Number used as the caption of the node</summary>
    private readonly int _Number;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="NumberedNode{T}"/> class.
    /// </summary>
    /// <param name="manager">The manager responsible for this node.</param>
    // --------------------------------------------------------------------------------------------
    protected NumberedNode(IHierarchyManager manager)
      : base(manager)
    {
      SortPriority = 0;
      _Number = NumberHierarchyManager.RandomNumber;
      CreateChildrenIfRequired();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates child nodesif this node allows it.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void CreateChildrenIfRequired()
    {
      if (ShouldCreateChildren) CreateChildren();
    }

    #endregion

    #region INumberedNode implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the number represented by this node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int Number
    {
      get { return _Number; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if Child nodes should be created at all.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual bool ShouldCreateChildren
    {
      get { return true; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates the children belonging to this node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void CreateChildren()
    {
      for (int i = 0; i < 10; i++)
      {
        var childNode = CreateInstance();
        if (childNode!= null)
        {
          AddChild(childNode);
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to create an instance of child node.
    /// </summary>
    /// <returns>Newly created child node</returns>
    // --------------------------------------------------------------------------------------------
    public virtual TChild CreateInstance()
    {
      return null;
    }

    #endregion

    #region Overridden members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the caption of the node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string Caption
    {
      get { return _Number.ToString(); }
    }

    #endregion
  }
}