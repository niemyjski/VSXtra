using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Package;

namespace DeepDiver.HelloVSXPackage
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideMenuResource("Menus.ctmenu", 1)]
  [Guid(GuidList.guidHelloVSXPackagePkgString)]
  public sealed class HelloVsxPackage : PackageBase
  {
  }
}