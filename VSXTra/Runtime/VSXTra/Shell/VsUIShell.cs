// ================================================================================================
// VsUIShell.cs
//
// Created: 2008.07.11, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Package;

namespace VSXtra.Shell
{
  /// <summary>
  /// This static class is a wrapper class around the SvSUIShell operations.
  /// </summary>
  // ================================================================================================
  public static class VsUIShell
  {
    #region Public methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Asynchronously sends status update messages to command UI elements.
    /// </summary>
    // --------------------------------------------------------------------------------
    public static void UpdateCommandUI()
    {
      UIShell.UpdateCommandUI(0);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Helper method used by editors that implement the IVsPersistDocData interface.
    /// </summary>
    /// <param name="grfSave">Specifies file Save options.</param>
    /// <param name="pPersistFile">
    /// Pointer to the IUnknown interface of the file in which the doc data is to 
    /// be saved.
    /// </param>
    /// <param name="pszUntitledPath">
    /// File path to which the doc data for an as-yet unsaved document is to be saved.
    /// </param>
    /// <param name="pbstrDocumentNew">New document file name.</param>
    /// <param name="pfCanceled">
    /// Set to true if the user aborts the save by clicking the Cancel button.
    /// </param>
    /// <returns>
    /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static int SaveDocDataToFile(VSSAVEFLAGS grfSave, object pPersistFile,
                                        string pszUntitledPath, out string pbstrDocumentNew, out int pfCanceled)
    {
      return UIShell.SaveDocDataToFile(grfSave, pPersistFile, pszUntitledPath,
                                       out pbstrDocumentNew, out pfCanceled);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Changes the cursor to the hourglass cursor.
    /// </summary>
    // --------------------------------------------------------------------------------
    public static void SetWaitCursor()
    {
      UIShell.SetWaitCursor();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// This method shows up the property window.
    /// </summary>
    // --------------------------------------------------------------------------------
    public static void ShowPropertiesWindow()
    {
      var propertyBrowser = new Guid(ToolWindowGuids.PropertyBrowser);
      IVsWindowFrame propFrame;
      UIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate,
                             ref propertyBrowser, out propFrame);
      if (propFrame != null) propFrame.Show();
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