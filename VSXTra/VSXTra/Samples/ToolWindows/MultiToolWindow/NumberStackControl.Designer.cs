namespace DeepDiver.MultiToolWindow
{
  partial class NumberStackControl
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
      this.InputPanel = new System.Windows.Forms.Panel();
      this.PushToStack = new System.Windows.Forms.Button();
      this.TypedTextEdit = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.TextStackPanel = new System.Windows.Forms.Panel();
      this.TextStack = new System.Windows.Forms.ListBox();
      this.NumberStackPanel = new System.Windows.Forms.Panel();
      this.NumberStack = new System.Windows.Forms.ListBox();
      this.InputPanel.SuspendLayout();
      this.TextStackPanel.SuspendLayout();
      this.NumberStackPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // InputPanel
      // 
      this.InputPanel.Controls.Add(this.PushToStack);
      this.InputPanel.Controls.Add(this.TypedTextEdit);
      this.InputPanel.Controls.Add(this.label1);
      this.InputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.InputPanel.Location = new System.Drawing.Point(0, 238);
      this.InputPanel.Name = "InputPanel";
      this.InputPanel.Padding = new System.Windows.Forms.Padding(8);
      this.InputPanel.Size = new System.Drawing.Size(625, 35);
      this.InputPanel.TabIndex = 0;
      // 
      // PushToStack
      // 
      this.PushToStack.Enabled = false;
      this.PushToStack.Location = new System.Drawing.Point(359, -1);
      this.PushToStack.Name = "PushToStack";
      this.PushToStack.Size = new System.Drawing.Size(134, 27);
      this.PushToStack.TabIndex = 2;
      this.PushToStack.Text = "Push to stack";
      this.PushToStack.UseVisualStyleBackColor = true;
      this.PushToStack.Click += new System.EventHandler(this.PushToStack_Click);
      // 
      // TypedTextEdit
      // 
      this.TypedTextEdit.Location = new System.Drawing.Point(178, 1);
      this.TypedTextEdit.Name = "TypedTextEdit";
      this.TypedTextEdit.Size = new System.Drawing.Size(175, 22);
      this.TypedTextEdit.TabIndex = 1;
      this.TypedTextEdit.TextChanged += new System.EventHandler(this.TypedTextEdit_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(5, 4);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(167, 17);
      this.label1.TabIndex = 0;
      this.label1.Text = "Text to push to the stack:";
      // 
      // TextStackPanel
      // 
      this.TextStackPanel.Controls.Add(this.TextStack);
      this.TextStackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.TextStackPanel.Location = new System.Drawing.Point(0, 0);
      this.TextStackPanel.Name = "TextStackPanel";
      this.TextStackPanel.Padding = new System.Windows.Forms.Padding(8);
      this.TextStackPanel.Size = new System.Drawing.Size(625, 238);
      this.TextStackPanel.TabIndex = 1;
      // 
      // TextStack
      // 
      this.TextStack.Dock = System.Windows.Forms.DockStyle.Fill;
      this.TextStack.FormattingEnabled = true;
      this.TextStack.IntegralHeight = false;
      this.TextStack.ItemHeight = 16;
      this.TextStack.Location = new System.Drawing.Point(8, 8);
      this.TextStack.Name = "TextStack";
      this.TextStack.Size = new System.Drawing.Size(609, 222);
      this.TextStack.TabIndex = 0;
      // 
      // NumberStackPanel
      // 
      this.NumberStackPanel.Controls.Add(this.NumberStack);
      this.NumberStackPanel.Dock = System.Windows.Forms.DockStyle.Right;
      this.NumberStackPanel.Location = new System.Drawing.Point(345, 0);
      this.NumberStackPanel.Name = "NumberStackPanel";
      this.NumberStackPanel.Padding = new System.Windows.Forms.Padding(8);
      this.NumberStackPanel.Size = new System.Drawing.Size(280, 238);
      this.NumberStackPanel.TabIndex = 2;
      // 
      // NumberStack
      // 
      this.NumberStack.Dock = System.Windows.Forms.DockStyle.Fill;
      this.NumberStack.FormattingEnabled = true;
      this.NumberStack.IntegralHeight = false;
      this.NumberStack.ItemHeight = 16;
      this.NumberStack.Location = new System.Drawing.Point(8, 8);
      this.NumberStack.Name = "NumberStack";
      this.NumberStack.Size = new System.Drawing.Size(264, 222);
      this.NumberStack.TabIndex = 0;
      // 
      // NumberStackControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.NumberStackPanel);
      this.Controls.Add(this.TextStackPanel);
      this.Controls.Add(this.InputPanel);
      this.Name = "NumberStackControl";
      this.Size = new System.Drawing.Size(625, 273);
      this.InputPanel.ResumeLayout(false);
      this.InputPanel.PerformLayout();
      this.TextStackPanel.ResumeLayout(false);
      this.NumberStackPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel InputPanel;
    private System.Windows.Forms.TextBox TypedTextEdit;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Panel TextStackPanel;
    private System.Windows.Forms.Button PushToStack;
    private System.Windows.Forms.ListBox TextStack;
    private System.Windows.Forms.Panel NumberStackPanel;
    private System.Windows.Forms.ListBox NumberStack;
  }
}
