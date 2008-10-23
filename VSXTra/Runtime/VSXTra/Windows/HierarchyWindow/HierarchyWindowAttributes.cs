// ================================================================================================
// HierarchyWindowAttributes.cs
//
// Created: 2008.09.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra.Windows
{
  // ================================================================================================
  /// <summary>
  /// This class is intended to be the root of all hierachy window attributes.
  /// </summary>
  // ================================================================================================
  public abstract class HierarchyWindowAttribute : Attribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public abstract __UIHWINFLAGS StyleFlag { get; }
  }

  // ================================================================================================
  /// <summary>
  /// Signs that the  UI hierarchy window tracks the environment's selection
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ActAsProjectTypeWindowAttribute: HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_ActAsProjectTypeWin style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_ActAsProjectTypeWin; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Default is alpha sort on caption enabled toolbars in UIHierarchyWindow tool window
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class DoNotSortRootNodesAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_DoNotSortRootNodes style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_DoNotSortRootNodes; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Forces selection of a single node in a hierarchy.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ForceSingleSelectAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_DoNotSortRootNodes style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_ForceSingleSelect; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Indicates that the IVsHierarchy pointer passed in with the call to Init is actually a special 
  /// hidden root hierarchy.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class InitWithHiddenRootHierarchyAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_InitWithHiddenRootHierarchy style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_InitWithHiddenRootHierarchy; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Indicates that the IVsHierarchy pointer passed in with the call to Init is actually a special 
  /// hidden root hierarchy.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class UseSolutionAsHiddenRootHierarchyAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_UseSolutionAsHiddenRootHierarchy style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_UseSolutionAsHiddenRootHierarchy; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Signs the plus marks should be used for the nodes.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class LinesAtRootAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_LinesAtRoot style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_LinesAtRoot; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Specifies whether non-root nodes in your hierarchy window should be sorted, or left in the 
  /// order in which the hierarchy enumerates them to the hierarchy window .
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class SortChildNodesAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_SortChildNodes style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_SortChildNodes; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Specifies whether your hierarchy window shows state icons.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class NoStateIconAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_NoStateIcon style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_NoStateIcon; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Creates a hidden root hierarchy that is the parent of your top-level nodes.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class InitWithHiddenParentRootAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_InitWithHiddenParentRoot style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_InitWithHiddenParentRoot; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// If specified, when UIHierarchy selects a node, UIHierarchy creates alternate hierarchies.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class PropagateAltHierarchyItemAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_PropagateAltHierarchyItem style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_PropagateAltHierarchyItem; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// Controls the handling of the delete command within the hierarchy window.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class RouteCmdidDeleteAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_RouteCmdidDelete style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_RouteCmdidDelete; }
    }
  }

  // ================================================================================================
  /// <summary>
  /// This is for windows that handle commands when they are the active hierarchy even if their 
  /// hierarchy doesn't have focus (like Project/Add Item).
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class HandlesCmdsAsActiveHierarchyAttribute : HierarchyWindowAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the __UIHWINFLAGS.UIHWF_HandlesCmdsAsActiveHierarchy style flag of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override __UIHWINFLAGS StyleFlag
    {
      get { return __UIHWINFLAGS.UIHWF_HandlesCmdsAsActiveHierarchy; }
    }
  }
}