using System;
using System.IO;
using VSXtra;
using VSXtra.Hierarchy;

namespace DeepDiver.UIHierarchyWindow
{
  public class FileHierarchyNode: HierarchyRoot<FileHierarchyNode>
  {
    #region Private fields

    private string _Caption;
    private bool _Loaded;

    #endregion

    #region Image indexes

    protected class ImageName
    {
      public const int Home = 0;
      public const int Folder = 1;
      public const int File = 2;
      public const int NotLoaded = 3;
    }

    #endregion

    #region Lifecycle methods

    private FileHierarchyNode() {}

    public FileHierarchyNode(FileHierarchyNode root) : base(root)
    {
      _Loaded = false;
    }

    public FileHierarchyNode(string fullPath, string caption, FileHierarchyNode root): base(root)
    {
      FullPath = fullPath;
      _Caption = caption;
    }

    public static FileHierarchyNode CreateRoot(string fullPath)
    {
      var root = CreateRoot();
      root.FullPath = fullPath;
      root._Caption = fullPath;
      root.ScanContent(fullPath, root);
      return root;
    }

    private void ScanContent(string basePath, FileHierarchyNode root)
    {
      try
      {
        foreach (var dir in Directory.GetDirectories(basePath))
        {
          var dirName = dir.Substring(basePath.Length + (basePath.EndsWith("\\") ? 0 : 1));
          var folderNode = new FolderNode(dir, dirName, root);
          AddChild(folderNode);
          folderNode.AddChild(new NotLoadedNode(root));
        }
        foreach (var file in Directory.GetFiles(basePath))
        {
          var fileNode = new FileNode(file, Path.GetFileName(file), root);
          AddChild(fileNode);
        }
        _Loaded = true;
      }
      catch (SystemException)
      {
      }
    }

    #endregion

    #region Public Properties

    public string FullPath { get; private set; }

    #endregion

    #region Overridden Properties and methods

    public override int ImageIndex
    {
      get { return ImageName.Home; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Defines the list of items that can be used is icon images.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override ImageHandler InitImageHandler()
    {
      return ImageHandler.CreateImageList(
        Resources.HomeImage,
        Resources.FolderImage,
        Resources.FileImage,
        Resources.NotLoaded);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the caption of the node.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override string Caption
    {
      get { return _Caption; }
    }

    protected override void OnBeforeExpanded()
    {
      if (!_Loaded)
      {
        if (FirstChild is NotLoadedNode)
        {
          RemoveChild(FirstChild);
          ScanContent(FullPath, ManagerNode);
          InvalidateItem();
        }
      }
      _Loaded = true;
    }

    #endregion
  }
}