using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider=Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra
{
  public abstract class UIHierarchyBase: HierarchyBase, IVsUIHierarchy
  {
    #region Implementation of IVsUIHierarchy

    int IVsUIHierarchy.SetSite(IServiceProvider psp)
    {
      return ((IVsHierarchy) this).SetSite(psp);
    }

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

    int IVsUIHierarchy.QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsUIHierarchy.ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
    {
      return VSConstants.E_NOTIMPL;
    }

    #endregion
  }
}