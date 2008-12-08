// ================================================================================================
// DynamicHierarchyPackage.cs
//
// Created: 2008.12.07, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Package;

namespace DeepDiver.DynamicHierarchy
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "DynamicHierarchy", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideToolWindow(typeof(DynamicUIHierarchyWindow))]
  [Guid(GuidList.guidDynamicHierarchyPkgString)]
  public sealed class DynamicHierarchyPackage : PackageBase
  {
    [CommandExecMethod]
    [CommandId(GuidList.guidDynamicHierarchyCmdSetString, CmdIDs.cmdidShowDynamicHierarchy)]
    [ShowToolWindowAction(typeof(DynamicUIHierarchyWindow))]
    private static void ShowBasicUIHierarchyWindow()
    {
    }
  }
}