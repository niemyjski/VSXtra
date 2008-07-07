// ================================================================================================
// CommandDispatcherAttributes.cs
//
// Created: 2008.07.06, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using VSXtra.Properties;

namespace VSXtra
{
  #region DefaultCommandGroupAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute is used to define the type providing a default command group attribute.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class DefaultCommandGroupAttribute : TypeAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an attribute instance with its initial value.
    /// </summary>
    /// <param name="value">Initial value.</param>
    // --------------------------------------------------------------------------------------------
    public DefaultCommandGroupAttribute(Type value)
      : base(value)
    {
      if (!typeof(ICommandGroupProvider).IsAssignableFrom(value))
      {
        throw new InvalidOperationException(Resources.ICommandGroupProvider_Expected);
      }
    }
  }

  #endregion

  #region CommandMethodAttribute

  // ================================================================================================
  /// <summary>
  /// This abstract attribute defines a base class for all command method attributes.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Method)]
  public abstract class CommandMethodAttribute : Attribute
  { }

  #endregion

  #region CommandStatusMethodAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute marks a method as a "Status Query" command method.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class CommandStatusMethodAttribute : CommandMethodAttribute
  { }

  #endregion

  #region CommandExecMethodAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute marks a method as an "Execute" command method.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class CommandExecMethodAttribute : CommandMethodAttribute
  { }

  #endregion

  #region CommandChangeMethodAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute marks a method as a "Change" command method.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class CommandChangeMethodAttribute : CommandMethodAttribute
  { }

  #endregion

  #region PromoteCommandAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute marks a command method to be promoted to the parent level.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class PromoteCommandAttribute : BoolAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an attribute instance with its initial value.
    /// </summary>
    /// <param name="value">Intial attribute value.</param>
    // --------------------------------------------------------------------------------------------
    public PromoteCommandAttribute(bool value)
      : base(value)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an attribute instance with "true" value.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public PromoteCommandAttribute()
      : base(true)
    {
    }
  }

  #endregion
}