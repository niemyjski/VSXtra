// ================================================================================================
// VsxConverter.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra.Diagnostics;

namespace VSXtra
{
  // ==================================================================================
  /// <summary>
  /// This static class provides conversion functions between interop types and managed
  /// VSX types.
  /// </summary>
  // ==================================================================================
  public static class VsxConverter
  {
    // --------------------------------------------------------------------------------
    /// <summary>
    /// Converts <see cref="MessageBoxButtons"/> values to corresponding 
    /// <see cref="OLEMSGBUTTON"/> values.
    /// </summary>
    /// <param name="buttons">MessageBoxButtons value to convert</param>
    /// <returns>
    /// OLEMSGBUTTON representation of the MessageBoxButtons input.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static OLEMSGBUTTON ConvertToOleMsgButton(MessageBoxButtons buttons)
    {
      switch (buttons)
      {
        case MessageBoxButtons.AbortRetryIgnore:
          return OLEMSGBUTTON.OLEMSGBUTTON_ABORTRETRYIGNORE;
        case MessageBoxButtons.OKCancel:
          return OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL;
        case MessageBoxButtons.RetryCancel:
          return OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL;
        case MessageBoxButtons.YesNo:
          return OLEMSGBUTTON.OLEMSGBUTTON_YESNO;
        case MessageBoxButtons.YesNoCancel:
          return OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL;
        default:
          return OLEMSGBUTTON.OLEMSGBUTTON_OK;
      }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Converts <see cref="MessageBoxDefaultButton"/> values to corresponding
    /// <see cref="OLEMSGDEFBUTTON"/> values.
    /// </summary>
    /// <param name="button">MessageBoxDefaultButton value to convert</param>
    /// <returns>
    /// OLEMSGDEFBUTTON representation of the MessageBoxDefaultButton input.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static OLEMSGDEFBUTTON ConvertToOleMsgDefButton(MessageBoxDefaultButton button)
    {
      switch (button)
      {
        case MessageBoxDefaultButton.Button1:
          return OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
        case MessageBoxDefaultButton.Button2:
          return OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND;
        case MessageBoxDefaultButton.Button3:
          return OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_THIRD;
        default:
          return OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FOURTH;
      }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Converts <see cref="MessageBoxIcon"/> values to corresponding
    /// <see cref="OLEMSGICON"/> values.
    /// </summary>
    /// <param name="icon">MessageBoxIcon value to convert</param>
    /// <returns>
    /// OLEMSGICON representation of the MessageBoxIcon input.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static OLEMSGICON ConvertToOleMsgIcon(MessageBoxIcon icon)
    {
      switch (icon)
      {
        case MessageBoxIcon.Asterisk:
          return OLEMSGICON.OLEMSGICON_INFO;
        case MessageBoxIcon.Error:
          return OLEMSGICON.OLEMSGICON_CRITICAL;
        case MessageBoxIcon.Exclamation:
          return OLEMSGICON.OLEMSGICON_WARNING;
        case MessageBoxIcon.Question:
          return OLEMSGICON.OLEMSGICON_QUERY;
        default:
          return OLEMSGICON.OLEMSGICON_NOICON;
      }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Converts the WIN32 dialog result values to DialogResult enumeration values.
    /// </summary>
    /// <param name="value">Integer value to convert</param>
    /// <returns>
    /// DialogResult representation.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static DialogResult Win32ResultToDialogResult(int value)
    {
      switch (value)
      {
        case 1:
          return DialogResult.OK;
        case 2:
          return DialogResult.Cancel;
        case 3:
          return DialogResult.Abort;
        case 4:
          return DialogResult.Retry;
        case 5:
          return DialogResult.Ignore;
        case 6:
          return DialogResult.Yes;
        default:
          return DialogResult.No;
      }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Converts <see cref="ActivityLogEntryType"/> values to corresponding UInt32 values.
    /// </summary>
    /// <param name="logType">ActivitiLogType value to convert.</param>
    /// <returns>
    /// UInt32 representation of the ActivityLogEntryType input.
    /// </returns>
    // --------------------------------------------------------------------------------
    public static UInt32 MapLogTypeToAle(ActivityLogEntryType logType)
    {
      switch (logType)
      {
        case ActivityLogEntryType.Information:
          return (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION;
        case ActivityLogEntryType.Warning:
          return (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_WARNING;
        default:
          return (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR;
      }
    }
  }
}
