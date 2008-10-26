// ================================================================================================
// VSXtraCommandsPackage.cs
//
// Created: 2008.07.19, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;
using VSXtra.Commands;
using VSXtra.Documents;
using VSXtra.Package;
using VSXtra.Shell;

namespace DeepDiver.VSXtraCommands
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "VSXtraCommands", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideAutoLoad(typeof (UIContext.NoSolution))]
  [ProvideService(typeof (SCommandManagerService), ServiceName = "CommandManagerService")]
  [XtraProvideOptionPage(typeof (GeneralPage), "VSXtraCommands", "General", 2000, 2001, true)]
  [ProvideProfile(typeof (GeneralPage), "VSXtraCommands", "General", 2000, 2001, true,
    DescriptionResourceID = 2002)]
  [XtraProvideOptionPage(typeof (CommandsPage), "VSXtraCommands", "Commands", 2000, 2003, true)]
  [ProvideProfile(typeof (CommandsPage), "VSXtraCommands", "Commands", 2000, 2003, true,
    DescriptionResourceID = 2004)]
  [Guid(GuidList.guidVSXtraCommandsPkgString)]
  public sealed class VSXtraCommandsPackage : PackageBase
  {
    #region Lifecycle methods

    protected override void Initialize()
    {
      RegisterCommandsWithManager();
      SolutionEvents.OnAfterOpenSolution += OnAfterOpenSolution;
      SolutionEvents.OnAfterCloseSolution += OnAfterCloseSolution;
    }

    #endregion

    #region Private event handler methods

    private void OnAfterOpenSolution(object sender, OpenSolutionEventArgs e)
    {
      VsDocumentEvents.OnDocumentClosing += OnDocumentClosing;
    }

    private void OnAfterCloseSolution(object sender, SolutionEventArgs e)
    {
      VsDocumentEvents.OnDocumentClosing -= OnDocumentClosing;
    }

    private void OnDocumentClosing(object sender, DocumentEventArgs e)
    {
    }

    #endregion

    #region Private methods

    private void RegisterCommandsWithManager()
    {
      ICommandManagerService commandService = GetService<SCommandManagerService, ICommandManagerService>();
      if (commandService != null)
      {
        foreach (MenuCommandHandler handler in GetCommandHandlerInstances<VSXtraCommandsPackage>())
        {
          commandService.RegisterCommand(handler);
        }
      }
    }

    #endregion
  }
}