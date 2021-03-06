﻿// ================================================================================================
// SolutionNodeAnalyzerPackage.cs
//
// Created: 2008.09.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Package;

namespace DeepDiver.SolutionNodeAnalyzer
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "SolutionNodeAnalyzer", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [XtraProvideToolWindow(typeof (NodeAnalyzerToolWindow))]
  [Guid(GuidList.guidSolutionNodeAnalyzerPkgString)]
  public sealed class SolutionNodeAnalyzerPackage : PackageBase
  {
  }
}