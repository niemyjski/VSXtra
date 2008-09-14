// ================================================================================================
// HierarchyBase.cs
//
// Created: 2008.09.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider=Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra
{
  #region IHierarchyBahavior

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
  public abstract class HierarchyBase<THier, TRoot> : IHierarchyBehavior
    where THier : HierarchyBase<THier, TRoot>
    where TRoot : HierarchyRoot<TRoot, THier>
  {
    #region Private Fields

    /// <summary>Stores the map for event sink subscribers</summary>
    private readonly ItemMap<IVsHierarchyEvents> _EventSinks =
      new ItemMap<IVsHierarchyEvents>();

    private readonly TRoot _ManagerNode;
    private HierarchyBase<THier, TRoot> _ParentNode;
    private HierarchyBase<THier, TRoot> _NextSibling;
    private HierarchyBase<THier, TRoot> _FirstChild;
    private HierarchyBase<THier, TRoot> _LastChild;
    private uint _HierarchyId;
    private SimpleOleServiceProvider _OleServiceProvider = new SimpleOleServiceProvider();
    private IVsHierarchy _ParentHierarchy;
    private int _ParentHierarchyItemId;
    private EventHandler<HierarchyNodeEventArgs> onChildAdded;
    private EventHandler<HierarchyNodeEventArgs> onChildRemoved;

    /// <summary>
    /// Has the object been disposed.
    /// </summary>
    /// <devremark>We will not specify a property for isDisposed, rather it is expected that the a private flag is defined
    /// on all subclasses. We do not want get in a situation where the base class's dipose is not called because a child sets the flag through the property.</devremark>
    private bool isDisposed;

    #endregion

    #region Lifecycle methods

    protected HierarchyBase()
    {
      IsExpanded = true;
    }

    protected HierarchyBase(TRoot root)
    {
      _ManagerNode = root;
      _HierarchyId = _ManagerNode.ManagedItems.Add(this);
      _OleServiceProvider.AddService(typeof(IVsHierarchy), root, false);
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

    #endregion

    #region IVsHierarchy implementation

    int IVsHierarchy.GetSite(out IServiceProvider ppSP)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.QueryClose(out int pfCanClose)
    {
      throw new NotImplementedException();
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

    int IVsUIHierarchy.QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      throw new System.NotImplementedException();
    }

    int IVsUIHierarchy.ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      throw new System.NotImplementedException();
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
  }

  #endregion
}