// ================================================================================================
// Toolbar.cs
//
// Created: 2008.07.05, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.ComponentModel.Design;

namespace VSXtra.Commands
{
  // ================================================================================================
  /// <summary>
  /// This markup interface indicates that the implementing class represents a toolbar.
  /// </summary>
  // ================================================================================================
  public interface IToolbarProvider
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the CommandID representing the toolbar.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    CommandID CommandId { get; }
  }

  // ================================================================================================
  /// <summary>
  /// This abstract class is intended to be the base class of a toolbar definition
  /// </summary>
  // ================================================================================================
  public abstract class ToolbarDefinition : IToolbarProvider
  {
    private readonly CommandID _CommandId;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an instance of a new toolbar definition.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected ToolbarDefinition()
    {
      // --- Check for command group containment
      Type thisType = GetType();

      // --- Obtain command GUID
      var commandGuid = thisType.DeclaringType != null &&
                        typeof(ICommandGroupProvider).IsAssignableFrom(thisType.DeclaringType)
                          ? thisType.DeclaringType.GUID
                          : thisType.GUID;

      foreach (var attr in GetType().GetCustomAttributes(false))
      {
        var idAttr = attr as CommandIdAttribute;
        if (idAttr != null)
        {
          _CommandId = new CommandID(commandGuid, (int)idAttr.Id);
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the CommandID representing this toolbar.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public CommandID CommandId
    {
      get { return _CommandId; }
    }
  }
}