// ================================================================================================
// ClientCommandHandlers.cs
//
// Created: 2008.07.20, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.ServicesReference
{
  // ================================================================================================
  /// <summary>
  /// This type represents a command group owned by the ClientPackage.
  /// </summary>
  /// <remarks>
  /// This type is a logical container, its only role is to group the commands having
  /// the same GUID in their comman ID. It inherits from CommandGroup signing the owner package type.
  /// </remarks>
  // ================================================================================================
  [Guid(GuidList.guidClientCmdSetString)]
  public sealed class ClientCommandGroup : CommandGroup<ClientPackage>
  {
    // ================================================================================================
    /// <summary>
    /// This class implements the "Get C# Global Service" command.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidClientGetGlobalService)]
    public sealed class GetGlobalServiceCommand : MenuCommandHandler
    {
      protected override void OnExecute(OleMenuCommand command)
      {
        var service = ServiceProvider.GetService<SMyGlobalService, IMyGlobalService>();
        if (null == service)
        {
          OutputWindow.Debug.WriteLine("Can not get the global service.");
          return;
        }
        service.GlobalServiceFunction();
      }
    }

    // ================================================================================================
    /// <summary>
    /// This class implements the "Get C# Local Service" command.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidClientGetLocalService)]
    public sealed class GetLocalServiceCommand : MenuCommandHandler
    {
      protected override void OnExecute(OleMenuCommand command)
      {
        var service = ServiceProvider.GetService<SMyLocalService, IMyLocalService>();
        if (null != service)
        {
          OutputWindow.Debug.WriteLine("GetService for the local service succeeded, but it should fail.");
          return;
        }
        var outputText = " ======================================\n" +
                         "\tGetLocalServiceCallback test succeeded.\n" +
                         " ======================================\n";
        OutputWindow.General.WriteLine(outputText);
      }
    }

    // ================================================================================================
    /// <summary>
    /// This class implements the "Get C# Local Using Global Service" command.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.cmdidClientGetLocalUsingGlobal)]
    public sealed class GetLocalUsinGlobalServiceCommand : MenuCommandHandler
    {
      protected override void OnExecute(OleMenuCommand command)
      {
        var service = ServiceProvider.GetService<SMyGlobalService, IMyGlobalService>();
        if (null == service)
        {
          OutputWindow.General.WriteLine("Can not get the global service.");
          return;
        }
        service.CallLocalService();
      }
    }
  }
}