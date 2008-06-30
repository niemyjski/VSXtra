// ================================================================================================
// UIContext.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================

using System.Runtime.InteropServices;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This type is a markup interface to sign that the type implementing it represents a 
  /// UIContext GUID.
  /// </summary>
  // ================================================================================================
  public interface IUIContextGuidType { }

  // ================================================================================================
  /// <summary>
  /// This class is a name provider for types representing UIContext GUIDs used
  /// </summary>
  // ================================================================================================
  public static class UIContext
  {
    [Guid("ADFC4E60-0397-11D1-9F4E-00A0C911004F")]
    public sealed class SolutionBuilding: IUIContextGuidType { }

    [Guid("ADFC4E61-0397-11D1-9F4E-00A0C911004F")]
    public sealed class Debugging: IUIContextGuidType { }

    [Guid("ADFC4E62-0397-11D1-9F4E-00A0C911004F")]
    public sealed class FullScreenMode: IUIContextGuidType { }

    [Guid("ADFC4E63-0397-11D1-9F4E-00A0C911004F")]
    public sealed class DesignMode: IUIContextGuidType { }

    [Guid("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    public sealed class NoSolution: IUIContextGuidType { }

    [Guid("F1536EF8-92EC-443C-9ED7-FDADF150DA82")]
    public sealed class SolutionExists: IUIContextGuidType { }

    [Guid("ADFC4E65-0397-11D1-9F4E-00A0C911004F")]
    public sealed class EmptySolution: IUIContextGuidType { }

    [Guid("ADFC4E66-0397-11D1-9F4E-00A0C911004F")]
    public sealed class SolutionHasSingleProject: IUIContextGuidType { }
  }
}
