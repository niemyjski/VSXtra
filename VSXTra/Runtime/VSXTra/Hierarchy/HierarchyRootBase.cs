// ================================================================================================
// HierarchyRootBase.cs
//
// Created: 2008.09.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  public class HierarchyRoot<TRoot, THier> : HierarchyBase<THier, TRoot>
    where TRoot : HierarchyRoot<TRoot, THier>
    where THier : HierarchyBase<THier, TRoot>
  {
    #region Private fields

    private readonly ItemMap<HierarchyBase<THier, TRoot>> _ManagedItems =
      new ItemMap<HierarchyBase<THier, TRoot>>();

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the item event sinks.
    /// </summary>
    /// <value>The item event sinks.</value>
    public ItemMap<HierarchyBase<THier, TRoot>> ManagedItems
    {
      get { return _ManagedItems; }
    }

    #endregion
  }
}