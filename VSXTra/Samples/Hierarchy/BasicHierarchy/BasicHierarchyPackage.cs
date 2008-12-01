// ================================================================================================
// BasicHierarchyPackage.cs
//
// Created: 2008.12.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Package;

namespace DeepDiver.BasicHierarchy
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "Basic Hierarchy", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideToolWindow(typeof(BasicUIHierarchyToolWindow))]
  [Guid(GuidList.guidBasicHierarchyPkgString)]
  public sealed class BasicHierarchyPackage : PackageBase
  {
    [CommandExecMethod]
    [CommandId(GuidList.guidBasicHierarchyCmdSetString, CmdIDs.cmdidShowBasicHierachyWIndow)]
    [ShowToolWindowAction(typeof(BasicUIHierarchyToolWindow))]
    private static void ShowBasicUIHierarchyWindow()
    {
    }
  }
}