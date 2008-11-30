// ================================================================================================
// ResourceResolver.cs
//
// Created: 2008.04.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using VSXtra.Package;
using VSXtra.Properties;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  // ==================================================================================
  /// <summary>
  /// This static class provides functions that allow obtaining strings from resources. 
  /// </summary>
  /// <remarks>
  /// This class can resolve string resources through the Resources static class and 
  /// through the resources related to the VSPackage. This type assumes that you have 
  /// only one VSPackage in your assembly!
  /// </remarks>
  // ==================================================================================
  public static class ResourceResolver<TPackage>
    where TPackage: IVsPackage
  {
    #region Private fields

    private static readonly Dictionary<Assembly, Type> _ResourceTypes = 
      new Dictionary<Assembly, Type>();

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Resolves a string from the Resource.resx in the assembly of the specified 
    /// package.
    /// </summary>
    /// <param name="key">String to resolve.</param>
    /// <returns>
    /// The resolved string value.
    /// </returns>
    /// <remarks>
    /// If the string starts with the "$" character, the subsequent characters are 
    /// used as a string property key for the Resources class in the assembly of the 
    /// current package. Use "$$" to represent the literal "$" character.
    /// If the string starts with the "#" character, the subsequent characters are 
    /// used as a string resource key for the package resources in the assembly of the 
    /// current package. Use "##" to represent the literal "#" character.
    /// In other cases the input string itself is returned.
    /// </remarks>
    // --------------------------------------------------------------------------------
    public static string GetString(string key)
    {
      if (string.IsNullOrEmpty(key) || key.Length < 2)
        return key;

      if (key.StartsWith("#"))
      {
        key = key.Substring(1);
        return key.StartsWith("#")
                 ? key
                 : GetStringFromPackageResources(key.Trim());
      }
      if (key.StartsWith("$"))
      {
        key = key.Substring(1);
        return key.StartsWith("$")
                 ? key
                 : GetStringFromResourcesClass(key.Trim());
      }
      return key;
    }

    public static Bitmap GetBitmap(string key)
    {
      return GetBitmapFromResourcesClass(key.Trim());
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Checks if the specified resource property exists in the Resources class of 
    /// the package assembly.
    /// </summary>
    /// <param name="key">Property key used for string resolution.</param>
    /// <returns>
    /// True, if the string exists; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static bool ExistsInResourcesClass(string key)
    {
      var resourceType = GetResourcesClassType();
      if (resourceType == null) return false;
      var propInfo = 
        resourceType.GetProperty(key, BindingFlags.Static | BindingFlags.NonPublic);
      return propInfo != null;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Obtains the string with the specified key from the properties of the Resources 
    /// class in the package assembly.
    /// </summary>
    /// <param name="key">Property key used for string resolution.</param>
    /// <returns>
    /// The resolved string if the referenced property name could be resolved;
    /// otherwise a string representing a missing resource message.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static string GetStringFromResourcesClass(string key)
    {
      var resourceType = GetResourcesClassType();
      if (resourceType == null) return Resources.ResourceNotFound;
      var propInfo = 
        resourceType.GetProperty(key, BindingFlags.Static | BindingFlags.NonPublic);
      return propInfo == null 
        ? Resources.ResourceNotFound 
        : propInfo.GetValue(null, null).ToString();
    }

    private static Bitmap GetBitmapFromResourcesClass(string key)
    {
      var resourceType = GetResourcesClassType();
      if (resourceType == null) return null;
      var propInfo =
        resourceType.GetProperty(key, BindingFlags.Static | BindingFlags.NonPublic);
      return propInfo == null
        ? null
        : propInfo.GetValue(null, null) as Bitmap;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Checks if the specified resource string exists in the package resources of 
    /// the package assembly.
    /// </summary>
    /// <param name="key">Property key used for string resolution.</param>
    /// <returns>
    /// True, if the string exists; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static bool ExistsInPackageResources(string key)
    {
      var packageGuid = GetPackageGuid();
      string resourceString;
      var resourceManager =
        PackageBase.GetGlobalService<SVsResourceManager, IVsResourceManager>();
      if (resourceManager == null) return false;
      var result = resourceManager.LoadResourceString(ref packageGuid, -1, key, 
        out resourceString);
      return result == VSConstants.S_OK;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Obtains the string with the specified resource key from the package resources.
    /// </summary>
    /// <param name="key">Resource key used for string resolution.</param>
    /// <returns>
    /// The resolved string if the referenced property name could be resolved;
    /// otherwise a string representing a missing resource message.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static string GetStringFromPackageResources(string key)
    {
      var packageGuid = GetPackageGuid();
      string resourceString;
      var resourceManager =
        PackageBase.GetGlobalService<SVsResourceManager, IVsResourceManager>();
      if (resourceManager == null) return Resources.PackageNotFound;
      var result = resourceManager.LoadResourceString(ref packageGuid, -1, key, 
        out resourceString);
      return result != VSConstants.S_OK ? Resources.PackageNotFound : resourceString;
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the compiler generated Resources type belonging to the direct external
    /// caller's assembly.
    /// </summary>
    /// <returns>
    /// Resources type if found; otherwise, false.
    /// </returns>
    // --------------------------------------------------------------------------------
    private static Type GetResourcesClassType()
    {
      Assembly callingAsm = typeof(TPackage).Assembly;

      // --- Check the cache for Resources type and return if found.
      Type resourceType;
      if (_ResourceTypes.TryGetValue(callingAsm, out resourceType))
      {
        return resourceType;
      }

      // --- Search for the Resources type. If found add it to the cache.
      foreach (Type type in callingAsm.GetTypes())
      {
        if (type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0
          && !type.IsVisible && type.Name == "Resources")
        {
          _ResourceTypes.Add(callingAsm, type);
          return type;
        }
      }
      return null;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package Guid belonging to the direct external caller's assembly.
    /// </summary>
    /// <returns>
    /// Package GUID if found; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------
    private static Guid GetPackageGuid()
    {
      return typeof(TPackage).GUID;
    }

    #endregion
  }
}