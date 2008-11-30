// ================================================================================================
// HierarchyNode.cs
//
// Created: 2008.09.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Package;

namespace VSXtra.Hierarchy
{
  // ================================================================================================
  /// <summary>
  /// This class implements the responsibilities of a hierarchy node.
  /// </summary>
  /// <remarks>
  /// This node represents a hierarchy element which is under the management of the root hierarchy
  /// element.
  /// </remarks>
  // ================================================================================================
  public abstract class HierarchyNode<TPackage>
    where TPackage : PackageBase 
  {
    #region Constant values

    private const int NoImageIndex = -1;

    #endregion

    #region Private Fields

    private HierarchyNode<TPackage> _ParentNode;
    private HierarchyNode<TPackage> _NextSibling;
    private HierarchyNode<TPackage> _FirstChild;
    private HierarchyNode<TPackage> _LastChild;
    private EventHandler<HierarchyNodeEventArgs<TPackage>> _OnChildAdded;
    private EventHandler<HierarchyNodeEventArgs<TPackage>> _OnChildRemoved;

    #endregion

    #region Lifecycle methods

    protected HierarchyNode()
    {
      IsExpanded = true;
      SortPriority = 1000;
      DefaultImageIndex = NoImageIndex;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyNode{TPackage}"/> class.
    /// </summary>
    /// <param name="manager">The manager object responsible for this node.</param>
    // --------------------------------------------------------------------------------------------
    protected HierarchyNode(HierarchyManager<TPackage> manager): this()
    {
      // --- manager is set to null indicating that this node is a root node as is assigned to 
      // --- its manager node later
      ManagerNode = manager;
      if (manager != null)
      {
        HierarchyId = ManagerNode.AddSubordinate(this);
      }

      // --- Check for the SortOrder attribute
      var attr = GetType().GetAttribute<SortPriorityAttribute>();
      if (attr != null) SortPriority = attr.Value;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the ID of the node within the hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public HierarchyId HierarchyId { get; internal set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the parent node.
    /// </summary>
    /// <value>The parent node.</value>
    // --------------------------------------------------------------------------------------------
    public HierarchyNode<TPackage> ParentNode
    {
      get { return _ParentNode; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the manager node.
    /// </summary>
    /// <value>The manager node.</value>
    // --------------------------------------------------------------------------------------------
    public HierarchyManager<TPackage> ManagerNode { get; protected internal set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the next sibling.
    /// </summary>
    /// <value>The next sibling.</value>
    // --------------------------------------------------------------------------------------------
    [System.ComponentModel.BrowsableAttribute(false)]
    public HierarchyNode<TPackage> NextSibling
    {
      get { return _NextSibling; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the previous sibling.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [System.ComponentModel.BrowsableAttribute(false)]
    public HierarchyNode<TPackage> PreviousSibling
    {
      get
      {
        if (_ParentNode == null) return null;
        HierarchyNode<TPackage> prev = null;
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
    public HierarchyNode<TPackage> FirstChild
    {
      get { return _FirstChild; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the last child.
    /// </summary>
    /// <value>The last child.</value>
    // --------------------------------------------------------------------------------------------
    public HierarchyNode<TPackage> LastChild
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
    internal event EventHandler<HierarchyNodeEventArgs<TPackage>> OnChildAdded
    {
      add { _OnChildAdded += value; }
      remove { _OnChildAdded -= value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Event raised when a child has been removed from this node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    internal event EventHandler<HierarchyNodeEventArgs<TPackage>> OnChildRemoved
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

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the default index of the hierarchy node
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected internal int DefaultImageIndex { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the index of the image used for this hierarchy item.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual int ImageIndex
    {
      get { return DefaultImageIndex; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the index of the image used for this hierarchy item when it is expanded.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual int ExpandedImageIndex
    {
      get { return ImageIndex; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the node is logically closed or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool IsClosed { get; private set; }

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
    /// Closes this hierarchy node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual void Close()
    {
      IsClosed = true;  
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified property of this hierarchy node.
    /// </summary>
    /// <param name="propId">Property identifier.</param>
    /// <returns>
    /// Property value object.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public virtual object GetProperty(int propId)
		{
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

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the specified property of this hierarchy node.
    /// </summary>
    /// <param name="propId">Property identifier.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public virtual int SetProperty(int propId, object value)
    {
      var id = (__VSHPROPID)propId;
      switch (id)
      {
        case __VSHPROPID.VSHPROPID_Expanded:
          var newValue = (bool) value;
          if (newValue != IsExpanded)
          {
            if (newValue)
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
      ManagerNode.RaiseHierarchyEvent(sink => 
        sink.OnInvalidateItems((uint)HierarchyId));
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Add a child node to this node, sorted in the right location.
    /// </summary>
    /// <param name="node">The child node to add to this node.</param>
    // --------------------------------------------------------------------------------------------
    public virtual void AddChild(HierarchyNode<TPackage> node)
    {
      if (node == null)
      {
        throw new ArgumentNullException("node");
      }

      // --- Make sure the node is in the map.
      var nodeWithSameID = ManagerNode[node.HierarchyId];
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

      HierarchyNode<TPackage> previous = null;
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
    protected void OnItemAdded(HierarchyNode<TPackage> parent, HierarchyNode<TPackage> child)
    {
      if (null != parent._OnChildAdded)
      {
        var args = new HierarchyNodeEventArgs<TPackage>(child);
        parent._OnChildAdded(parent, args);
      }
      if (parent == null) throw new ArgumentNullException("parent");
      if (child == null)throw new ArgumentNullException("child");

      // --- Check if the manager node wants to trigger this event
      var prev = child.PreviousSibling;
      uint prevId = (prev != null) ? (uint)prev.HierarchyId : VSConstants.VSITEMID_NIL;
      ManagerNode.RaiseHierarchyEvent(
        sink => sink.OnItemAdded((uint)parent.HierarchyId, prevId, (uint)child.HierarchyId));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Removes a node from the hierarchy.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    // --------------------------------------------------------------------------------------------
    public virtual void RemoveChild(HierarchyNode<TPackage> node)
    {
      if (node == null)
        throw new ArgumentNullException("node");

      ManagerNode.ManagedItems.Remove(node);
      HierarchyNode<TPackage> last = null;
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
    public IEnumerable<HierarchyNode<TPackage>> Children
    {
      get
      {
        for (var n = _FirstChild; n != null; n = n._NextSibling)
          yield return n;
      }
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

    #endregion
  }
}