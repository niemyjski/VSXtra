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

  #region CommandMapAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute defines a command map.
  /// </summary>
  /// <remarks>
  /// This attribute can be attached to a class to refine command mappings used there.
  /// </remarks>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class CommandMapAttribute: Attribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new command mapping with the specified parameters.
    /// </summary>
    /// <param name="guid">Guid of the related mapping.</param>
    /// <param name="idFrom">Start of ID range mapped.</param>
    /// <param name="idTo">End of ID range mapped.</param>
    /// <param name="offset">Offset used to define the new maping.</param>
    // --------------------------------------------------------------------------------------------
    public CommandMapAttribute(Guid guid, uint idFrom, uint idTo, int offset)
    {
      Guid = guid;
      IdFrom = idFrom;
      IdTo = idTo;
      Offset = offset;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new command mapping with the specified parameters.
    /// </summary>
    /// <param name="idFrom">Start of ID range mapped.</param>
    /// <param name="idTo">End of ID range mapped.</param>
    /// <param name="offset">Offset used to define the new maping.</param>
    // --------------------------------------------------------------------------------------------
    public CommandMapAttribute(uint idFrom, uint idTo, int offset)
    {
      Guid = Guid.Empty;
      IdFrom = idFrom;
      IdTo = idTo;
      Offset = offset;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the Guid of the related mapping.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public Guid Guid { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the start of ID range mapped.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint IdFrom { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the end of ID range mapped.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public uint IdTo { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the offset used for mapping.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int Offset { get; private set; }
  }

  #endregion
}