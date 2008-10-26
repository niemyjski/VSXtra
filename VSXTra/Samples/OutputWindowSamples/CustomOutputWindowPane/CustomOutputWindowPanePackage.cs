// ================================================================================================
// CustomOutputWindowPanePackage.cs
//
// Created: 2008.08.07, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Commands;
using VSXtra.Package;
using VSXtra.Windows;

namespace DeepDiver.CustomOutputWindowPane
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "CustomOutputWindowPane", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [Guid(GuidList.guidCustomOutputWindowPanePkgString)]
  public sealed class CustomOutputWindowPanePackage : PackageBase
  {
    [CommandExecMethod]
    [CommandId(GuidList.guidCustomOutputWindowPaneCmdSetString, CmdIDs.cmdidCreateOutput)]
    private static void CreateOutput()
    {
      Console.WriteLine("*** Turn to the CutomPane1 and CustomePane2 output panes!");
      OutputWindowPane pane1 = OutputWindow.GetPane<CustomPane1>();
      pane1.WriteLine("Welcome on CustomPane1!");
      OutputWindowPane pane2 = OutputWindow.GetPane<CustomPane2>();
      pane2.WriteLine("Welcome on CustomPane2!");
    }

    #region Nested type: CustomPane1

    [DisplayName("CustomPane1")]
    [AutoActivate(true)]
    private class CustomPane1 : OutputPaneDefinition
    {
    }

    #endregion

    #region Nested type: CustomPane2

    [DisplayName("CustomPane2")]
    [AutoActivate(true)]
    [ClearWithSolution(true)]
    private class CustomPane2 : OutputPaneDefinition
    {
    }

    #endregion
  }
}