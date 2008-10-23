// ================================================================================================
// OptionsPagePackage.cs
//
// Created: 2008.07.16, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;
using VSXtra.Package;

namespace DeepDiver.OptionsPage
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "OptionsPage", "DeepDiver", 1)]
  [XtraProvideOptionPageAttribute(typeof(OptionsPageGeneral), "My Options Page (C#)", "General", 101, 106, true)]
  [ProvideProfileAttribute(typeof(OptionsPageGeneral), "My Options Page (C#)", "General Options", 101, 106, true, DescriptionResourceID = 101)]
  [XtraProvideOptionPageAttribute(typeof(OptionsPageCustom), "My Options Page (C#)", "Custom", 101, 107, true)]
  [ProvideProfileAttribute(typeof(OptionsPageCustom), "My Options Page (C#)", "Custom", 101, 106, true, DescriptionResourceID = 101)]
  [Guid(GuidList.guidOptionsPagePkgString)]
  public sealed class OptionsPagePackage : PackageBase
  {
  }
}