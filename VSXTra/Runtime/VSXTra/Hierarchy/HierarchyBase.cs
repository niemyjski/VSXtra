// ================================================================================================
// HierarchyBase.cs
//
// Created: 2008.09.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Shell;
using IServiceProvider=Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra.Hierarchy
{
  #region IHierarchyBehavior

  // ================================================================================================
  /// <summary>
  /// This interface defines what we call a hierarchy.
  /// </summary>
  // ================================================================================================
  public interface IHierarchyBehavior : IVsUIHierarchy
  {
  }

  #endregion

  #region HierarchyBase<THier, TRoot> definition

  // ================================================================================================
  /// <summary>
  /// This class is intended to be the root class of all IVsHierarchy implementations.
  /// </summary>
  /// <typeparam name="THier">Type of hierarchy element</typeparam>
  /// <typeparam name="TRoot">Type of root element</typeparam>
  /// <remarks>
  /// This node represents a hierarchy element which is under the management of the root hierarchy
  /// element.
  /// </remarks>
  // ================================================================================================
  public abstract class HierarchyBase<THier, TRoot> : IHierarchyBehavior,
    IDisposable
    where THier : HierarchyBase<THier, TRoot>
    where TRoot : HierarchyRoot<TRoot, THier>
  {
    #region Constant values

    private const int NoImageIndex = -1;

    #endregion

    #region Private Fields

    /// <summary>Stores the map for event sink subscribers</summary>
    private readonly ItemMap<IVsHierarchyEvents> _EventSinks =
      new ItemMap<IVsHierarchyEvents>();

    private HierarchyBase<THier, TRoot> _ParentNode;
    private HierarchyBase<THier, TRoot> _NextSibling;
    private HierarchyBase<THier, TRoot> _FirstChild;
    private HierarchyBase<THier, TRoot> _LastChild;
    private HierarchyId _HierarchyId;
    private readonly SimpleOleServiceProvider _OleServiceProvider = new SimpleOleServiceProvider();
    private IVsHierarchy _ParentHierarchy;
    private int _ParentHierarchyItemId;
    private EventHandler<HierarchyNodeEventArgs> _OnChildAdded;
    private EventHandler<HierarchyNodeEventArgs> _OnChildRemoved;

    /// <summary>
    /// Has the object been disposed.
    /// </summary>
    /// <devremark>We will not specify a property for isDisposed, rather it is expected that the a private flag is defined
    /// on all subclasses. We do not want get in a situation where the base class's dipose is not called because a child sets the flag through the property.</devremark>
    private bool _IsDisposed;

    #endregion

    #region Lifecycle methods

    protected HierarchyBase()
    {
      SortPriority = 1000;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyBase&lt;THier, TRoot&gt;"/> class.
    /// </summary>
    /// <param name="root">The root instance.</param>
    // --------------------------------------------------------------------------------------------
    protected HierarchyBase(TRoot root): this()
    {
      ManagerNode = root;
      _HierarchyId = ManagerNode.AddSubordinate(this);
      _OleServiceProvider.AddService(typeof(IVsHierarchy), root, false);
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

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the ID of the node within the hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public HierarchyId HierarchyId
    {
      get { return _HierarchyId;  }
      internal set { _HierarchyId = value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent node.
    /// </summary>
    /// <value>The parent node.</value>
    // --------------------------------------------------------------------------------------------
    public HierarchyBase<THier, TRoot> ParentNode
    {
      get { return _ParentNode; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the manager node.
    /// </summary>
    /// <value>The manager node.</value>
    // --------------------------------------------------------------------------------------------
    //public HierarchyRoot<TRoot, THier> ManagerNode { get; protected set; }
    public TRoot ManagerNode { get; protected set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the root node.
    /// </summary>
    /// <value>The root node.</value>
    // --------------------------------------------------------------------------------------------
    public TRoot RootNode { get; protected set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the next sibling.
    /// </summary>
    /// <value>The next sibling.</value>
    // --------------------------------------------------------------------------------------------
    public HierarchyBase<THier, TRoot> NextSibling
    {
      get { return _NextSibling; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the previous sibling.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [System.ComponentModel.BrowsableAttribute(false)]
    public HierarchyBase<THier, TRoot> PreviousSibling
    {
      get
      {
        if (_ParentNode == null) return null;
        HierarchyBase<THier, TRoot> prev = null;
        foreach (var child in Children)
        {
          if (child == this)
            break;
          prev = child;
        }
        return prev;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the first child.
    /// </summary>
    /// <value>The first child.</value>
    // --------------------------------------------------------------------------------------------
    public HierarchyBase<THier, TRoot> FirstChild
    {
      get { return _FirstChild; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the last child.
    /// </summary>
    /// <value>The last child.</value>
    // --------------------------------------------------------------------------------------------
    public HierarchyBase<THier, TRoot> LastChild
    {
      get { return _LastChild; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if this node has a parent node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool HasParent
    {
      get { return _ParentNode != null; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if this node is a leaf node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool IsLeaf
    {
      get { return _FirstChild == null; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether this instance is expanded.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
    /// </value>
    // --------------------------------------------------------------------------------------------
    public bool IsExpanded { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the value indicating if this node is expanded by default or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool ExpandByDefault { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the canonical name of the hierarchy node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual string CanonicalName
    {
      get { return Caption; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when a child has been added to this node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    internal event EventHandler<HierarchyNodeEventArgs> OnChildAdded
    {
      add { _OnChildAdded += value; }
      remove { _OnChildAdded -= value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when a child has been removed from this node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    internal event EventHandler<HierarchyNodeEventArgs> OnChildRemoved
    {
      add { _OnChildRemoved += value; }
      remove { _OnChildRemoved -= value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the hierarchy (sort) order.
    /// </summary>
    /// <remarks>
    /// This value is used when a new child node is added to the hierarchy.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public int SortPriority { get; set; }

    public virtual int ImageIndex
    {
      get { return NoImageIndex; }
    }

    public virtual int ExpandedImageIndex
    {
      get { return ImageIndex; }
    }

    #endregion

    #region Virtual and abstract methods 

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the caption of the node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public abstract string Caption { get; }

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
      return VSConstants.E_FAIL;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified property of this hierarchy node.
    /// </summary>
    /// <param name="propId">Property identifier.</param>
    /// <returns></returns>
    // --------------------------------------------------------------------------------------------
    public virtual object GetProperty(int propId)
		{
      Console.WriteLine("GetProperty {0}:{1}", _HierarchyId, propId);
      object result = null;
      switch ((__VSHPROPID)propId)
      {
        case __VSHPROPID.VSHPROPID_Expandable:
          result = (_FirstChild != null);
          break;

        case __VSHPROPID.VSHPROPID_Caption:
        case __VSHPROPID.VSHPROPID_Name:
        case __VSHPROPID.VSHPROPID_SaveName:
          result = Caption;
          break;

        case __VSHPROPID.VSHPROPID_ExpandByDefault:
          result = ExpandByDefault;
          break;

        case __VSHPROPID.VSHPROPID_IconImgList:
          result = ManagerNode.ImageListHandle;
          break;

        case __VSHPROPID.VSHPROPID_OpenFolderIconIndex:
          result = ExpandedImageIndex;
          break;
        case __VSHPROPID.VSHPROPID_IconIndex:
          result = ImageIndex;
          break;

        case __VSHPROPID.VSHPROPID_StateIconIndex:
          //result = (int)this.StateIconIndex;
          break;

        case __VSHPROPID.VSHPROPID_IconHandle:
          //result = GetIconHandle(false);
          break;

        case __VSHPROPID.VSHPROPID_OpenFolderIconHandle:
          //result = GetIconHandle(true);
          break;

        case __VSHPROPID.VSHPROPID_NextVisibleSibling:
        case __VSHPROPID.VSHPROPID_NextSibling:
          result = (int)((_NextSibling != null) ? _NextSibling.HierarchyId : VSConstants.VSITEMID_NIL);
          break;

        case __VSHPROPID.VSHPROPID_FirstChild:
        case __VSHPROPID.VSHPROPID_FirstVisibleChild:
          result = (int)((_FirstChild != null) ? _FirstChild.HierarchyId : VSConstants.VSITEMID_NIL);
          break;

        case __VSHPROPID.VSHPROPID_Parent:
          if (_ParentNode == null)
          {
            unchecked { result = new IntPtr((int)VSConstants.VSITEMID_NIL); }
          }
          else
          {
            result = new IntPtr((int)_ParentNode.HierarchyId);
          }
          break;

        case __VSHPROPID.VSHPROPID_ParentHierarchyItemid:
          //if (parentHierarchy != null)
          //{
          //  result = (IntPtr)parentHierarchyItemId; // VS requires VT_I4 | VT_INT_PTR
          //}
          break;

        case __VSHPROPID.VSHPROPID_ParentHierarchy:
          //result = parentHierarchy;
          break;

        case __VSHPROPID.VSHPROPID_Root:
          result = Marshal.GetIUnknownForObject(ManagerNode);
          break;

        case __VSHPROPID.VSHPROPID_Expanded:
          result = IsExpanded;
          break;

        case __VSHPROPID.VSHPROPID_BrowseObject:
          //result = this.NodeProperties;
          //if (result != null) result = new DispatchWrapper(result);
          break;

        case __VSHPROPID.VSHPROPID_EditLabel:
          //if (this.ProjectMgr != null && !this.ProjectMgr.IsClosed && !this.ProjectMgr.IsCurrentStateASuppressCommandsMode())
          //{
          //  result = GetEditLabel();
          //}
          break;

        case __VSHPROPID.VSHPROPID_ItemDocCookie:
          //if (this.docCookie != 0) return (IntPtr)this.docCookie; //cast to IntPtr as some callers expect VT_INT
          break;

        case __VSHPROPID.VSHPROPID_ExtObject:
          //result = GetAutomationObject();
          break;
      }
      return result;
    }

    public virtual int SetProperty(int propid, object value)
    {
      var id = (__VSHPROPID)propid;
      switch (id)
      {
        case __VSHPROPID.VSHPROPID_Expanded:
          if ((bool) value)
          {
            OnBeforeExpanded();
            IsExpanded = true;
            OnAfterExpanded();
          }
          else
          {
            OnBeforeCollapsed();
            IsExpanded = false;
            OnAfterCollapsed();
          }
          break;

        //case __VSHPROPID.VSHPROPID_ParentHierarchy:
        //  parentHierarchy = (IVsHierarchy)value;
        //  break;

        //case __VSHPROPID.VSHPROPID_ParentHierarchyItemid:
        //  parentHierarchyItemId = (int)value;
        //  break;

        //case __VSHPROPID.VSHPROPID_EditLabel:
        //  return SetEditLabel((string)value);

        default:
          break;
      }
      return VSConstants.S_OK;
    }

    protected virtual void OnBeforeExpanded()
    {
    }

    protected virtual void OnAfterExpanded()
    {
    }

    protected virtual void OnBeforeCollapsed()
    {
    }

    protected virtual void OnAfterCollapsed()
    {
    }

    public void InvalidateItem()
    {
      RaiseHierarchyEvent(sink => sink.OnInvalidateItems((uint)HierarchyId));
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Add a child node to this node, sorted in the right location.
    /// </summary>
    /// <param name="node">The child node to add to this node.</param>
    // --------------------------------------------------------------------------------------------
    public virtual void AddChild(HierarchyBase<THier, TRoot> node)
    {
      if (node == null)
      {
        throw new ArgumentNullException("node");
      }

      // --- Make sure the node is in the map.
      var nodeWithSameID = ManagerNode.NodeById(node.HierarchyId);
      if (!ReferenceEquals(node, nodeWithSameID))
      {
        if (nodeWithSameID == null && (int)node.HierarchyId <= ManagerNode.ManagedItems.Count)
        { 
          ManagerNode.ManagedItems.SetAt(node.HierarchyId, this);
        }
        else
        {
          throw new InvalidOperationException();
        }
      }

      HierarchyBase<THier, TRoot> previous = null;
      foreach (var n in Children)
      {
        if (ManagerNode.CompareNodes(node, n) > 0) break;
        previous = n;
      }
      // --- Insert "node" after "previous".
      if (previous != null)
      {
        node._NextSibling = previous._NextSibling;
        previous._NextSibling = node;
        if (previous == _LastChild)
        {
          _LastChild = node;
        }
      }
      else
      {
        if (_LastChild == null)
        {
          _LastChild = node;
        }
        node._NextSibling = _FirstChild;
        _FirstChild = node;
      }
      node._ParentNode = this;
      OnItemAdded(this, node);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method is executed when a new item is added to the hierarchy node.
    /// </summary>
    /// <param name="parent">Parent of the node.</param>
    /// <param name="child">Child node added.</param>
    // --------------------------------------------------------------------------------------------
    protected void OnItemAdded(HierarchyBase<THier, TRoot> parent, HierarchyBase<THier, TRoot> child)
    {
      if (null != parent._OnChildAdded)
      {
        var args = new HierarchyNodeEventArgs(child);
        parent._OnChildAdded(parent, args);
      }
      if (parent == null) throw new ArgumentNullException("parent");
      if (child == null)throw new ArgumentNullException("child");

      // --- Check if the manager node wants to trigger this event
      var foo = ManagerNode ?? this;
      if (foo == ManagerNode && (ManagerNode.DoNotTriggerHierarchyEvents)) return;

      var prev = child.PreviousSibling;
      uint prevId = (prev != null) ? (uint)prev.HierarchyId : VSConstants.VSITEMID_NIL;
      foreach (var sink in foo._EventSinks)
      {
        int result = sink.OnItemAdded((uint)parent.HierarchyId, prevId, (uint)child.HierarchyId);
        if (ErrorHandler.Failed(result) && result != VSConstants.E_NOTIMPL)
        {
          ErrorHandler.ThrowOnFailure(result);
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes a node from the hierarchy.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    // --------------------------------------------------------------------------------------------
    public virtual void RemoveChild(HierarchyBase<THier, TRoot> node)
    {
      if (node == null)
      {
        throw new ArgumentNullException("node");
      }

      ManagerNode.ManagedItems.Remove(node);
      HierarchyBase<THier, TRoot> last = null;
      foreach (var n in Children)
      {
        if (n == node)
        {
          if (last != null)
          {
            last._NextSibling = n._NextSibling;
          }
          if (n == _LastChild)
          {
            _LastChild = last == _LastChild ? null : last;
          }
          if (n == _FirstChild)
          {
            _FirstChild = n._NextSibling;
          }
          return;
        }
        last = n;
      }
      throw new InvalidOperationException("Node not found");
    }

    #endregion

    #region Iterators

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the enumerable list of node items.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IEnumerable<HierarchyBase<THier, TRoot>> Children
    {
      get
      {
        for (var n = _FirstChild; n != null; n = n._NextSibling)
          yield return n;
      }
    }

    #endregion

    #region IVsHierarchy implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service provider from which to access the services.
    /// </summary>
    /// <param name="site">IServiceProvider interface of the service provider.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    int IVsHierarchy.GetSite(out IServiceProvider site)
    {
      site = ManagerNode.Site.GetService(typeof(IServiceProvider)) as IServiceProvider;
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

    int IVsHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
    {
      pguid = Guid.Empty;
      return VSConstants.S_OK;
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
      var node = ManagerNode.NodeById(itemId);
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
      var node = ManagerNode.NodeById(itemId);
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
      var node = ManagerNode.NodeById(itemId);
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

    int IVsHierarchy.SetSite(IServiceProvider psp)
    {
      return VSConstants.S_OK;
    }

    #endregion

    #region IVsUIHierarchy implementation

    int IVsUIHierarchy.QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_UNKNOWNGROUP;
    }

    int IVsUIHierarchy.ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      if (pguidCmdGroup.Equals(VSConstants.GUID_VsUIHierarchyWindowCmds))
      {
        switch (nCmdID)
        {
          case (uint)VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_DoubleClick:
            VsMessageBox.Show("DoubleClick for " + itemid);
            break;
        }
        return VSConstants.S_OK; // return S_FALSE to avoid the exec of the original function
      }
      return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
    }

    #endregion

    #region IVsUIHierarchy implementation (Calling IVsHierarchy implementations)

    int IVsUIHierarchy.GetSite(out IServiceProvider ppSP)
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

    int IVsUIHierarchy.SetSite(IServiceProvider psp)
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
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    protected T GetProperty<T>(__VSHPROPID propId)
    {
      return (T)GetProperty(propId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    protected T GetProperty<T>(int propId)
    {
      return (T)GetProperty(propId);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified hierarchy property.
    /// </summary>
    /// <param name="propId">Property identifier.</param>
    /// <returns>Property value ofthe specified property.</returns>
    // --------------------------------------------------------------------------------------------
    protected object GetProperty(__VSHPROPID propId)
    {
      return GetProperty((int)propId);
    }

    //// --------------------------------------------------------------------------------------------
    ///// <summary>
    ///// Gets the specified GUID hierarchy property.
    ///// </summary>
    ///// <param name="propId">Property identifier.</param>
    ///// <returns>Property value ofthe specified property.</returns>
    //// --------------------------------------------------------------------------------------------
    //protected object GetGuidProperty(int propId)
    //{
    //  if (propId == (int)__VSHPROPID.VSHPROPID_NIL) return null;
    //  Guid propValue;
    //  _Hierarchy.GetGuidProperty(_ItemId, propId, out propValue);
    //  return propValue;
    //}

    private void RaiseHierarchyEvent(Action<IVsHierarchyEvents> sinkAction)
    {
      if (ManagerNode == null) return;
      foreach (var sink in ManagerNode._EventSinks)
      {
        sinkAction(sink);
      }
    }

    #endregion
  }

  #endregion
}