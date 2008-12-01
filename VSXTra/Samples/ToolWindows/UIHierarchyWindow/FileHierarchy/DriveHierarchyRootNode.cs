// ================================================================================================
// DriveHierarchyRootNode.cs
//
// Created: 2008.12.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.UIHierarchyWindow
{
  // ================================================================================================
  /// <summary>
  /// This class represents the root of the drive hierarchy node.
  /// </summary>
  // ================================================================================================
  [HierarchyBitmap("ComputerImage")]
  public class DriveHierarchyRootNode : HierarchyNode 
  {
    #region Overrides of HierarchyNode

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the caption of the node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string Caption
    {
      get { return "Drives on My Computer"; }
    }

    #endregion
  }
}
