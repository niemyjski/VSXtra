﻿// ================================================================================================
// VSXtraMenuPlaceHolderPackage.cs
//
// Created: 2008.09.06, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra;

namespace DeepDiver.VSXtraMenuPlaceHolder
{
  [PackageRegistration(UseManagedResourcesOnly = true)]
  [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
  [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
  [ProvideLoadKey("Standard", "1.0", "VSXtraMenuPlaceHolder", "DeepDiver", 1)]
  [ProvideMenuResource(1000, 1)]
  [Guid(GuidList.guidVSXtraMenuPlaceHolderPkgString)]
  public sealed class VSXtraMenuPlaceHolderPackage : PackageBase
  {
  }
}