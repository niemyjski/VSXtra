// ================================================================================================
// MenuCommandHandler.cs
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using VSXtra.Properties;

namespace VSXtra
{
  // ====================================================================================
  /// <summary>
  /// This interface signs that a class is used as a command group for its nested
  /// menu command handler types. The GUID assigned to the command group type is used
  /// when binding the nested command handler types.
  /// </summary>
  // ====================================================================================
  public interface ICommandGroupProvider
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gtes the package instance that owns the command group.
    /// </summary>
    /// <returns>
    /// Package instance, if package has been sited; otherwise, null.
    /// </returns>
    // --------------------------------------------------------------------------------------------
    PackageBase GetPackageInstance();
  }

  // ====================================================================================
  /// <summary>
  /// This abstract class is intended to be the base class for simple menu command 
  /// handlers.
  /// </summary>
  /// <remarks>
  /// A simple menu command handler contains methods to execute the command or respond to
  /// the status query or change events.
  /// </remarks>
  // ====================================================================================
  public abstract class MenuCommandHandler
  {
    #region Private fields

    private readonly PackageBase _Package;
    private readonly CommandID _CommandId;
    private OleMenuCommand _MenuCommand;
    private readonly ActionAttribute _CommandAction;
    private readonly CommandCheckedAttribute _CheckedAttribute;
    private readonly CommandEnabledAttribute _EnabledAttribute;
    private readonly CommandParametersDescriptionAttribute _DescriptionsAttribute;
    private readonly CommandSupportedAttribute _SupportedAttribute;
    private readonly CommandVisibleAttribute _VisibleAttribute;
    private readonly CommandTextAttribute _TextAttribute;

    private static readonly Dictionary<Type, MenuCommandHandler> _Handlers = 
      new Dictionary<Type, MenuCommandHandler>();

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an instance of this menu command handler class.
    /// </summary>
    /// <remarks>
    /// This constructor reads the <see cref="CommandIdAttribute"/> used to decorate
    /// this class in order to obtain command ID information.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    protected internal MenuCommandHandler()
    {
      // --- Check for command group containment
      Type thisType = GetType();
      if (thisType.DeclaringType == null || 
        !typeof(ICommandGroupProvider).IsAssignableFrom(thisType.DeclaringType))
      {
        throw new InvalidOperationException(Resources.CommandHandler_NoGroupType);
      }

      // --- Obtain the package type
      var commandProvider = Activator.CreateInstance(thisType.DeclaringType) as ICommandGroupProvider;
      if (commandProvider == null)
      {
        throw new InvalidOperationException(Resources.CommandHandler_NoProvider);
      }
      _Package = commandProvider.GetPackageInstance();
      if (_Package == null)
      {
        throw new InvalidOperationException(Resources.CommandHandler_NoPackage);
      }

      // --- Obtain command GUID
      var commandGuid = Attribute.IsDefined(GetType(), typeof(GuidAttribute))
        ? thisType.GUID
        : thisType.DeclaringType.GUID;

      // --- Obtain attribute values
      foreach (object attr in GetType().GetCustomAttributes(false))
      {
        var idAttr = attr as CommandIdAttribute;
        if (idAttr != null && attr.GetType() == typeof(CommandIdAttribute))
        {
          if (idAttr.Guid != Guid.Empty) commandGuid = idAttr.Guid;
          _CommandId = new CommandID(commandGuid, (int)idAttr.Id);
        }
        var actionAttr = attr as ActionAttribute;
        if (actionAttr != null) _CommandAction = actionAttr;
        var checkedAttr = attr as CommandCheckedAttribute;
        if (checkedAttr != null) _CheckedAttribute = checkedAttr;
        var enabledAttr = attr as CommandEnabledAttribute;
        if (enabledAttr != null) _EnabledAttribute = enabledAttr;
        var suppAttr = attr as CommandSupportedAttribute;
        if (suppAttr != null) _SupportedAttribute = suppAttr;
        var visibleAttr = attr as CommandVisibleAttribute;
        if (visibleAttr != null) _VisibleAttribute = visibleAttr;
        var textAttr = attr as CommandTextAttribute;
        if (textAttr != null) _TextAttribute = textAttr;
        var descrAttr = attr as CommandParametersDescriptionAttribute;
        if (descrAttr != null) _DescriptionsAttribute = descrAttr;
      }
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the Guid part of the CommandID belonging to the menu command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public Guid Guid
    {
      get { return _CommandId == null ? Guid.Empty : _CommandId.Guid; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the Id part of the CommandID belonging to the menu command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int Id
    {
      get { return _CommandId == null ? 0 : _CommandId.ID; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the CommandID belonging to the menu command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public CommandID CommandId
    {
      get { return _CommandId; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the package owning this menu command.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public PackageBase Package
    {
      get { return _Package; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the service provider belonging to the owner package.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IServiceProvider ServiceProvider
    {
      get { return _Package; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the OleMenuCommand instance belonging to this instance.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public OleMenuCommand MenuCommand
    {
      get { return _MenuCommand; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the menu command has already bound to a menu item.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool IsBound
    {
      get { return _MenuCommand != null; }
    }

    #endregion

    #region Static methods and properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the menu command handler of the specified type.
    /// </summary>
    /// <typeparam name="THandler">Type of menu command handler to obtain</typeparam>
    /// <returns>Menu command handler instance</returns>
    // --------------------------------------------------------------------------------------------
    public static THandler GetHandler<THandler>()
      where THandler: MenuCommandHandler
    {
      MenuCommandHandler result;
      _Handlers.TryGetValue(typeof (THandler), out result);
      return result as THandler;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves an iterator for the registered command menu handlers.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<Type> GetRegisteredHandlerTypes<TPackage>()
      where TPackage: PackageBase
    {
      return
        from type in _Handlers.Keys
        where type.BaseType.GenericParameterOfType(typeof (CommandGroup<>), 0) == 
          typeof (TPackage)
        select type;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Retrieves an iterator for the registered command menu handlers.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public static IEnumerable<MenuCommandHandler> GetRegisteredHandlerInstances<TPackage>()
      where TPackage : PackageBase
    {
      return
        from handler in _Handlers.Values
        where handler.GetType().DeclaringType.GenericParameterOfType(typeof(CommandGroup<>), 0) ==
          typeof(TPackage)
        select handler;
    }

    #endregion

    #region Methods to override

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Callback method called when the command is to be executed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void ExecuteMenuCommandCallback(object sender, EventArgs e)
    {
      var command = sender as OleMenuCommand;
      if (command != null) OnExecute(command);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to define how the command should be executed.
    /// </summary>
    /// <param name="command">OleMenuCommand instance</param>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnExecute(OleMenuCommand command)
    {
      if (_CommandAction != null) _CommandAction.ExecuteAction(this);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to define how the command status should be queried.
    /// </summary>
    /// <param name="command">OleMenuCommand instance</param>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnQueryStatus(OleMenuCommand command)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to define how to respond command property changes.
    /// </summary>
    /// <param name="command">OleMenuCommand instance</param>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnChange(OleMenuCommand command)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to set up the command instance before it gets registered
    /// with the OleMenuCommandService.
    /// </summary>
    /// <param name="command">OleMenuCommand instance</param>
    // --------------------------------------------------------------------------------------------
    protected virtual void BeforeBind(OleMenuCommand command)
    {
      if (_CheckedAttribute != null) command.Checked = _CheckedAttribute.Value;
      if (_EnabledAttribute != null) command.Enabled = _EnabledAttribute.Value;
      if (_SupportedAttribute != null) command.Supported = _SupportedAttribute.Value;
      if (_VisibleAttribute != null) command.Visible = _VisibleAttribute.Value;
      if (_TextAttribute != null) command.Text = _TextAttribute.Value;
      if (_DescriptionsAttribute != null)
        command.ParametersDescription = _DescriptionsAttribute.Value;
    }

    #endregion

    #region Internal methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Binds the command to the related menu and toolbar items.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual void Bind()
    {
      if (_Package == null) return;
      var mcs = ServiceProvider.GetService<IMenuCommandService, OleMenuCommandService>();
      if (mcs == null) return;
      _MenuCommand = new OleMenuCommand(
        ExecuteMenuCommandCallback, 
        ChangeCallback, 
        BeforeStatusQueryCallback, 
        _CommandId);
      BeforeBind(_MenuCommand);
      _Handlers.Add(GetType(), this);
      mcs.AddCommand(_MenuCommand);
    }

    #endregion

    #region Private event handler methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Callback method called when the command is about to be changed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void ChangeCallback(object sender, EventArgs e)
    {
      var command = sender as OleMenuCommand;
      if (command != null) OnChange(command);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Callback method called when the command status is queried.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void BeforeStatusQueryCallback(object sender, EventArgs e)
    {
      var command = sender as OleMenuCommand;
      if (command != null) OnQueryStatus(command);
    }

    #endregion
  }
}
