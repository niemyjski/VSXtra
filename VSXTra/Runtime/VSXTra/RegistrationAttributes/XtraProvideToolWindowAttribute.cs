// ================================================================================================
// XtraProvideToolWindowAttribute.cs
//
// Created: 2008.07.09, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Drawing;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using VSXtra.Properties;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// Notifies Visual Studio that a VSPackage owns a tool window.
  /// </summary>
  /// <remarks>
  /// This attribute declares that a package own a tool window.  Visual Studio uses this information 
  /// to handle the positioning and persistance of your window. The attributes on a package do not 
  /// control the behavior of the package, but they can be used by registration tools to register 
  /// the proper information with Visual Studio.
  /// </remarks>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class XtraProvideToolWindowAttribute : RegistrationAttribute
  {
    private readonly string _Name;
    private Type _DockedWith;
    private Rectangle _Position = Rectangle.Empty;

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of the attribute.
    /// </summary>
    /// <param name="toolType">Type of tool window.</param>
    // --------------------------------------------------------------------------------------------
    public XtraProvideToolWindowAttribute(Type toolType)
    {
      ToolType = toolType;
      _Name = ToolType.FullName;
      Style = VsDockStyle.none;
      Orientation = ToolWindowOrientation.none;
      Transient = false;
    }

    #region Properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Default DockStyle for the ToolWindow
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public VsDockStyle Style { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary></summary>
    /// Default horizontal component of the position for the to top left corner of the ToolWindow.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int PositionX
    {
      get { return _Position.X; }
      set { _Position.X = value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary></summary>
    /// Default vertical component of the position for the to top left corner of the ToolWindow.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int PositionY
    {
      get { return _Position.Y; }
      set { _Position.Y = value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Default width of the ToolWindow.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int Width
    {
      get { return _Position.Width; }
      set { _Position.Width = value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Default height of the ToolWindow.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public int Height
    {
      get { return _Position.Height; }
      set { _Position.Height = value; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Default Orientation for the ToolWindow, relative to the window specified by the Window 
    /// Property
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ToolWindowOrientation Orientation { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Type of the ToolWindow.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public Type ToolType { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Default Window that the ToolWindow will be docked with.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public Type Window
    {
      get { return _DockedWith; }
      set
      {
        if (!typeof(IWindowKindGuidType).IsAssignableFrom(value))
        {
          throw new InvalidOperationException(Resources.IWindowKindGuidType_Expected);
        }
        _DockedWith = value;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Default Window that the ToolWindow will be docked with.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool MultiInstances { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Set to true if you want to prevent window from loading on IDE start up. Default is false 
    /// which makes the toolwindow persistent (if the IDE is closed while the window is showing, 
    /// the window will show up the next time the IDE starts).
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool Transient { get; set; }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called to register this attribute with the given context. The context contains the 
    /// location where the registration information should be placed. It also contains such as 
    /// the type being registered, and path information.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void Register(RegistrationContext context)
    {
      context.Log.WriteLine(string.Format(Resources.Culture, Resources.Reg_NotifyToolResource, _Name,
                                          ToolType.GUID.ToString("B")));

      using (Key childKey = context.CreateKey(RegKeyName))
      {
        // --- Package owning this tool window
        childKey.SetValue(string.Empty, context.ComponentType.GUID.ToString("B"));
        if (_Name != null)
          childKey.SetValue("Name", _Name);
        if (Orientation != ToolWindowOrientation.none)
          childKey.SetValue("Orientation", OrientationToString(Orientation));
        if (Style != VsDockStyle.none)
          childKey.SetValue("Style", StyleToString(Style));
        if (_DockedWith.GUID != Guid.Empty)
          childKey.SetValue("Window", _DockedWith.GUID.ToString("B"));
        if (_Position.Width != 0 && _Position.Height != 0)
        {
          string positionString = string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}",
                                                _Position.Left,
                                                _Position.Top,
                                                _Position.Right,
                                                _Position.Bottom);
          childKey.SetValue("Float", positionString);
        }
        if (Transient)
          childKey.SetValue("DontForceCreate", 1);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Unregister this Tool Window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void Unregister(RegistrationContext context)
    {
      context.RemoveKey(RegKeyName);
    }

    #endregion

    #region Private methods and properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The reg key name of this Tool Window.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private string RegKeyName
    {
      get { return string.Format(CultureInfo.InvariantCulture, "ToolWindows\\{0}", ToolType.GUID.ToString("B")); }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts a VsDockStyle enum to string.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static string StyleToString(VsDockStyle style)
    {
      switch (style)
      {
        case VsDockStyle.MDI:
            return "MDI";
        case VsDockStyle.Float:
            return "Float";
        case VsDockStyle.Linked:
            return "Linked";
        case VsDockStyle.Tabbed:
            return "Tabbed";
        case VsDockStyle.AlwaysFloat:
            return "AlwaysFloat";
        case VsDockStyle.none:
            return string.Empty;
        default:
          throw new ArgumentException(string.Format(Resources.Culture, Resources.Attributes_UnknownDockingStyle, style));
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Converts a ToolWindowOrientation enum to string.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private static string OrientationToString(ToolWindowOrientation position)
    {
      switch (position)
      {
        case ToolWindowOrientation.Top:
            return "Top";
        case ToolWindowOrientation.Left:
            return "Left";
        case ToolWindowOrientation.Right:
            return "Right";
        case ToolWindowOrientation.Bottom:
            return "Bottom";
        case ToolWindowOrientation.none:
            return string.Empty;
        default:
          throw new ArgumentException(string.Format(Resources.Culture, Resources.Attributes_UnknownPosition, position));
      }
    }

    #endregion
  }
}