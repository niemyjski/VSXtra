// ================================================================================================
// OutputPaneAttributes.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  // ==================================================================================
  /// <summary>
  /// This attribute declares the initial visibility of the output window pane.
  /// </summary>
  // ==================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class InitiallyVisibleAttribute: BoolAttribute
  {
    public InitiallyVisibleAttribute(bool value): base(value) {}
  }

  // ==================================================================================
  /// <summary>
  /// This attribute declares if the output window pane should be cleared when the
  /// current solutiopn is closed.
  /// </summary>
  // ==================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ClearWithSolutionAttribute: BoolAttribute
  {
    public ClearWithSolutionAttribute(bool value): base(value) {}
  }

  // ==================================================================================
  /// <summary>
  /// This attribute declares if output window pane should be activated automatically
  /// after the first write operation.
  /// </summary>
  // ==================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class AutoActivateAttribute : BoolAttribute
  {
    public AutoActivateAttribute(bool value): base(value) {}
  }
}
