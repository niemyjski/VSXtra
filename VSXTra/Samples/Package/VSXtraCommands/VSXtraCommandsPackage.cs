// ================================================================================================
// VSXtraCommandsPackage.cs
//
// Created: 2008.07.19, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.VSXtraCommands
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "VSXtraCommands", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideOptionPageAttribute(typeof(GeneralPage), "VSXtraCommands", "General", 2000, 2001, true)]
  [ProvideProfileAttribute(typeof(GeneralPage), "VSXtraCommands", "General", 2000, 2001, true, DescriptionResourceID = 2002)]
  [Guid(GuidList.guidVSXtraCommandsPkgString)]
  public sealed class VSXtraCommandsPackage : PackageBase
  {
  }
}