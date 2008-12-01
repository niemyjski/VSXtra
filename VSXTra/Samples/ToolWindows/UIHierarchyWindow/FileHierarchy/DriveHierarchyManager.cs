// ================================================================================================
// DriveHierarchyManager.cs
//
// Created: 2008.12.01, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;

namespace DeepDiver.UIHierarchyWindow
{
  // ================================================================================================
  /// <summary>
  /// This class manages a hierarchy of drives to show their files and folders.
  /// </summary>
  // ================================================================================================
  public class DriveHierarchyManager : HierarchyManager<UIHierarchyWindowPackage>
  {
    #region Overrides of HierarchyManager<UIHierarchyWindowPackage>

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates the root node of the hierarchy.
    /// </summary>
    /// <returns>The newly created hierarchy node</returns>
    /// <remarks>
    /// Override this method to create the root instance of the hierarchy node. In this method you
    /// cannot refer to the manager node of the newly created root. After returning the root node
    /// instance, the manager instance will site it and then calls the 
    /// <see cref="HierarchyManager{TPackage}.InitializeHierarchyRoot"/> method. Override that method to finish the 
    /// initialization of the root node.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected override HierarchyNode CreateHierarchyRoot()
    {
      return new DriveHierarchyRootNode();
    }

    #endregion
  }
}
