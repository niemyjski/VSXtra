// ================================================================================================
// WindowKind.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================

using System.Runtime.InteropServices;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This type is a markup interface to sign that the type implementing it represents a 
  /// Window kind GUID.
  /// </summary>
  // ================================================================================================
  public interface IWindowKindGuidType { }

  // ================================================================================================
  /// <summary>
  /// This type is a markup interface to sign that the type implementing it represents a 
  /// UI Hierarchy Window kind GUID.
  /// </summary>
  // ================================================================================================
  public interface IUIHierarchyWindowKindGuidType : IWindowKindGuidType { }

  // ================================================================================================
  /// <summary>
  /// This class is a name provider for types representing Window kind GUIDs.
  /// </summary>
  // ================================================================================================
  public static class WindowKind
  {
    [Guid("3AE79031-E1BC-11D0-8F78-00A0C9110057")]
    public sealed class SolutionExplorer : IUIHierarchyWindowKindGuidType { }
  }
}
