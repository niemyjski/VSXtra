// ================================================================================================
// LogicalView.cs
//
// Created: 2008.08.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This type is a markup interface to sign that the type implementing it represents a 
  /// Logical view GUID.
  /// </summary>
  // ================================================================================================
  public interface ILogicalViewGuidType { }

  public static class LogicalView
  {
    [Guid("00000000-0000-0000-0000-000000000000")]
    public sealed class Primary: ILogicalViewGuidType { }

    [Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
    public sealed class Any : ILogicalViewGuidType { }

    [Guid("7651a700-06e5-11d1-8ebd-00a0c90f26ea")]
    public sealed class Debugging : ILogicalViewGuidType { }

    [Guid("7651a701-06e5-11d1-8ebd-00a0c90f26ea")]
    public sealed class Code : ILogicalViewGuidType { }

    [Guid("7651a702-06e5-11d1-8ebd-00a0c90f26ea")]
    public sealed class Designer : ILogicalViewGuidType { }

    [Guid("7651a703-06e5-11d1-8ebd-00a0c90f26ea")]
    public sealed class TextView : ILogicalViewGuidType { }

    [Guid("7651a704-06e5-11d1-8ebd-00a0c90f26ea")]
    public sealed class UserChhoseView : ILogicalViewGuidType { }

    [Guid("80a3471a-6b87-433e-a75a-9d461de0645f")]
    public sealed class ProjectSpecificEditor : ILogicalViewGuidType { }
  }
}