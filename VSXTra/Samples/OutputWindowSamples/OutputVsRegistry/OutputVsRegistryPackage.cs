// ================================================================================================
// OutputVsRegistryPackage.cs
//
// Created: 2008.07.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.OutputVsRegistry
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "OutputVsRegistry", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [Guid(GuidList.guidOutputVsRegistryPkgString)]
  public sealed class OutputVsRegistryPackage : PackageBase
  {

  }
}