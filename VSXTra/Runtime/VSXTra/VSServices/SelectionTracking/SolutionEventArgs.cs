// ================================================================================================
// SolutionEventArgs.cs
//
// Created: 2008.08.06, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  #region SolutionEventType

  // ================================================================================================
  /// <summary>
  /// This enumeration represents the RDT event types.
  /// </summary>
  // ================================================================================================
  public enum SolutionEventType
  {
    AfterOpenProject,
    QueryCloseProject,
    BeforeCloseProject,
    AfterLoadProject,
    QueryUnloadProject,
    BeforeUnloadProject,
    AfterOpenSolution,
    QueryCloseSolution,
    BeforeCloseSolution,
    AfterCloseSolution,
    AfterMergeSolution,
    BeforeOpeningChildren,
    AfterOpeningChildren,
    BeforeClosingChildren,
    AfterClosingChildren,
    AfterRenameProject,
    QueryChangeProjectParent,
    AfterChangeProjectParent,
    AfterAsynchOpenProject
  }

  #endregion

  #region SolutionEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents a solution event argument.
  /// </summary>
  // ================================================================================================
  public class SolutionEventArgs : EventArgs 
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the type of event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public SolutionEventType EventType { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of solution event</param>
    // --------------------------------------------------------------------------------------------
    public SolutionEventArgs(SolutionEventType eventType)
    {
      EventType = eventType;
    }
  }

  #endregion

  #region SolutionNodeEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents solution event argument with a hierarchy node.
  /// </summary>
  // ================================================================================================
  public class SolutionNodeEventArgs : SolutionEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the hierarchy belonging to the item.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IVsHierarchy Hierarchy { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of solution event</param>
    // --------------------------------------------------------------------------------------------
    public SolutionNodeEventArgs(SolutionEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region SolutionNodeCancelEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents cancellable event arguments related to a node.
  /// </summary>
  // ================================================================================================
  public class SolutionNodeCancelEventArgs : SolutionNodeEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the event is to be cancelled.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool Cancel { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of solution event</param>
    // --------------------------------------------------------------------------------------------
    public SolutionNodeCancelEventArgs(SolutionEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region CloseProjectEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents arguments related to closing project events.
  /// </summary>
  // ================================================================================================
  public class CloseProjectEventArgs : SolutionNodeCancelEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the project is to be removed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool Removed { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of solution event</param>
    // --------------------------------------------------------------------------------------------
    public CloseProjectEventArgs(SolutionEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region OpenProjectEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents arguments related to opening project events.
  /// </summary>
  // ================================================================================================
  public class OpenProjectEventArgs : SolutionNodeEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the project is added.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool Added { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of solution event</param>
    // --------------------------------------------------------------------------------------------
    public OpenProjectEventArgs(SolutionEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region LoadProjectEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents arguments related to load/unload project events.
  /// </summary>
  // ================================================================================================
  public class LoadProjectEventArgs : SolutionNodeEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the placeholder hierarchy of the event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IVsHierarchy PlaceHolder { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of solution event</param>
    // --------------------------------------------------------------------------------------------
    public LoadProjectEventArgs(SolutionEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region OpenSolutionEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents arguments related to open solution events.
  /// </summary>
  // ================================================================================================
  public class OpenSolutionEventArgs : SolutionEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating that the opened solution is a new solution.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool NewSolution { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of solution event</param>
    // --------------------------------------------------------------------------------------------
    public OpenSolutionEventArgs(SolutionEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region OpenSolutionEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents arguments related to open solution events.
  /// </summary>
  // ================================================================================================
  public class CloseSolutionEventArgs : SolutionEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the event is to be cancelled.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool Cancel { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of solution event</param>
    // --------------------------------------------------------------------------------------------
    public CloseSolutionEventArgs(SolutionEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion

  #region OpenSolutionEventArgs

  // ================================================================================================
  /// <summary>
  /// This class represents arguments related to open solution events.
  /// </summary>
  // ================================================================================================
  public class ChangeProjectParentEventArgs : SolutionNodeCancelEventArgs
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the new parent of the project
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IVsHierarchy NewHierarchy { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates event argument for the specified event type.
    /// </summary>
    /// <param name="eventType">Type of solution event</param>
    // --------------------------------------------------------------------------------------------
    public ChangeProjectParentEventArgs(SolutionEventType eventType)
      : base(eventType)
    {
    }
  }

  #endregion
}