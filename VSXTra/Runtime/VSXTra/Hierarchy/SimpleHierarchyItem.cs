namespace VSXtra.Hierarchy
{
  public class SimpleHierarchyItem: HierarchyBase<SimpleHierarchyItem, SimpleHierarchyRoot>
  {
    #region Overrides of HierarchyBase<SimpleHierarchyItem,SimpleHierarchyRoot>

    /// <summary>
    /// Gets the caption of the node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string Caption
    {
      get { throw new System.NotImplementedException(); }
    }

    #endregion
  }

  public class SimpleHierarchyRoot: HierarchyRoot<SimpleHierarchyRoot, SimpleHierarchyItem>
  {
    #region Overrides of HierarchyBase<SimpleHierarchyItem,SimpleHierarchyRoot>

    /// <summary>
    /// Gets the caption of the node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string Caption
    {
      get { throw new System.NotImplementedException(); }
    }

    #endregion
  }

  public class SimpleHierarchy: HierarchyRoot<SimpleHierarchy, SimpleHierarchy>
  {
    #region Overrides of HierarchyBase<SimpleHierarchy,SimpleHierarchy>

    /// <summary>
    /// Gets the caption of the node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string Caption
    {
      get { throw new System.NotImplementedException(); }
    }

    #endregion
  }
}