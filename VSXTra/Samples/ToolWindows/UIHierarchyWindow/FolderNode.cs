namespace DeepDiver.UIHierarchyWindow
{
  public class FolderNode : FileHierarchyNode
  {
    public FolderNode(string fullPath, string caption, FileHierarchyNode root) : 
      base(fullPath, caption, root)
    {
      SortPriority = 20;
    }

    public override int ImageIndex
    {
      get { return ImageName.Folder; }
    }
  }

}