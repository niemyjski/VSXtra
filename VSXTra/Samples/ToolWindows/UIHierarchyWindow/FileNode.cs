namespace DeepDiver.UIHierarchyWindow
{
  public class FileNode : FileHierarchyNode
  {
    public FileNode(string fullPath, string caption, FileHierarchyNode root) : 
      base(fullPath, caption, root)
    {
      SortPriority = 30;
    }

    public override int ImageIndex
    {
      get { return ImageName.File; }
    }
  }
}