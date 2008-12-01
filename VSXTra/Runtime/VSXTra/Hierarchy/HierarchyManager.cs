// ================================================================================================
// HierarchyRootBase.cs
//
// Created: 2008.09.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Commands;
using VSXtra.Package;
using VSXtra.Properties;
using VSXtra.Shell;
using VSXtra.Windows;
using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

namespace VSXtra.Hierarchy
{
  // ================================================================================================
  /// <summary>
  /// This interface defines the responsibilities of a hierarchy manager.
  /// </summary>
  // ================================================================================================
  public interface IHierarchyManager :
    // --- The manager represents a hierarchy, so implements IVsUIHierarchy
    IVsUIHierarchy
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the root node of the hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    HierarchyNode HierarchyRoot { get; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the handle of the image list used by this manager node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    IntPtr ImageListHandle { get; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raises the hierarchy events according to the specified action.
    /// </summary>
    /// <param name="sinkAction">Action to execute</param>
    // --------------------------------------------------------------------------------------------
    void RaiseHierarchyEvent(Func<IVsHierarchyEvents, int> sinkAction);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the node specified by itemId
    /// </summary>
    /// <param name="id">Item of the node managed by this root node.</param>
    /// <returns>
    /// Node specified by itemId
    /// </returns>
    // --------------------------------------------------------------------------------------------
    HierarchyNode this[HierarchyId id] { get; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the number of items handled by this manager
    /// </summary>
    // --------------------------------------------------------------------------------------------
    int ItemCount { get; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a subordinate node to this hierarchy.
    /// </summary>
    /// <param name="node">Node to add as a subordinate</param>
    // --------------------------------------------------------------------------------------------
    uint AddSubordinate(HierarchyNode node);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the node at the specified ID.
    /// </summary>
    /// <param name="id">ID to set the node at</param>
    /// <param name="node">Node to set</param>
    // --------------------------------------------------------------------------------------------
    void SetNodeAtId(HierarchyId id, HierarchyNode node);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Compares the sort order of the specified nodes.
    /// </summary>
    /// <param name="node1">First node</param>
    /// <param name="node2">Second node</param>
    // --------------------------------------------------------------------------------------------
    int CompareNodes(HierarchyNode node1, HierarchyNode node2);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes the specified node from the hierarchy.
    /// </summary>
    /// <param name="node">Node to remove</param>
    // --------------------------------------------------------------------------------------------
    void RemoveNode(HierarchyNode node);

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy of this hierarchy
    /// </summary>
    // --------------------------------------------------------------------------------------------
    IHierarchyManager ParentHierarchy { get; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy of this hierarchy
    /// </summary>
    // --------------------------------------------------------------------------------------------
    HierarchyId IdInParentHierarchy { get; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy of this hierarchy
    /// </summary>
    /// <param name="parent">The parent hierarchy.</param>
    /// <param name="id">The id of the item in the parent hierarchy.</param>
    // --------------------------------------------------------------------------------------------
    void SetParentHierarchy(IHierarchyManager parent, HierarchyId id);
  }

  // ================================================================================================
  /// <summary>
  /// This class is responsible to organize and manage a set of hierarchy items to form an 
  /// IVsUIHierarchy. Hierarchy management is the responsibility of this type while events and 
  /// certain services are delgated down to the hierarchy items.
  /// </summary>
  // ================================================================================================
  public abstract class HierarchyManager<TPackage> : 
    // --- Hierarchy manager behavior
    IHierarchyManager,
    // --- Commands coming from the IDE are processed by the hierarchy manager
    IOleCommandTarget,
    // --- Cleaning up resources is the responsibility of this object
    IDisposable
    where TPackage : PackageBase
  {
    #region Private fields

    /// <summary>Package hosting this hierarchy manager</summary>
    private readonly TPackage _Package;

    /// <summary>Root node of the hierarchy</summary>
    private HierarchyNode _HierarchyRoot;

    /// <summary>Stores the hierarchy items managed by this instance</summary>
    private readonly ItemMap<HierarchyNode> _ManagedItems =
      new ItemMap<HierarchyNode>();

    /// <summary>Stores the map for event sink subscribers</summary>
    private readonly ItemMap<IVsHierarchyEvents> _EventSinks =
      new ItemMap<IVsHierarchyEvents>();

    /// <summary>Stores the image indexes belonging tohierarchy node types</summary>
    private readonly Dictionary<Type, int> _ImageIndexes =
      new Dictionary<Type, int>();

    private readonly SimpleOleServiceProvider _OleServiceProvider = new SimpleOleServiceProvider();

    /// <summary>
    /// Object responsible to translate command methods to OleMenuCommand instances
    /// </summary>
    private CommandDispatcher<TPackage> _CommandDispatcher;

    /// <summary>A service provider call back object provided by the IDE hosting the project manager</summary>
    private IServiceProvider _Site;

    /// <summary>Service instance processing commands</summary>
    private OleMenuCommandService _OleMenuCommandService;

    /// <summary>Object holding images for this hierarchy</summary>
    private ImageHandler _ImageHandler;

    /// <summary>Parent hierarchy of this manager</summary>
    private IHierarchyManager _ParentHierarchy;

    /// <summary>Item ID in Parent hierarchy</summary>
    private HierarchyId _IdInParentHierarchy;

    /// <summary>Has the object been disposed.</summary>
    /// <remarks>
    /// We will not specify a property for isDisposed, rather it is expected that the a private 
    /// flag is defined on all subclasses. We do not want get in a situation where the base 
    /// class's dipose is not called because a child sets the flag through the property.
    /// </remarks>
    private bool _IsDisposed;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyManager{TPackage}"/> class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected HierarchyManager()
    {
      _Package = PackageBase.GetPackageInstance<TPackage>();
      _OleServiceProvider.AddService(typeof(IVsHierarchy), this, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing">
    /// Is the Dispose called by some internal member, or it is called by from GC.
    /// </param>
    // --------------------------------------------------------------------------------------------
    protected virtual void Dispose(bool disposing)
    {
      if (_IsDisposed) return;
      if (disposing)
      {
        // --- This will dispose any subclassed project node that implements IDisposable.
        if (_OleServiceProvider != null)
        {
          // --- Dispose the ole service provider object.
          _OleServiceProvider.Dispose();
        }
      }
      _IsDisposed = true;
    }

    #endregion

    #region Public Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the number of items handled by this manager
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int ItemCount
    {
      get { return ManagedItems.Count; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the manager is closed or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual bool IsClosed
    {
      get { return _HierarchyRoot == null ? true : _HierarchyRoot.IsClosed; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package owning this hierarchy manager.
    /// </summary>
    /// <value>The package owning this hierarchy manager.</value>
    // --------------------------------------------------------------------------------------------
    public TPackage Package
    {
      get { return _Package; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the hierarchy window hosting this hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IVsUIHierarchyWindow UIHierarchyWindow { get; protected internal set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the UI hierarchy tool window hosting this hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public UIHierarchyToolWindow<TPackage> UIHierarchyToolWindow { get; protected internal set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the root node of the hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public HierarchyNode HierarchyRoot
    {
      get
      {
        EnsureHierarchyRoot();
        return _HierarchyRoot;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Provides a root object for this hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void EnsureHierarchyRoot()
    {
      if (_HierarchyRoot == null)
      {
        _HierarchyRoot = CreateHierarchyRoot();
        _HierarchyRoot.ManagerNode = this;
        _HierarchyRoot.HierarchyId = HierarchyId.Root;
        SetDefaultImage(_HierarchyRoot);
        InitializeHierarchyRoot(_HierarchyRoot);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the item event sinks.
    /// </summary>
    /// <value>The item event sinks.</value>
    // --------------------------------------------------------------------------------------------
    public ItemMap<HierarchyNode> ManagedItems
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

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy of this hierarchy
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IHierarchyManager ParentHierarchy
    {
      get { return _ParentHierarchy; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy of this hierarchy
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public HierarchyId IdInParentHierarchy
    {
      get { return _IdInParentHierarchy; }
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
    public HierarchyNode this[uint itemId]
    {
      get
      {
        if (VSConstants.VSITEMID_ROOT == itemId) return HierarchyRoot;
        return VSConstants.VSITEMID_NIL == itemId ? null : _ManagedItems[itemId];
      }
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
    public HierarchyNode this[HierarchyId id]
    {
      get { return this[(uint) id]; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Makes it possible for a node of a given hierarchy to be a shortcut to the middle of 
    /// another hierarchy. 
    /// </summary>
    /// <param name="itemId">
    /// Item identifier of the node whose nested hierarchy information is requested.
    /// </param>
    /// <param name="iidHierarchyNested">
    /// Identifier of the interface to be returned in ppHierarchyNested.
    /// </param>
    /// <param name="ppHierarchyNested">
    /// Pointer to the interface whose identifier was passed in iidHierarchyNested.
    /// </param>
    /// <param name="pitemidNested">
    /// Pointer to an item identifier of the root node of the nested hierarchy.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected int GetNestedHierarchy(uint itemId, ref Guid iidHierarchyNested,
                                        out IntPtr ppHierarchyNested, out uint pitemidNested)
    {
      ppHierarchyNested = IntPtr.Zero;
      pitemidNested = 0;
      var node = this[itemId];
      if (node != null && node.NestedHierarchy != null)
      {
        // --- At this point we have a nested hierarchy node. Check if it supports the specified
        // --- interface.
        var hierarchy = ComHelper.GetComObject(node.NestedHierarchy, iidHierarchyNested);
        if (hierarchy != IntPtr.Zero)
        {
          ppHierarchyNested = hierarchy;
          pitemidNested = (uint)HierarchyId.Root;
          Console.WriteLine("GetNestedHierarchy {0}, {1}", 
            node.Caption, node.NestedHierarchy.HierarchyRoot.Caption);
          return VSConstants.S_OK;
        }
      }
      Console.WriteLine("GetNestedHierarchy {0}, <none>", node == null ? "<unknown>" : node.Caption);
      return VSConstants.E_FAIL;
    }

    #endregion

    #region Internally used methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the default image of the specified node.
    /// </summary>
    /// <param name="node">Node to set the default image for.</param>
    // --------------------------------------------------------------------------------------------
    protected internal void SetDefaultImage(HierarchyNode node)
    {
      int imageIndex;
      if (_ImageIndexes.TryGetValue(node.GetType(), out imageIndex))
      {
        // --- There is a default image index for this node type
        node.DefaultImageIndex = imageIndex;
      }
      else
      {
        // --- Check for default imageIndex
        var hbAttr = node.GetType().GetAttribute<HierarchyBitmapAttribute>();
        if (hbAttr != null)
        {
          var bitmap = ResourceResolver<TPackage>.GetBitmap(hbAttr.Value);
          if (bitmap != null)
          {
            // --- We can add this bitmap as the default image for the node
            ImageHandler.AddImage(bitmap);
            node.DefaultImageIndex = ImageHandler.LastImageIndex;
            _ImageIndexes.Add(node.GetType(), node.DefaultImageIndex);
          }
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes a subordiante node from this hierarchy.
    /// </summary>
    /// <param name="node">Node to remove from hierarchy</param>
    // --------------------------------------------------------------------------------------------
    protected internal void RemoveSubordinate(HierarchyNode node)
    {
      _ManagedItems.Remove(node);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Compares the sort order of the specified nodes.
    /// </summary>
    /// <param name="node1">First node</param>
    /// <param name="node2">Second node</param>
    // --------------------------------------------------------------------------------------------
    public virtual int CompareNodes(HierarchyNode node1, HierarchyNode node2)
    {
      if (node1 == null)
        throw new ArgumentNullException("node1");
      if (node2 == null)
        throw new ArgumentNullException("node2");
      return node1.SortPriority == node2.SortPriority
               ? String.Compare(node2.Caption, node1.Caption, true, CultureInfo.CurrentCulture)
               : node2.SortPriority - node1.SortPriority;
    }

    #endregion

    #region Abstract and virtual members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates the root node of the hierarchy.
    /// </summary>
    /// <returns>The newly created hierarchy node</returns>
    /// <remarks>
    /// Override this method to create the root instance of the hierarchy node. In this method you
    /// cannot refer to the manager node of the newly created root. After returning the root node
    /// instance, the manager instance will site it and then calls the 
    /// <see cref="InitializeHierarchyRoot"/> method. Override that method to finish the 
    /// initialization of the root node.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected abstract HierarchyNode CreateHierarchyRoot();

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The hierarchy manager calls this method after the root node is created and sited. Override
    /// this method to provide custom initialization.
    /// </summary>
    /// <param name="root">Root node instance.</param>
    // --------------------------------------------------------------------------------------------
    protected virtual void InitializeHierarchyRoot(HierarchyNode root)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Closes and cleans up a hierarchy once the environment determines that it is no longer used.
    /// </summary>
    /// <remarks>
    /// Override this method to clean up the node information.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public virtual void Close()
    {
      Dispose(true);
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

    #region IVsHierarchy implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service provider from which to access the services.
    /// </summary>
    /// <param name="site">IOleServiceProvider interface of the service provider.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsHierarchy.GetSite(out IOleServiceProvider site)
    {
      site = Site.GetService(typeof(IOleServiceProvider)) as IOleServiceProvider;
      return VSConstants.S_OK;
    }

    int IVsHierarchy.QueryClose(out int pfCanClose)
    {
      pfCanClose = 1;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Closes and cleans up a hierarchy once the environment determines that it is no longer used.
    /// </summary>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsHierarchy.Close()
    {
      Close();
      return VSConstants.S_OK;
    }

    int IVsHierarchy.GetGuidProperty(uint itemId, int propId, out Guid pguid)
    {
      // --- Obtain the node by the specified ID
      pguid = Guid.Empty;
      var node = this[itemId];
      if (node != null)
      {
        node.GetGuidProperty(propId, out pguid);
      }
      return pguid == Guid.Empty
        ? VSConstants.DISP_E_MEMBERNOTFOUND
        : VSConstants.S_OK;
    }

    int IVsHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
    {
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets properties of a given node or of the hierarchy.
    /// </summary>
    /// <param name="itemId">Item identifier of an item in the hierarchy.</param>
    /// <param name="propId">Identifier of the hierarchy property.</param>
    /// <param name="propVal">Pointer to a VARIANT containing the property value</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsHierarchy.GetProperty(uint itemId, int propId, out object propVal)
    {
      propVal = null;
      // --- IconImgList is available only for the root item
      if (itemId != VSConstants.VSITEMID_ROOT && propId == (int)__VSHPROPID.VSHPROPID_IconImgList)
      {
        return VSConstants.DISP_E_MEMBERNOTFOUND;
      }

      // --- Obtain the node by the specified ID
      var node = this[itemId];
      if (node != null)
      {
        propVal = node.GetProperty(propId);
      }
      return propVal == null 
        ? VSConstants.DISP_E_MEMBERNOTFOUND 
        : VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets properties of a specific node or of the hierarchy.
    /// </summary>
    /// <param name="itemId">Item identifier of an item in the hierarchy.</param>
    /// <param name="propId">Identifier of the hierarchy property.</param>
    /// <param name="value">Variant that contains property information</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsHierarchy.SetProperty(uint itemId, int propId, object value)
    {
      var node = this[itemId];
      if (node != null)
      {
        return node.SetProperty(propId, value);
      }
      return VSConstants.DISP_E_MEMBERNOTFOUND;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Makes it possible for a node of a given hierarchy to be a shortcut to the middle of 
    /// another hierarchy. 
    /// </summary>
    /// <param name="itemId">
    /// Item identifier of the node whose nested hierarchy information is requested.
    /// </param>
    /// <param name="iidHierarchyNested">
    /// Identifier of the interface to be returned in ppHierarchyNested.
    /// </param>
    /// <param name="ppHierarchyNested">
    /// Pointer to the interface whose identifier was passed in iidHierarchyNested.
    /// </param>
    /// <param name="pitemidNested">
    /// Pointer to an item identifier of the root node of the nested hierarchy.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsHierarchy.GetNestedHierarchy(uint itemId, ref Guid iidHierarchyNested,
                                        out IntPtr ppHierarchyNested, out uint pitemidNested)
    {
      return GetNestedHierarchy(itemId, ref iidHierarchyNested, 
        out ppHierarchyNested, out pitemidNested);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the canonical name of this hierarchy node.
    /// </summary>
    /// <param name="itemId">The item id.</param>
    /// <param name="name">The name.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsHierarchy.GetCanonicalName(uint itemId, out string name)
    {
      var node = this[itemId];
      name = (node != null) ? node.CanonicalName : null;
      return VSConstants.S_OK;
    }

    int IVsHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
    {
      pitemid = VSConstants.VSITEMID_NIL;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Establishes client notification of hierarchy events.
    /// </summary>
    /// <param name="pEventSink">
    /// IVsHierarchyEvents interface on the object requesting notification of hierarchy events.
    /// </param>
    /// <param name="pdwCookie">
    /// Pointer to a unique identifier for the referenced event sink. This value is required to 
    /// unadvise the event sink using UnadviseHierarchyEvents. 
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
    {
      pdwCookie = _EventSinks.Add(pEventSink) + 1;
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Disables client notification of hierarchy events.
    /// </summary>
    /// <param name="dwCookie">
    /// Abstract handle to the client that was disabled from receiving notifications of 
    /// hierarchy events.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsHierarchy.UnadviseHierarchyEvents(uint dwCookie)
    {
      _EventSinks.RemoveAt(dwCookie - 1);
      return VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    /// <param name="site"></param>
    /// <returns></returns>
    // --------------------------------------------------------------------------------------------
    public virtual int SetSite(IOleServiceProvider site)
    {
      if (_Site != null)
      {
        throw new InvalidOperationException(String.Format(Resources.Hierarchy_SiteAlreadySet,
          GetType().Name));
      }

      _Site = new ServiceProvider(site);

      // --- Set up command dispatching
      _CommandDispatcher = new CommandDispatcher<TPackage>(this);

      // --- Register command handlers
      var parentService = Package.GetService<IMenuCommandService, OleMenuCommandService>();
      _OleMenuCommandService = new OleMenuCommandService(_Site, parentService);
      _CommandDispatcher.RegisterCommandHandlers(_OleMenuCommandService, parentService);
      return VSConstants.S_OK;
    }

    #endregion

    #region IVsUIHierarchy implementation

    int IVsUIHierarchy.QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      return (int)OleConstants.OLECMDERR_E_UNKNOWNGROUP;
    }

    int IVsUIHierarchy.ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      if (pguidCmdGroup.Equals(VSConstants.GUID_VsUIHierarchyWindowCmds))
      {
        switch (nCmdID)
        {
          case (uint)VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_DoubleClick:
            var node = this[itemid];
            VsMessageBox.Show("Hierarchy: " + _HierarchyRoot.Caption);
            break;
          case (uint)VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_EnterKey:
            var nodeB = this[itemid];
            if (nodeB != null)
            {
              UIHierarchyToolWindow.AddSelectNode(nodeB);
            }
            return VSConstants.S_FALSE;
          case (uint)VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_RightClick:
            var nodeUB = this[itemid];
            if (nodeUB != null)
            {
              UIHierarchyToolWindow.EditNodeLabel(nodeUB);
            }
            return VSConstants.S_OK;
        }
        return VSConstants.S_OK; // return S_FALSE to avoid the exec of the original function
      }
      return (int)OleConstants.OLECMDERR_E_NOTSUPPORTED;
    }

    #endregion

    #region IVsUIHierarchy implementation (Calling IVsHierarchy implementations)

    int IVsUIHierarchy.GetSite(out IOleServiceProvider ppSP)
    {
      return ((IVsHierarchy)this).GetSite(out ppSP);
    }

    int IVsUIHierarchy.QueryClose(out int pfCanClose)
    {
      return ((IVsHierarchy)this).QueryClose(out pfCanClose);
    }

    int IVsUIHierarchy.Close()
    {
      return ((IVsHierarchy)this).Close();
    }

    int IVsUIHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
    {
      return ((IVsHierarchy)this).GetGuidProperty(itemid, propid, out pguid);
    }

    int IVsUIHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
    {
      return ((IVsHierarchy)this).SetGuidProperty(itemid, propid, ref rguid);
    }

    int IVsUIHierarchy.GetProperty(uint itemid, int propid, out object pvar)
    {
      return ((IVsHierarchy)this).GetProperty(itemid, propid, out pvar);
    }

    int IVsUIHierarchy.SetProperty(uint itemid, int propid, object var)
    {
      return ((IVsHierarchy)this).SetProperty(itemid, propid, var);
    }

    int IVsUIHierarchy.GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
    {
      return ((IVsHierarchy)this).GetNestedHierarchy(itemid, ref iidHierarchyNested, out ppHierarchyNested, out pitemidNested);
    }

    int IVsUIHierarchy.GetCanonicalName(uint itemid, out string pbstrName)
    {
      return ((IVsHierarchy)this).GetCanonicalName(itemid, out pbstrName);
    }

    int IVsUIHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
    {
      return ((IVsHierarchy)this).ParseCanonicalName(pszName, out pitemid);
    }

    int IVsUIHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
    {
      return ((IVsHierarchy)this).AdviseHierarchyEvents(pEventSink, out pdwCookie);
    }

    int IVsUIHierarchy.UnadviseHierarchyEvents(uint dwCookie)
    {
      return ((IVsHierarchy)this).UnadviseHierarchyEvents(dwCookie);
    }

    int IVsUIHierarchy.SetSite(IOleServiceProvider psp)
    {
      return ((IVsHierarchy)this).SetSite(psp);
    }

    #endregion

    #region Unused IVsHierarchy and IVsUIHierarchy methods

    int IVsHierarchy.Unused0()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.Unused1()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.Unused2()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.Unused3()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.Unused4()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.Unused0()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.Unused1()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.Unused2()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.Unused3()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.Unused4()
    {
      return VSConstants.E_NOTIMPL;
    }

    #endregion

    #region Implementation of IDisposable

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting 
    /// unmanaged resources.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
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

    #region Implementation of IOleCommandTarget

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries the object for the status of one or more commands generated by user interface events.
    /// </summary>
    /// <param name="pguidCmdGroup">
    /// Unique identifier of the command group; can be NULL to specify the standard group. All the 
    /// commands that are passed in the prgCmds array must belong to the group specified by 
    /// pguidCmdGroup.
    /// </param>
    /// <param name="cCmds">The number of commands in the prgCmds array.</param>
    /// <param name="prgCmds">
    /// A caller-allocated array of OLECMD structures that indicate the commands for which the 
    /// caller needs status information. This method fills the cmdf member of each structure with 
    /// values taken from the OLECMDF enumeration.
    /// </param>
    /// <param name="pCmdText">
    /// Pointer to an OLECMDTEXT structure in which to return name and/or status information of a 
    /// single command. Can be NULL to indicate that the caller does not need this information.
    /// </param>
    /// <returns>
    /// This method supports the standard return values E_FAIL and E_UNEXPECTED, 
    /// as well as the following: 
    /// <para>S_OK: The command was executed successfully.</para>
    /// <para>E_POINTER: The prgCmds argument is NULL.</para>
    /// <para>
    /// OLECMDERR_E_UNKNOWNGROUP: The pguidCmdGroup parameter is not NULL but does not specify 
    /// a recognized command group.
    /// </para>
    /// </returns>
    /// <remarks>
    /// This is called by Visual Studio when it needs the status of our menu commands.  There
    /// is no need to override this method.  If you need access to menu commands use
    /// IMenuCommandService.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      IOleCommandTarget target = _OleMenuCommandService;
      return target.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Executes a specified command or displays help for a command.
    /// </summary>
    /// <param name="pguidCmdGroup">
    /// Unique identifier of the command group; can be NULL to specify the standard group.
    /// </param>
    /// <param name="nCmdID">
    /// The command to be executed. This command must belong to the group specified with pguidCmdGroup.
    /// </param>
    /// <param name="nCmdexecopt">
    /// Values taken from the OLECMDEXECOPT enumeration, which describe how the object should 
    /// execute the command.
    /// </param>
    /// <param name="pvaIn">
    /// Pointer to a VARIANTARG structure containing input arguments. Can be NULL.
    /// </param>
    /// <param name="pvaOut">
    /// Pointer to a VARIANTARG structure to receive command output. Can be NULL.
    /// </param>
    /// <returns>
    /// This method supports the standard return values E_FAIL and E_UNEXPECTED, 
    /// as well as the following: 
    /// <para>S_OK: The command was executed successfully.</para>
    /// <para>
    /// OLECMDERR_E_UNKNOWNGROUP: The pguidCmdGroup parameter is not NULL but does not specify 
    /// a recognized command group.
    /// </para>
    /// <para>
    /// OLECMDERR_E_NOTSUPPORTED: The nCmdID parameter is not a valid command in the group 
    /// identified by pguidCmdGroup.
    /// </para>
    /// <para>
    /// OLECMDERR_E_DISABLED: The command identified by nCmdID is currently disabled and 
    /// cannot be executed.
    /// </para>
    /// <para>
    /// OLECMDERR_E_NOHELP: The caller has asked for help on the command identified by 
    /// nCmdID, but no help is available.
    /// </para>
    /// <para>OLECMDERR_E_CANCELED: The user canceled the execution of the command.</para>
    /// </returns>
    /// <remarks>
    /// This is called by Visual Studio when the user has requested to execute a particular
    /// command.  There is no need to override this method.  If you need access to menu
    /// commands use IMenuCommandService.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      IOleCommandTarget target = _OleMenuCommandService;
      return target.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
    }

    #endregion

    #region Command execution methods

    protected virtual int QueryStatusSelection(Guid cmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText, CommandOrigin commandOrigin)
    {
      //// --- A closed node does not support any command
      //if (IsClosed) return (int) OleConstants.OLECMDERR_E_NOTSUPPORTED;

      //// --- Empty command group is not supported
      //if (cmdGroup == Guid.Empty) return (int) OleConstants.OLECMDERR_E_UNKNOWNGROUP;

      //uint cmd = prgCmds[0].cmdID;
      //bool handled = false;

      //if (commandOrigin == CommandOrigin.OleCommandTarget)
      //{
      //  queryResult = this.QueryStatusCommandFromOleCommandTarget(cmdGroup, cmd, out handled);
      //}

      //if (!handled)
      //{
      //  IList<HierarchyNode> selectedNodes = this.projectMgr.GetSelectedNodes();

      //  // Want to disable in multiselect case.
      //  if (selectedNodes != null && selectedNodes.Count > 1)
      //  {
      //    queryResult = this.DisableCommandOnNodesThatDoNotSupportMultiSelection(cmdGroup, cmd, selectedNodes,
      //                                                                           out handled);
      //  }

      //  // Now go and do the job on the nodes.
      //  if (!handled)
      //  {
      //    queryResult = this.QueryStatusSelectionOnNodes(selectedNodes, cmdGroup, cmd, pCmdText);
      //  }
      //}

      //// Process the results set in the QueryStatusResult
      //if (queryResult != QueryStatusResult.NOTSUPPORTED)
      //{
      //  // Set initial value
      //  prgCmds[0].cmdf = (uint) OLECMDF.OLECMDF_SUPPORTED;

      //  if ((queryResult & QueryStatusResult.ENABLED) != 0)
      //  {
      //    prgCmds[0].cmdf |= (uint) OLECMDF.OLECMDF_ENABLED;
      //  }

      //  if ((queryResult & QueryStatusResult.INVISIBLE) != 0)
      //  {
      //    prgCmds[0].cmdf |= (uint) OLECMDF.OLECMDF_INVISIBLE;
      //  }

      //  if ((queryResult & QueryStatusResult.LATCHED) != 0)
      //  {
      //    prgCmds[0].cmdf |= (uint) OLECMDF.OLECMDF_LATCHED;
      //  }

      //  return VSConstants.S_OK;
      //}
      return (int) OleConstants.OLECMDERR_E_NOTSUPPORTED;
    }

    #endregion

    #region IHierarchyManager explicit implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the node at the specified ID.
    /// </summary>
    /// <param name="id">ID to set the node at</param>
    /// <param name="node">Node to set</param>
    // --------------------------------------------------------------------------------------------
    void IHierarchyManager.SetNodeAtId(HierarchyId id, HierarchyNode node)
    {
      ManagedItems.SetAt(id, node);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raises the hierarchy events according to the specified action.
    /// </summary>
    /// <param name="sinkAction">Action to execute</param>
    // --------------------------------------------------------------------------------------------
    void IHierarchyManager.RaiseHierarchyEvent(Func<IVsHierarchyEvents, int> sinkAction)
    {
      foreach (var sink in _EventSinks)
      {
        int result = sinkAction(sink);
        if (ErrorHandler.Failed(result) && result != VSConstants.E_NOTIMPL)
        {
          ErrorHandler.ThrowOnFailure(result);
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes the specified node from the hierarchy.
    /// </summary>
    /// <param name="node">Node to remove</param>
    // --------------------------------------------------------------------------------------------
    void IHierarchyManager.RemoveNode(HierarchyNode node)
    {
      ManagedItems.Remove(node);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a subordinate node to this hierarchy.
    /// </summary>
    /// <param name="node">Node to add as a subordinate</param>
    // --------------------------------------------------------------------------------------------
    uint IHierarchyManager.AddSubordinate(HierarchyNode node)
    {
      if (node == null) throw new ArgumentNullException("node");
      SetDefaultImage(node);
      return _ManagedItems.Add(node);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent hierarchy of this hierarchy
    /// </summary>
    /// <param name="parent">The parent hierarchy.</param>
    /// <param name="id">The id of the item in the parent hierarchy.</param>
    // --------------------------------------------------------------------------------------------
    void IHierarchyManager.SetParentHierarchy(IHierarchyManager parent, HierarchyId id)
    {
      _ParentHierarchy = parent;
      _IdInParentHierarchy = id;
    }

    #endregion
  }

  // ================================================================================================
  /// <summary>
  /// This enum is used in HierarchyManager to declare if the command raised is coming from the 
  /// hierarchy itself or from the related OleCommandTarget.
  /// </summary>
  // ================================================================================================
  public enum CommandOrigin
  {
    /// <summary>The command is coming from the IVsUIHierarchy instance</summary>
    UIHierarchy,
    /// <summary>The command is coming from the IOleCommandTarget instance</summary>
    OleCommandTarget
  }
}