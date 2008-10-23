// ================================================================================================
// PackageEntry.cs
//
// Created: 2008.07.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.Win32;

namespace VSXtra.Package
{
  public class PackageEntry
  {
    public const string MsCorEE = "mscoree.dll";
    public Guid Guid { get; internal set; }
    public string Name { get; internal set; }
    public string InProcServer32 { get; internal set; }

    public bool IsManagedPackage
    {
      get 
      { 
        var lower = InProcServer32.ToLowerInvariant();
        return lower == MsCorEE|| lower.EndsWith(MsCorEE);
      }
    }

    public RegistryKey RegistryKey
    { 
      get { return null; }
    }
  }
}