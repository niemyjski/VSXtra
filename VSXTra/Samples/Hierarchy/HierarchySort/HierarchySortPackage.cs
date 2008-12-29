// ================================================================================================
// HierarchySortPackage.cs
//
// Created: 2008.12.10, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Package;

namespace DeepDiver.HierarchySort
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "HierarchySort", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideToolWindow(typeof(SortedUIHierarchyWindow))]
  [Guid(GuidList.guidHierarchySortPkgString)]
  public sealed class HierarchySortPackage : PackageBase
  {
    [CommandExecMethod]
    [CommandId(GuidList.guidHierarchySortCmdSetString, CmdIDs.cmdidShowHierarchyWindow)]
    [ShowToolWindowAction(typeof(SortedUIHierarchyWindow))]
    private static void ShowSortedHierarchyWindow()
    {
    }
  }
}