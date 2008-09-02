// ================================================================================================
// ClsIdToolWindowPane.cs
//
// Created: 2008.09.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class defines a Tool Window Pane based on a CLSID.
  /// </summary>
  // ================================================================================================
  public abstract class ClsIdToolWindowPane<TPackage> :
    ToolWindowPane<TPackage, WindowPanePlaceHolderControl>
    where TPackage : PackageBase
  {
    protected abstract Guid ToolWindowClassGuid { get; }
    
    public override Guid ToolClsid
    {
      get { return ToolWindowClassGuid; }
    }
  }
}