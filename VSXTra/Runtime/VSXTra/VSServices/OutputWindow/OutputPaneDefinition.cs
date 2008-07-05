// ================================================================================================
// OutputPaneDefinition.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This abstract class is intended to be a base class for uotput window pane 
  /// definitions.
  /// </summary>
  // ================================================================================================
  public abstract class OutputPaneDefinition
  {
    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an instance of the class by obtaining the attributes decorating the class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected OutputPaneDefinition()
    {
      InitiallyVisible = true;
      Name = "<Name not defined>";
      foreach (var attr in GetType().GetCustomAttributes(false))
      {
        var paneNameAttr = attr as DisplayNameAttribute;
        if (paneNameAttr != null)
        {
          Name = paneNameAttr.DisplayName;
          continue;
        }
        var initVisAttr = attr as InitiallyVisibleAttribute;
        if (initVisAttr != null)
        {
          InitiallyVisible = initVisAttr.Value;
          continue;
        }
        var clearWithSolAttr = attr as ClearWithSolutionAttribute;
        if (clearWithSolAttr != null)
        {
          ClearWithSolution = clearWithSolAttr.Value;
          continue;
        }
        var activateAttr = attr as AutoActivateAttribute;
        if (activateAttr != null)
        {
          AutoActivate = activateAttr.Value;
          continue;
        }
      }
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the GUID of the output window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual Guid GUID
    {
      get { return GetType().GUID; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the default name of the output window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string Name { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the output window pane is initially visible or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool InitiallyVisible { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the output window pane is to be cleared when the
    /// current solution is closed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool ClearWithSolution { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if output window pane should be automatically 
    /// activated after the first write operation.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool AutoActivate { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or internally sets the flag indicating if this output window pane is a 
    /// silent pane or not.
    /// </summary>
    /// <remarks>
    /// Silent panes do not provide output.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public bool IsSilent { get; internal set; }

    #endregion
  }
}
