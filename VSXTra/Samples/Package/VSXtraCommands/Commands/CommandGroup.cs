// ================================================================================================
// CommandGroup.cs
//
// Created: 2008.07.23, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra.Commands;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// This class represents the logical container of VSXtra commands.
  /// </summary>
  /// <remarks>
  /// The Guid attribute is set for the corresponding GUID of all commands. When implementing the
  /// menu handler classes this Guid declaration in not required on the other partitions of the class
  /// definition.
  /// </remarks>
  // ================================================================================================
  [Guid(GuidList.guidVSXtraCommandsCmdSetString)]
  public partial class VSXtraCommandGroup: CommandGroup<VSXtraCommandsPackage>
  {
  }
}