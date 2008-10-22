// ================================================================================================
// MenuCommandAttributes.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel.Design;

namespace VSXtra
{
  #region CommandIdAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the Command Id of a menu command.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = true)]
  public class CommandIdAttribute : Attribute
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
    /// <param name="guidString">Command Guid string</param>
    /// <param name="id">Command identifier.</param>
    // --------------------------------------------------------------------------------------------
    public CommandIdAttribute(string guidString, uint id)
    {
      Id = id;
      Guid = new Guid(guidString);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute with the specified initial value.
    /// </summary>
    /// <param name="guid">Command Guid string</param>
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

  #region VsCommandIdAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the Command Id of a menu command belonging to standard Visual 
  /// Studio commands.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public sealed class VsCommandIdAttribute : CommandIdAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute with the specified initial value.
    /// </summary>
    /// <param name="id">Command identifier.</param>
    // --------------------------------------------------------------------------------------------
    public VsCommandIdAttribute(uint id)
      : base(VSConstants.GUID_VSStandardCommandSet97, id)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute with the specified initial value.
    /// </summary>
    /// <param name="id">Command identifier.</param>
    // --------------------------------------------------------------------------------------------
    public VsCommandIdAttribute(VSConstants.VSStd97CmdID id)
      : base(VSConstants.GUID_VSStandardCommandSet97, (uint)id)
    {
    }
  }

  #endregion

  #region ListCommandIdAttribute

  // ================================================================================================
  /// <summary>
  /// This attribute class defines the Command Id for combo controls. This command is responsible for
  /// getting the combo box list values.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public sealed class ListCommandIdAttribute : CommandIdAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute with the specified initial value.
    /// </summary>
    /// <param name="id">Command identifier.</param>
    // --------------------------------------------------------------------------------------------
    public ListCommandIdAttribute(uint id)
      : base(id)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute with the specified initial value.
    /// </summary>
    /// <param name="guid">Command Guid</param>
    /// <param name="id">Command identifier.</param>
    // --------------------------------------------------------------------------------------------
    public ListCommandIdAttribute(Guid guid, uint id)
      : base(guid, id)
    {
    }
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
  [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
  public abstract class ActionAttribute : Attribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Carries out the actions expected.
    /// </summary>
    /// <param name="package">Package the action is used from</param>
    /// <param name="id">Command ID activating the action.</param>
    // --------------------------------------------------------------------------------------------
    public abstract void ExecuteAction(PackageBase package, CommandID id);
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
