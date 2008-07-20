// ================================================================================================
// VsxService.cs
//
// Created: 2008.07.19, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This markup interface signs that the implementor class implements a service of TService for
  /// package TPackage.
  /// </summary>
  /// <typeparam name="TPackage">Package owning the service</typeparam>
  /// <typeparam name="TService">Service implemented by the package</typeparam>
  // ================================================================================================
  public interface IVsxService<TPackage, TService>
    where TPackage: PackageBase
    where TService : class
  {
  }

  public class VsxService<TPackage, TService>:
    IVsxService<TPackage, TService>
    where TPackage : PackageBase
    where TService : class
  {
    #region Lifecycle methods

    public VsxService()
    {
      Package = PackageBase.GetPackageInstance<TPackage>();
      ServiceProvider = Package;
    }

    #endregion

    #region Public properties

    public IServiceProvider ServiceProvider { get; private set; }
    public TPackage Package { get; private set; }

    #endregion
  }
}