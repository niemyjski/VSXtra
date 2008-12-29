// ================================================================================================
// CommandGroup.cs
//
// Created: 2008.12.15, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra.Commands;

namespace DeepDiver.HierarchySort
{
  // ================================================================================================
  /// <summary>
  /// Logical command group for all commands in this package
  /// </summary>
  // ================================================================================================
  [Guid(GuidList.guidHierarchySortCmdSetString)]
  public sealed class CommandGroup : CommandGroup<HierarchySortPackage>
  {
    [CommandId(CmdIDs.HierarchyWindowToolbar)]
    public sealed class HierarchyWindowToolbar : ToolbarDefinition
    {
    }
  }
}