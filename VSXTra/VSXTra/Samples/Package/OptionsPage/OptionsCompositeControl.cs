// ================================================================================================
// OptionsCompositeControl.cs
//
// Created: 2008.07.18, by Istvan Novak (DeepDiver)
// ================================================================================================
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace DeepDiver.OptionsPage
{
  // ================================================================================================
  /// <summary>
  /// This class implements UI for the Custom options page.
  /// It uses OptionsPageCustom object as a data objects.
  /// </summary>
  // ================================================================================================
  public class OptionsCompositeControl : UserControl
  {
    #region Fields

    private PictureBox pictureBox;
    private OpenFileDialog openImageFileDialog;
    private Button buttonChooseImage;
    private Button buttonClearImage;
    private OptionsPageCustom customOptionsPage;

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    #endregion

    #region Constructors

    /// <summary>
    /// Explicitly defined default constructor.
    /// Initializes new instance of OptionsCompositeControl class.
    /// </summary>
    public OptionsCompositeControl()
    {
      InitializeComponent();
    }

    #endregion

    #region IDisposable implementation

    // --------------------------------------------------------------------------------------------
    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (openImageFileDialog != null)
        {
          openImageFileDialog.Dispose();
          openImageFileDialog = null;
        }
        if (components != null)
        {
          components.Dispose();
        }
        GC.SuppressFinalize(this);
      }
      base.Dispose(disposing);
    }

    #endregion

    #region Component Designer generated code
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.openImageFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.buttonChooseImage = new System.Windows.Forms.Button();
      this.buttonClearImage = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox
      // 
      this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox.Location = new System.Drawing.Point(16, 16);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(295, 231);
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      // 
      // buttonChooseImage
      // 
      this.buttonChooseImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonChooseImage.Location = new System.Drawing.Point(317, 16);
      this.buttonChooseImage.Name = "buttonChooseImage";
      this.buttonChooseImage.Size = new System.Drawing.Size(120, 40);
      this.buttonChooseImage.TabIndex = 1;
      this.buttonChooseImage.Text = global::DeepDiver.OptionsPage.Resources.ChooseImageButtonText;
      this.buttonChooseImage.Click += new System.EventHandler(this.OnChooseImage);
      // 
      // buttonClearImage
      // 
      this.buttonClearImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonClearImage.Location = new System.Drawing.Point(317, 62);
      this.buttonClearImage.Name = "buttonClearImage";
      this.buttonClearImage.Size = new System.Drawing.Size(120, 40);
      this.buttonClearImage.TabIndex = 2;
      this.buttonClearImage.Text = global::DeepDiver.OptionsPage.Resources.ButtonClearImageText;
      this.buttonClearImage.Click += new System.EventHandler(this.OnClearImage);
      // 
      // OptionsCompositeControl
      // 
      this.AllowDrop = true;
      this.Controls.Add(this.buttonClearImage);
      this.Controls.Add(this.buttonChooseImage);
      this.Controls.Add(this.pictureBox);
      this.Name = "OptionsCompositeControl";
      this.Size = new System.Drawing.Size(447, 260);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.ResumeLayout(false);

    }
    #endregion

    #region Private methods

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the ChooseImage event. 
    /// </summary>
    /// <param name="sender">The reference to contained object.</param>
    /// <param name="e">The event arguments.</param>
    // --------------------------------------------------------------------------------------------
    private void OnChooseImage(object sender, EventArgs e)
    {
      openImageFileDialog = new OpenFileDialog();

      if ((openImageFileDialog == null) || (DialogResult.OK != openImageFileDialog.ShowDialog()))
        return;
      if (customOptionsPage != null)
        customOptionsPage.CustomBitmap = openImageFileDialog.FileName;
      RefreshImage();
    }
    
    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Handles the ClearImage event. 
    /// </summary>
    /// <param name="sender">The reference to contained object.</param>
    /// <param name="e">The event arguments.</param>
    // --------------------------------------------------------------------------------------------
    private void OnClearImage(object sender, EventArgs e)
    {
      if (customOptionsPage != null)
        customOptionsPage.CustomBitmap = null;
      RefreshImage();
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Refresh PictureBox Image data.
    /// </summary>
    /// <remarks>
    /// Image was reloaded from the file, specified by CustomBitmap (full path to the file).
    /// </remarks>
    // --------------------------------------------------------------------------------------------
    public void RefreshImage()
    {
      if (customOptionsPage == null) return;
      var fileName = customOptionsPage.CustomBitmap;
      if (!string.IsNullOrEmpty(fileName))
      {
        // --- Avoid to use Image.FromFile() method for loading image to exclude file locks
        using (var lStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
          pictureBox.Image = Image.FromStream(lStream);
        }
      }
      else pictureBox.Image = null;
    }

    // --------------------------------------------------------------------------------------------
    /// <summary>
    /// Gets or Sets the reference to the underlying OptionsPage object.
    /// </summary>
    // --------------------------------------------------------------------------------------------
    public OptionsPageCustom OptionsPage
    {
      get { return customOptionsPage; }
      set
      {
        customOptionsPage = value;
        RefreshImage();
      }
    }

    #endregion
  }
}
