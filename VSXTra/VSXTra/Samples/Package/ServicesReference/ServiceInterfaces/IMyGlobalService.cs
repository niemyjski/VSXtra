// ================================================================================================
// IMyGlobalService.cs
//
// Created: 2008.07.20, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;

namespace DeepDiver.ServicesReference
{
  // ================================================================================================
  /// <summary>
  /// This type defines the functional interface of MyGlobalService.
  /// </summary>
  // ================================================================================================
  [Guid("ECD7206C-DBA2-407d-B77D-725A0E3251B6")]
  [ComVisible(true)]
  public interface IMyGlobalService
  {
    void GlobalServiceFunction();
    int CallLocalService();
  }

  // ================================================================================================
  /// <summary>
  /// This type defines the markup (address) interface of MyGlobalService.
  /// </summary>
  // ================================================================================================
  [Guid("4560D44E-5782-4237-A058-317B3ED801B7")]
  public interface SMyGlobalService
  {
  }
}
