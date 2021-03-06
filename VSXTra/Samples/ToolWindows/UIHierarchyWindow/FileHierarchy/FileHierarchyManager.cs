// ================================================================================================
// FileHierarchyManager.cs
//
// Created: 2008.11.27, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.IO;
using VSXtra.Hierarchy;

namespace DeepDiver.UIHierarchyWindow
{
  // ================================================================================================
  /// <summary>
  /// This class is repsonsible the File Hierarchy
  /// </summary>
  // ================================================================================================
  public class FileHierarchyManager : HierarchyManager<UIHierarchyWindowPackage>
  {
    #region Private fields
    
    /// <summary>Stores the full path of the root node</summary>
    private readonly string _FullPath;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="FileHierarchyManager"/> class.
    /// </summary>
    /// <param name="fullPath">The path representing the root of the hierarchy.</param>
    // --------------------------------------------------------------------------------------------
    public FileHierarchyManager(string fullPath)
    {
      _FullPath = fullPath;
    }

    #endregion

    #region Overrides of HierarchyManager

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an instance of <see cref="RootNode"/> as the root instance of the hierarchy.
    /// </summary>
    /// <returns>
    /// Newly created root node.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    protected override HierarchyNode CreateHierarchyRoot()
    {
      return new RootNode(null, _FullPath, _FullPath);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the root node by scanning the folders and files in the root folder.
    /// </summary>
    /// <param name="root">Root hierachy node to initialize</param>
    // --------------------------------------------------------------------------------------------
    protected override void InitializeHierarchyRoot(HierarchyNode root)
    {
      var fileRoot = root as FileHierarchyNode;
      if (fileRoot == null) return;
      ScanContent(fileRoot);
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Scans the content of the specified folder and creates child nodes for the specified node
    /// </summary>
    /// <param name="node">Node to scan for its content</param>
    // --------------------------------------------------------------------------------------------
    public void ScanContent(FileHierarchyNode node)
    {
      if (node == null) throw new ArgumentNullException("node");
      try
      {
        var path = node.FullPath;
        foreach (var dir in Directory.GetDirectories(node.FullPath))
        {
          var dirInfo = new DirectoryInfo(dir);
          var dirName = dir.Substring(path.Length + (path.EndsWith("\\") ? 0 : 1));
          FolderNode folderNode;
          if ((dirInfo.Attributes & FileAttributes.System) != 0)
          {
            folderNode = new SystemFolderNode(this, dir, dirName);
            var systemFolderHierarchy = new FileHierarchyManager(dir);
            systemFolderHierarchy.EnsureHierarchyRoot();
            folderNode.NestHierarchy(systemFolderHierarchy);
          }
          else
          {
            folderNode = new FolderNode(this, dir, dirName);
            folderNode.AddChild(new NotLoadedNode(this));
          }
          node.AddChild(folderNode);
          folderNode.IsExpanded = false;
        }
        foreach (var file in Directory.GetFiles(path))
        {
          var fileNode = new FileNode(this, file, Path.GetFileName(file));
          node.AddChild(fileNode);
        }
      }
      catch (SystemException)
      {
        // --- This exception is intentionally supressed.
      }
    }

    #endregion
  }
}