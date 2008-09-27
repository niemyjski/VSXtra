// ================================================================================================
// DynamicToolWindowPackage.cs
//
// Created: 2008.07.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.DynamicToolWindow
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "DynamicToolWindow", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideToolWindow(typeof(DynamicWindowPane), MultiInstances = true)]
  [Guid(GuidList.guidDynamicToolWindowPkgString)]
  public sealed class DynamicToolWindowPackage : PackageBase
  {
  }
}