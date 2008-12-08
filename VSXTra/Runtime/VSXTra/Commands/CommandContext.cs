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
  }
}
