// ================================================================================================
// MonitorSelection.cs
//
// Created: 2008.12.03, by Istvan Novak (DeepDiver)
// ================================================================================================
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Package;

namespace VSXtra.Selection
{
  // ================================================================================================
  /// <summary>
  /// This class provides a singleton object for IVsMonitorSelection functionality.
  /// </summary>
  // ================================================================================================
  public static class MonitorSelection
  {
    #region Private fields

    private static IVsMonitorSelection _monitor;

    #endregion

    #region Public Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the IVsMonitorSelection service object behind this class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static IVsMonitorSelection Monitor
    {
      get
      {
        if (_monitor == null)
            _monitor = PackageBase.GetGlobalService<SVsShellMonitorSelection>() as IVsMonitorSelection;
        return _monitor;
      }
    }

    #endregion
  }
}