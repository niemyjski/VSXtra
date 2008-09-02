using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra;
using Constants=Microsoft.VisualStudio.OLE.Interop.Constants;
using IDataObject=Microsoft.VisualStudio.OLE.Interop.IDataObject;
using IDropTarget=Microsoft.VisualStudio.OLE.Interop.IDropTarget;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;


namespace DeepDiver.UIHierarchyWindow
{
  internal class SimpleHierarchy : IVsUIHierarchy
  {
    internal static ImageList _imageList;
    internal static ServiceProvider _serviceProvider;
    internal static SimpleHierarchy Hierarchy;
    private IVsUIHierarchyWindow _vsHierarchyWindow;

    internal SimpleItem childItem1 = new SimpleItem("MSDN", 1, VSConstants.VSITEMID_ROOT);
    internal SimpleItem childItem2 = new SimpleItem("MSNBC", 2, VSConstants.VSITEMID_ROOT);
    internal SimpleItem rootItem = new SimpleItem("Simple Hierarchy", unchecked((uint) -2));

    public SimpleHierarchy(IVsUIHierarchyWindow hierWin)
    {
      _vsHierarchyWindow = hierWin;
      Hierarchy = this;

      if (_imageList == null)
      {
        _imageList = new ImageList
                       {
                         ColorDepth = ColorDepth.Depth24Bit,
                         ImageSize = new Size(16, 16),
                         TransparentColor = Color.Magenta
                       };
        _imageList.Images.AddStrip(SHResources.SimpleHierarchyImages);
      }
    }

    #region IVsUIHierarchy Members

    public int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
    {
      pdwCookie = 0;
      return VSConstants.S_OK;
    }

    public int Close()
    {
      return VSConstants.S_OK;
    }

    public int ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt,
                           IntPtr pvaIn, IntPtr pvaOut)
    {
      if (pguidCmdGroup.Equals(VSConstants.GUID_VsUIHierarchyWindowCmds))
      {
        switch (nCmdID)
        {
          case (uint) VSConstants.VsUIHierarchyWindowCmdIds.UIHWCMDID_DoubleClick:
            {
              switch (itemid)
              {
                case 1:
                  childItem1.NavigateTo("http://www.msdn.microsoft.com");
                  break;
                case 2:
                  childItem2.NavigateTo("http://www.msnbc.com");
                  break;
              }
            }
            break;
        }

        return (int) Constants.OLECMDERR_E_NOTSUPPORTED;
      }

      return (int) Constants.OLECMDERR_E_NOTSUPPORTED;
    }

    public int GetCanonicalName(uint itemid, out string pbstrName)
    {
      pbstrName = GetItem(itemid)._Caption;
      if (pbstrName != null)
        return VSConstants.S_OK;

      return VSConstants.E_INVALIDARG;
    }

    public int GetGuidProperty(uint itemid, int propid, out Guid pguid)
    {
      string s = string.Format("GetGuidProperty for itemID({0})", itemid);
      Trace.WriteLine(s);
      pguid = Guid.Empty;
      return VSConstants.DISP_E_MEMBERNOTFOUND;
    }

    public int GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested,
                                  out IntPtr ppHierarchyNested, out uint pitemidNested)
    {
      ppHierarchyNested = IntPtr.Zero;
      pitemidNested = 0;
      return VSConstants.E_NOTIMPL;
    }

    public int GetProperty(uint itemid, int propid, out object pvar)
    {
      // GetProperty is called many many times for this particular property
      if (propid != (int) __VSHPROPID.VSHPROPID_ParentHierarchy)
      {
        string s = string.Format("GetProperty for itemId ({0}) called with propid = {1}", itemid,
                                 propid);
        Trace.WriteLine(s);
      }

      pvar = null;
      switch (propid)
      {
        case (int) __VSHPROPID.VSHPROPID_CmdUIGuid:
          pvar = typeof (UIHierarchyWindowPackage).GUID;
          break;

        case (int) __VSHPROPID.VSHPROPID_Parent:
          if (itemid == VSConstants.VSITEMID_ROOT)
            pvar = VSConstants.VSITEMID_NIL;
          else
            pvar = VSConstants.VSITEMID_ROOT;
          break;

        case (int) __VSHPROPID.VSHPROPID_FirstChild:
          if (itemid == VSConstants.VSITEMID_ROOT)
            pvar = childItem1._Id;
          else
            pvar = VSConstants.VSITEMID_NIL;
          break;

        case (int) __VSHPROPID.VSHPROPID_NextSibling:
          if (itemid == childItem1._Id)
            pvar = childItem2._Id;
          else
            pvar = VSConstants.VSITEMID_NIL;
          break;

        case (int) __VSHPROPID.VSHPROPID_Expandable:
          if (itemid == VSConstants.VSITEMID_ROOT)
            pvar = true;
          else
            pvar = false;
          break;

        case (int) __VSHPROPID.VSHPROPID_IconImgList:
        case (int) __VSHPROPID.VSHPROPID_OpenFolderIconHandle:
          pvar = (int) _imageList.Handle;
          break;

        case (int) __VSHPROPID.VSHPROPID_IconIndex:
        case (int) __VSHPROPID.VSHPROPID_OpenFolderIconIndex:
          pvar = GetItem(itemid)._IconIndex;
          break;

        case (int) __VSHPROPID.VSHPROPID_Caption:
        case (int) __VSHPROPID.VSHPROPID_SaveName:
          pvar = GetItem(itemid)._Caption;
          break;

        case (int) __VSHPROPID.VSHPROPID_ShowOnlyItemCaption:
          pvar = true;
          break;

        case (int) __VSHPROPID.VSHPROPID_ParentHierarchy:
          if (itemid == childItem1._Id || itemid == childItem2._Id)
            pvar = this;
          break;
      }

      if (pvar != null)
        return VSConstants.S_OK;

      return VSConstants.DISP_E_MEMBERNOTFOUND;
    }

    public int GetSite(out IOleServiceProvider ppSP)
    {
      ppSP = _serviceProvider.GetService(typeof (IOleServiceProvider)) as IOleServiceProvider;
      return VSConstants.S_OK;
    }

    public int ParseCanonicalName(string pszName, out uint pitemid)
    {
      pitemid = 0;
      return VSConstants.E_NOTIMPL;
    }

    public int QueryClose(out int pfCanClose)
    {
      pfCanClose = 1;
      return VSConstants.S_OK;
    }

    public int QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds,
                                  IntPtr pCmdText)
    {
      return (int) Constants.OLECMDERR_E_UNKNOWNGROUP;
    }

    public int SetGuidProperty(uint itemid, int propid, ref Guid rguid)
    {
      return VSConstants.E_NOTIMPL;
    }

    public int SetProperty(uint itemid, int propid, object var)
    {
      return VSConstants.E_NOTIMPL;
    }

    public int SetSite(IOleServiceProvider psp)
    {
      _serviceProvider = new ServiceProvider(psp, true);
      return VSConstants.S_OK;
    }

    public int UnadviseHierarchyEvents(uint dwCookie)
    {
      return VSConstants.S_OK;
    }

    public int Unused0()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public int Unused1()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public int Unused2()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public int Unused3()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public int Unused4()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion

    private SimpleItem GetItem(uint itemid)
    {
      switch (itemid)
      {
        case VSConstants.VSITEMID_ROOT:
          return rootItem;
        case 1:
          return childItem1;
        case 2:
          return childItem2;
      }
      return null;
    }

    #region Nested type: SimpleItem

    internal class SimpleItem : IVsWebBrowserUser
    {
      public string _Caption;
      private IVsWindowFrame _frameWindow;
      public int _IconIndex;
      public uint _Id;
      public uint _ParentId;

      private IVsWebBrowser _webBrowser;

      public SimpleItem(string caption, uint id)
      {
        _Caption = caption;
        _Id = id;
        _ParentId = VSConstants.VSITEMID_ROOT;
        _IconIndex = 0;
      }

      public SimpleItem(string caption, uint id, uint parentId)
      {
        _Caption = caption;
        _Id = id;
        _ParentId = parentId;
        _IconIndex = 1;
      }

      #region IVsWebBrowserUser Members

      public int Disconnect()
      {
        return VSConstants.S_OK;
      }

      public int FilterDataObject(IDataObject pDataObjIn, out IDataObject ppDataObjOut)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public int GetCmdUIGuid(out Guid pguidCmdUI)
      {
        pguidCmdUI = new Guid(GuidList.guidUIHierarchyWindowCmdSetString);
        return VSConstants.S_OK;
      }

      public int GetCustomMenuInfo(object pUnkCmdReserved, object pDispReserved, uint dwType,
                                   uint dwPosition, out Guid pguidCmdGroup, out int pdwMenuID)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public int GetCustomURL(uint nPage, out string pbstrURL)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public int GetDropTarget(IDropTarget pDropTgtIn, out IDropTarget ppDropTgtOut)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public int GetExternalObject(out object ppDispObject)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public int GetOptionKeyPath(uint dwReserved, out string pbstrKey)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public int Resize(int cx, int cy)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public int TranslateAccelarator(MSG[] lpmsg)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      public int TranslateUrl(uint dwReserved, string lpszURLIn, out string lppszURLOut)
      {
        throw new Exception("The method or operation is not implemented.");
      }

      #endregion

      public void NavigateTo(string strURL)
      {
        int hr = 0;

        // create a webbrowser instance and tie it to our hierarchy item
        if (_webBrowser == null)
        {
          var svc =
            PackageBase.GetPackageInstance<UIHierarchyWindowPackage>().
              GetService<SVsWebBrowsingService, IVsWebBrowsingService>();
          uint dwCreateFlags = (uint) __VSCREATEWEBBROWSER.VSCWB_FrameMdiChild |
                               (uint) __VSCREATEWEBBROWSER.VSCWB_StartCustom;
          Guid guidEmpty = GuidList.guidHierPersistance;
          hr = svc.CreateWebBrowser(dwCreateFlags, ref guidEmpty, _Caption, strURL, this,
                                    out _webBrowser, out _frameWindow);

          _frameWindow.SetProperty((int) __VSFPROPID.VSFPROPID_Hierarchy, Hierarchy);
          _frameWindow.SetProperty((int) __VSFPROPID.VSFPROPID_ItemID, _Id);
        }
        else
          _webBrowser.Navigate(0, strURL);


        if (_frameWindow != null)
          _frameWindow.Show();
      }
    }

    #endregion
  }
}