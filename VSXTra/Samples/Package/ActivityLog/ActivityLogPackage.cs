// ================================================================================================
// ActivityLogPackage.cs
//
// Created: 2008.08.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;
using VSXtra.Commands;
using VSXtra.Diagnostics;
using VSXtra.Package;

namespace DeepDiver.ActivityLogPackage
{
  // ================================================================================================
  /// <summary>
  /// Puts a few messages into the ActivityLog
  /// </summary>
  /// <remarks>
  /// Plase do not forget to add thew /log switch to the debug command line in order to allow this
  /// package writing into the activity log (ActivityLog.xml file).
  /// You can find the activity log file within your user profile folder under the
  /// Microsoft\VisualStudio\'Hive'\UserSettings folder. For example, if you have a roaming profile 
  /// and your login name is jsmith then you find the file in the
  /// C:\Users\jsmith\AppData\Roaming\Microsoft\VisualStudio\9.0Exp\UserSettings folder.
  /// </remarks>
  // ================================================================================================
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "ActivityLog", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [Guid(GuidList.guidActivityLogPkgString)]
  public sealed class ActivityLogPackage : PackageBase
  {
    protected override void Initialize()
    {
      ActivityLog.Write(GetType().Name, "Package initialized.");
    }

    protected override void Dispose(bool disposing)
    {
      ActivityLog.Write(GetType().Name, "Package is being disposed.");
      base.Dispose(disposing);
    }

    [CommandExecMethod]
    [CommandId(GuidList.guidActivityLogCmdSetString, CmdIDs.cmdidWriteEntry)]
    [ShowMessageAction("An entry is being written to the Activity Log.")]
    private void WriteEntry()
    {
      ActivityLog.Write(GetType().Name, "WriteEntry method called.");
    }
  }
}