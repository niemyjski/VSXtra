// ================================================================================================
// VsUIShell.cs
//
// Created: 2008.07.11, by Istvan Novak (DeepDiver)
// ================================================================================================
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This static class is a wrapper class around the SvSUIShell operations.
  /// </summary>
  // ================================================================================================
  public static class VsUIShell
  {
    #region Public methods

    public static void UpdateCommandUI()
    {
      UIShell.UpdateCommandUI(0);
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the IVsUIShell service instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static IVsUIShell UIShell
    {
      get { return PackageBase.GetGlobalService<SVsUIShell, IVsUIShell>(); }
    }

    #endregion
  }
}