// ================================================================================================
// MenuCommandAttributes.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  // ==================================================================================
  /// <summary>
  /// This attribute class defines the Command Id of a menu command.
  /// </summary>
  // ==================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CommandIdAttribute : Attribute
  {
    #region Lifecycle methods

    // ------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute with the specified initial value.
    /// </summary>
    /// <param name="command">Command identifier.</param>
    // ------------------------------------------------------------------------------
    public CommandIdAttribute(uint command)
    {
      Value = command;
    }

    #endregion

    #region Public properties

    // ------------------------------------------------------------------------------
    /// <summary>
    /// Gets the uint part ofthe command ID.
    /// </summary>
    // ------------------------------------------------------------------------------
    public uint Value { get; private set; }

    #endregion
  }

  // ==================================================================================
  /// <summary>
  /// This attribute signs that a menu command handler is to be bound with the 
  /// OleMenuCommandService manually.
  /// </summary>
  // ==================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ManualBindAttribute : Attribute
  {
  }
}
