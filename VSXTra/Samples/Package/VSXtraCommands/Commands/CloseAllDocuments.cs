// ================================================================================================
// CloseAllDocuments.cs
//
// Created: 2008.07.26, by Istvan Novak (DeepDiver)
// ================================================================================================
using System.ComponentModel;
using VSXtra.Commands;

namespace DeepDiver.VSXtraCommands
{
  public partial class VSXtraCommandGroup
  {
    // ================================================================================================

    #region Nested type: CloseAllDocuments

    /// <summary>
    /// This command closes all open documents.
    /// </summary>
    // ================================================================================================
    [CommandId(CmdIDs.CloseAllDocumentsCommand)]
    [DisplayName("Close All Documents")]
    [ExecuteCommandAction("Window.CloseAllDocuments")]
    public sealed class CloseAllDocuments : CommandHandlerBase
    {
    }

    #endregion
  }
}