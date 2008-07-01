// ================================================================================================
// MenuCommandAttributes.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio.Shell;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This attribute class defines the Command Id of a menu command.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CommandIdAttribute : Attribute
  {
    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute with the specified initial value.
    /// </summary>
    /// <param name="command">Command identifier.</param>
    // --------------------------------------------------------------------------------------------
    public CommandIdAttribute(uint command)
    {
      Value = command;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the uint part ofthe command ID.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint Value { get; private set; }

    #endregion
  }

  // ================================================================================================
  /// <summary>
  /// This attribute signs that a menu command handler is to be bound with the 
  /// OleMenuCommandService manually.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ManualBindAttribute : Attribute
  {
  }

  // ================================================================================================
  /// <summary>
  /// This attribute is intended to the base class of all action attributes.
  /// </summary>
  /// <remarks>
  /// Action attributes provide a default action for a command handler.
  /// </remarks>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public abstract class ActionAttribute : Attribute
  {
    public abstract void ExecuteAction(OleMenuCommand command);
  }

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the initial value of the Checked property.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CommandCheckedAttribute : BoolAttribute
  {
    public CommandCheckedAttribute(bool value)
      : base(value)
    {
    }
  }

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the initial value of the Enabled property.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CommandEnabledAttribute : BoolAttribute
  {
    public CommandEnabledAttribute(bool value)
      : base(value)
    {
    }
  }

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the initial value of the Visible property.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CommandVisibleAttribute : BoolAttribute
  {
    public CommandVisibleAttribute(bool value)
      : base(value)
    {
    }
  }

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the initial value of the Supported property.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CommandSupportedAttribute : BoolAttribute
  {
    public CommandSupportedAttribute(bool value)
      : base(value)
    {
    }
  }

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the initial value of the Text property.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CommandTextAttribute : StringAttribute
  {
    public CommandTextAttribute(string value)
      : base(value)
    {
    }
  }

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the initial value of the ParametersDescription property.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class CommandParametersDescriptionAttribute : StringAttribute
  {
    public CommandParametersDescriptionAttribute(string value)
      : base(value)
    {
    }
  }

}
