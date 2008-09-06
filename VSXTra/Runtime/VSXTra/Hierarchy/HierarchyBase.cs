// ================================================================================================
// HierarchyBase.cs
//
// Created: 2008.09.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider=Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to be the root class of all IVsHierarchy implementations.
  /// </summary>
  // ================================================================================================
  public abstract class HierarchyBase : IVsHierarchy
  {
    #region Implementation of IVsHierarchy

    int IVsHierarchy.SetSite(IServiceProvider psp)
    {
      return VSConstants.E_NOTIMPL;
    }

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

    int IVsHierarchy.Unused0()
    {
      return VSConstants.E_NOTIMPL;
    }

    int IVsHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
    {
      throw new NotImplementedException();
    }

    int IVsHierarchy.UnadviseHierarchyEvents(uint dwCookie)
    {
      throw new NotImplementedException();
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

    #endregion
  }
}