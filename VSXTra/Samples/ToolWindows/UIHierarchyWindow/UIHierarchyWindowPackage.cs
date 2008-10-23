// ================================================================================================
// UIHierarchyWindowPackage.cs
//
// Created: 2008.09.02, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;
using VSXtra.Commands;
using VSXtra.Package;

namespace DeepDiver.UIHierarchyWindow
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "UIHierarchyWindow", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideToolWindow(typeof(SimpleUIHierarchyToolWindow))]
  [Guid(GuidList.guidUIHierarchyWindowPkgString)]
  public sealed class UIHierarchyWindowPackage : PackageBase
  {
    [CommandExecMethod]
    [CommandId(GuidList.guidUIHierarchyWindowCmdSetString, CmdIDs.cmdidShowUIHierarchy)]
    [ShowToolWindowAction(typeof(SimpleUIHierarchyToolWindow))]
    private static void ShowUIHierarchyWindow()
    {
    }

  }
}