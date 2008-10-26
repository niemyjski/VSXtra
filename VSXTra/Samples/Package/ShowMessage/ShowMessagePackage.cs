// ================================================================================================
// ShowMessagePackage.cs
//
// Created: 2008.08.07, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Package;

namespace DeepDiver.ShowMessage
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "ShowMessage", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [Guid(GuidList.guidShowMessagePkgString)]
  public sealed class ShowMessagePackage : PackageBase
  {
    [CommandExecMethod]
    [CommandId(GuidList.guidShowMessageCmdSetString, CmdIDs.cmdidDisplayMyMessage)]
    [ShowMessageAction("Hello, World from ShowMessagePackage")]
    private static void DisplayMessage()
    {
    }
  }
}