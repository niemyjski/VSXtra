// ================================================================================================
// HierarchyRootBase.cs
//
// Created: 2008.09.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using Microsoft.VisualStudio;
using VSXtra.Diagnostics;

namespace VSXtra.Hierarchy
{
  public abstract class HierarchyRoot<TRoot>: HierarchyRoot<TRoot, TRoot>
    where TRoot: HierarchyRoot<TRoot, TRoot>
  {
    protected HierarchyRoot()
    {
    }

    protected HierarchyRoot(TRoot root) : base(root)
    {
    }
  }

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

    private ImageHandler _ImageHandler;

    #endregion

    #region Lifecycle methods

    protected HierarchyRoot()
    {
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

    public static TRoot CreateRoot()
    {
      if (typeof(TRoot).IsAbstract)
        throw new InvalidOperationException(
          String.Format("{0} is not a concrete type", typeof(TRoot)));
      var ci = typeof (TRoot).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, 
        null, Type.EmptyTypes, null);
      if (ci == null)
        throw new InvalidOperationException(
          String.Format("{0} does not have a parameterless constructor", typeof(TRoot)));
      var root = ci.Invoke(new object[] {}) as TRoot;
      if (root == null)
        throw new InvalidOperationException(
          String.Format("{0} constructor does not result the expected instance", typeof(TRoot)));
      root.ManagerNode = root;
      root.HierarchyId = HierarchyId.Root;
      root.InitRootNode();
      return root;
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
    /// Gets the handle of the image list used by this manager node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IntPtr ImageListHandle
    {
      get { return ImageHandler.ImageList.Handle; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets an ImageHandler for the root node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected ImageHandler ImageHandler
    {
      get
      {
        if (_ImageHandler == null)
        {
          _ImageHandler = InitImageHandler();
        }
        if (_ImageHandler == null)
        {
          _ImageHandler = GetDefaultImageHandler();
        }
        return _ImageHandler;
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

    #region Abstract and virtual members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to do additional initializatios steps on the root node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected void InitRootNode()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to initialize the ImageList on your own way.
    /// </summary>
    /// <returns>
    /// ImageList object if initialization is succesful; otherwise null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected virtual ImageHandler InitImageHandler()
    {
      if (ImageResourceAssembly != null && !String.IsNullOrEmpty(ImageResourceStreamName))
      {
        try
        {
          return new ImageHandler(
            ImageResourceAssembly.GetManifestResourceStream(ImageResourceStreamName));
        }
        catch (SystemException)
        {
        }
      }
      return null;  
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the assembly holding the ImageList resource information.
    /// </summary>
    /// <remarks>
    /// Override this property if the resource is stored in a different assembly than the type 
    /// representing the root hierarchy node.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual Assembly ImageResourceAssembly
    {
      get { return GetType().Assembly; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the resource name where the ImageList is stored.
    /// </summary>
    /// <value>Resource name for the ImageList or null if no resource is specified.</value>
    /// <remarks>
    /// Override this to specify the resource stream name.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected virtual string ImageResourceStreamName
    {
      get { return null; }
    }

    #endregion

    #region Helper methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a default ImageList for this manager node.
    /// </summary>
    /// <returns>Default ImageList</returns>
    // --------------------------------------------------------------------------------------------
    private static ImageHandler GetDefaultImageHandler()
    {
      const int bitmapSize = 16;
      var pixelBackColor = Color.LightBlue;
      var pixelForeColor = Color.DarkBlue;

      var bitmap = new Bitmap(bitmapSize, bitmapSize);
      for (int i = 0; i < bitmapSize; i++)
        for (int j = 0; j < bitmapSize; j++)
        {
          bitmap.SetPixel(i, j, 
            (i == 0 || i == bitmapSize - 1 || j == 0 || j == bitmapSize - 1 || i == j 
            ? pixelForeColor : pixelBackColor)
            );
        }
      var result = new ImageHandler();
      result.AddImage(bitmap);
      return result;
    }

    #endregion
  }
}