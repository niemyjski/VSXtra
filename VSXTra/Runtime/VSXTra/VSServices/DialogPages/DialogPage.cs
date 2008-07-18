// ================================================================================================
// DialogPage.cs
//
// This source code is created by using the source code provided with the VS 2008 SDK. Many 
// patterns and implementation details are defined there. The code here is intended to be the base
// of a new framework for developing VSPackages.
// The code here is experimental and fully opened for community.
//
// Created: 2008.07.16, by Istvan Novak (DeepDiver)
// ================================================================================================
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms.Design;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using IServiceProvider = System.IServiceProvider;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This interface summarizes the behavior expected from a DialogPage.
  /// </summary>
  // ================================================================================================
  public interface IDialogPageBehavior: 
    IComponent,
    IWin32Window
  {
    object AutomationObject { get;  }
    void ResetContainer();
  }

  // ================================================================================================
  /// <summary>
  /// DialogPage encompasses a tools dialog page. The default dialog page examines itself for public 
  /// properties, and offers these properties to the user in a property grid. You can customize this 
  /// behavior, however by overriding various methods on the page. The dialog page will 
  /// automatically persist any changes made to it to the user's section of the registry, provided 
  /// that those properties provide support for to/from string conversions on their type converter.
  /// </summary>
  // ================================================================================================
  [CLSCompliant(false), ComVisible(true)]
  public class DialogPage<TPackage, TUIControl> : Component,
    // --- This interface makes our dialog page recognizable by a package as a dialog page.
    IDialogPageBehavior,
    // --- We need to expose Win32 window handles, so we implement this interface 
    IProfileManager
    where TPackage: PackageBase
    where TUIControl: Control, new()
  {
    #region Private fields

    /// <summary>UI of the dialog page</summary>
    private readonly TUIControl _UIControl;

    /// <summary>Represents the window behind the dialog page</summary>
    private IWin32Window _Window;

    /// <summary>Represents the native window behind the dialog page</summary>
    private DialogSubclass _Subclass;

    private DialogContainer _Container;
    private string _SettingsPath;
    private bool _Initializing;
    private bool _UIActive;
    private bool _PropertyChangedHooked;
    private EventHandler _OnPropertyChanged;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Constructs the Dialog Page.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public DialogPage()
    {
      HookProperties(true);
      _UIControl = new TUIControl();
      InitPage();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initialize the option page.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void InitPage()
    {
      UIControl.Location = new Point(0, 0);
      OnPageCreated();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Disposes this object.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // --- First dispose the dialog container
        if (_Container != null)
        {
          try
          {
            _Container.Dispose();
          }
          catch (Exception)
          {
            VsDebug.Fail("Failed to dispose container");
          }
          _Container = null;
        }

        // --- Dispose the window itself
        if (_Window != null && _Window is IDisposable)
        {
          try
          {
            ((IDisposable)_Window).Dispose();
          }
          catch (Exception)
          {
            Debug.Fail("Failed to dispose window");
          }
          _Window = null;
        }

        // --- Reset the dialog subclass
        if (_Subclass != null)
        {
          _Subclass = null;
        }

        // --- Unhook dialog page properties
        HookProperties(false);
      }
      base.Dispose(disposing);
    }

    #endregion

    #region Public and protected properties

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to initialize control behind the page before 
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnPageCreated()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override this method to initialize the page after page data has been loaded.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnPageDataLoaded()
    {
    }


    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The object the dialog page is going to browse. The default returns "this", but you can 
    /// change it to browse any object you want.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual object AutomationObject
    {
      get { return this; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Override for the site property. This override is used so we can load and save our settings 
    /// at the appropriate time.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public override ISite Site
    {
      get { return base.Site; }
      set
      {
        base.Site = value;
        if (value != null)
        {
          LoadSettingsFromStorage();
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// The window this dialog page will use for its UI. This window handle must be constant, so 
    /// if you are returning a Windows Forms control you must make sure it does not recreate its 
    /// handle. If the window object implements IComponent it will be sited by the dialog page so 
    /// it can get access to global services.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected virtual IWin32Window Window
    {
      get { return _UIControl; }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This is where the settings are stored under [UserRegistryRoot]\DialogPage, the default
    /// is the full type name of your AutomationObject.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected string SettingsRegistryPath
    {
      get
      {
        return _SettingsPath ??
               (_SettingsPath = "DialogPage\\" + AutomationObject.GetType().FullName);
      }
      set
      {
        _SettingsPath = value;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets the control representing the UI of this option page.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public TUIControl UIControl
    {
      get { return _UIControl; }
    }

    #endregion

    #region IProfileManager implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Loads the settings belonging to this dialog page from the store.
    /// </summary>
    /// <remarks>
    /// This method is called when the dialog page should load its default settings from the 
    /// registry. The default implementation gets the Package service, gets the user registry key, 
    /// and reads in all properties for this page that could be converted from strings.
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public virtual void LoadSettingsFromStorage()
    {
      _Initializing = true;
      try
      {
        // --- Obtain registry key for the package
        var package = PackageBase.GetPackageInstance<TPackage>();
        VsDebug.Assert(package != null, "No package service; we cannot load settings");
        if (package != null)
        {
          using (var rootKey = package.UserRegistryRoot)
          {
            // --- Obtain the path where settings are stored
            var path = SettingsRegistryPath;
            object automationObject = AutomationObject;
            var key = rootKey.OpenSubKey(path, false);
            if (key != null)
            {
              using (key)
              {
                // --- Read each property from the store using the invariant string form
                string[] valueNames = key.GetValueNames();
                var properties = TypeDescriptor.GetProperties(automationObject);
                foreach (var valueName in valueNames)
                {
                  var value = key.GetValue(valueName).ToString();
                  var prop = properties[valueName];
                  if (prop != null && prop.Converter.CanConvertFrom(typeof(string)))
                  {
                    prop.SetValue(automationObject, prop.Converter.ConvertFromInvariantString(value));
                  }
                }
              }
            }
          }
        }
      }
      finally
      {
        _Initializing = false;
      }

      // --- Hook in the properties
      HookProperties(true);
      OnPageDataLoaded();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Writes a VSPackage's configuration to disk using the Visual Studio settings mechanism when 
    /// the export option of an Import/Export Settings feature available on the IDE’s Tools menu 
    /// is selected by a user.
    /// </summary>
    /// <remarks>
    /// This method is called when the dialog page should load its default settings from the 
    /// profile XML file.  
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public virtual void LoadSettingsFromXml(IVsSettingsReader reader)
    {
      _Initializing = true;
      try
      {
        object automationObject = AutomationObject;
        var properties = TypeDescriptor.GetProperties(automationObject, 
          new Attribute[] { DesignerSerializationVisibilityAttribute.Visible });
        foreach (PropertyDescriptor property in properties)
        {
          var converter = property.Converter;
          if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
          {
            // --- read from the xml feed
            object cv = null;
            try
            {
              string value;
              if (NativeMethods.Succeeded(reader.ReadSettingString(property.Name, out value)) && (value != null))
              {
                cv = property.Converter.ConvertFromInvariantString(value);
              }
            }
            catch (Exception)
            {
              // --- ReadSettingString throws an exception if the property is not found and we also 
              // --- catch ConvertFromInvariantString exceptions so that we gracefully handle bad 
              // --- vssettings.
              VsDebug.Fail("Error in ReadSettingsString ignored.");
            }
            // --- Not all values have to be present
            if (cv != null)
            {
              property.SetValue(automationObject, cv);
            }
          }
        }
      }
      finally
      {
        _Initializing = false;
      }

      // --- Hook in properties
      HookProperties(true);
      OnPageDataLoaded();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Resets the user settings.
    /// </summary>
    /// <remarks>
    /// Override this method in order to reset your settings to your default values.</devdoc>
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public virtual void ResetSettings()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Writes a VSPackage's configuration to local storage (typically the registry) following 
    /// state update.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual void SaveSettingsToStorage()
    {
      var package = PackageBase.GetPackageInstance<TPackage>();
      VsDebug.Assert(package != null, "No package service; we cannot load settings");
      if (package != null)
      {
        // --- Oper the user registry key to save back settings.
        using (var rootKey = package.UserRegistryRoot)
        {
          var path = SettingsRegistryPath;
          var automationObject = AutomationObject;
          var key = rootKey.OpenSubKey(path, true) ?? rootKey.CreateSubKey(path);
          if (key == null)
          {
            VsDebug.Fail("Could not obtain storage key.");
            return;
          }
          using (key)
          {
            var properties = TypeDescriptor.GetProperties(automationObject, 
              new Attribute[] { DesignerSerializationVisibilityAttribute.Visible });
            foreach (PropertyDescriptor property in properties)
            {
              var converter = property.Converter;
              if (converter.CanConvertTo(typeof(string)) && 
                converter.CanConvertFrom(typeof(string)))
              {
                key.SetValue(property.Name, 
                  converter.ConvertToInvariantString(property.GetValue(automationObject)));
              }
            }
          }
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Writes a VSPackage's configuration to disk using the Visual Studio settings mechanism when
    /// an import option of the Import/Export Settings command on the IDE’s Tools menu is selected 
    /// by a user.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public virtual void SaveSettingsToXml(IVsSettingsWriter writer)
    {
      object automationObject = AutomationObject;
      PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(automationObject, new Attribute[] { DesignerSerializationVisibilityAttribute.Visible });
      // --- [clovett] Sort the names so that tests can depend on the order returned, otherwise the 
      // --- order changes randomly based on some internal hashtable seed. Besides it makes it 
      // --- easier for the user to read the .vssettings files.
      var sortedNames = from PropertyDescriptor prop in properties
                        orderby prop.Name
                        select prop.Name;
      foreach (string name in sortedNames)
      {
        var property = properties[name];
        var converter = property.Converter;
        if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
        {
          NativeMethods.ThrowOnFailure(
              writer.WriteSettingString(property.Name, converter.ConvertToInvariantString(property.GetValue(automationObject)))
          );
        }
      }
    }

    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This function hooks property change events so that we automatically serialize
    /// if the value changes outside of UI and loading
    /// </summary>
    /// <param name="hook">
    /// True if properties should be hooked; otherwise they should be unhooked.
    /// </param>
    // --------------------------------------------------------------------------------------------
    private void HookProperties(bool hook)
    {
      if (_PropertyChangedHooked != hook)
      {
        if (_OnPropertyChanged == null) _OnPropertyChanged = OnPropertyChanged;
        object automationObject = null;
        try
        {
          automationObject = AutomationObject;
        }
        catch (Exception e)
        {
          VsDebug.Fail(e.ToString());
        }
        if (automationObject != null)
        {
          var properties = TypeDescriptor.GetProperties(automationObject, 
            new Attribute[] { DesignerSerializationVisibilityAttribute.Visible });

          foreach (PropertyDescriptor property in properties)
          {
            if (hook)
              property.AddValueChanged(automationObject, _OnPropertyChanged);
            else
              property.RemoveValueChanged(automationObject, _OnPropertyChanged);
          }
          _PropertyChangedHooked = hook;
        }
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Convert an item property value changed event into a list changed event
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void OnPropertyChanged(object sender, EventArgs e)
    {
      if (!_Initializing && !_UIActive)
        SaveSettingsToStorage();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Resets the dialog container.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    void IDialogPageBehavior.ResetContainer()
    {
      var component = _Window as IComponent;
      if (_Container != null && component != null)
      {
        _Container.ResetAmbientProperties();
        _Container.Remove(component);
        _Container.Add(component);
      }
    }

    #endregion

    #region Virtual event methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method is called when VS wants to activate this page. If true is returned, the page is activated.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnActivate(CancelEventArgs e)
    {
      _UIActive = true;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This event is raised when the page is closed.   
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnClosed(EventArgs e)
    {
      _UIActive = false;
      LoadSettingsFromStorage();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method is called when VS wants to deatviate this page. If true is returned, the page 
    /// is deactivated.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnDeactivate(CancelEventArgs e)
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// This method is called when VS wants to save the user's changes then the dialog is dismissed.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected virtual void OnApply(PageApplyEventArgs e)
    {
      SaveSettingsToStorage();
    }

    #endregion

    #region IWin32Window implementation

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// IWin32Window implementation. This just delegates to the Window property.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    IntPtr IWin32Window.Handle
    {
      get
      {
        if (_Window == null)
        {
          _Window = Window;
          if (_Window is IComponent)
          {
            if (_Container == null)
            {
              _Container = new DialogContainer(Site);
            }
            _Container.Add((IComponent)_Window);
          }
          if (_Subclass == null)
          {
            _Subclass = new DialogSubclass(this);
          }
        }
        if (_Subclass.Handle != _Window.Handle)
        {
          _Subclass.AssignHandle(_Window.Handle);
        }
        return _Window.Handle;
      }
    }

    #endregion

    #region PageApplyEventArgs

    // ================================================================================================
    /// <summary>
    /// Event arguments to allow the OnApply method to indicate how to handle the apply event.
    /// </summary>
    // ================================================================================================
    protected class PageApplyEventArgs : EventArgs
    {
      public PageApplyKind ApplyBehavior { get; set; }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Creates a new instance of the event argument class.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public PageApplyEventArgs()
      {
        ApplyBehavior = PageApplyKind.Apply;
      }
    }

    #endregion

    #region DialogContainer

    // ================================================================================================
    /// <summary>
    /// This class derives from container to provide a service provider connection to the dialog page.
    /// </summary>
    // ================================================================================================
    private sealed class DialogContainer : Container
    {
      private readonly IServiceProvider _ServiceProvider;
      private AmbientProperties _AmbientProperties;

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Creates a new container using the given service provider.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public DialogContainer(IServiceProvider provider)
      {
        _ServiceProvider = provider;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Override to GetService so we can route requests to the package's service provider.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override object GetService(Type serviceType)
      {
        if (serviceType == null)
        {
          throw new ArgumentNullException("serviceType");
        }

        // --- Handle AmbinetProperties as special type.
        if (serviceType == typeof(AmbientProperties))
        {
          if (_AmbientProperties == null)
          {
            var uis = GetService(typeof(IUIService)) as IUIService;
            _AmbientProperties = new AmbientProperties();
            if (uis != null) _AmbientProperties.Font = (Font)uis.Styles["DialogFont"];
          }
          return _AmbientProperties;
        }

        // --- Route requests to the owner package's service provider.
        if (_ServiceProvider != null)
        {
          var service = _ServiceProvider.GetService(serviceType);
          if (service != null) return service;
        }
        return base.GetService(serviceType);
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Resets the AmbientProperties to null.
      /// </summary>
      // --------------------------------------------------------------------------------------------
      public void ResetAmbientProperties()
      {
        _AmbientProperties = null;
      }
    }

    #endregion

    #region DialogSubclass 

    // ================================================================================================
    /// <summary>
    /// This class derives from NativeWindow to provide a hook into the window handle. We use this 
    /// hook so we can respond to property sheet window messages that VS will send us.
    /// </summary>
    // ================================================================================================
    private sealed class DialogSubclass : NativeWindow
    {
      private readonly DialogPage<TPackage, TUIControl> _Page;
      private bool _CloseCalled;

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Create a new DialogSubclass instance
      /// </summary>
      // --------------------------------------------------------------------------------------------
      internal DialogSubclass(DialogPage<TPackage, TUIControl> page)
      {
        _Page = page;
        _CloseCalled = false;
      }

      // --------------------------------------------------------------------------------------------
      /// <summary>
      /// Override for WndProc to handle our PSP messages
      /// </summary>
      // --------------------------------------------------------------------------------------------
      protected override void WndProc(ref Message m)
      {
        CancelEventArgs ce;
        switch (m.Msg)
        {
          case NativeMethods.WM_NOTIFY:
            var nmhdr = (NativeMethods.NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NMHDR));
            switch (nmhdr.code)
            {
              case NativeMethods.PSN_RESET:
                _CloseCalled = true;
                _Page.OnClosed(EventArgs.Empty);
                return;
              case NativeMethods.PSN_APPLY:
                var pae = new PageApplyEventArgs();
                _Page.OnApply(pae);
                switch (pae.ApplyBehavior)
                {
                  case PageApplyKind.Cancel:
                    m.Result = (IntPtr)NativeMethods.PSNRET_INVALID;
                    break;

                  case PageApplyKind.CancelNoNavigate:
                    m.Result = (IntPtr)NativeMethods.PSNRET_INVALID_NOCHANGEPAGE;
                    break;

                  default:
                    m.Result = IntPtr.Zero;
                    break;
                }
                UnsafeNativeMethods.SetWindowLong(m.HWnd, NativeMethods.DWL_MSGRESULT, m.Result);
                return;
              case NativeMethods.PSN_KILLACTIVE:
                ce = new CancelEventArgs();
                _Page.OnDeactivate(ce);
                m.Result = (IntPtr)(ce.Cancel ? 1 : 0);
                UnsafeNativeMethods.SetWindowLong(m.HWnd, NativeMethods.DWL_MSGRESULT, m.Result);
                return;
              case NativeMethods.PSN_SETACTIVE:
                _CloseCalled = false;
                ce = new CancelEventArgs();
                _Page.OnActivate(ce);
                m.Result = (IntPtr)(ce.Cancel ? -1 : 0);
                UnsafeNativeMethods.SetWindowLong(m.HWnd, NativeMethods.DWL_MSGRESULT, m.Result);
                return;
            }
            break;

          case NativeMethods.WM_DESTROY:
            // --- We can't tell the difference between OK and Apply (see above), so if we get a 
            // --- destroy and close hasn't been called, make sure we call it
            if (!_CloseCalled && _Page != null)
            {
              _Page.OnClosed(EventArgs.Empty);
            }
            break;
        }
        base.WndProc(ref m);
      }
    }

    #endregion
  }

  public class DialogPage<TPackage>: DialogPage<TPackage, PropertyGrid>
    where TPackage: PackageBase
  {
    protected override void OnPageCreated()
    {
      UIControl.ToolbarVisible = false;
      UIControl.CommandsVisibleIfAvailable = false;
      UIControl.SelectedObject = AutomationObject;
    }
  }

  #region PageApplyKind

  // ================================================================================================
  /// <summary>
  /// This enum defines the possible kinds of page apply behaviour.
  /// </summary>
  /// <remarks>
  /// Allows the OnApply event to be canceled with optional navigation instructions.
  /// </remarks>
  // ================================================================================================
  public enum PageApplyKind
  {
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Apply - Allows the changes to be applied
    /// </summary>
    // --------------------------------------------------------------------------------------------
    Apply = 0,

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// CancelNavigate - Cancels the apply event and navigates to the page cancelling the event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    Cancel = 1,

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// CancelNoNavigate - Cancels the apply event and returns the active page, not the page cancelling the event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    CancelNoNavigate = 2
  };

  #endregion
}

