// ================================================================================================
// FileHierarchyManager.cs
//
// Created: 2008.11.27, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Hierarchy;

namespace DeepDiver.DynamicHierarchy
{
  // ================================================================================================
  /// <summary>
  /// This class is repsonsible the File Hierarchy
  /// </summary>
  // ================================================================================================
  public sealed class FileHierarchyManager : HierarchyManager<DynamicHierarchyPackage>
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

    #region Overrides of InitialHierarchy

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
      return new RootNode(this, _FullPath, _FullPath);
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
        var dirInfo = new DirectoryInfo(node.FullPath);
        foreach (var dir in dirInfo.GetDirectories())
        {
          var dirName = dir.Name;
          FolderNode folderNode;
          if ((dir.Attributes & FileAttributes.System) != 0)
          {
            folderNode = new SystemFolderNode(this, dir.FullName, dirName);
            folderNode.NestHierarchy(new FileHierarchyManager(dir.FullName));
          }
          else
          {
            folderNode = new FolderNode(this, dir.FullName, dirName);
            folderNode.AddChild(new NotLoadedNode(this));
          }
          node.AddChild(folderNode);
          folderNode.IsExpanded = false;
        }
        foreach (var file in Directory.GetFiles(node.FullPath))
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

    #region Command methods

    [CommandStatusMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Cut)]
    [VsCommandId(VSConstants.VSStd97CmdID.Copy)]
    public void CutCopyStatus(OleMenuCommand command)
    {
      command.Enabled = GetSelectedNodes(true).Count % 2 == 0;
    }

    #endregion
  }
}