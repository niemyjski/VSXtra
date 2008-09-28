// ================================================================================================
// HierarchyBase.cs
//
// Created: 2008.09.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider=Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra
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
  public abstract class HierarchyBase<THier, TRoot> : 
    IHierarchyBehavior
    where THier : HierarchyBase<THier, TRoot>
    where TRoot : HierarchyRoot<TRoot, THier>
  {
    #region Private Fields

    /// <summary>
    /// Has the object been disposed.
    /// </summary>
    /// <devremark>We will not specify a property for isDisposed, rather it is expected that the a private flag is defined
    /// on all subclasses. We do not want get in a situation where the base class's dipose is not called because a child sets the flag through the property.</devremark>
    private bool _IsDisposed;

    /// <summary>Stores the map for event sink subscribers</summary>
    private readonly ItemMap<IVsHierarchyEvents> _EventSinks =
      new ItemMap<IVsHierarchyEvents>();

    private readonly TRoot _ManagerNode;

    private readonly SimpleOleServiceProvider _OleServiceProvider =
      new SimpleOleServiceProvider();

    private IVsHierarchy _ParentHierarchy;
    private int _ParentHierarchyItemId;

    private EventHandler<HierarchyNodeEventArgs> _OnChildAdded;
    private EventHandler<HierarchyNodeCancelEventArgs> _OnChildAdding;
    private EventHandler<HierarchyNodeEventArgs> _OnChildRemoved;
    private EventHandler<HierarchyNodeCancelEventArgs> _OnChildRemoving;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an empty hierarchy node
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected HierarchyBase()
    {
      IsExpanded = true;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an empty hierachy node under the management of the specified root node.
    /// </summary>
    /// <param name="root">Root node managing this hierarchy node.</param>
    // --------------------------------------------------------------------------------------------
    protected HierarchyBase(TRoot root)
    {
      _ManagerNode = root;
      Id = _ManagerNode.ManagedItems.Add(this);
      _OleServiceProvider.AddService(typeof (IVsHierarchy), root, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; 
    /// <c>false</c> to release only unmanaged resources.
    /// </param>
    // --------------------------------------------------------------------------------------------
    protected virtual void Dispose(bool disposing)
    {
      if (_IsDisposed) return;
      if (disposing)
      {
        if (_OleServiceProvider != null) _OleServiceProvider.Dispose();
      }
      _IsDisposed = true;
    }

    #endregion

    #region Public properties

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
    /// Gets the OLE service provider of this hierarchy item.
    /// </summary>
    /// <value>The OLE service provider.</value>
    // --------------------------------------------------------------------------------------------
    public SimpleOleServiceProvider OleServiceProvider
    {
      get { return _OleServiceProvider; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the manager node of this hierarchy item.
    /// </summary>
    /// <value>The manager node of this item.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public TRoot ManagerNode
    {
      get { return _ManagerNode; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the first child.
    /// </summary>
    /// <value>The first child.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public HierarchyBase<THier, TRoot> FirstChild { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the last child.
    /// </summary>
    /// <value>The last child.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public HierarchyBase<THier, TRoot> LastChild { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the next sibling.
    /// </summary>
    /// <value>The next sibling.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public HierarchyBase<THier, TRoot> NextSibling { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the previous sibling node.
    /// </summary>
    /// <value>The previous sibling node.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public HierarchyBase<THier, TRoot> PreviousSibling
    {
      get
      {
        if (ParentNode == null) return null;
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
    /// Gets or sets the parent node.
    /// </summary>
    /// <value>The parent node.</value>
    // --------------------------------------------------------------------------------------------
    [Browsable(false)]
    public HierarchyBase<THier, TRoot> ParentNode { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the hierarchy id.
    /// </summary>
    /// <value>The hierarchy id.</value>
    // --------------------------------------------------------------------------------------------
    public uint Id { get; set; }

    #endregion

    #region Event Properties

    internal event EventHandler<HierarchyNodeEventArgs> OnChildAdded
    {
      add { _OnChildAdded += value; }
      remove { _OnChildAdded -= value; }
    }

    internal event EventHandler<HierarchyNodeEventArgs> OnChildRemoved
    {
      add { _OnChildRemoved += value; }
      remove { _OnChildRemoved -= value; }
    }

    internal event EventHandler<HierarchyNodeCancelEventArgs> OnChildAdding
    {
      add { _OnChildAdding += value; }
      remove { _OnChildAdding -= value; }
    }

    internal event EventHandler<HierarchyNodeCancelEventArgs> OnChildRemoving
    {
      add { _OnChildRemoving += value; }
      remove { _OnChildRemoving -= value; }
    }

    #endregion

    #region Iterators

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the child nodes of this node from the first to the last.
    /// </summary>
    /// <value>The child nodes of this node from the first to the last.</value>
    // --------------------------------------------------------------------------------------------
    public IEnumerable<HierarchyBase<THier, TRoot>> Children
    {
      get
      {
        for (var child = ParentNode.FirstChild; child != null; child = child.NextSibling)
        {
          yield return child;
        }
      }
    }

    #endregion

    #region Virtual and Abstract Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the caption of this node.
    /// </summary>
    /// <value>The caption.</value>
    // --------------------------------------------------------------------------------------------
    public abstract string Caption
    {
      get;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the sort priority value.
    /// </summary>
    /// <value>The sort priority value.</value>
    /// <remarks>
    /// The less this value is, the smaller the sort order of this node is.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public virtual int SortPriority
    {
      get { return 0; }
    }

    #endregion

    #region Child management methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Adds a child node to this node. Inserts the node into the right location determined by the
    /// rank of the child node.
    /// </summary>
    /// <param name="node">The node to add.</param>
    // --------------------------------------------------------------------------------------------
    public virtual void AddChild(HierarchyBase<THier, TRoot> node)
    {
      if (node == null) throw new ArgumentNullException("node");

      // --- Make sure the node is in the map.
      Object nodeWithSameID = _ManagerNode.ManagedItems[node.Id];
      if (!ReferenceEquals(node, nodeWithSameID as HierarchyBase<THier, TRoot>))
      {
        if (nodeWithSameID == null && node.Id <= ManagerNode.ManagedItems.Count)
        { 
          // --- Reuse our hierarchy Id if possible.
          ManagerNode.ManagedItems.SetAt(node.Id, this);
        }
        else
        {
          throw new InvalidOperationException();
        }
      }

      // --- Excute the OnChildAdding event method
      if (_OnChildAdding != null)
      {
        var e = new HierarchyNodeCancelEventArgs(node);
        _OnChildAdding(this, e);
        if (e.Cancel) return;
      }

      // --- Determine the previous node
      HierarchyBase<THier, TRoot> previous = null;
      foreach (var n in Children)
      {
        if (ManagerNode.CompareNodes(node, n) > 0) break;
        previous = n;
      }

      // --- Insert "node" after "previous".

      if (previous != null)
      {
        node.NextSibling = previous.NextSibling;
        previous.NextSibling = node;
        if (previous == LastChild)
        {
          LastChild = node;
        }
      }
      else
      {
        if (LastChild == null)
        {
          LastChild = node;
        }
        node.NextSibling = FirstChild;
        FirstChild = node;
      }
      node.ParentNode = this;

      // --- Now child item is added, sign the event
      OnItemAdded(this, node);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called when an item added is added to this node.
    /// </summary>
    /// <param name="parent">The parent item.</param>
    /// <param name="child">The child item.</param>
    // --------------------------------------------------------------------------------------------
    protected void OnItemAdded(HierarchyBase<THier, TRoot> parent, HierarchyBase<THier, TRoot> child)
    {
      if (parent == null) throw new ArgumentNullException("parent");
      if (child == null) throw new ArgumentNullException("child");

      // --- Fires the OnChildAdded event of this node
      if (parent._OnChildAdded != null)
      {
        var args = new HierarchyNodeEventArgs(child);
        parent._OnChildAdded(parent, args);
      }

      // --- Triggers the events of the manager node if not disabled
      var foo = ManagerNode ?? this;
      if (foo == ManagerNode && ManagerNode.DoNotTriggerHierarchyEvents) return;

      var prev = child.PreviousSibling;
      uint prevId = (prev != null) ? prev.Id : VSConstants.VSITEMID_NIL;
      foreach (var sink in foo._EventSinks)
      {
        var result = sink.OnItemAdded(parent.Id, prevId, child.Id);
        if (ErrorHandler.Failed(result) && result != VSConstants.E_NOTIMPL)
        {
          ErrorHandler.ThrowOnFailure(result);
        }
      }
    }


    #endregion

    #region IVsHierarchy implementation

    int IVsHierarchy.GetSite(out IServiceProvider ppSP)
    {
      throw new NotImplementedException();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks, if the current hierarchy node can be closed or not.
    /// </summary>
    /// <param name="pfCanClose">Non-zero value if the node can be closed.</param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public virtual int QueryClose(out int pfCanClose)
    {
      pfCanClose = 1;
      return VSConstants.S_OK;
    }

    int IVsHierarchy.Close()
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.GetProperty(uint itemid, int propid, out object pvar)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.SetProperty(uint itemid, int propid, object var)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested,
                                        out IntPtr ppHierarchyNested, out uint pitemidNested)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.GetCanonicalName(uint itemid, out string pbstrName)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.UnadviseHierarchyEvents(uint dwCookie)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.SetSite(IServiceProvider psp)
    {
      return VSConstants.E_NOTIMPL;
    }

    #endregion

    #region IVsUIHierarchy implementation

    int IVsUIHierarchy.QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds,
                                          IntPtr pCmdText)
    {
      throw new NotImplementedException();
    }

    int IVsUIHierarchy.ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn,
                                   IntPtr pvaOut)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IVsUIHierarchy implementation (Calling IVsHierarchy implementations)

    int IVsUIHierarchy.GetSite(out IServiceProvider ppSP)
    {
      return ((IVsHierarchy) this).GetSite(out ppSP);
    }

    int IVsUIHierarchy.QueryClose(out int pfCanClose)
    {
      return ((IVsHierarchy) this).QueryClose(out pfCanClose);
    }

    int IVsUIHierarchy.Close()
    {
      return ((IVsHierarchy) this).Close();
    }

    int IVsUIHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
    {
      return ((IVsHierarchy) this).GetGuidProperty(itemid, propid, out pguid);
    }

    int IVsUIHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
    {
      return ((IVsHierarchy) this).SetGuidProperty(itemid, propid, ref rguid);
    }

    int IVsUIHierarchy.GetProperty(uint itemid, int propid, out object pvar)
    {
      return ((IVsHierarchy) this).GetProperty(itemid, propid, out pvar);
    }

    int IVsUIHierarchy.SetProperty(uint itemid, int propid, object var)
    {
      return ((IVsHierarchy) this).SetProperty(itemid, propid, var);
    }

    int IVsUIHierarchy.GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested,
                                          out uint pitemidNested)
    {
      return ((IVsHierarchy) this).GetNestedHierarchy(itemid, ref iidHierarchyNested, out ppHierarchyNested,
                                                      out pitemidNested);
    }

    int IVsUIHierarchy.GetCanonicalName(uint itemid, out string pbstrName)
    {
      return ((IVsHierarchy) this).GetCanonicalName(itemid, out pbstrName);
    }

    int IVsUIHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
    {
      return ((IVsHierarchy) this).ParseCanonicalName(pszName, out pitemid);
    }

    int IVsUIHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
    {
      return ((IVsHierarchy) this).AdviseHierarchyEvents(pEventSink, out pdwCookie);
    }

    int IVsUIHierarchy.UnadviseHierarchyEvents(uint dwCookie)
    {
      return ((IVsHierarchy) this).UnadviseHierarchyEvents(dwCookie);
    }

    int IVsUIHierarchy.SetSite(IServiceProvider psp)
    {
      return ((IVsHierarchy) this).SetSite(psp);
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
  }

  #endregion
}