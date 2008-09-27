namespace DeepDiver.BlogItemEditor
{
  partial class BlogItemEditorControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.TopLabelPanel = new System.Windows.Forms.Panel();
      this.HeaderLabel = new System.Windows.Forms.Label();
      this.HeaderPanel = new System.Windows.Forms.Panel();
      this.CategoriesLabel = new System.Windows.Forms.Label();
      this.CategoriesEdit = new System.Windows.Forms.TextBox();
      this.TitleEdit = new System.Windows.Forms.TextBox();
      this.TitleLabel = new System.Windows.Forms.Label();
      this.BodyLabelPanel = new System.Windows.Forms.Panel();
      this.BodyLabel = new System.Windows.Forms.Label();
      this.BodyPanel = new System.Windows.Forms.Panel();
      this.BodyEdit = new System.Windows.Forms.TextBox();
      this.TopLabelPanel.SuspendLayout();
      this.HeaderPanel.SuspendLayout();
      this.BodyLabelPanel.SuspendLayout();
      this.BodyPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // TopLabelPanel
      // 
      this.TopLabelPanel.Controls.Add(this.HeaderLabel);
      this.TopLabelPanel.Dock = System.Windows.Forms.DockStyle.Top;
      this.TopLabelPanel.Location = new System.Drawing.Point(8, 8);
      this.TopLabelPanel.Name = "TopLabelPanel";
      this.TopLabelPanel.Padding = new System.Windows.Forms.Padding(4);
      this.TopLabelPanel.Size = new System.Drawing.Size(628, 40);
      this.TopLabelPanel.TabIndex = 0;
      // 
      // HeaderLabel
      // 
      this.HeaderLabel.BackColor = System.Drawing.Color.SlateBlue;
      this.HeaderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.HeaderLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.HeaderLabel.ForeColor = System.Drawing.Color.White;
      this.HeaderLabel.Location = new System.Drawing.Point(4, 4);
      this.HeaderLabel.Name = "HeaderLabel";
      this.HeaderLabel.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
      this.HeaderLabel.Size = new System.Drawing.Size(620, 32);
      this.HeaderLabel.TabIndex = 0;
      this.HeaderLabel.Text = "Blog post header information";
      this.HeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // HeaderPanel
      // 
      this.HeaderPanel.Controls.Add(this.CategoriesLabel);
      this.HeaderPanel.Controls.Add(this.CategoriesEdit);
      this.HeaderPanel.Controls.Add(this.TitleEdit);
      this.HeaderPanel.Controls.Add(this.TitleLabel);
      this.HeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
      this.HeaderPanel.Location = new System.Drawing.Point(8, 48);
      this.HeaderPanel.Name = "HeaderPanel";
      this.HeaderPanel.Padding = new System.Windows.Forms.Padding(8, 4, 4, 4);
      this.HeaderPanel.Size = new System.Drawing.Size(628, 68);
      this.HeaderPanel.TabIndex = 1;
      // 
      // CategoriesLabel
      // 
      this.CategoriesLabel.AutoSize = true;
      this.CategoriesLabel.Location = new System.Drawing.Point(5, 40);
      this.CategoriesLabel.Name = "CategoriesLabel";
      this.CategoriesLabel.Size = new System.Drawing.Size(77, 17);
      this.CategoriesLabel.TabIndex = 3;
      this.CategoriesLabel.Text = "Categories:";
      // 
      // CategoriesEdit
      // 
      this.CategoriesEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.CategoriesEdit.Location = new System.Drawing.Point(88, 37);
      this.CategoriesEdit.Name = "CategoriesEdit";
      this.CategoriesEdit.Size = new System.Drawing.Size(533, 23);
      this.CategoriesEdit.TabIndex = 2;
      this.CategoriesEdit.TextChanged += new System.EventHandler(this.ControlContentChanged);
      // 
      // TitleEdit
      // 
      this.TitleEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.TitleEdit.Location = new System.Drawing.Point(88, 8);
      this.TitleEdit.Name = "TitleEdit";
      this.TitleEdit.Size = new System.Drawing.Size(533, 23);
      this.TitleEdit.TabIndex = 1;
      this.TitleEdit.TextChanged += new System.EventHandler(this.ControlContentChanged);
      // 
      // TitleLabel
      // 
      this.TitleLabel.AutoSize = true;
      this.TitleLabel.Location = new System.Drawing.Point(3, 11);
      this.TitleLabel.Name = "TitleLabel";
      this.TitleLabel.Size = new System.Drawing.Size(37, 17);
      this.TitleLabel.TabIndex = 0;
      this.TitleLabel.Text = "Title:";
      // 
      // BodyLabelPanel
      // 
      this.BodyLabelPanel.Controls.Add(this.BodyLabel);
      this.BodyLabelPanel.Dock = System.Windows.Forms.DockStyle.Top;
      this.BodyLabelPanel.Location = new System.Drawing.Point(8, 116);
      this.BodyLabelPanel.Name = "BodyLabelPanel";
      this.BodyLabelPanel.Padding = new System.Windows.Forms.Padding(4);
      this.BodyLabelPanel.Size = new System.Drawing.Size(628, 40);
      this.BodyLabelPanel.TabIndex = 2;
      // 
      // BodyLabel
      // 
      this.BodyLabel.BackColor = System.Drawing.Color.SlateBlue;
      this.BodyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.BodyLabel.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.BodyLabel.ForeColor = System.Drawing.Color.White;
      this.BodyLabel.Location = new System.Drawing.Point(4, 4);
      this.BodyLabel.Name = "BodyLabel";
      this.BodyLabel.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
      this.BodyLabel.Size = new System.Drawing.Size(620, 32);
      this.BodyLabel.TabIndex = 0;
      this.BodyLabel.Text = "Blog post body information";
      this.BodyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // BodyPanel
      // 
      this.BodyPanel.Controls.Add(this.BodyEdit);
      this.BodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.BodyPanel.Location = new System.Drawing.Point(8, 156);
      this.BodyPanel.Name = "BodyPanel";
      this.BodyPanel.Padding = new System.Windows.Forms.Padding(8);
      this.BodyPanel.Size = new System.Drawing.Size(628, 262);
      this.BodyPanel.TabIndex = 3;
      // 
      // BodyEdit
      // 
      this.BodyEdit.AcceptsReturn = true;
      this.BodyEdit.Dock = System.Windows.Forms.DockStyle.Fill;
      this.BodyEdit.Location = new System.Drawing.Point(8, 8);
      this.BodyEdit.Multiline = true;
      this.BodyEdit.Name = "BodyEdit";
      this.BodyEdit.Size = new System.Drawing.Size(612, 246);
      this.BodyEdit.TabIndex = 0;
      this.BodyEdit.TextChanged += new System.EventHandler(this.ControlContentChanged);
      // 
      // BlogItemEditorControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.BodyPanel);
      this.Controls.Add(this.BodyLabelPanel);
      this.Controls.Add(this.HeaderPanel);
      this.Controls.Add(this.TopLabelPanel);
      this.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
      this.Name = "BlogItemEditorControl";
      this.Padding = new System.Windows.Forms.Padding(8);
      this.Size = new System.Drawing.Size(644, 426);
      this.TopLabelPanel.ResumeLayout(false);
      this.HeaderPanel.ResumeLayout(false);
      this.HeaderPanel.PerformLayout();
      this.BodyLabelPanel.ResumeLayout(false);
      this.BodyPanel.ResumeLayout(false);
      this.BodyPanel.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel TopLabelPanel;
    private System.Windows.Forms.Label HeaderLabel;
    private System.Windows.Forms.Panel HeaderPanel;
    private System.Windows.Forms.TextBox TitleEdit;
    private System.Windows.Forms.Label TitleLabel;
    private System.Windows.Forms.Label CategoriesLabel;
    private System.Windows.Forms.TextBox CategoriesEdit;
    private System.Windows.Forms.Panel BodyLabelPanel;
    private System.Windows.Forms.Label BodyLabel;
    private System.Windows.Forms.Panel BodyPanel;
    private System.Windows.Forms.TextBox BodyEdit;
  }
}
