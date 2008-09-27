// ================================================================================================
// HelpService.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new Managed Package Framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created by: Istvan Novak (DiveDeeper), 05/05/2008
// ================================================================================================
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.VSHelp;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class provides a basic implementation of the IHelpService interface.
  /// </summary>
  // ================================================================================================
  internal sealed class HelpService :
    // Provides methods for showing Help topics and adding and removing Help keywords at design time.
    IHelpService,
    // Defines a method to release allocated resources.
    IDisposable
  {
    #region Private fields

    /// <summary>Context used to manage attributes and keywords</summary>
    private IVsUserContext _Context;

    /// <summary>Cookie resulted from IVsUserContext.AddSubcontext</summary>
    private uint _Cookie;

    private bool _NeedsRecreate;
    private HelpService _ParentService;
    private readonly HelpContextType _Priority;
    private IServiceProvider _ServiceProvider;
    private ArrayList _SubContextList;

    #endregion

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpService"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider to access shell services.</param>
    // --------------------------------------------------------------------------------------------
    internal HelpService(IServiceProvider serviceProvider)
    {
      _ServiceProvider = serviceProvider;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpService"/> class.
    /// </summary>
    /// <param name="parentService">The parent service.</param>
    /// <param name="subContext">The sub context.</param>
    /// <param name="cookie">The cookie.</param>
    /// <param name="provider">The provider.</param>
    /// <param name="priority">The priority.</param>
    // --------------------------------------------------------------------------------------------
    private HelpService(HelpService parentService, IVsUserContext subContext, uint cookie,
                        IServiceProvider provider, HelpContextType priority)
    {
      _Context = subContext;
      _ServiceProvider = provider;
      _Cookie = cookie;
      _ParentService = parentService;
      _Priority = priority;
    }

    private IHelpService CreateLocalContext(HelpContextType contextType, bool recreate, out IVsUserContext localContext,
                                            out uint cookie)
    {
      cookie = 0;
      localContext = null;
      if (_ServiceProvider != null)
      {
        localContext = null;
        int hr = 0;
        IVsMonitorUserContext context =
          (IVsMonitorUserContext) _ServiceProvider.GetService(typeof (IVsMonitorUserContext));
        if (context != null)
        {
          try
          {
            hr = context.CreateEmptyContext(out localContext);
          }
          catch (COMException exception)
          {
            hr = exception.ErrorCode;
          }
        }
        if (NativeMethods.Succeeded(hr) && (localContext != null))
        {
          VSUSERCONTEXTPRIORITY vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_None;
          switch (contextType)
          {
            case HelpContextType.Ambient:
              vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Ambient;
              break;

            case HelpContextType.Window:
              vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Window;
              break;

            case HelpContextType.Selection:
              vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Selection;
              break;

            case HelpContextType.ToolWindowSelection:
              vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_ToolWndSel;
              break;
          }
          IVsUserContext userContext = GetUserContext();
          if (userContext != null)
          {
            try
            {
              hr = userContext.AddSubcontext(localContext, (int) vsusercontextpriority, out cookie);
            }
            catch (COMException exception2)
            {
              hr = exception2.ErrorCode;
            }
          }
          if ((NativeMethods.Succeeded(hr) && (cookie != 0)) && !recreate)
          {
            HelpService service = new HelpService(this, localContext, cookie, _ServiceProvider, contextType);
            if (_SubContextList == null)
            {
              _SubContextList = new ArrayList();
            }
            _SubContextList.Add(service);
            return service;
          }
        }
      }
      return null;
    }

    private IVsUserContext GetUserContext()
    {
      RecreateContext();
      if (_Context == null)
      {
        if (_ServiceProvider == null)
        {
          return null;
        }
        IVsWindowFrame frame = (IVsWindowFrame) _ServiceProvider.GetService(typeof (IVsWindowFrame));
        if (frame != null)
        {
          object obj2;
          NativeMethods.ThrowOnFailure(frame.GetProperty(-3010, out obj2));
          _Context = (IVsUserContext) obj2;
        }
        if (_Context == null)
        {
          IVsMonitorUserContext context =
            (IVsMonitorUserContext) _ServiceProvider.GetService(typeof (IVsMonitorUserContext));
          if (context != null)
          {
            NativeMethods.ThrowOnFailure(context.CreateEmptyContext(out _Context));
            if (((_Context != null) && (frame != null)) && IsToolWindow(frame))
            {
              NativeMethods.ThrowOnFailure(frame.SetProperty(-3010, _Context));
            }
          }
        }
        if ((_SubContextList != null) && (_Context != null))
        {
          foreach (object obj3 in _SubContextList)
          {
            HelpService service = obj3 as HelpService;
            if (service != null)
            {
              service.RecreateContext();
            }
          }
        }
      }
      return _Context;
    }

    private bool IsToolWindow(IVsWindowFrame frame)
    {
      object obj2;
      int num = 0;
      NativeMethods.ThrowOnFailure(frame.GetProperty(-3000, out obj2));
      if (obj2 is int)
      {
        num = (int) obj2;
      }
      return (num == 2);
    }

    private void NotifyContextChange(IVsUserContext cxt)
    {
      if ((_ServiceProvider != null) && (_ParentService == null))
      {
        IVsUserContext ppContext = null;
        IVsMonitorUserContext service =
          (IVsMonitorUserContext) _ServiceProvider.GetService(typeof (IVsMonitorUserContext));
        if (service != null)
        {
          NativeMethods.ThrowOnFailure(service.get_ApplicationContext(out ppContext));
        }
        if (ppContext != cxt)
        {
          IVsWindowFrame frame = (IVsWindowFrame) _ServiceProvider.GetService(typeof (IVsWindowFrame));
          if ((frame != null) && !IsToolWindow(frame))
          {
            IVsTrackSelectionEx ex = (IVsTrackSelectionEx) _ServiceProvider.GetService(typeof (IVsTrackSelectionEx));
            if (ex != null)
            {
              object varValue = cxt;
              NativeMethods.ThrowOnFailure(ex.OnElementValueChange(5, 0, varValue));
            }
          }
        }
      }
    }

    private void RecreateContext()
    {
      if ((_ParentService != null) && _NeedsRecreate)
      {
        _NeedsRecreate = false;
        if (_Context == null)
        {
          _ParentService.CreateLocalContext(_Priority, true, out _Context, out _Cookie);
        }
        else
        {
          VSUSERCONTEXTPRIORITY vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_None;
          switch (_Priority)
          {
            case HelpContextType.Ambient:
              vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Ambient;
              break;

            case HelpContextType.Window:
              vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Window;
              break;

            case HelpContextType.Selection:
              vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Selection;
              break;

            case HelpContextType.ToolWindowSelection:
              vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_ToolWndSel;
              break;
          }
          IVsUserContext userContext = _ParentService.GetUserContext();
          IVsUserContext pSubCtx = GetUserContext();
          if ((pSubCtx != null) && (userContext != null))
          {
            NativeMethods.ThrowOnFailure(userContext.AddSubcontext(pSubCtx, (int) vsusercontextpriority, out _Cookie));
          }
        }
      }
    }

    void IHelpService.AddContextAttribute(string name, string value, HelpKeywordType keywordType)
    {
      if (_ServiceProvider != null)
      {
        IVsUserContext userContext = GetUserContext();
        if (userContext != null)
        {
          VSUSERCONTEXTATTRIBUTEUSAGE usage = VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1;
          switch (keywordType)
          {
            case HelpKeywordType.F1Keyword:
              usage = VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1;
              break;

            case HelpKeywordType.GeneralKeyword:
              usage = VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Lookup;
              break;

            case HelpKeywordType.FilterKeyword:
              usage = VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter;
              break;
          }
          NativeMethods.ThrowOnFailure(userContext.AddAttribute(usage, name, value));
          NotifyContextChange(userContext);
        }
      }
    }

    void IHelpService.ClearContextAttributes()
    {
      if (_Context != null)
      {
        NativeMethods.ThrowOnFailure(_Context.RemoveAttribute(null, null));
        if (_SubContextList != null)
        {
          foreach (object obj2 in _SubContextList)
          {
            IHelpService service = obj2 as IHelpService;
            if (service != null)
            {
              service.ClearContextAttributes();
            }
          }
        }
      }
      NotifyContextChange(_Context);
    }

    IHelpService IHelpService.CreateLocalContext(HelpContextType contextType)
    {
      IVsUserContext localContext = null;
      uint cookie = 0;
      return CreateLocalContext(contextType, false, out localContext, out cookie);
    }

    void IHelpService.RemoveContextAttribute(string name, string value)
    {
      if (_ServiceProvider != null)
      {
        IVsUserContext userContext = GetUserContext();
        if (userContext != null)
        {
          NativeMethods.ThrowOnFailure(userContext.RemoveAttribute(name, value));
          NotifyContextChange(userContext);
        }
      }
    }

    void IHelpService.RemoveLocalContext(IHelpService localContext)
    {
      if ((_SubContextList != null) && (_SubContextList.IndexOf(localContext) != -1))
      {
        _SubContextList.Remove(localContext);
        if (_Context != null)
        {
          NativeMethods.ThrowOnFailure(_Context.RemoveSubcontext(((HelpService) localContext)._Cookie));
        }
        ((HelpService) localContext)._ParentService = null;
      }
    }

    void IHelpService.ShowHelpFromKeyword(string helpKeyword)
    {
      if ((_ServiceProvider != null) && (helpKeyword != null))
      {
        Help service = (Help) _ServiceProvider.GetService(typeof (Help));
        if (service != null)
        {
          try
          {
            service.DisplayTopicFromF1Keyword(helpKeyword);
          }
          catch
          {
          }
        }
      }
    }

    void IHelpService.ShowHelpFromUrl(string helpUrl)
    {
      if ((_ServiceProvider != null) && (helpUrl != null))
      {
        Help service = (Help) _ServiceProvider.GetService(typeof (Help));
        if (service != null)
        {
          try
          {
            service.DisplayTopicFromURL(helpUrl);
          }
          catch
          {
          }
        }
      }
    }

    void IDisposable.Dispose()
    {
      if ((_SubContextList != null) && (_SubContextList.Count > 0))
      {
        foreach (HelpService service in _SubContextList)
        {
          service._ParentService = null;
          if (_Context != null)
          {
            try
            {
              _Context.RemoveSubcontext(service._Cookie);
            }
            catch
            {
            }
          }
          ((IDisposable) service).Dispose();
        }
        _SubContextList = null;
      }
      if (_ParentService != null)
      {
        IHelpService parentService = _ParentService;
        _ParentService = null;
        parentService.RemoveLocalContext(this);
      }
      if (_ServiceProvider != null)
      {
        _ServiceProvider = null;
      }
      if (_Context != null)
      {
        Marshal.ReleaseComObject(_Context);
        _Context = null;
      }
      _Cookie = 0;
    }
  }
}