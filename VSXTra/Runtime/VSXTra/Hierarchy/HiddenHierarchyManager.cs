// ================================================================================================
// HiddenHierarchyManager.cs
//
// Created: 2008.12.20, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Package;

namespace VSXtra.Hierarchy
{
  public sealed class HiddenHierarchyManager<TPackage> : HierarchyManager<TPackage>
    where TPackage : PackageBase
  {
    #region Hidden Root Node

    private class HiddenRootNode: HierarchyNode
    {
      public HiddenRootNode(IHierarchyManager manager) : base(manager)
      {
      }
    }

    #endregion

    #region Overrides of HierarchyManager<TPackage>

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates the hidden root node of the hierarchy.
    /// </summary>
    /// <returns>The newly created hierarchy node</returns>
    /// <remarks>
    /// This root node cannot have children
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected override HierarchyNode CreateHierarchyRoot()
    {
      return new HiddenRootNode(this);
    }

    #endregion
  }
}