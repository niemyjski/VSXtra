// ================================================================================================
// MyLocalService.cs
//
// Created: 2008.07.20, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Package;
using VSXtra.Windows;

namespace DeepDiver.ServicesReference
{
  // ================================================================================================
  /// <summary>
  /// This is the class that implements the local service. It implements IMyLocalService
  /// because this is the interface that we want to use, but it also implements the empty
  /// interface SMyLocalService in order to notify the service creator that it actually
  /// implements this service.
  /// </summary>
  // ================================================================================================
  [AutoCreateService]
  public class MyLocalService : VsxService<ServicesPackage, SMyLocalService>,
                                IMyLocalService
  {
    #region IMyLocalService Members

    public int LocalServiceFunction()
    {
      string outputText = " ======================================\n" +
                          "\tLocalServiceFunction called.\n" +
                          " ======================================\n";
      OutputWindow.General.WriteLine(outputText);
      return 0;
    }

    #endregion
  }
}