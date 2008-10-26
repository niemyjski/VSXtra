// ================================================================================================
// BlogItemEditorControl.cs
//
// Created: 2008.08.30, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.Windows.Forms;
using VSXtra.Editors;

namespace DeepDiver.BlogItemEditor
{
  // ==================================================================================
  /// <summary>
  /// This class represents the user interface of a BlogItemEditor.
  /// </summary>
  /// <remarks>
  /// The user control implement the ICommonCommandSupport interface in order to be
  /// supported by the SimpleEditorPane.
  /// </remarks>
  // ==================================================================================
  public partial class BlogItemEditorControl :
    UserControl,
    ICommonEditorCommand
  {
    #region Lifecycle methods

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Creates and initializes a new control instance.
    /// </summary>
    // --------------------------------------------------------------------------------
    public BlogItemEditorControl()
    {
      InitializeComponent();
    }

    #endregion

    #region View management functions

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Copies data item properties to the controls representing the view.
    /// </summary>
    /// <param name="data">Editor data instance.</param>
    // --------------------------------------------------------------------------------
    public void RefreshView(BlogItemEditorData data)
    {
      TitleEdit.Text = data.Title ?? string.Empty;
      CategoriesEdit.Text = data.Categories ?? String.Empty;
      BodyEdit.Text = data.Body ?? String.Empty;
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Copies control content into data item properties.
    /// </summary>
    /// <param name="data">Editor data instance.</param>
    // --------------------------------------------------------------------------------
    public void RefreshData(BlogItemEditorData data)
    {
      data.Title = TitleEdit.Text;
      data.Categories = CategoriesEdit.Text;
      data.Body = BodyEdit.Text;
    }

    #endregion

    // --------------------------------------------------------------------------------

    #region ICommonEditorCommand Members

    /// <summary>
    /// Get the flag indicating if "SelectAll" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    bool ICommonEditorCommand.SupportsSelectAll
    {
      get { return false; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Copy" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    bool ICommonEditorCommand.SupportsCopy
    {
      get { return ActiveControlHasSelection; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Cut" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    bool ICommonEditorCommand.SupportsCut
    {
      get { return ActiveControlHasSelection; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Paste" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    bool ICommonEditorCommand.SupportsPaste
    {
      get { return ActiveCanPasteFromClipboard; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Redo" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    bool ICommonEditorCommand.SupportsRedo
    {
      get { return false; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Get the flag indicating if "Undo" command is supported or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    bool ICommonEditorCommand.SupportsUndo
    {
      get { return false; }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "SelectAll" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    void ICommonEditorCommand.DoSelectAll()
    {
      throw new NotImplementedException();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Copy" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    void ICommonEditorCommand.DoCopy()
    {
      var active = ActiveControl as TextBox;
      if (active != null) active.Copy();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Cut" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    void ICommonEditorCommand.DoCut()
    {
      var active = ActiveControl as TextBox;
      if (active != null) active.Cut();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Paste" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    void ICommonEditorCommand.DoPaste()
    {
      var active = ActiveControl as TextBox;
      if (active != null) active.Paste();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Redo" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    void ICommonEditorCommand.DoRedo()
    {
      throw new NotImplementedException();
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Executes the "Undo" command.
    /// </summary>
    // --------------------------------------------------------------------------------
    void ICommonEditorCommand.DoUndo()
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Content change handling

    // --------------------------------------------------------------------------------
    /// <summary>
    /// The pane can subscribe to this event and watch when the content changes.
    /// </summary>
    // --------------------------------------------------------------------------------
    public event EventHandler ContentChanged;

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Raises the event signingone of the controls' content has been changed.
    /// </summary>
    /// <param name="sender">Control instance</param>
    /// <param name="e">Event arguments</param>
    // --------------------------------------------------------------------------------
    private void RaiseContentChanged(object sender, EventArgs e)
    {
      if (ContentChanged != null) ContentChanged.Invoke(sender, e);
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Event handler method for controls to sign their content has been changed.
    /// </summary>
    /// <param name="sender">Control instance</param>
    /// <param name="e">Event arguments</param>
    // --------------------------------------------------------------------------------
    private void ControlContentChanged(object sender, EventArgs e)
    {
      RaiseContentChanged(sender, e);
    }

    #endregion

    #region Private methods and properties

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the active control has a selection or not.
    /// </summary>
    // --------------------------------------------------------------------------------
    private bool ActiveControlHasSelection
    {
      get
      {
        var active = ActiveControl as TextBox;
        return active == null
                 ? false
                 : active.SelectionLength > 0;
      }
    }

    // --------------------------------------------------------------------------------
    /// <summary>
    /// Gets the flag indicating if the active control can paste text from 
    /// the clipboard.
    /// </summary>
    // --------------------------------------------------------------------------------
    private bool ActiveCanPasteFromClipboard
    {
      get
      {
        var active = ActiveControl as TextBox;
        return (active != null && Clipboard.ContainsText());
      }
    }

    #endregion
  }
}