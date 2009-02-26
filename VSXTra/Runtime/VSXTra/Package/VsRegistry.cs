// ================================================================================================
// VsRegistry.cs
//
// Created: 2008.07.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace VSXtra.Package
{
  // ================================================================================================
  /// <summary>
  /// This class represents operations with the registry entries belonging to currently running 
  /// instance of Visual Studio.
  /// </summary>
  // ================================================================================================
  public static class VsRegistry
  {
    #region Public constants

    public const string RecentFilesListKey = "FileMRUList";
    public const string RecentProjectsListKey = "ProjectMRUList";
    public const string InstallDirKey = "InstallDir";
    public const string PackagesKey = "Packages";

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the local Visual Studio registry root name.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static string LocalRegistryRoot
    {
      get
      {
        string root;
        uint handle;
        LocalRegistry.GetLocalRegistryRootEx(
          (uint)__VsLocalRegistryType.RegType_UserSettings,
          out handle,
          out root);
        return string.Format(@"{0}\{1}",
            (__VsLocalRegistryRootHandle)handle == __VsLocalRegistryRootHandle.RegHandle_CurrentUser 
            ? "HKEY_CURRENT_USER" : "HKEY_LOCAL_MACHINE",
          root);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Get the file entries belonging to the specified registry key.
    /// </summary>
    /// <param name="key">Registry key.</param>
    /// <returns>List of file entries belonging to the key.</returns>
    // --------------------------------------------------------------------------------------------
    public static IList<FileEntry> GetFileEntryList(RegistryKey key)
    {
      var result = new List<FileEntry>();
      key.GetValueNames().ForEach(
        item => result.Add(new FileEntry(item, key.GetValue(item).ToString())));
      return result;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the list of recent projects
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static IList<FileEntry> RecentProjectsList
    {
      get
      {
        var key = GetRecentProjectsListKey(false);
        if (key == null) return FileEntry.EmptyList;
        using (key)
        {
          return GetFileEntryList(key);
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the list of recent files
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static IList<FileEntry> RecentFilesList
    {
      get
      {
        var key = GetRecentFilesListKey(false);
        if (key == null) return FileEntry.EmptyList;
        using (key)
        {
          return GetFileEntryList(key);
        }
      }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the local Visual Studio root key holding the list of recent projects.
    /// </summary>
    /// <param name="writeable">True, if the key should be opened writable.</param>
    /// <returns>
    /// The local Visual Studio root key holding the list of recent projects.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static RegistryKey GetRecentProjectsListKey(bool writeable)
    {
      var key = GetLocalRegistryRoot(__VsLocalRegistryType.RegType_UserSettings, false);
      return key.OpenSubKey(RecentProjectsListKey, writeable);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the local Visual Studio root key holding the list of recent files.
    /// </summary>
    /// <param name="writeable">True, if the key should be opened writable.</param>
    /// <returns>
    /// The local Visual Studio root key holding the list of recent files.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static RegistryKey GetRecentFilesListKey(bool writeable)
    {
      var key = GetLocalRegistryRoot(__VsLocalRegistryType.RegType_UserSettings, false);
      return key.OpenSubKey(RecentFilesListKey, writeable);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the local Visual Studio configuration key.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static RegistryKey GetConfigurationKey()
    {
      return GetLocalRegistryRoot(__VsLocalRegistryType.RegType_Configuration, false);
    }

    #endregion

    #region Private properties and methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets an ILocalRegistry2 instance
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static ILocalRegistry4 LocalRegistry
    {
      get { return SiteManager.GetGlobalService<SLocalRegistry, ILocalRegistry4>();  }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the registry key for the specified Visual Studio Local registry root.
    /// </summary>
    /// <param name="regType">Type of local registry.</param>
    /// <param name="writeable">Set true to return a writeable key.</param>
    /// <returns>
    /// Registry key representing the requested local registry root.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    private static RegistryKey GetLocalRegistryRoot(__VsLocalRegistryType regType, bool writeable)
    {
      string regRoot;
      uint handle;
      LocalRegistry.GetLocalRegistryRootEx((uint)regType, out handle, out regRoot);
      RegistryKey rootHandle = (__VsLocalRegistryRootHandle) handle ==
                               __VsLocalRegistryRootHandle.RegHandle_CurrentUser
                                 ? Registry.CurrentUser
                                 : Registry.LocalMachine;
      return rootHandle.OpenSubKey(regRoot, writeable);
    }

    #endregion
  }
}