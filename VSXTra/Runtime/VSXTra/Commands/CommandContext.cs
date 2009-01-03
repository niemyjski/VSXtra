// ================================================================================================
// CommandContext.cs
//
// Created: 2008.12.03, by Istvan Novak (DeepDiver)
// ================================================================================================
using VSXtra.Package;

namespace VSXtra.Commands
{
  // ================================================================================================
  /// <summary>
  /// This object represents the context of a command. It can be passed to VS command status query 
  /// or command execution methods.
  /// </summary>
  // ================================================================================================
  public class CommandContext
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandContext"/> class.
    /// </summary>
    /// <param name="package">The package belonging to this context.</param>
    // --------------------------------------------------------------------------------------------
    public CommandContext(PackageBase package)
    {
      Package = package;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package belonging to this command context.
    /// </summary>
    /// <value>The package.</value>
    // --------------------------------------------------------------------------------------------
    public PackageBase Package { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the IOleCommandTarget return status was explicitly set or not.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool ExplicitReturnStatusSet { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the IOleCommandTarget return status.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int ReturnStatus { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Sets the IOleCommandTarget return status.
    /// </summary>
    /// <param name="status">The status to set.</param>
    // --------------------------------------------------------------------------------------------
    public void SetReturnStatus(int status)
    {
      ExplicitReturnStatusSet = true;
      ReturnStatus = status;
    }
  }
}
