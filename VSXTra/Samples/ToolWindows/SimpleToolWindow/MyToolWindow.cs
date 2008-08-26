// ================================================================================================
// MyToolWindow.cs
//
// Created: 2008.08.26, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.SimpleToolWindow
{
  [Guid("37159cbf-a17d-4d3a-8277-5f847dc59c92")]
  public class MyToolWindow : ToolWindowPane<SimpleToolWindowPackage, MyControl>
  {
  }
}
