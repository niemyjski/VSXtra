// ================================================================================================
// HierarchyCommand.cs
//
// Created: 2008.11.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using Microsoft.VisualStudio;

namespace VSXtra.Hierarchy
{
  // ================================================================================================
  /// <summary>
  /// This enumeration defines the values of hierarchy commands.
  /// </summary>
  // ================================================================================================
  public enum HierarchyCommand : uint
  {
    CancelLabelEdit = VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_CancelLabelEdit,
    CommitLabelEdit = VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_CommitLabelEdit,
    DoubleClick = VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_DoubleClick,
    EnterKey = VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_EnterKey,
    RightClick = VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_RightClick,
    StartLabelEdit = VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_StartLabelEdit
  }
}