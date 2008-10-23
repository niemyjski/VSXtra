// ================================================================================================
// SolutionEvents.cs
//
// Created: 2008.08.03, by Istvan Novak (DeepDiver)
// ================================================================================================

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Package;

namespace VSXtra.Shell
{
  // ================================================================================================
  /// <summary>
  /// This class provides access to solution events.
  /// </summary>
  // ================================================================================================
  public static class SolutionEvents
  {
    #region Private fields

    private static readonly IVsSolution _Solution;
    private static readonly SolutionEventHooker _EventHook;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the event listener.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    static SolutionEvents()
    {
      _Solution = PackageBase.GetGlobalService<SVsSolution, IVsSolution>();
      if (_Solution == null)
      {
        throw new NotSupportedException(typeof(SVsSolution).FullName);
      }
      _EventHook = new SolutionEventHooker();
    }

    #endregion

    #region Public event properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raised before the solution's subprojects are opened.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionNodeEventArgs> OnBeforeOpeningChildren
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.BeforeOpeningChildren += value;
      }
      remove
      {
        _EventHook.Events.BeforeOpeningChildren -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raised after opening all nested projects.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionNodeEventArgs> OnAfterOpeningChildren
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterOpeningChildren += value;
      }
      remove
      {
        _EventHook.Events.AfterOpeningChildren -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raised before the solution's subprojects are closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionNodeEventArgs> OnBeforeClosingChildren
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.BeforeClosingChildren += value;
      }
      remove
      {
        _EventHook.Events.BeforeClosingChildren -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raised after closing all the nested projects owned by a parent hierarchy.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionNodeEventArgs> OnAfterClosingChildren
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterClosingChildren += value;
      }
      remove
      {
        _EventHook.Events.AfterClosingChildren -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raised to drop and re-add the renamed project.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionNodeEventArgs> OnAfterRenameProject
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterRenameProject += value;
      }
      remove
      {
        _EventHook.Events.AfterRenameProject -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raised after a project has been moved.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionNodeEventArgs> OnAfterChangeProjectParent
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterChangeProjectParent += value;
      }
      remove
      {
        _EventHook.Events.AfterChangeProjectParent -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries listening clients as to whether the project can be unloaded.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionNodeCancelEventArgs> OnQueryUnloadProject
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.QueryUnloadProject += value;
      }
      remove
      {
        _EventHook.Events.QueryUnloadProject -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Queries listening clients as to whether the project can be closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<CloseProjectEventArgs> OnQueryCloseProject
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.QueryCloseProject += value;
      }
      remove
      {
        _EventHook.Events.QueryCloseProject -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies listening clients that the project is about to be closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<CloseProjectEventArgs> OnBeforeCloseProject
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.BeforeCloseProject += value;
      }
      remove
      {
        _EventHook.Events.BeforeCloseProject -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies listening clients that the project has opened.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<OpenProjectEventArgs> OnAfterOpenProject
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterOpenProject += value;
      }
      remove
      {
        _EventHook.Events.AfterOpenProject -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raised after a project has been opened asynchronously.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<OpenProjectEventArgs> OnAfterAsynchOpenProject
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterAsynchOpenProject += value;
      }
      remove
      {
        _EventHook.Events.AfterAsynchOpenProject -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies listening clients that the project has loaded.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<LoadProjectEventArgs> OnAfterLoadProject
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterLoadProject += value;
      }
      remove
      {
        _EventHook.Events.AfterLoadProject -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies listening clients that the project is about to be unloaded.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<LoadProjectEventArgs> BeforeUnloadProject
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.BeforeUnloadProject += value;
      }
      remove
      {
        _EventHook.Events.BeforeUnloadProject -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies listening clients that the solution is about to be closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionEventArgs> OnBeforeCloseSolution
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.BeforeCloseSolution += value;
      }
      remove
      {
        _EventHook.Events.BeforeCloseSolution -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies listening clients that the solution has closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionEventArgs> OnAfterCloseSolution
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterCloseSolution += value;
      }
      remove
      {
        _EventHook.Events.AfterCloseSolution -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raised after all projects have been merged into the open solution.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<SolutionEventArgs> OnAfterMergeSolution
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterMergeSolution += value;
      }
      remove
      {
        _EventHook.Events.AfterMergeSolution -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies listening clients that the solution has been opened.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<OpenSolutionEventArgs> OnAfterOpenSolution
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.AfterOpenSolution += value;
      }
      remove
      {
        _EventHook.Events.AfterOpenSolution -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Notifies listening clients that the solution has closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<CloseSolutionEventArgs> OnQueryCloseSolution
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.QueryCloseSolution += value;
      }
      remove
      {
        _EventHook.Events.QueryCloseSolution -= value;
        EnsureUnadvise();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Raised to ask listening clients whether a project can be moved from one parent to another 
    /// in the solution explorer.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static event EventHandler<ChangeProjectParentEventArgs> QueryChangeProjectParent
    {
      add
      {
        EnsureAdvise();
        _EventHook.Events.QueryChangeProjectParent += value;
      }
      remove
      {
        _EventHook.Events.QueryChangeProjectParent -= value;
        EnsureUnadvise();
      }
    }


    #endregion

    #region SolutionEventHandler class

    // ================================================================================================
    /// <summary>
    /// This class provides events for the IVsSolutionEvents3 event interface.
    /// </summary>
    // ================================================================================================
    private sealed class SolutionEventHandler:
      IVsSolutionEvents3,
      IVsSolutionEvents4
    {
      #region Public events

      public EventHandler<SolutionNodeEventArgs> BeforeOpeningChildren;
      public EventHandler<SolutionNodeEventArgs> AfterOpeningChildren;
      public EventHandler<SolutionNodeEventArgs> BeforeClosingChildren;
      public EventHandler<SolutionNodeEventArgs> AfterClosingChildren;
      public EventHandler<SolutionNodeEventArgs> AfterRenameProject;
      public EventHandler<SolutionNodeEventArgs> AfterChangeProjectParent;
      public EventHandler<SolutionNodeCancelEventArgs> QueryUnloadProject;
      public EventHandler<CloseProjectEventArgs> QueryCloseProject;
      public EventHandler<CloseProjectEventArgs> BeforeCloseProject;
      public EventHandler<OpenProjectEventArgs> AfterOpenProject;
      public EventHandler<OpenProjectEventArgs> AfterAsynchOpenProject;
      public EventHandler<LoadProjectEventArgs> AfterLoadProject;
      public EventHandler<LoadProjectEventArgs> BeforeUnloadProject;
      public EventHandler<SolutionEventArgs> BeforeCloseSolution;
      public EventHandler<SolutionEventArgs> AfterCloseSolution;
      public EventHandler<SolutionEventArgs> AfterMergeSolution;
      public EventHandler<OpenSolutionEventArgs> AfterOpenSolution;
      public EventHandler<CloseSolutionEventArgs> QueryCloseSolution;
      public EventHandler<ChangeProjectParentEventArgs> QueryChangeProjectParent;

      #endregion

      #region IVsSolutionEvents implementation

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Notifies listening clients that the project has opened.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
      {
        if (AfterOpenProject != null)
        {
          var e = new OpenProjectEventArgs(SolutionEventType.AfterOpenProject)
                    {
                      Hierarchy = pHierarchy,
                      Added = fAdded != 0
                    };
          AfterOpenProject(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Queries listening clients as to whether the project can be closed.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
      {
        if (QueryCloseProject != null)
        {
          var e = new CloseProjectEventArgs(SolutionEventType.QueryCloseProject)
                    {
                      Hierarchy = pHierarchy,
                      Cancel = pfCancel != 0
                    };
          QueryCloseProject(this, e);
          pfCancel = e.Cancel ? 1 : 0;
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Notifies listening clients that the project is about to be closed.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
      {
        if (QueryCloseProject != null)
        {
          var e = new CloseProjectEventArgs(SolutionEventType.BeforeCloseProject)
                    {
                      Hierarchy = pHierarchy,
                      Cancel = false
                    };
          QueryCloseProject(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Notifies listening clients that the project has loaded.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
      {
        if (AfterLoadProject != null)
        {
          var e = new LoadProjectEventArgs(SolutionEventType.AfterLoadProject)
                    {
                      Hierarchy = pRealHierarchy,
                      PlaceHolder = pStubHierarchy
                    };
          AfterLoadProject(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Queries listening clients as to whether the project can be unloaded.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
      {
        if (QueryUnloadProject != null)
        {
          var e = new SolutionNodeCancelEventArgs(SolutionEventType.QueryUnloadProject)
                    {
                      Hierarchy = pRealHierarchy,
                      Cancel = pfCancel != 0
                    };
          QueryUnloadProject(this, e);
          pfCancel = e.Cancel ? 1 : 0;
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Notifies listening clients that the project is about to be unloaded.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
      {
        if (BeforeUnloadProject != null)
        {
          var e = new LoadProjectEventArgs(SolutionEventType.BeforeUnloadProject)
                    {
                      Hierarchy = pRealHierarchy,
                      PlaceHolder = pStubHierarchy
                    };
          BeforeUnloadProject(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Notifies listening clients that the solution has been opened.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
      {
        if (AfterOpenSolution != null)
        {
          var e = new OpenSolutionEventArgs(SolutionEventType.AfterOpenSolution)
                    {
                      NewSolution = fNewSolution != 0
                    };
          AfterOpenSolution(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Notifies listening clients that the solution has closed.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
      {
        if (QueryCloseSolution != null)
        {
          var e = new CloseSolutionEventArgs(SolutionEventType.QueryCloseSolution)
                    {
                      Cancel = pfCancel != 0
                    };
          QueryCloseSolution(this, e);
          pfCancel = e.Cancel ? 1 : 0;
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Notifies listening clients that the solution is about to be closed.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnBeforeCloseSolution(object pUnkReserved)
      {
        if (BeforeCloseSolution != null)
        {
          BeforeCloseSolution(this, new SolutionEventArgs(SolutionEventType.BeforeCloseSolution));
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Notifies listening clients that the solution has closed.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterCloseSolution(object pUnkReserved)
      {
        if (AfterCloseSolution != null)
        {
          AfterCloseSolution(this, new SolutionEventArgs(SolutionEventType.AfterCloseSolution));
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Raised after all projects have been merged into the open solution.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterMergeSolution(object pUnkReserved)
      {
        if (AfterMergeSolution != null)
        {
          AfterMergeSolution(this, new SolutionEventArgs(SolutionEventType.AfterMergeSolution));
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Raised before the solution's subprojects are opened.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy)
      {
        if (BeforeOpeningChildren != null)
        {
          var e = new SolutionNodeEventArgs(SolutionEventType.BeforeOpeningChildren)
                    {
                      Hierarchy = pHierarchy
                    };
          BeforeOpeningChildren(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Raised after opening all nested projects.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterOpeningChildren(IVsHierarchy pHierarchy)
      {
        if (AfterOpeningChildren != null)
        {
          var e = new SolutionNodeEventArgs(SolutionEventType.AfterOpeningChildren)
                    {
                      Hierarchy = pHierarchy
                    };
          AfterOpeningChildren(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Raised before the solution's subprojects are closed.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnBeforeClosingChildren(IVsHierarchy pHierarchy)
      {
        if (BeforeClosingChildren != null)
        {
          var e = new SolutionNodeEventArgs(SolutionEventType.BeforeClosingChildren)
                    {
                      Hierarchy = pHierarchy
                    };
          BeforeClosingChildren(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Raised after closing all the nested projects owned by a parent hierarchy.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterClosingChildren(IVsHierarchy pHierarchy)
      {
        if (AfterClosingChildren != null)
        {
          var e = new SolutionNodeEventArgs(SolutionEventType.AfterClosingChildren)
                    {
                      Hierarchy = pHierarchy
                    };
          AfterClosingChildren(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Raised to drop and re-add the renamed project.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterRenameProject(IVsHierarchy pHierarchy)
      {
        if (AfterRenameProject != null)
        {
          var e = new SolutionNodeEventArgs(SolutionEventType.AfterRenameProject)
                    {
                      Hierarchy = pHierarchy
                    };
          AfterRenameProject(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Raised to ask listening clients whether a project can be moved from one parent to another 
      /// in the solution explorer.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, 
                                            ref int pfCancel)
      {
        if (QueryChangeProjectParent != null)
        {
          var e = new ChangeProjectParentEventArgs(SolutionEventType.QueryChangeProjectParent)
                    {
                      NewHierarchy = pNewParentHier,
                      Cancel = pfCancel != 0
                    };
          QueryChangeProjectParent(this, e);
          pfCancel = e.Cancel ? 1 : 0;
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Raised after a project has been moved.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
      {
        if (AfterChangeProjectParent != null)
        {
          var e = new SolutionNodeEventArgs(SolutionEventType.AfterChangeProjectParent)
                    {
                      Hierarchy = pHierarchy
                    };
          AfterChangeProjectParent(this, e);
        }
        return VSConstants.S_OK;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Raised after a project has been opened asynchronously.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
      {
        if (AfterAsynchOpenProject != null)
        {
          var e = new OpenProjectEventArgs(SolutionEventType.AfterAsynchOpenProject)
                    {
                      Hierarchy = pHierarchy,
                      Added = fAdded != 0
                    };
          AfterAsynchOpenProject(this, e);
        }
        return VSConstants.S_OK;
      }

      #endregion
    }

    #endregion

    #region SolutionEventHooker class

    // ================================================================================================
    /// <summary>
    /// This class is responsible for hooking solution events.
    /// </summary>
    // ================================================================================================
    private sealed class SolutionEventHooker : EventHooker<SolutionEventHandler>
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Calls the advise method on the related event provider class.
      /// </summary>
      /// <param name="cookie">The cookie to be used in the Unadvise method.</param>
      // --------------------------------------------------------------------------------------------
      protected override int Advise(out uint cookie)
      {
        return _Solution.AdviseSolutionEvents(Events, out cookie);
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Calls the unadvise method on the related event provider class.
      /// </summary>
      /// <param name="cookie">The cookie created in the Advise method.</param>
      // --------------------------------------------------------------------------------------------
      protected override int Unadvise(uint cookie)
      {
        return _Solution.UnadviseSolutionEvents(cookie);
      }
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Ensures that IVsRunningDocTableEvents is advised when required.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void EnsureAdvise()
    {
      _EventHook.EnsureAdvise();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Ensures that IVsRunningDocTableEvents is unadvised when required.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static void EnsureUnadvise()
    {
      _EventHook.EnsureUnadvise();
    }

    #endregion

  }
}