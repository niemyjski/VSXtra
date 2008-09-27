// ================================================================================================
// IMyLocalService.cs
//
// Created: 2008.07.20, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Runtime.InteropServices;

namespace DeepDiver.ServicesReference
{
  // ================================================================================================
  /// <summary>
  /// This type defines the functional interface of MyLocalService.
  /// </summary>
  // ================================================================================================
  [Guid("54DB054C-2AE0-4204-915F-6B7EA96FC63E")]
  [ComVisible(true)]
  public interface IMyLocalService
  {
    int LocalServiceFunction();
  }

  // ================================================================================================
  /// <summary>
  /// This type defines the markup (address) interface of MyLocalService.
  /// </summary>
  // ================================================================================================
  [Guid("E582E3A6-24AD-41d6-9E7C-88255BD60627")]
  public interface SMyLocalService
  {
  }
}
