// ================================================================================================
// CommandGroup.cs
//
// Created: 2008.06.30, by Istvan Novak (DeepDiver)
// ================================================================================================

using VSXtra.Package;

namespace VSXtra.Commands
{
  // ====================================================================================
  /// <summary>
  /// This abstract class represents a grouping type of command handlers.
  /// </summary>
  /// <typeparam name="TPackage">Package owning the menu command handlers</typeparam>
  // ====================================================================================
  public abstract class CommandGroup<TPackage> : ICommandGroupProvider
    where TPackage: PackageBase
  {
    #region Public methods

    // ------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package instance owning this command group.
    /// </summary>
    /// <returns>Package instance owning this command group.</returns>
    // ------------------------------------------------------------------------------
    TPackage GetPackageInstance()
    {
      return PackageBase.GetPackageInstance<TPackage>();
    }

    #endregion

    #region ICommandGroupProvider implementation

    // ------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package instance that owns the command group.
    /// </summary>
    /// <returns>
    /// Package instance, if package has been sited; otherwise, null.
    /// </returns>
    // ------------------------------------------------------------------------------
    PackageBase ICommandGroupProvider.GetPackageInstance()
    {
      return GetPackageInstance();
    }

    #endregion
  }
}