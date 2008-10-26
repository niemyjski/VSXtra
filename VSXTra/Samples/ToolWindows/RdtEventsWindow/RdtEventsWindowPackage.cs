// ================================================================================================
// RdtEventsWindowPackage.cs
//
// Created: 2008.08.03, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Package;

namespace DeepDiver.RdtEventsWindow
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "RdtEventsWindow", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideOptionPage(typeof (RdtEventsOptionsPage), "RDT Event Explorer", "Explorer Options", 0, 0, true)]
  [ProvideProfile(typeof (RdtEventsOptionsPage), "RDT Event Explorer", "Explorer Options", 0, 0, true)]
  [XtraProvideToolWindow(typeof (RdtEventsWindowPane))]
  [Guid(GuidList.guidRdtEventsWindowPkgString)]
  public sealed class RdtEventsWindowPackage : PackageBase
  {
  }
}