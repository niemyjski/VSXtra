// ================================================================================================
// VSRegistry.cs
//
// This source code is created by using the source code provided with the VS 2010 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2010.07.06, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra.Core
{
  // ================================================================================================
  /// <summary>
  /// Helper class to handle the registry of the instance of VS that is hosting this code.
  /// </summary>
  // ================================================================================================
  [CLSCompliant(false)]
  public static class VSRegistry
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the global service provider object.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static ServiceProvider GlobalProvider
    {
      get { return ServiceProvider.GlobalProvider; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns a read-only RegistryKey object for the root of a given storage type.
    /// It is up to the caller to dispose the returned object.
    /// </summary>
    /// <param name="registryType">The type of registry storage to open.</param>
    // --------------------------------------------------------------------------------------------
    public static RegistryKey RegistryRoot(LocalRegistryType registryType)
    {
      return RegistryRoot(GlobalProvider, registryType, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns a RegistryKey object for the root of a given storage type.
    /// It is up to the caller to dispose the returned object.
    /// </summary>
    /// <param name="registryType">The type of registry storage to open.</param>
    /// <param name="writable">Flag to indicate is the key should be writable.</param>
    // --------------------------------------------------------------------------------------------
    public static RegistryKey RegistryRoot(LocalRegistryType registryType, bool writable)
    {
      return RegistryRoot(GlobalProvider, registryType, writable);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns a RegistryKey object for the root of a given storage type.
    /// It is up to the caller to dispose the returned object.
    /// </summary>
    /// <param name="provider">The service provider to use to access the Visual Studio's services.</param>
    /// <param name="registryType">The type of registry storage to open.</param>
    /// <param name="writable">Flag to indicate is the key should be writable.</param>
    // --------------------------------------------------------------------------------------------
    public static RegistryKey RegistryRoot(IServiceProvider provider, 
      LocalRegistryType registryType, bool writable)
    {
      if (null == provider)
      {
        throw new ArgumentNullException("provider");
      }

      // --- The current implementation of the shell supports only RegType_UserSettings and
      // --- RegType_Configuration, so for any other values we have to return not implemented.
      if ((LocalRegistryType.UserSettings != registryType) &&
          (LocalRegistryType.Configuration != registryType))
      {
        throw new NotSupportedException();
      }

      // --- Try to get the new ILocalRegistry4 interface that is able to handle the new registry paths.
      var localRegistry = provider.GetService(typeof(SLocalRegistry)) as ILocalRegistry4;
      if (null != localRegistry)
      {
        uint rootHandle;
        string rootPath;
        if (ErrorHandler.Succeeded(localRegistry.GetLocalRegistryRootEx((uint)registryType, 
          out rootHandle, out rootPath)))
        {
          // --- Check if we have valid data.
          if (!string.IsNullOrEmpty(rootPath) && ((uint)LocalRegistryRootHandle.Invalid != rootHandle))
          {
            // --- Check if the root is inside HKLM or HKCU. Observe that this does not depends only from
            // --- the registry type, but also from instance-specific data like the RANU flag.
            var root = ((uint)LocalRegistryRootHandle.LocalMachine == rootHandle) 
              ? Registry.LocalMachine 
              : Registry.CurrentUser;
            return root.OpenSubKey(rootPath, writable);
          }
        }
      }

      // We are here if the usage of the new interface failed for same reason, so we have to fall back to
      // the ond way to access the registry.
      var oldRegistry = provider.GetService(typeof(SLocalRegistry)) as ILocalRegistry2;
      if (null == oldRegistry)
      {
        // There is something wrong with this installation or this service provider.
        return null;
      }
      string registryPath;
      NativeMethods.ThrowOnFailure(oldRegistry.GetLocalRegistryRoot(out registryPath));
      if (string.IsNullOrEmpty(registryPath))
      {
        return null;
      }

      var regRoot = (LocalRegistryType.Configuration == registryType) 
        ? Registry.LocalMachine 
        : Registry.CurrentUser;
      return regRoot.OpenSubKey(registryPath, writable);
    }
  }

  // ================================================================================================
  /// <summary>
  /// This type enumerates the types of local registry to access.
  /// </summary>
  // ================================================================================================
  public enum LocalRegistryType
  {
    UserSettings = 1,
    Configuration = 2
  }

  // ================================================================================================
  /// <summary>
  /// This type enumerates handles for the local registry roots.
  /// </summary>
  // ================================================================================================
  public enum LocalRegistryRootHandle: uint
  {
    Invalid = 0x00,
    CurrentUser = 0x80000000,
    LocalMachine = 0x80000001
  }

}
