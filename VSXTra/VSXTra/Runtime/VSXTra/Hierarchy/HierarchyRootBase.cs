// ================================================================================================
// HierarchyRootBase.cs
//
// Created: 2008.09.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Globalization;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to be the base class of all hierarchy nodes that manage their 
  /// subordinated items.
  /// </summary>
  /// <typeparam name="THier">Type of hierarchy element</typeparam>
  /// <typeparam name="TRoot">Type of root element</typeparam>
  /// <remarks>
  /// This node represents a hierarchy element which is under the management of the root hierarchy
  /// element.
  /// </remarks>
  // ================================================================================================
  public abstract class HierarchyRoot<TRoot, THier> : HierarchyBase<THier, TRoot>
    where TRoot : HierarchyRoot<TRoot, THier>
    where THier : HierarchyBase<THier, TRoot>
  {
    #region Private fields

    private readonly ItemMap<HierarchyBase<THier, TRoot>> _ManagedItems =
      new ItemMap<HierarchyBase<THier, TRoot>>();

    #endregion

    #region Lifecycle methods

    protected HierarchyRoot()
    {
      ScanHierarchyAttributes();
    }

    private void ScanHierarchyAttributes()
    {
      GetType().AttributesOfType<HierarchyAttribute>().ForEach(
        attr =>
        {
          DoNotTriggerHierarchyEvents = attr is DoNotTriggerHierarchyEventsAttribute;
          DoNotTriggerTrackerEvents = attr is DoNotTriggerTrackerEventsAttribute;
        }
      );
    }

    #endregion

    #region Public Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the ItemMap for managed items.
    /// </summary>
    /// <value>The ItemMap for managed items.</value>
    // --------------------------------------------------------------------------------------------
    public ItemMap<HierarchyBase<THier, TRoot>> ManagedItems
    {
      get { return _ManagedItems; }
    }

    public bool DoNotTriggerHierarchyEvents { get; private set; }
    public bool DoNotTriggerTrackerEvents { get; private set; }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Compares two hierarchy nodes managed by this node.
    /// </summary>
    /// <param name="node1">The first node.</param>
    /// <param name="node2">The second node.</param>
    /// <returns>
    /// Zero, if the two nodes have the same rank number. Less than zero, if the second node has a
    /// less rank number than the second one. Greater than zero if the second node has a greater
    /// rank number than the first one.
    /// </returns>
    /// <remarks>
    /// Comparison is done by the SortPriority property of the nodes. If SortPriority happens to 
    /// be equal both nodes, then the Caption is used for comparison.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected internal virtual int CompareNodes(HierarchyBase<THier, TRoot> node1, HierarchyBase<THier, 
      TRoot> node2)
    {
      if (node1 == null) throw new ArgumentNullException("node1");
      if (node2 == null) throw new ArgumentNullException("node2");

      if (node1.SortPriority == node2.SortPriority)
      {
        return String.Compare(node2.Caption, node1.Caption, true, CultureInfo.CurrentCulture);
      }
      return node2.SortPriority - node1.SortPriority;
    }

    #endregion
  }
}