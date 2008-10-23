// ================================================================================================
// MyGlobalService.cs
//
// Created: 2008.07.20, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra;
using VSXtra.Package;
using VSXtra.Windows;

namespace DeepDiver.ServicesReference
{
  // ================================================================================================
  /// <summary>
  /// This is the class that implements the global service.
  /// </summary>
  // ================================================================================================
  [Promote]
  public class MyGlobalService : VsxService<ServicesPackage, SMyGlobalService>,
    IMyGlobalService
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Implementation of the function that does not access the local service.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void GlobalServiceFunction()
    {
      var outputText = " ======================================\n" +
                       "\tGlobalServiceFunction called.\n" +
                       " ======================================\n";
      OutputWindow.General.WriteLine(outputText);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Implementation of the function that will call a method of the local service.
    /// Notice that this class will access the local service using as service provider the one
    /// implemented by ServicesPackage.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int CallLocalService()
    {
      // --- Query the service provider for the local service. This object is supposed to be 
      // --- build by ServicesPackage and it pass its service provider to the constructor, 
      // --- so the local service should be found.
      var localService = ServiceProvider.GetService<SMyLocalService, IMyLocalService>();
      if (null == localService)
      {
        // --- The local service was not found; write a message on the debug output and exit.
        OutputWindow.Debug.WriteLine("Can not get the local service from the global one.");
        return -1;
      }

      // --- Now call the method of the local service. This will write a message on the output window.
      return localService.LocalServiceFunction();
    }
  }
}
