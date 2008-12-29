// ================================================================================================
// HierarchyNode.cs
//
// Created: 2008.09.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

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
  public abstract class HierarchyNode
  {
    #region Constant values

    private const int NoImageIndex = -1;

    #endregion

    #region Private Fields

    private HierarchyNode _ParentNode;
    private HierarchyNode _NextSibling;
    private HierarchyNode _FirstChild;
    private HierarchyNode _LastChild;
    private EventHandler<HierarchyNodeEventArgs> _OnChildAdded;
    private EventHandler<HierarchyNodeEventArgs> _OnChildRemoved;
    private IHierarchyManager _NestedHierarchy;
    private string _DefaultCaption;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyNode"/> class.
    /// </summary>
    /// <remarks>
    /// Sets up the default values and parses decorating attributes.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected HierarchyNode(IHierarchyManager manager)
    {
      if (manager == null) throw new ArgumentNullException();
      ManagerNode = manager;
      IsExpanded = true;
      SortPriority = -1;
      DefaultImageIndex = NoImageIndex;

      // --- Check for the SortOrder attribute
      var spAttr = GetType().GetAttribute<SortPriorityAttribute>(true);
      if (spAttr != null) SortPriority = spAttr.Value;

      // --- Check for the UseInnerHierarchyImages attribute
      var uihAttr = GetType().GetAttribute<UseInnerHierarchyImagesAttribute>(true);
      UseInnerHierarchyImages = uihAttr != null;

      // --- Check for the UseInnerHierarchyCaption attribute
      var uicAttr = GetType().GetAttribute<UseInnerHierarchyCaptionAttribute>(true);
      UseInnerHierarchyCaption = uicAttr != null;
      HierarchyId = ManagerNode.AddSubordinate(this);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyNode"/> class.
    /// </summary>
    /// <param name="manager">The manager object responsible for this node.</param>
    /// <param name="caption">The caption of the node.</param>
    // --------------------------------------------------------------------------------------------
    protected HierarchyNode(IHierarchyManager manager, string caption)
      : this(manager)
    {
      _DefaultCaption = caption;
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
    public HierarchyNode ParentNode
    {
      get { return _ParentNode; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the manager node.
    /// </summary>
    /// <value>The manager node.</value>
    // --------------------------------------------------------------------------------------------
    public IHierarchyManager ManagerNode { get; protected internal set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the next sibling.
    /// </summary>
    /// <value>The next sibling.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public HierarchyNode NextSibling
    {
      get { return _NextSibling; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the previous sibling.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [BrowsableAttribute(false)]
    public HierarchyNode PreviousSibling
    {
      get
      {
        if (_ParentNode == null) return null;
        HierarchyNode prev = null;
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
    public HierarchyNode FirstChild
    {
      get { return _FirstChild; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the last child.
    /// </summary>
    /// <value>The last child.</value>
    // --------------------------------------------------------------------------------------------
    public HierarchyNode LastChild
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
    /// This value is used when a new child node is added to the hierarchy. If this value is less
    /// than zero, we do not sort child nodes, we always add them to the parent node in their 
    /// physical order.
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

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the shortcut to a nested hierarchy in this node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IHierarchyManager NestedHierarchy
    {
      get { return _NestedHierarchy; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Signs if the node should use the images of the nested hierarchy if the node has a shortcut 
    /// to another hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool UseInnerHierarchyImages { get; protected set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Signs if the node should use the caption of the nested hierarchy if the node has a shortcut 
    /// to another hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool UseInnerHierarchyCaption { get; protected set; }

    #endregion

    #region Virtual and abstract methods 

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the caption of the node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual string Caption
    {
      get { return _DefaultCaption; }
      set { _DefaultCaption = value; }
    }

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
      switch ((__VSHPROPID) propId)
      {
        // --- A node is expandable only if it has a child or it has a nested hierarchy with a
        // --- non-empty root node (a root node having at least one child)
        case __VSHPROPID.VSHPROPID_Expandable:
          result =
            (_FirstChild != null) ||
            (_FirstChild == null && _NestedHierarchy != null && _NestedHierarchy.HierarchyRoot.FirstChild != null);
          break;

        // --- Gets the caption of the node. This property is used as the name of the node as well 
        // --- as the name used for saving the document behind. We can declare by the UseInnerHierarchyCaption
        // --- flag to use the caption of the nested hierarchy's root node, if this node has a shortcut
        // --- to a nested hierarchy at all.
        case __VSHPROPID.VSHPROPID_Caption:
        case __VSHPROPID.VSHPROPID_Name:
        case __VSHPROPID.VSHPROPID_SaveName:
          result = UseInnerHierarchyCaption && _NestedHierarchy != null
                     ? (_NestedHierarchy.HierarchyRoot == null
                          ? Caption
                          : _NestedHierarchy.HierarchyRoot.Caption)
                     : Caption;
          break;

        // --- Sort priority used in the UIHierarchyWindow where this hierarchy is hosted.
        case __VSHPROPID.VSHPROPID_SortPriority:
          result = SortPriority;
          break;

        // --- This property tells if the node is expanded by default or not.
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
          break;

        case __VSHPROPID.VSHPROPID_IconHandle:
          result = ManagerNode.ImageListHandle;
          break;

        case __VSHPROPID.VSHPROPID_OpenFolderIconHandle:
          result = ManagerNode.ImageListHandle;
          break;

        case __VSHPROPID.VSHPROPID_NextVisibleSibling:
        case __VSHPROPID.VSHPROPID_NextSibling:
          result = (int) ((_NextSibling != null) ? _NextSibling.HierarchyId : VSConstants.VSITEMID_NIL);
          break;

        case __VSHPROPID.VSHPROPID_FirstChild:
        case __VSHPROPID.VSHPROPID_FirstVisibleChild:
          result = (int) ((_FirstChild != null) ? _FirstChild.HierarchyId : VSConstants.VSITEMID_NIL);
          break;

        case __VSHPROPID.VSHPROPID_Parent:
          if (_ParentNode == null)
          {
            unchecked
            {
              result = new IntPtr((int) VSConstants.VSITEMID_NIL);
            }
          }
          else
          {
            result = new IntPtr((int) _ParentNode.HierarchyId);
          }
          break;

        case __VSHPROPID.VSHPROPID_ParentHierarchyItemid:
          if (ManagerNode.ParentHierarchy != null)
          {
            result = (IntPtr) ((uint) ManagerNode.IdInParentHierarchy);
          }
          break;

        case __VSHPROPID.VSHPROPID_ParentHierarchy:
          result = ManagerNode.ParentHierarchy;
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
      var id2 = (__VSHPROPID2) propId;
      switch (id2)
      {
        case __VSHPROPID2.VSHPROPID_NoDefaultNestedHierSorting:
          result = false;
          break;
        // We are doing the sorting ourselves through VSHPROPID_FirstChild and VSHPROPID_NextSibling
        case __VSHPROPID2.VSHPROPID_UseInnerHierarchyIconList:
          result = UseInnerHierarchyImages;
          break;
      }
      return result;
    }

    public virtual int GetGuidProperty(int propId, out Guid guid)
    {
      guid = Guid.Empty;
      //if (propid == (int)__VSHPROPID.VSHPROPID_TypeGuid)
      //{
      //  guid = this.ItemTypeGuid;
      //}

      var propName = Enum.GetName(typeof(__VSHPROPID), propId);
      if (String.IsNullOrEmpty(propName))
        propName = Enum.GetName(typeof(__VSHPROPID2), propId);
      if (String.IsNullOrEmpty(propName))
        propName = Enum.GetName(typeof(__VSHPROPID3), propId);
      Console.WriteLine("{0}:{1} = {2}", Caption, propName, guid);

      if (guid.CompareTo(Guid.Empty) == 0)
      {
        return VSConstants.DISP_E_MEMBERNOTFOUND;
      }
      return VSConstants.S_OK;
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

        case __VSHPROPID.VSHPROPID_ParentHierarchy:
          //parentHierarchy = (IVsHierarchy)value;
          break;

        case __VSHPROPID.VSHPROPID_ParentHierarchyItemid:
          //parentHierarchyItemId = (int)value;
          break;

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
    public virtual void AddChild(HierarchyNode node)
    {
      if (node == null)
      {
        throw new ArgumentNullException("node");
      }

      // --- Make sure the node is in the map.
      var nodeWithSameID = ManagerNode[node.HierarchyId];
      if (!ReferenceEquals(node, nodeWithSameID))
      {
        if (nodeWithSameID == null && (int)node.HierarchyId <= ManagerNode.ItemCount)
        { 
          ManagerNode.SetNodeAtId(node.HierarchyId, this);
        }
        else
        {
          throw new InvalidOperationException();
        }
      }

      HierarchyNode previous = null;
      if (SortPriority >= 0)
      {
        foreach (var n in Children)
        {
          if (ManagerNode.CompareNodes(node, n) > 0) break;
          previous = n;
        }
      }
      else
      {
        previous = _LastChild;
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
    protected void OnItemAdded(HierarchyNode parent, HierarchyNode child)
    {
      if (null != parent._OnChildAdded)
      {
        var args = new HierarchyNodeEventArgs(child);
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
    public virtual void RemoveChild(HierarchyNode node)
    {
      if (node == null)
        throw new ArgumentNullException("node");

      ManagerNode.RemoveNode(node);
      HierarchyNode last = null;
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

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Nests the specified hierarchy into this hierarchy node.
    /// </summary>
    /// <param name="nestedHierachy">The nested hierachy.</param>
    // --------------------------------------------------------------------------------------------
    public void NestHierarchy(IHierarchyManager nestedHierachy)
    {
      if (nestedHierachy == null) return;
      _NestedHierarchy = nestedHierachy;
      _NestedHierarchy.EnsureHierarchyRoot();
      _NestedHierarchy.SetParentHierarchy(ManagerNode, HierarchyId);
    }

    #endregion

    #region Iterators

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the enumerable list of node items.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IEnumerable<HierarchyNode> Children
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