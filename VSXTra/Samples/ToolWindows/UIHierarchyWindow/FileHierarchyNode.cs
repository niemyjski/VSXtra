using System;
using System.IO;
using VSXtra.Hierarchy;

namespace DeepDiver.UIHierarchyWindow
{
  public class FileHierarchyNode: HierarchyRoot<FileHierarchyNode, FileHierarchyNode>
  {
    #region Private fields

    private string _FullPath;

    #endregion

    #region Lifecycle methods

    public FileHierarchyNode(string fullPath)
    {
      _FullPath = fullPath;
      ScanContent(fullPath, this);
    }

    public FileHierarchyNode(string fullPath, FileHierarchyNode root): base(root)
    {
      _FullPath = fullPath;
      ScanContent(fullPath, root);
    }

    private void ScanContent(string basePath, FileHierarchyNode root)
    {
      try
      {
        foreach (var dir in Directory.GetDirectories(basePath))
        {
          AddChild(new FileHierarchyNode(dir, root));
        }
      }
      catch (SystemException)
      {
      }
    }

    #endregion

    #region Public Properties

    public string FullPath
    {
      get { return _FullPath; }
    }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the caption of the node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string Caption
    {
      get
      {
        var parent = ParentNode as FileHierarchyNode;
        return parent != null && _FullPath.StartsWith(parent.FullPath + "\\")
                 ? _FullPath.Substring(parent.FullPath.Length + 1)
                 : _FullPath;
      }
    }

    #endregion
  }
}