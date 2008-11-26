namespace DeepDiver.UIHierarchyWindow
{
  public class NotLoadedNode: FileHierarchyNode
  {
    public NotLoadedNode(FileHierarchyNode root) : base(root)
    {
      SortPriority = 10;
    }

    public override string Caption
    {
      get { return "Loading..."; }
    }

    public override int ImageIndex
    {
      get { return ImageName.NotLoaded; }
    }
  }
}