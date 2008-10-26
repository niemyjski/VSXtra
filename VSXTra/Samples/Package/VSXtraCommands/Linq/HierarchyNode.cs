// ================================================================================================
// HierarchyNode.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.08.29, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using VSXtra;

namespace DeepDiver.VSXtraCommands
{
  /// <summary>
  /// IVsHierarchy Wrapper
  /// </summary>
  public class HierarchyNode : IDisposable
  {
    private readonly IVsHierarchy _Hierarchy;
    private readonly uint _ItemId;
    private Icon _Icon;
    private bool disposed;
    private string fullName;

    /// <summary>
    /// Initializes a new instance of the <see cref="HierarchyNode"/> class.
    /// </summary>
    /// <param name="hierarchy">The hierarchy.</param>
    /// <param name="itemId">The item id.</param>
    public HierarchyNode(IVsHierarchy hierarchy, uint itemId)
    {
      _Hierarchy = hierarchy;
      _ItemId = itemId;
    }

    /// <summary>
    /// Gets the item id.
    /// </summary>
    /// <value>The item id.</value>
    public uint ItemId
    {
      get { return _ItemId; }
    }

    /// <summary>
    /// Gets the hierarchy.
    /// </summary>
    /// <value>The hierarchy.</value>
    public IVsHierarchy Hierarchy
    {
      get { return _Hierarchy; }
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return GetProperty<string>(__VSHPROPID.VSHPROPID_Name); }
    }

    /// <summary>
    /// Gets the full name.
    /// </summary>
    /// <value>The full name.</value>
    public string FullName
    {
      get
      {
        if (string.IsNullOrEmpty(fullName))
        {
          try
          {
            var prj = Hierarchy as IVsProject;
            if (prj != null)
              ErrorHandler.ThrowOnFailure(prj.GetMkDocument(_ItemId, out fullName));
          }
          catch
          {
            fullName = string.Empty;
          }
        }

        return fullName;
      }
    }

    /// <summary>
    /// Gets the ext object.
    /// </summary>
    /// <value>The ext object.</value>
    public object ExtObject
    {
      get { return GetProperty<object>(__VSHPROPID.VSHPROPID_ExtObject); }
    }

    /// <summary>
    /// Gets the subtype.
    /// </summary>
    /// <value>The subtype.</value>
    public string Subtype
    {
      get { return GetProperty<string>(__VSHPROPID.VSHPROPID_ItemSubType); }
    }

    /// <summary>
    /// Gets the icon.
    /// </summary>
    /// <value>The icon.</value>
    public Icon Icon
    {
      get
      {
        if (_Icon == null)
        {
          var iconHandle = GetProperty<int>(__VSHPROPID.VSHPROPID_IconHandle);
          try
          {
            if (iconHandle != IntPtr.Zero.ToInt32())
            {
              _Icon = Icon.FromHandle((IntPtr) iconHandle).Clone() as Icon;
            }
            else
            {
              var imageList = GetProperty<int>(__VSHPROPID.VSHPROPID_IconImgList);
              var index = GetProperty<int>(__VSHPROPID.VSHPROPID_IconIndex);

              int countImages = NativeMethods.ImageList_GetImageCount(imageList);

              if (imageList != IntPtr.Zero.ToInt32()
                  && index >= 0
                  && index < countImages)
              {
                const int hbmMask = 0x0F00;
                int handleIcon = NativeMethods.ImageList_GetIcon(imageList, index, hbmMask);

                _Icon = Icon.FromHandle((IntPtr) handleIcon).Clone() as Icon;
              }
            }

            if (_Icon == null)
            {
              var project = Hierarchy as IVsProject;
              if (project != null)
              {
                string file;
                ErrorHandler.ThrowOnFailure(project.GetMkDocument(ItemId, out file));
                _Icon = NativeMethods.GetIcon(file);
              }
            }
          }
          catch (COMException)
          {
          }
        }

        return _Icon;
      }
    }

    #region IDisposable Members

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion

    /// <summary>
    /// Gets the object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetObject<T>()
      where T : class
    {
      return (_Hierarchy as T);
    }

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propId">The prop id.</param>
    /// <returns></returns>
    public T GetProperty<T>(__VSHPROPID propId)
    {
      return GetProperty<T>(propId, _ItemId);
    }

    private T GetProperty<T>(__VSHPROPID propId, uint itemid)
    {
      object value;
      int hr = _Hierarchy.GetProperty(itemid, (int) propId, out value);
      if (hr != VSConstants.S_OK || value == null)
      {
        return default(T);
      }
      return (T) value;
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        if (disposing)
        {
          // Dispose managed resources.
        }
      }
      disposed = true;
    }

    /// <summary>
    /// Releases unmanaged resources and performs other cleanup operations before the
    /// <see cref="HierarchyNode"/> is reclaimed by garbage collection.
    /// </summary>
    ~HierarchyNode()
    {
      Dispose(false);
    }
  }
}