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
    [Guid("8FE2DF1D-E0DA-4EBE-9D5C-415D40E487B5")]
    public sealed class CodeWindow : IUIHierarchyWindowKindGuidType { }

    [Guid("BA09E2AF-9DF2-4068-B2F0-4C7E5CC19E2F")]
    public sealed class WindowsFormsDesigner : IUIHierarchyWindowKindGuidType { }

    [Guid("D78612C7-9962-4B83-95D9-268046DAD23A")]
    public sealed class ErrorList : IUIHierarchyWindowKindGuidType { }

    [Guid("4A9B7E51-AA16-11D0-A8C5-00A0C921A4D2")]
    public sealed class TaskList : IUIHierarchyWindowKindGuidType { }

    [Guid("B1E99781-AB81-11D0-B683-00AA00A3EE26")]
    public sealed class Toolbox : IUIHierarchyWindowKindGuidType { }

    [Guid("0504FF91-9D61-11D0-A794-00A0C9110051")]
    public sealed class CallStack : IUIHierarchyWindowKindGuidType { }

    [Guid("E62CE6A0-B439-11D0-A79D-00A0C9110051")]
    public sealed class Threads : IUIHierarchyWindowKindGuidType { }

    [Guid("4A18F9D0-B838-11D0-93EB-00A0C90F2734")]
    public sealed class Locals : IUIHierarchyWindowKindGuidType { }

    [Guid("F2E84780-2AF1-11D1-A7FA-00A0C9110051")]
    public sealed class AutoLocals : IUIHierarchyWindowKindGuidType { }

    [Guid("90243340-BD7A-11D0-93EF-00A0C90F2734")]
    public sealed class Watch : IUIHierarchyWindowKindGuidType { }

    [Guid("EEFA5220-E298-11D0-8F78-00A0C9110057")]
    public sealed class Properties : IUIHierarchyWindowKindGuidType { }

    [Guid("3AE79031-E1BC-11D0-8F78-00A0C9110057")]
    public sealed class SolutionExplorer : IUIHierarchyWindowKindGuidType { }

    [Guid("34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3")]
    public sealed class Output : IUIHierarchyWindowKindGuidType { }

    [Guid("269A02DC-6AF8-11D3-BDC4-00C04F688E50")]
    public sealed class ObjectBrowser : IUIHierarchyWindowKindGuidType { }

    [Guid("07CD18B4-3BA1-11D2-890A-0060083196C6")]
    public sealed class MacroExplorer : IUIHierarchyWindowKindGuidType { }

    [Guid("66DBA47C-61DF-11D2-AA79-00C04F990343")]
    public sealed class DynamicHelp : IUIHierarchyWindowKindGuidType { }

    [Guid("C9C0AE26-AA77-11D2-B3F0-0000F87570EE")]
    public sealed class ClassView : IUIHierarchyWindowKindGuidType { }

    [Guid("2D7728C2-DE0A-45B5-99AA-89B609DFDE73")]
    public sealed class ResourceView : IUIHierarchyWindowKindGuidType { }

    [Guid("25F7E850-FFA1-11D0-B63F-00A0C922E851")]
    public sealed class DocumentOutline : IUIHierarchyWindowKindGuidType { }

    [Guid("74946827-37A0-11D2-A273-00C04F8EF4FF")]
    public sealed class ServerExplorer : IUIHierarchyWindowKindGuidType { }

    [Guid("28836128-FC2C-11D2-A433-00C04F72D18A")]
    public sealed class CommandWindow : IUIHierarchyWindowKindGuidType { }

    [Guid("53024D34-0EF5-11D3-87E0-00C04F7971A5")]
    public sealed class FindSymbol : IUIHierarchyWindowKindGuidType { }

    [Guid("68487888-204A-11D3-87EB-00C04F7971A5")]
    public sealed class FindSymbolResults : IUIHierarchyWindowKindGuidType { }

    [Guid("CF2DDC32-8CAD-11D2-9302-005345000000")]
    public sealed class FindReplace : IUIHierarchyWindowKindGuidType { }

    [Guid("0F887920-C2B6-11D2-9375-0080C747D9A0")]
    public sealed class FindResults1 : IUIHierarchyWindowKindGuidType { }

    [Guid("0F887921-C2B6-11D2-9375-0080C747D9A0")]
    public sealed class FindResults2 : IUIHierarchyWindowKindGuidType { }

    [Guid("9DDABE98-1D02-11D3-89A1-00C04F688DDE")]
    public sealed class MainWindow : IUIHierarchyWindowKindGuidType { }
  }
}
