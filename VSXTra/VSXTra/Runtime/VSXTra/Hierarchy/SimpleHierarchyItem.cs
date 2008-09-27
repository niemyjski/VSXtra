namespace VSXtra
{
  public class SimpleHierarchyItem: HierarchyBase<SimpleHierarchyItem, SimpleHierarchyRoot>
  {
    
  }

  public class SimpleHierarchyRoot: HierarchyRoot<SimpleHierarchyRoot, SimpleHierarchyItem>
  {
    
  }

  public class SimpleHierarchy: HierarchyRoot<SimpleHierarchy, SimpleHierarchy>
  {
    
  }
}