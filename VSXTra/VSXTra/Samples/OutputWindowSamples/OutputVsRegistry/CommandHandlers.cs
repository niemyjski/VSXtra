// ================================================================================================
// CommandHandlers.cs
//
// Created: 2008.07.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Runtime.InteropServices;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.OutputVsRegistry
{
  [Guid(GuidList.guidOutputVsRegistryCmdSetString)]
  public sealed class CommandGroup: CommandGroup<OutputVsRegistryPackage>
  {
    // ================================================================================================
    /// <summary>
    /// Represents the only command in this package.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidDisplayRegistryValues)]
    public sealed class OutputRegistryHandler: MenuCommandHandler
    {
      protected override void OnExecute(OleMenuCommand command)
      {
        Console.WriteLine("*** Local registry root key: {0}", VsRegistry.LocalRegistryRoot);
        Console.WriteLine("*** List of recent projects:");
        foreach (var item in VsRegistry.RecentProjectsList.OrderBy(k => k.Key))
        {
          Console.WriteLine("    {0}", item.Value);
        }
        Console.WriteLine("*** List of recent files:");
        foreach (var item in VsRegistry.RecentFilesList.OrderBy(k => k.Key))
        {
          Console.WriteLine("    {0}", item.Value);
        }
        OutputWindow.General.Activate();
      }
    }
  }
}