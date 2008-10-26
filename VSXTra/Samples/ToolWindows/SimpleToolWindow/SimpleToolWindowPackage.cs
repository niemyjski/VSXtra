// ================================================================================================
// SimpleToolWindowPackage.cs
//
// Created: 2008.08.26, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Package;

namespace DeepDiver.SimpleToolWindow
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "SimpleToolWindow", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideToolWindow(typeof (MyToolWindow))]
  [Guid(GuidList.guidSimpleToolWindowPkgString)]
  public sealed class SimpleToolWindowPackage : PackageBase
  {
    [CommandExecMethod]
    [CommandId(GuidList.guidSimpleToolWindowCmdSetString, CmdIDs.cmdidShowToolWindow)]
    [ShowToolWindowAction(typeof (MyToolWindow))]
    private void ShowToolWindow()
    {
    }
  }
}