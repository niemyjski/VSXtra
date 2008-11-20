// ================================================================================================
// HierarchyRootBase.cs
//
// Created: 2008.09.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Globalization;
using Microsoft.VisualStudio;
using VSXtra.Diagnostics;

namespace VSXtra.Hierarchy
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to be the root class of all IVsHierarchy implementation representing 
  /// root nodes in a hierarchy. Root nodes have extended responsibilities compared to other 
  /// hierarchy nodes.
  /// </summary>
  /// <typeparam name="TRoot">Type of root node.</typeparam>
  /// <typeparam name="THier">Type of hierarchy nodes managed by this root node.</typeparam>
  // ================================================================================================
  public abstract class HierarchyRoot<TRoot, THier> : HierarchyBase<THier, TRoot>
    where TRoot : HierarchyRoot<TRoot, THier>
    where THier : HierarchyBase<THier, TRoot>
  {
    #region Private fields

    private readonly ItemMap<HierarchyBase<THier, TRoot>> _ManagedItems =
      new ItemMap<HierarchyBase<THier, TRoot>>();

    /// <summary>A service provider call back object provided by the IDE hosting the project manager</summary>
    private IServiceProvider _Site;

    private bool _DoNotTriggerHierarchyEvents = true;
    private bool _DoNotTriggerTrackerEvents = true;

    private ImageHandler imageHandler;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyRoot&lt;TRoot, THier&gt;"/> class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected HierarchyRoot()
    {
      ManagerNode = this;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyRoot&lt;TRoot, THier&gt;"/> class.
    /// </summary>
    /// <param name="root">The root.</param>
    // --------------------------------------------------------------------------------------------
    protected HierarchyRoot(TRoot root)
      : base(root)
    {
    }

    #endregion

    #region Public Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the item event sinks.
    /// </summary>
    /// <value>The item event sinks.</value>
    // --------------------------------------------------------------------------------------------
    public ItemMap<HierarchyBase<THier, TRoot>> ManagedItems
    {
      get { return _ManagedItems; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the site where this hierarchy node is sited.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IServiceProvider Site
    {
      get { return _Site; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if hierachy events should be triggered or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool DoNotTriggerHierarchyEvents
    {
      get { return _DoNotTriggerHierarchyEvents; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if tracker events should be triggered or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool DoNotTriggerTrackerEvents
    {
      get { return _DoNotTriggerTrackerEvents; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets an ImageHandler for the root node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ImageHandler ImageHandler
    {
      get
      {
        if (null == imageHandler)
        {
          imageHandler = new ImageHandler(typeof(ImageHandler).Assembly.GetManifestResourceStream("VSXtra.Hierarchy.imagelis.bmp"));
        }
        return imageHandler;
      }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the node specified by itemId
    /// </summary>
    /// <param name="itemId">Item of the node managed by this root node.</param>
    /// <returns>
    /// Node specified by itemId
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public HierarchyBase<THier, TRoot> NodeById(uint itemId)
    {
      if (VSConstants.VSITEMID_ROOT == itemId)
      {
        return this;
      }
      if (VSConstants.VSITEMID_NIL == itemId)
      {
        return null;
      }
      return _ManagedItems[itemId];
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the node specified by itemId
    /// </summary>
    /// <param name="id">Item of the node managed by this root node.</param>
    /// <returns>
    /// Node specified by itemId
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public HierarchyBase<THier, TRoot> NodeById(HierarchyId id)
    {
      return NodeById((uint) id);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a subordiante node to this root node.
    /// </summary>
    /// <param name="node">Node to add as a subordinate</param>
    // --------------------------------------------------------------------------------------------
    internal uint AddSubordinate(HierarchyBase<THier, TRoot> node)
    {
      return _ManagedItems.Add(node);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Compares the sort order of the specified nodes.
    /// </summary>
    /// <param name="node1">First node</param>
    /// <param name="node2">Second node</param>
    // --------------------------------------------------------------------------------------------
    public virtual int CompareNodes(HierarchyBase<THier, TRoot> node1, 
      HierarchyBase<THier, TRoot> node2)
    {
      VsDebug.Assert(node1 != null && node2 != null);

      return node1.SortPriority == node2.SortPriority
               ? String.Compare(node2.Caption, node1.Caption, true, CultureInfo.CurrentCulture)
               : node2.SortPriority - node1.SortPriority;
    }

    #endregion
  }
}