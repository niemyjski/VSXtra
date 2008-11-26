// ================================================================================================
// ImageHandler.cs
//
// This project file was originally created by Microsoft as part of the MPFProj published on 
// CodePlex (http://www.codeplex.com/MPFProj) marked with the following notice:
// "Copyright (c) Microsoft Corporation.  All rights reserved."
//
// Revised: 2008.11.21, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace VSXtra
{
  // ================================================================================================
  /// <summary>
  /// This class implements image handling functions for hierarchies.
  /// </summary>
  // ================================================================================================
  public class ImageHandler : IDisposable
  {
    #region Private fields

    private ImageList _ImageList;
    private List<IntPtr> _IconHandles;
    private static volatile object _Mutex;
    private bool _IsDisposed;

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static ImageHandler()
    {
      _Mutex = new object();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Builds an empty ImageHandler object.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ImageHandler()
    {
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Builds an ImageHandler object from a Stream providing the bitmap that stores the images 
    /// for the image list.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ImageHandler(Stream resourceStream)
    {
      if (null == resourceStream)
      {
        throw new ArgumentNullException("resourceStream");
      }
      _ImageList = GetImageList(resourceStream);
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Builds an ImageHandler object from an ImageList object.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ImageHandler(ImageList list)
    {
      if (null == list)
      {
        throw new ArgumentNullException("list");
      }
      _ImageList = list;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; 
    /// <c>false</c> to release only unmanaged resources.
    /// </param>
    // --------------------------------------------------------------------------------------------
    private void Dispose(bool disposing)
    {
      if (_IsDisposed) return;
      lock (_Mutex)
      {
        if (disposing)
        {
          _ImageList.Dispose();
        }
        _IsDisposed = true;
      }
    }

    #endregion

    #region Public methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Closes the ImageHandler object freeing its resources.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Close()
    {
      if (null != _IconHandles)
      {
        foreach (var hnd in _IconHandles)
        {
          if (hnd != IntPtr.Zero)
          {
            NativeMethods.DestroyIcon(hnd);
          }
        }
        _IconHandles = null;
      }
      if (null == _ImageList) return;
      _ImageList.Dispose();
      _ImageList = null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Add an image to the ImageHandler.
    /// </summary>
    /// <param name="image">the image object to be added.</param>
    // --------------------------------------------------------------------------------------------
    public void AddImage(Image image)
    {
      if (image == null)
      {
        throw new ArgumentNullException("image");
      }
      if (_ImageList == null)
      {
        _ImageList = new ImageList();
      }
      _ImageList.Images.Add(image);
      if (_IconHandles != null)
      {
        _IconHandles.Add(IntPtr.Zero);
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Get or set the ImageList object for this ImageHandler.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public ImageList ImageList
    {
      get { return _ImageList; }
      set
      {
        Close();
        _ImageList = value;
      }
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Returns the handle to an icon build from the image of index iconIndex in the image list.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public IntPtr GetIconHandle(int iconIndex)
    {
      // --- Verify that the object is in a consistent state.
      if ((_ImageList == null))
        throw new InvalidOperationException();
      // --- Make sure that the list of handles is initialized.
      if (_IconHandles == null)
      {
        InitHandlesList();
      }

      if (_IconHandles == null)
        throw new NullReferenceException();

      // --- Verify that the index is inside the expected range.
      if ((iconIndex < 0) || (iconIndex >= _IconHandles.Count))
        throw new ArgumentOutOfRangeException("iconIndex");

      // Check if the icon is in the cache.
      if (IntPtr.Zero == _IconHandles[iconIndex])
      {
        var bitmap = _ImageList.Images[iconIndex] as Bitmap;
        // --- If the image is not a bitmap, then we can not build the icon,
        // --- so we have to return a null handle.
        if (bitmap == null) return IntPtr.Zero;
        _IconHandles[iconIndex] = bitmap.GetHicon();
      }
      return _IconHandles[iconIndex];
    }

    #endregion

    #region Static methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Splits a bitmap from a Stream into an ImageList
    /// </summary>
    /// <param name="imageStream">A Stream representing a Bitmap</param>
    /// <returns>An ImageList object representing the images from the given stream</returns>
    // --------------------------------------------------------------------------------------------
    public static ImageList GetImageList(Stream imageStream)
    {
      var ilist = new ImageList();

      if (imageStream == null)
      {
        return ilist;
      }
      ilist.ColorDepth = ColorDepth.Depth24Bit;
      ilist.ImageSize = new Size(16, 16);
      var bitmap = new Bitmap(imageStream);
      ilist.Images.AddStrip(bitmap);
      ilist.TransparentColor = Color.Magenta;
      return ilist;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates a new ImageHandler instance from the provided bitmaps.
    /// </summary>
    /// <param name="bitmaps"></param>
    /// <returns></returns>
    // --------------------------------------------------------------------------------------------
    public static ImageHandler CreateImageList(params Bitmap[] bitmaps)
    {
      var result = new ImageHandler();
      foreach (var item in bitmaps)
      {
        result.AddImage(item);
      }
      return result;
    }

    #endregion

    #region Helper methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes the handle list.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void InitHandlesList()
    {
      _IconHandles = new List<IntPtr>(_ImageList.Images.Count);
      for (int i = 0; i < _ImageList.Images.Count; ++i)
      {
        _IconHandles.Add(IntPtr.Zero);
      }
    }

    #endregion

    #region IDisposable Members

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting 
    /// unmanaged resources.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion
  }
}