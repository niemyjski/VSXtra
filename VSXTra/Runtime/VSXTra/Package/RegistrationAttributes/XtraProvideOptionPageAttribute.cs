// ================================================================================================
// XtraProvideOptionPageAttribute.cs
//
// Created: 2008.07.16, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using VSXtra.Properties;

namespace VSXtra.Package
{
  // ================================================================================================
  /// <summary>
  /// This attribute declares that a package offers one or more option pages. Option pages are 
  /// exposed to the user through Visual Studio's Tools->Options dialog. The first parameter to 
  /// this attribute is the type of option page, which is a type that must derive from DialogPage.
  /// Option page attributes are read by the package class when Visual Studio requests a particular 
  /// option page GUID. Package will walk the attributes and try to match the requested GUID to a 
  /// GUID on a type in the package. 
  /// </summary>
  // ================================================================================================
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class XtraProvideOptionPageAttribute : XtraProvideOptionDialogPageAttribute
  {
    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The page type is a type that derives from DialogPage<>. The nameResourceID parameter 
    /// specifies a Win32 resource ID in the stored in the native UI resource satellite that 
    /// describes the name of this page.
    /// </summary>
    /// <param name="pageType"></param>
    /// <param name="categoryName"></param>
    /// <param name="pageName"></param>
    /// <param name="categoryResourceID">Specifies the page category name</param>
    /// <param name="pageNameResourceID">
    /// Specifies a Win32 resource ID in the stored in the native UI resource satellite that 
    /// describes the name of this page
    /// </param>
    /// <param name="supportsAutomation"></param>
    // --------------------------------------------------------------------------------------------
    public XtraProvideOptionPageAttribute(Type pageType, string categoryName, string pageName, 
      short categoryResourceID, short pageNameResourceID, bool supportsAutomation)
      : base(pageType, "#" + pageNameResourceID)
    {
      if (categoryName == null)
      {
        throw new ArgumentNullException("categoryName");
      }
      if (pageName == null)
      {
        throw new ArgumentNullException("pageName");
      }
      CategoryName = categoryName;
      PageName = pageName;
      CategoryResourceID = categoryResourceID;
      SupportsAutomation = supportsAutomation;
      ProfileMigrationType = ProfileMigrationType.None;
    }

    #endregion

    #region Public properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The VB Simplified option page is visible only for "simply" pages, that is a page that sets this
    /// parameter to true.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool NoShowAllView { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Identity of this instance of the attribute.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override object TypeId
    {
      get { return this; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The programmatic name for this category (non localized).
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string CategoryName { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The native resourceID of the category name for this page.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public short CategoryResourceID { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The programmatic name for this page (non localized).
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public string PageName { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// True if this page should be registered as supporting automation.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool SupportsAutomation { get; private set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// True if this page should be registered as supporting profiles. Note: Only works if 
    /// SupportsAutomation is true.  The ProvideProfile attribute can also be used to specify 
    /// profile support for Tools/Options pages.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public bool SupportsProfiles { get; set; }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Specifies the migration action to take for this category.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ProfileMigrationType ProfileMigrationType { get; set; }

    #endregion

    #region Private properties describing registry keys

    private string ToolsOptionsPagesRegKey
    {
      get { return string.Format(CultureInfo.InvariantCulture, "ToolsOptionsPages\\{0}", CategoryName); }
    }

    private string AutomationCategoryRegKey
    {
      get { return string.Format(CultureInfo.InvariantCulture, "AutomationProperties\\{0}", CategoryName); }
    }

    private string AutomationRegKey
    {
      get { return String.Format(CultureInfo.InvariantCulture, "{0}\\{1}", AutomationCategoryRegKey, PageName); }
    }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called to register this attribute with the given context. The context contains the 
    /// location where the registration inforomation should be placed. It also contains such as 
    /// the type being registered, and path information. This method is called both for 
    /// registration and unregistration. The difference is that unregistering just uses a hive 
    /// that reverses the changes applied to it.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void Register(RegistrationContext context)
    {
      context.Log.WriteLine(string.Format(Resources.Culture, Resources.Reg_NotifyOptionPage, CategoryName, PageName));

      using (var toolsOptionsKey = context.CreateKey(ToolsOptionsPagesRegKey))
      {
        toolsOptionsKey.SetValue(string.Empty, string.Format(CultureInfo.InvariantCulture, "#{0}", CategoryResourceID));
        toolsOptionsKey.SetValue("Package", context.ComponentType.GUID.ToString("B"));

        using (Key pageKey = toolsOptionsKey.CreateSubkey(PageName))
        {
          pageKey.SetValue(string.Empty, PageNameResourceId);
          pageKey.SetValue("Package", context.ComponentType.GUID.ToString("B"));
          pageKey.SetValue("Page", PageType.GUID.ToString("B"));
          if (NoShowAllView)
            pageKey.SetValue("NoShowAllView", 1);
        }
      }

      if (!SupportsAutomation) return;
      using (var automationKey = context.CreateKey(AutomationRegKey))
      {
        automationKey.SetValue("Name", string.Format(CultureInfo.InvariantCulture, "{0}.{1}", CategoryName, PageName));
        automationKey.SetValue("Package", context.ComponentType.GUID.ToString("B"));
        if (SupportsProfiles)
        {
          automationKey.SetValue("ProfileSave", 1);
          automationKey.SetValue("VSSettingsMigration", (int)ProfileMigrationType);
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Called to remove this attribute from the given context.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override void Unregister(RegistrationContext context)
    {
      context.RemoveKey(ToolsOptionsPagesRegKey);
      if (SupportsAutomation)
      {
        context.RemoveKey(AutomationRegKey);
        context.RemoveKeyIfEmpty(AutomationCategoryRegKey);
      }
    }

    #endregion
  }
}

