// ================================================================================================
// DisplaySitedPackages.cs
//
// Created: 2008.08.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Package;

namespace DeepDiver.DisplayLoadedPackages
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "DisplayLoaded", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [Guid(GuidList.guidDisplayLoadedPackagesPkgString)]
  public sealed class DisplaySitedPackages : PackageBase
  {
    [CommandExecMethod]
    [CommandId(GuidList.guidDisplayLoadedPackagesCmdSetString, CmdIDs.cmdidDisplayPackages)]
    private static void DisplayPackageInfo()
    {
      List<PackageBase> sitedPackages = SitedVSXtraPackages;
      Console.WriteLine("There are currently {0} sited VSXtra packages:", sitedPackages.Count);
      foreach (PackageBase package in sitedPackages)
      {
        Console.WriteLine("  {0}", package.GetType().FullName);
        IEnumerable<MenuCommandHandler> registeredHandlers = GetCommandHandlerInstances(package.GetType());
        if (registeredHandlers.Count() > 0)
        {
          Console.WriteLine("    Registered command handlers:");
          foreach (MenuCommandHandler command in registeredHandlers)
          {
            Console.WriteLine("      {0}", command.GetType().FullName);
          }
        }
      }
    }
  }
}