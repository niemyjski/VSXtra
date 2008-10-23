// ================================================================================================
// ServicesReferencePackage.cs
//
// Created: 2008.07.20, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;
using VSXtra.Package;

namespace DeepDiver.ServicesReference
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [ProvideService(typeof(SMyGlobalService))]
  [Guid(GuidList.guidServicesPkgString)]
  public sealed class ServicesPackage : PackageBase
  {
  }
}