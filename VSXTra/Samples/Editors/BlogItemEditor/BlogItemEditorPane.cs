// ================================================================================================
// BlogItemEditorPane.cs
//
// Created: 2008.08.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using VSXtra.Editors;

namespace DeepDiver.BlogItemEditor
{
  public sealed class BlogItemEditorPane :
    EditorPaneBase<BlogItemEditorPackage, BlogItemEditorFactory, BlogItemEditorControl>
  {
    #region Private fields

    private readonly BlogItemEditorData _EditorData = new BlogItemEditorData();

    #endregion

    #region Lifecycle methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Creates an item of the editor pane, subscribes to the content change event.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public BlogItemEditorPane()
    {
      UIControl.ContentChanged += DataChangedInView;
    }

    #endregion

    #region Overridden methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the file extension used by this editor.
    /// </summary>
    /// <returns>
    /// File extension used by the editor.
    /// </returns>
    /// <remarks>
    /// This method is called only once at construction time. The returned file 
    /// extension must contain the leading "." character.
    /// </remarks>
    // --------------------------------------------------------------------------------
    protected override string GetFileExtension()
    {
      return BlogItemEditorPackage.BlogFileExtension;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Override this abstract method to define how to load the editor content from a
    /// file.
    /// </summary>
    /// <param name="fileName">Name of the file to load the content from.</param>
    // --------------------------------------------------------------------------------
    protected override void LoadFile(string fileName)
    {
      _EditorData.ReadFrom(fileName);
      UIControl.RefreshView(_EditorData);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Override this abstract method to define how to save the editor content into a
    /// file.
    /// </summary>
    /// <param name="fileName">Name of the file to save the content to.</param>
    // --------------------------------------------------------------------------------
    protected override void SaveFile(string fileName)
    {
      UIControl.RefreshData(_EditorData);
      _EditorData.SaveTo(fileName);
    }

    #endregion

    #region Content change management

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Occurs when the content changed at the control.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    private void DataChangedInView(object sender, EventArgs e)
    {
      OnContentChanged();
    }

    #endregion
  }
}