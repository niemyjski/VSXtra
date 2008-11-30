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
using VSXtra.Shell;

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
    protected override HierarchyNode<UIHierarchyWindowPackage> CreateHierarchyRoot()
    {
      return new RootNode(_FullPath, _FullPath, null);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the root node by scanning the folders and files in the root folder.
    /// </summary>
    /// <param name="root">Root hierachy node to initialize</param>
    // --------------------------------------------------------------------------------------------
    protected override void InitializeHierarchyRoot(HierarchyNode<UIHierarchyWindowPackage> root)
    {
      var fileRoot = root as FileHierarchyNode;
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
          var dirName = dir.Substring(path.Length + (path.EndsWith("\\") ? 0 : 1));
          var folderNode = new FolderNode(this, dir, dirName);
          node.AddChild(folderNode);
          folderNode.AddChild(new NotLoadedNode(this));
          folderNode.IsExpanded = false;
        }
        foreach (var file in Directory.GetFiles(path))
        {
          var fileNode = new FileNode(file, Path.GetFileName(file), this);
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
    [VsCommandId(VSConstants.VSStd97CmdID.Paste)]
    private static void CanPaste(OleMenuCommand command)
    {
      command.Enabled = true;
    }

    [CommandExecMethod]
    [VsCommandId(VSConstants.VSStd97CmdID.Paste)]
    private static void ExecPaste()
    {
      VsMessageBox.Show("Paste command executed");
    }

    #endregion
  }
}