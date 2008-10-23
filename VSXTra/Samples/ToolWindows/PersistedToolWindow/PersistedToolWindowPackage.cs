// ================================================================================================
// PersistedToolWindowPackage.cs
//
// Created: 2008.07.04, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;
using VSXtra.Package;

namespace DeepDiver.PersistedToolWindow
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "PersistedToolWindow", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideToolWindow(typeof(PersistedWindowPane), Style = VsDockStyle.Tabbed, 
    Window = typeof(WindowKind.SolutionExplorer))]
  [Guid(GuidList.guidPersistedToolWindowPkgString)]
  public sealed class PersistedToolWindowPackage : PackageBase
  {
  }
}