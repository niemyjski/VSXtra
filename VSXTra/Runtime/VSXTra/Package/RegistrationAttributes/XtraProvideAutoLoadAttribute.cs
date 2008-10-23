// ================================================================================================
// ProvideAutoloadAttribute.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new Managed Package Framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2008.06.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using VSXtra.Properties;

namespace VSXtra.Package
{
  // ================================================================================================
  /// <summary>
  /// This attribute registers the package as an extender. The GUID passed in determines what is 
  /// being extended. The attributes on a package do not control the behavior of the package, but 
  /// they can be used by registration tools to register the proper information with Visual Studio.
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class XtraProvideAutoLoadAttribute : RegistrationAttribute
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Specifies that the package should get loaded when this context is active.
    /// </summary>
    /// <param name="type">
    /// Type representing the GUID of context which should trigger the loading of your package.
    /// </param>
    // --------------------------------------------------------------------------------------------
    public XtraProvideAutoLoadAttribute(Type type)
    {
      if (!typeof(IUIContextGuidType).IsAssignableFrom(type))
      {
        throw new ArgumentException(Resources.IUIContextGuidType_Expected);
      }
      LoadGuid = type.GUID;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Context Guid which triggers the loading of the package.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public Guid LoadGuid { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The reg key name of this AutoLoad.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private string RegKeyName
    {
      get
      {
        return string.Format(CultureInfo.InvariantCulture, 
          "AutoLoadPackages\\{0}", LoadGuid.ToString("B"));
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called to register this attribute with the given context. The context contains the 
    /// location where the registration information should be placed. It also contains such as 
    /// the type being registered, and path information.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void Register(RegistrationContext context)
    {
      context.Log.WriteLine(string.Format(Resources.Culture, Resources.Reg_NotifyAutoLoad, 
        LoadGuid.ToString("B")));
      using (Key childKey = context.CreateKey(RegKeyName))
      {
        childKey.SetValue(context.ComponentType.GUID.ToString("B"), 0);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Unregister this AutoLoad specification.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void Unregister(RegistrationContext context)
    {
      context.RemoveValue(RegKeyName, context.ComponentType.GUID.ToString("B"));
    }
  }
}

