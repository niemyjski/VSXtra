// ================================================================================================
// MyToolWindow.cs
//
// Created: 2008.09.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.SolutionNodeAnalyzer
{
  [Guid("9a19b5d2-f13b-489b-8def-37a7ab18dc51")]
  [InitialCaption("$ToolWindowTitle")]
  [BitmapResourceId(301)]
  public class NodeAnalyzerToolWindow : ToolWindowPane<SolutionNodeAnalyzerPackage, NodeAnalyzerControl>
  {
  }
}
