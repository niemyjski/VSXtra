// ================================================================================================
// SolutionEvents.cs
//
// Created: 2008.08.03, by Istvan Novak (DeepDiver)
// ================================================================================================

using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
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
    private static SolutionEventHooker _EventHook;

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
      public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
      {
        throw new System.NotImplementedException();
      }

      public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
      {
        throw new System.NotImplementedException();
      }

      public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
      {
        throw new System.NotImplementedException();
      }

      public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
      {
        throw new System.NotImplementedException();
      }

      public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
      {
        throw new System.NotImplementedException();
      }

      public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
      {
        throw new System.NotImplementedException();
      }

      public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
      {
        throw new System.NotImplementedException();
      }

      public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
      {
        throw new System.NotImplementedException();
      }

      public int OnBeforeCloseSolution(object pUnkReserved)
      {
        throw new System.NotImplementedException();
      }

      public int OnAfterCloseSolution(object pUnkReserved)
      {
        throw new System.NotImplementedException();
      }

      public int OnAfterMergeSolution(object pUnkReserved)
      {
        throw new System.NotImplementedException();
      }

      public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy)
      {
        throw new System.NotImplementedException();
      }

      public int OnAfterOpeningChildren(IVsHierarchy pHierarchy)
      {
        throw new System.NotImplementedException();
      }

      public int OnBeforeClosingChildren(IVsHierarchy pHierarchy)
      {
        throw new System.NotImplementedException();
      }

      public int OnAfterClosingChildren(IVsHierarchy pHierarchy)
      {
        throw new System.NotImplementedException();
      }

      /// <summary>
      /// Notifies listening clients that a project has been renamed.
      /// </summary>
      /// <returns>
      /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
      /// </returns>
      /// <param name="pHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the renamed project.</param>
      public int OnAfterRenameProject(IVsHierarchy pHierarchy)
      {
        throw new System.NotImplementedException();
      }

      /// <summary>
      /// Queries listening clients as to whether a parent project has changed.
      /// </summary>
      /// <returns>
      /// </returns>
      /// <param name="pHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project parent.</param>
      /// <param name="pNewParentHier">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the changed project parent.</param>
      /// <param name="pfCancel">[in, out] true if the client vetoed the closing of the project. false if the client approved the closing of the project.</param>
      public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel)
      {
        throw new System.NotImplementedException();
      }

      /// <summary>
      /// Notifies listening clients that a project parent has changed.
      /// </summary>
      /// <returns>
      /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
      /// </returns>
      /// <param name="pHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the changed project parent.</param>
      public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy)
      {
        throw new System.NotImplementedException();
      }

      /// <summary>
      /// Notifies listening clients that a project has been opened asynchronously.
      /// </summary>
      /// <returns>
      /// If the method succeeds, it returns <see cref="F:Microsoft.VisualStudio.VSConstants.S_OK" />. If it fails, it returns an error code.
      /// </returns>
      /// <param name="pHierarchy">[in] Pointer to the <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsHierarchy" /> interface of the project being loaded.</param>
      /// <param name="fAdded">[in] true if the project is added to the solution after the solution is opened. false if the project is added to the solution while the solution is being opened.</param>
      public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded)
      {
        throw new System.NotImplementedException();
      }
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
  }
}