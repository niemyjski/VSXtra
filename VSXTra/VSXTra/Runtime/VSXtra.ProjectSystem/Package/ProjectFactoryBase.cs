// ================================================================================================
// ProjectFactoryBase.cs
//
// This project file was originally created by Microsoft as part of the MPFProj published on 
// CodePlex (http://www.codeplex.com/MPFProj) marked with the following notice:
// "Copyright (c) Microsoft Corporation.  All rights reserved."
//
// Revised: 2008.09.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Build.BuildEngine;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra.ProjectSystem
{
  // ================================================================================================
  /// <summary>
  /// This factory is used by Visual Studio Shell to create projects within the solution.
  /// </summary>
  // ================================================================================================
  [CLSCompliant(false)]
  public abstract class ProjectFactoryBase<TPackage> : FlavoredProjectFactoryBase
    where TPackage: PackageBase
  {
    #region Constant values

    /// <summary>Security warning when opening/creating client project on UNC share.</summary>
    private const string ProjectFolderFotSecureHelp = "vs.projectfolder_not_secure";

    #endregion

    #region Private fields

    /// <summary>Package owning the project factory</summary>
    private readonly TPackage _Package;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the project factory.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected ProjectFactoryBase()
    {
      _Package = PackageBase.GetPackageInstance<TPackage>();
      // --- Please be aware that this methods needs that ServiceProvider is valid, thus 
      // --- the ordering of calls in the ctor matters.
      BuildEngine = Utilities.InitializeMsBuildEngine(BuildEngine, Site);
    }

    #endregion

    #region Public Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package owning this factory.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected TPackage Package 
    { 
      get { return _Package; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service provider related to the project factory. It is actually the service 
    /// provider of the package owning this project factory.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected IServiceProvider Site
    {
      get { return _Package; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The MSBuild engine that we are going to use in the project.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected Engine BuildEngine { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The MSBuild project for the temporary project file.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected Project BuildProject { get; set; }

    #endregion

    #region Abstract methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates the root node of the project hierarchy.
    /// </summary>
    /// <returns>
    /// The newly created project node.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected abstract ProjectNode CreateProject();

    #endregion

    #region overriden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Rather than directly creating the project, ask VS to initate the process of creating an 
    /// aggregated project in case we are flavored. We will be called on the 
    /// IVsAggregatableProjectFactory to do the real project creation.
    /// </summary>
    /// <param name="fileName">Project file</param>
    /// <param name="location">Path of the project</param>
    /// <param name="name">Project Name</param>
    /// <param name="flags">Creation flags</param>
    /// <param name="projectGuid">Guid of the project</param>
    /// <param name="project">Project that end up being created by this method</param>
    /// <param name="canceled">Was the project creation canceled</param>
    // --------------------------------------------------------------------------------------------
    protected override void CreateProject(string fileName, string location, string name, 
      uint flags, ref Guid projectGuid, out IntPtr project, out int canceled)
    {
      canceled = 0;

      // --- Get the list of GUIDs from the project/template
      string guidsList = ProjectTypeGuids(fileName);
      string projectDirectory = String.IsNullOrEmpty(location)
                                  ? Path.GetDirectoryName(fileName)
                                  : location;
      if (!IsProjectLocationSecure(projectDirectory))
      {
        canceled = 1;
        ErrorHandler.ThrowOnFailure(VSConstants.VS_E_WIZARDBACKBUTTONPRESS);
      }

      // Launch the aggregate creation process (we should be called back on our IVsAggregatableProjectFactoryCorrected implementation)
      var aggregateProjectFactory =
        (IVsCreateAggregateProject) Site.GetService(typeof (SVsCreateAggregateProject));
      int hr = aggregateProjectFactory.CreateAggregateProject(guidsList, fileName, location, name,
                                                              flags, ref projectGuid, out project);
      if (hr == VSConstants.E_ABORT)
        canceled = 1;
      ErrorHandler.ThrowOnFailure(hr);

      // This needs to be done after the aggregation is completed (to avoid creating a non-aggregated CCW) and as a result we have to go through the interface
      var eventsProvider =
        (IProjectEventsProvider)
        Marshal.GetTypedObjectForIUnknown(project, typeof (IProjectEventsProvider));
      eventsProvider.ProjectEventsProvider = GetProjectEventsProvider();

      BuildProject = null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Instantiate the project class, but do not proceed with the initialization just yet.
    /// Delegate to CreateProject implemented by the derived class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override object PreCreateForOuter(IntPtr outerProjectIUnknown)
    {
      VsDebug.Assert(BuildProject != null,
                   "The build project should have been initialized before calling PreCreateForOuter.");
      var globalPropertyHandler = new GlobalPropertyHandler(BuildProject);
      globalPropertyHandler.InitializeGlobalProperties();

      // --- Please be very carefull what is initialized here on the ProjectNode. Normally this 
      // --- should only instantiate and return a project node. The reason why one should very 
      // --- carefully add state to the project node here is that at this point the aggregation 
      // --- has not yet been created and anything that would cause a CCW for the project to be 
      // --- created would cause the aggregation to fail. Our reasoning is that there is no other 
      // --- place where state on the project node can be set that is known by the Factory and 
      // --- has to execute before the Load method.
      var node = CreateProject();
      if (node == null) return null;
      VsDebug.Assert(node != null, "The project failed to be created");
      node.BuildEngine = BuildEngine;
      node.BuildProject = BuildProject;
      node.GlobalPropetyHandler = globalPropertyHandler;
      globalPropertyHandler.RegisterConfigurationChangeListener(node, Site);
      node.Package = Package as ProjectPackageBase;
      return node;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrives the list of project guids from the project file. If you don't want your project 
    /// to be flavorable, override to only return your project factory Guid:
    ///      return this.GetType().GUID.ToString("B");
    /// </summary>
    /// <param name="file">Project file to look into to find the Guid list</param>
    /// <returns>List of semi-colon separated GUIDs</returns>
    // --------------------------------------------------------------------------------------------
    protected override string ProjectTypeGuids(string file)
    {
      // --- Load the project so we can extract the list of GUIDs
      BuildProject = Utilities.ReinitializeMsBuildProject(BuildEngine, file, BuildProject);

      // --- Retrieve the list of GUIDs, if it is not specify, make it our GUID
      string guids = BuildProject.GetEvaluatedProperty(ProjectFileConstants.ProjectTypeGuids);
      if (String.IsNullOrEmpty(guids))
        guids = GetType().GUID.ToString("B");
      return guids;
    }

    #endregion

    #region Helper methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the event provider for project events.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private IProjectEvents GetProjectEventsProvider()
    {
      var projectPackage = Package as ProjectPackageBase;
      VsDebug.Assert(projectPackage != null, "Package not inherited from framework");
      if (projectPackage != null)
      {
        foreach (var listener in projectPackage.SolutionListeners)
        {
          var projectEvents = listener as IProjectEvents;
          if (projectEvents != null)
          {
            return projectEvents;
          }
        }
      }
      return null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Checks if the project location is secure. If it is not then it will launch a dialog box 
    /// prompting the user to acknowledge that the project location is not secure.
    /// </summary>
    /// <param name="location">The location to check</param>
    /// <returns>true if it is secure, false otherwise.</returns>
    // --------------------------------------------------------------------------------------------
    private bool IsProjectLocationSecure(string location)
    {
      if (!Utilities.IsProjectLocationSecure(location))
      {
        if (Utilities.IsShellInCommandLineMode(Site) || 
          Utilities.IsInAutomationFunction(Site))
        {
          return false;
        }
        string errorMessage = String.Format(CultureInfo.CurrentCulture,
                                            SR.GetString(SR.ProjectLocationNotTrusted,
                                                         CultureInfo.CurrentUICulture),
                                            Environment.NewLine, Path.GetDirectoryName(location));
        return
          (DontShowAgainDialog.LaunchDontShowAgainDialog(
             Site, errorMessage,
             ProjectFolderFotSecureHelp,
             DontShowAgainDialog.DefaultButton.OK,
             ProjectConstants.DontShowProjectSecurityWarningAgain) == DialogResult.OK);
      }
      return true;
    }

    #endregion
  }
}