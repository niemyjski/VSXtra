// ================================================================================================
// VsMessageBox.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This static class is a wrapper class around the message box functionality of the SvSUIShell 
  /// service.
  /// </summary>
  // ================================================================================================
  public static class VsMessageBox
  {
    #region MessageBox methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays a Visual Studio message box.
    /// </summary>
    /// <param name="text">Message text</param>
    /// <returns>
    /// Result of the MessageBox.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static DialogResult Show(string text)
    {
      return ShowInternal(null, text, string.Empty, 0, MessageBoxButtons.OK,
        MessageBoxDefaultButton.Button1, MessageBoxIcon.Information, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays a Visual Studio message box.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="text">Message text</param>
    /// <returns>
    /// Result of the MessageBox.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static DialogResult Show(string text, string title)
    {
      return ShowInternal(title, text, string.Empty, 0, MessageBoxButtons.OK,
        MessageBoxDefaultButton.Button1, MessageBoxIcon.Information, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays a Visual Studio message box.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="text">Message text</param>
    /// <param name="buttons">Buttons to show on the message box</param>
    /// <returns>
    /// Result of the MessageBox.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static DialogResult Show(string text, string title, 
      MessageBoxButtons buttons)
    {
      return ShowInternal(title, text, string.Empty, 0, buttons,
        MessageBoxDefaultButton.Button1, MessageBoxIcon.Information, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays a Visual Studio message box.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="text">Message text</param>
    /// <param name="buttons">Buttons to show on the message box</param>
    /// <param name="icon">Icon to display in the message box.</param>
    /// <returns>
    /// Result of the MessageBox.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static DialogResult Show(string text, string title, 
      MessageBoxButtons buttons, MessageBoxIcon icon)
    {
      return ShowInternal(title, text, string.Empty, 0, buttons,
        MessageBoxDefaultButton.Button1, icon, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays a Visual Studio message box.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="text">Message text</param>
    /// <param name="buttons">Buttons to show on the message box</param>
    /// <param name="defaultButton">Default message box button.</param>
    /// <param name="icon">Icon to display in the message box.</param>
    /// <returns>
    /// Result of the MessageBox.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static DialogResult Show(string text, string title, 
      MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
    {
      return ShowInternal(title, text, string.Empty, 0, buttons,
        defaultButton, icon, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays a Visual Studio message box.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="text">Message text</param>
    /// <param name="helpFilePath">Help file name</param>
    /// <param name="buttons">Buttons to show on the message box</param>
    /// <param name="defaultButton">Default message box button.</param>
    /// <param name="icon">Icon to display in the message box.</param>
    /// <returns>
    /// Result of the MessageBox.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static DialogResult Show(string text, string title, 
      MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, 
      string helpFilePath)
    {
      return ShowInternal(title, text, helpFilePath, 0, buttons,
        defaultButton, icon, false);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays a Visual Studio message box.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="text">Message text</param>
    /// <param name="helpFilePath">Help file name</param>
    /// <param name="helpTopic">Help topic identifier</param>
    /// <param name="buttons">Buttons to show on the message box</param>
    /// <param name="defaultButton">Default message box button.</param>
    /// <param name="icon">Icon to display in the message box.</param>
    /// <returns>
    /// Result of the MessageBox.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    public static DialogResult Show(string text, string title, 
      MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, 
      string helpFilePath, uint helpTopic)
    {
      return ShowInternal(title, text, helpFilePath, helpTopic, buttons,
        defaultButton, icon, false);
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

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Displays a Visual Studio message box.
    /// </summary>
    /// <param name="title">Title of the message</param>
    /// <param name="message">Message text</param>
    /// <param name="helpFile">Help file name</param>
    /// <param name="helpTopic">Help topic identifier</param>
    /// <param name="buttons">Buttons to show on the message box</param>
    /// <param name="defButton">Default message box button.</param>
    /// <param name="icon">Icon to display in the message box.</param>
    /// <param name="sysAlert">MB_SYSTEMMODAL flag</param>
    /// <returns>
    /// MessageBox result converted to DialogResult.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    private static DialogResult ShowInternal(string title, string message, string helpFile, 
      uint helpTopic, MessageBoxButtons buttons, MessageBoxDefaultButton defButton,
      MessageBoxIcon icon, bool sysAlert)
    {
      Guid clsid = Guid.Empty;
      int result;
      ErrorHandler.ThrowOnFailure(UIShell.ShowMessageBox(
                 0,
                 ref clsid,
                 title,
                 message, 
                 helpFile,
                 helpTopic,
                 VsxConverter.ConvertToOleMsgButton(buttons),
                 VsxConverter.ConvertToOleMsgDefButton(defButton),
                 VsxConverter.ConvertToOleMsgIcon(icon),
                 sysAlert ? 1 : 0,
                 out result));
      return VsxConverter.Win32ResultToDialogResult(result);
    }

    #endregion
  }
}
