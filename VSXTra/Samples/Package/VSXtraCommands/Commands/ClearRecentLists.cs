// ================================================================================================
// ClearRecentFileList.cs
//
// Created: 2008.07.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using VSXtra;
using VSXtra.Commands;
using VSXtra.Package;
using VSXtra.Shell;

namespace DeepDiver.VSXtraCommands
{
  public partial class VSXtraCommandGroup
  {
    // ================================================================================================

    // ================================================================================================

    #region Nested type: ClearRecentFileList

    /// <summary>
    /// This class implements the command handler Clear Recent Files commands.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.ClearRecentFileListCommand)]
    [DisplayName("Clear Recent Files")]
    public sealed class ClearRecentFileList : ClearRecentListBase
    {
      protected override RegistryKey RecentListKey
      {
        get { return VsRegistry.GetRecentFilesListKey(true); }
      }
    }

    #endregion

    #region Nested type: ClearRecentListBase

    /// <summary>
    /// This class implements an abstract command handler class for the Clear Recent Projects and 
    /// Clear Recent Files commands.
    /// </summary>
    // ================================================================================================
    public abstract class ClearRecentListBase : CommandHandlerBase
    {
      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Gets the registry key containing the list of recent objects.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected abstract RegistryKey RecentListKey { get; }

      /// <summary>
      /// Checks if the command should be enabled or not.
      /// </summary>
      /// <param name="command">Command instance.</param>
      /// <returns>True, if the command should be enabled; otherwise, false.</returns>
      // --------------------------------------------------------------------------------------------
      protected override bool CanExecute(OleMenuCommand command)
      {
        return VsRegistry.GetFileEntryList(RecentListKey).Count > 0;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Executes the command.
      /// </summary>
      /// <param name="command">Command instance.</param>
      /// <remarks>
      /// First saves all open documents then deletes the list and restarts Visual Studio.
      /// </remarks>
      // --------------------------------------------------------------------------------------------
      protected override void OnExecute(OleMenuCommand command)
      {
        var view = new ClearListView();
        VsRegistry.GetFileEntryList(RecentListKey).
          ForEach(item => view.Model.ListEntries.Add(item));
        if ((bool) view.ShowDialog())
        {
          VsIde.File.SaveAll();
          DeleteRecentFileList(view.Model.SelectedListEntries);
          VsIde.RestartVS();
        }
      }

      // --------------------------------------------------------------------------------------------

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Deletes the list of files passed in.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      private void DeleteRecentFileList(List<FileEntry> entriesToDelete)
      {
        using (RegistryKey key = RecentListKey)
        {
          // --- Remove entries from the registry marked by the user to delete
          entriesToDelete.ForEach(entry => key.DeleteValue(entry.Key));

          // --- Reenumerate the items, if there are any
          string[] valueNames = key.GetValueNames();
          if (valueNames.Length <= 0) return;

          int fileCounter = 1;

          // --- Rename items 
          valueNames.ForEach(
            valueName =>
              {
                key.SetValue(string.Concat(valueName, "_"), key.GetValue(valueName));
                key.DeleteValue(valueName);
              });

          // --- Reorder items
          key.GetValueNames().ForEach(
            valueName =>
              {
                key.SetValue(string.Format("File{0}", fileCounter++), key.GetValue(valueName));
                key.DeleteValue(valueName);
              });
        }
      }
    }

    #endregion

    // ================================================================================================

    #region Nested type: ClearRecentProjectList

    /// <summary>
    /// This class implements the command handler Clear Recent Projects commands.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.ClearRecentProjectListCommand)]
    [DisplayName("Clear Recent Projects")]
    public sealed class ClearRecentProjectList : ClearRecentListBase
    {
      protected override RegistryKey RecentListKey
      {
        get { return VsRegistry.GetRecentProjectsListKey(true); }
      }
    }

    #endregion
  }
}