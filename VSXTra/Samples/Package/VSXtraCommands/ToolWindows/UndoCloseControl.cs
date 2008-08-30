// ================================================================================================
// UndoCloseControl.cs
//
// This file was taken from the source of PowerCommands for Visual Studio 2008. I added only some
// comments and made some refactorings, but the essence of the code has not been changed.
//
// Created: 2008, by Pablo Galiano
// Revised: 2008.08.28, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using VSXtra;

namespace DeepDiver.VSXtraCommands
{
  // ================================================================================================
  /// <summary>
  /// User interface for the Undo Close Tool Window
  /// </summary>
  // ================================================================================================
  public partial class UndoCloseControl : UserControl
  {
    #region Fields

    readonly IServiceProvider _ServiceProvider;
    IUndoCloseManagerService _UndoCloseManager;
    readonly ImageList _Images = new ImageList();
    readonly ListViewGroup _RootGroup = new ListViewGroup(Resources.RecentlyClosedDocuments);

    #endregion

    #region Constructors

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UndoCloseControl"/> class.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public UndoCloseControl()
    {
      InitializeComponent();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="UndoCloseControl"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    // --------------------------------------------------------------------------------------------
    public UndoCloseControl(IServiceProvider provider)
    {
      _ServiceProvider = provider;
      InitializeComponent();
    }

    #endregion

    #region Event Handlers

    private void UndoCloseControl_Load(object sender, EventArgs e)
    {
      _UndoCloseManager = _ServiceProvider.GetService<SUndoCloseManagerService, IUndoCloseManagerService>();
      InitializeList();
    }

    private void lstDocuments_DoubleClick(object sender, EventArgs e)
    {
      OpenDocument();
    }

    private void lstDocuments_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (e.KeyChar == (char)13)
      {
        OpenDocument();
      }
    }

    #endregion

    #region Public Implementation
    /// <summary>
    /// Updates the document list.
    /// </summary>
    public void UpdateDocumentList()
    {
      lstDocuments.BeginUpdate();

      ClearDocumentList();

      _UndoCloseManager.GetDocuments().ForEach(
          info =>
          {
            string imageKey;

            imageKey = Path.GetExtension(info.DocumentPath);

            if (!lstDocuments.SmallImageList.Images.ContainsKey(imageKey))
            {
              var icon = NativeMethods.GetIcon(info.DocumentPath);

              if (icon != null)
              {
                lstDocuments.SmallImageList.Images.Add(imageKey, icon);
              }
            }

            var item =
                new ListViewItem(info.DocumentPath, _RootGroup) { Tag = info, ImageKey = imageKey };

            lstDocuments.Items.Add(item);
          });

      lstDocuments.EndUpdate();
    }

    /// <summary>
    /// Clears the document list.
    /// </summary>
    public void ClearDocumentList()
    {
      lstDocuments.Items.Clear();
    }
    #endregion

    #region Private Implementation
    private void OpenDocument()
    {
      var item = lstDocuments.SelectedItems.OfType<ListViewItem>().First();
      var docInfo = item.Tag as UndoDocumentInfo;

      if (docInfo != null)
      {
        _UndoCloseManager.PopDocument(docInfo);
        lstDocuments.Items.Remove(item);

        if (File.Exists(docInfo.DocumentPath))
        {
          try
          {
            docInfo.OpenDocument();
          }
          catch (COMException)
          { }
        }
      }
    }

    private HierarchyNode GetHierarchy(string file)
    {
      IVsSolution solution = _ServiceProvider.GetService<SVsSolution, IVsSolution>();

      HierarchyNodeIterator iterator = new HierarchyNodeIterator(solution);

      return iterator.SingleOrDefault(
          node => node.FullName.Equals(file));
    }

    private void InitializeList()
    {
      _Images.ColorDepth = ColorDepth.Depth32Bit;
      lstDocuments.SmallImageList = _Images;
      lstDocuments.Columns[0].Width = lstDocuments.Width - 8;
      lstDocuments.Groups.Add(_RootGroup);
    }
    #endregion
  }
}