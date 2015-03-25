// ================================================================================================
// OutputWindow.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Shell;

namespace VSXtra.Windows
{
  // ================================================================================================
  /// <summary>
  /// This static class is responsible for obtaining built-in output window panel and
  /// managing (creating, obtaining and deleting) window panes through output pane 
  /// definition types.
  /// </summary>
  // ================================================================================================
  public static class OutputWindow
  {
    #region Private fields

    private static MissingOutputPaneHandling _MissingOutputPaneHandling =
      MissingOutputPaneHandling.Silent;

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This property defines how output pane management exceptions should be handled.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static MissingOutputPaneHandling MissingOutputPaneHandling
    {
      get { return _MissingOutputPaneHandling; }
      set { _MissingOutputPaneHandling = value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the General window pane instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static OutputWindowPane General
    {
      get { return GetPane(typeof(GeneralPane)); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the Build window pane instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static OutputWindowPane Build
    {
      get { return GetPane(typeof(BuildPane)); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the Debug window pane instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static OutputWindowPane Debug
    {
      get { return GetPane(typeof(DebugPane)); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets a virtual output window pane that does not show any output.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static OutputWindowPane Silent
    {
      get { return GetPane(typeof(SilentPane)); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Iterates through the current collection of output window panes.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<OutputWindowPane> OutputWindowPanes
    {
      get
      {
        foreach (EnvDTE.OutputWindowPane pane in VsIde.ToolWindows.OutputWindow.OutputWindowPanes)
        {
          IVsOutputWindowPane outPane;
          var guid = new Guid(pane.Guid);
          OutputWindowInstance.GetPane(ref guid, out outPane);
          yield return new OutputWindowPane(null, outPane);
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an output window pane according to the specified definition type.
    /// </summary>
    /// <param name="type">Pane definition type.</param>
    /// <returns>
    /// The newly created window pane.
    /// </returns>
    /// <remarks>
    /// The pane definition type should be a type deriving from WindowPaneDefinition.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public static OutputWindowPane CreatePane(Type type)
    {
      OutputPaneDefinition paneDef = CreatePaneDefinition(type);
      if (paneDef == null) return HandleError(type);

      // --- No physical IVsOutputWindowPane belongs to a virtual window.
      if (!paneDef.IsSilent)
      {
        int createSuccess = CreateWindowPane(paneDef);
        if (createSuccess != VSConstants.S_OK)
        {
          return HandleError(type);
        }
      }
      return GetPane(type);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the output window pane according to the specified definition type.
    /// </summary>
    /// <typeparam name="TPane">Pane definition type.</typeparam>
    /// <returns>
    /// The newly created window pane.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static OutputWindowPane GetPane<TPane>()
      where TPane : OutputPaneDefinition
    {
      return GetPane(typeof(TPane));
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the output window pane according to the specified definition type.
    /// </summary>
    /// <param name="type">Pane definition type.</param>
    /// <returns>
    /// The newly created window pane.
    /// </returns>
    /// <remarks>
    /// The pane definition type should be a type deriving from WindowPaneDefinition.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public static OutputWindowPane GetPane(Type type)
    {
      // --- Obtain the window pane
      OutputPaneDefinition paneDef = CreatePaneDefinition(type);
      if (paneDef == null) return HandleError(type);

      // --- No physical IVsOutputWindowPane belongs to a virtual window.
      if (paneDef.IsSilent) return new OutputWindowPane(paneDef, null);

      IVsOutputWindowPane pane;
      int getSuccess = GetWindowPane(paneDef, out pane);
      if (getSuccess != VSConstants.S_OK || pane == null)
      {
        // --- Pane cannot be obtained, try to create first
        int createSuccess = CreateWindowPane(paneDef);
        if (createSuccess != VSConstants.S_OK)
        {
          return HandleError(type);
        }

        // --- Now, obtain the newly created pane
        getSuccess = GetWindowPane(paneDef, out pane);
        if (getSuccess != VSConstants.S_OK || pane == null)
        {
          return HandleError(type);
        }
      }

      // --- Retrieve the pane instance
      return new OutputWindowPane(paneDef, pane);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Deletes an output window pane according to the specified definition type.
    /// </summary>
    /// <param name="type">Pane definition type.</param>
    /// <returns>
    /// True, if the window pane is successfully deleted; otherwise, false.
    /// </returns>
    /// <remarks>
    /// The pane definition type should be a type deriving from WindowPaneDefinition.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public static bool DeletePane(Type type)
    {
      OutputPaneDefinition paneDef = CreatePaneDefinition(type);
      if (paneDef == null) HandleError(type);
      return DeleteWindowPane(paneDef) == VSConstants.S_OK;
    }

    #endregion

    #region Output pane definitions

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This class is a definition for the General output window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private sealed class GeneralPane : OutputPaneDefinition
    {
      public override Guid GUID
      {
        get { return VSConstants.GUID_OutWindowGeneralPane; }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This class is a definition for the Debug output window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [AutoActivate(true)]
    private sealed class DebugPane : OutputPaneDefinition
    {
      public override Guid GUID
      {
        get { return VSConstants.GUID_OutWindowDebugPane; }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This class is a definition for the Build output window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [AutoActivate(true)]
    private sealed class BuildPane : OutputPaneDefinition
    {
      public override Guid GUID
      {
        get { return VSConstants.GUID_BuildOutputWindowPane; }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This class is a definition for the Silent virtual output window pane.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private sealed class SilentPane : OutputPaneDefinition
    {
      public SilentPane()
      {
        IsSilent = true;
      }
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the SVsOutputWindow service instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static IVsOutputWindow OutputWindowInstance
    {
      get
      {
        if (!SiteManager.HasGlobalServiceProvider)
          throw new InvalidOperationException("The framework has not been sited!");
        return SiteManager.GlobalServiceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a pane definition type instance.
    /// </summary>
    /// <param name="type">Pane definition type.</param>
    /// <returns>
    /// Pane definition instance.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    private static OutputPaneDefinition CreatePaneDefinition(Type type)
    {
      OutputPaneDefinition paneDef = null;
      try
      {
        paneDef = Activator.CreateInstance(type) as OutputPaneDefinition;
      }
      catch (SystemException)
      {
        // --- This exception is intentionally supressed.
      }
      return paneDef;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an output window pane by using the SVsOutputWindow service.
    /// </summary>
    /// <param name="paneDef">Pane definition instance.</param>
    /// <returns>HRESULT indicating the success or failure.</returns>
    // --------------------------------------------------------------------------------------------
    private static int CreateWindowPane(OutputPaneDefinition paneDef)
    {
      Guid paneGuid = paneDef.GUID;
      return OutputWindowInstance != null
               ? OutputWindowInstance.CreatePane(
                   ref paneGuid,
                   paneDef.Name,
                   paneDef.InitiallyVisible ? -1 : 0,
                   paneDef.ClearWithSolution ? -1 : 0)
               : VSConstants.E_FAIL;
      //return OutputWindowInstance.CreatePane(
      //  ref paneGuid,
      //  paneDef.Name,
      //  paneDef.InitiallyVisible ? -1 : 0,
      //  paneDef.ClearWithSolution ? -1 : 0);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets an output window pane by using the SVsOutputWindow service.
    /// </summary>
    /// <param name="paneDef">Pane definition instance.</param>
    /// <param name="pane">Pane instance</param>
    /// <returns>HRESULT indicating the success or failure.</returns>
    // --------------------------------------------------------------------------------------------
    private static int GetWindowPane(OutputPaneDefinition paneDef,
      out IVsOutputWindowPane pane)
    {
      Guid paneGuid = paneDef.GUID;
      if (OutputWindowInstance != null)
        return OutputWindowInstance.GetPane(ref paneGuid, out pane);
      pane = null;
      return VSConstants.E_FAIL;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Deletes an output window pane by using the SVsOutputWindow service.
    /// </summary>
    /// <param name="paneDef">Pane definition instance.</param>
    /// <returns>HRESULT indicating the success or failure.</returns>
    // --------------------------------------------------------------------------------------------
    private static int DeleteWindowPane(OutputPaneDefinition paneDef)
    {
      Guid paneGuid = paneDef.GUID;
      return OutputWindowInstance.DeletePane(ref paneGuid);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the error related to the specified window pane definition type.
    /// </summary>
    /// <param name="type">Window pane definition type.</param>
    /// <returns>
    /// Output window pane to redirect the output to.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    private static OutputWindowPane HandleError(Type type)
    {
      OutputWindowPane newPane;

      // --- Guess out where to redirect the output
      switch (_MissingOutputPaneHandling)
      {
        case MissingOutputPaneHandling.ThrowException:
          throw new WindowPaneNotFoundException(type);
        case MissingOutputPaneHandling.Silent:
          newPane = Silent;
          break;
        case MissingOutputPaneHandling.RedirectToGeneral:
          newPane = General;
          break;
        default:
          newPane = Debug;
          break;
      }

      // --- Activate the pane
      if (newPane != null) newPane.Activate();
      return newPane;
    }

    #endregion
  }
}
