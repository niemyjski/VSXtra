// ================================================================================================
// SimpleUIHierarchyToolWindow.cs
//
// Created: 2008.09.02, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Hierarchy;
using VSXtra.Windows;

namespace DeepDiver.UIHierarchyWindow
{
  // ================================================================================================
  /// <summary>
  /// This class implements a simple hierarchy window
  /// </summary>
  // ================================================================================================
  [InitialCaption("Simple UI Hierarchy window")]
  [BitmapResourceId(301)]
  [LinesAtRoot]
  [RouteCmdidDelete]
  [DoNotSortRootNodes]
  public class SimpleUIHierarchyToolWindow : UIHierarchyToolWindow<UIHierarchyWindowPackage>
  {
    protected override HierarchyManager<UIHierarchyWindowPackage> HierarchyManager
    {
      get
      {
        var driveManager = new DriveHierarchyManager();
        var DriveD = new FileHierarchyManager("D:\\");
        DriveD.EnsureHierarchyRoot();
        var DriveDRoot1 = new DriveNode(driveManager, "Drive1");
        DriveDRoot1.NestHierarchy(DriveD);
        driveManager.HierarchyRoot.AddChild(DriveDRoot1);
        var DriveC = new FileHierarchyManager("C:\\");
        DriveC.EnsureHierarchyRoot();
        var DriveCRoot1 = new DriveNode(driveManager, "Drive1");
        DriveCRoot1.NestHierarchy(DriveC);
        driveManager.HierarchyRoot.AddChild(DriveCRoot1);
        return driveManager;
      }
    }
  }
}