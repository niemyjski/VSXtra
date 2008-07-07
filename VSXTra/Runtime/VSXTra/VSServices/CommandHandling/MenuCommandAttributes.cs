// ================================================================================================
// MenuCommandAttributes.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;

namespace VSXtra
{
  #region CommandIdAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the Command Id of a menu command.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
  public sealed class CommandIdAttribute : Attribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute with the specified initial value.
    /// </summary>
    /// <param name="id">Command identifier.</param>
    // --------------------------------------------------------------------------------------------
    public CommandIdAttribute(uint id)
    {
      Id = id;
      Guid = Guid.Empty;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute with the specified initial value.
    /// </summary>
    /// <param name="guid">Command Guid</param>
    /// <param name="id">Command identifier.</param>
    // --------------------------------------------------------------------------------------------
    public CommandIdAttribute(Guid guid, uint id)
    {
      Id = id;
      Guid = guid;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the uint part of the command ID.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint Id { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the Guid part of the command ID.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public Guid Guid { get; private set; }
  }

  #endregion

  #region ManualBindAttribute

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

  #endregion

  #region ActionAttribute

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
    public abstract void ExecuteAction(MenuCommandHandler handler);
  }

  #endregion

  #region CommandCheckedAttribute

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

  #endregion

  #region CommandEnabledAttribute

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

  #endregion

  #region CommandVisibleAttribute

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

  #endregion

  #region CommandSupportedAttribute

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

  #endregion

  #region CommandTextAttribute

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

  #endregion

  #region CommandParametersDescriptionAttribute

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

  #endregion
}
