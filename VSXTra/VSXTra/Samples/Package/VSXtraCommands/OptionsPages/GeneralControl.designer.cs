namespace DeepDiver.VSXtraCommands
{
    partial class GeneralControl
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
            if(disposing && (components != null))
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
            this.grpGeneral = new System.Windows.Forms.GroupBox();
            this.chkRemoveAndSortUsingsOnSave = new System.Windows.Forms.CheckBox();
            this.chkFormatOnSave = new System.Windows.Forms.CheckBox();
            this.grpGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpGeneral
            // 
            this.grpGeneral.Controls.Add(this.chkRemoveAndSortUsingsOnSave);
            this.grpGeneral.Controls.Add(this.chkFormatOnSave);
            this.grpGeneral.Location = new System.Drawing.Point(3, 3);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(379, 89);
            this.grpGeneral.TabIndex = 0;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "General";
            // 
            // chkRemoveAndSortUsingsOnSave
            // 
            this.chkRemoveAndSortUsingsOnSave.AutoSize = true;
            this.chkRemoveAndSortUsingsOnSave.Location = new System.Drawing.Point(16, 57);
            this.chkRemoveAndSortUsingsOnSave.Name = "chkRemoveAndSortUsingsOnSave";
            this.chkRemoveAndSortUsingsOnSave.Size = new System.Drawing.Size(185, 17);
            this.chkRemoveAndSortUsingsOnSave.TabIndex = 1;
            this.chkRemoveAndSortUsingsOnSave.Text = "Remove and Sort Usings on save";
            this.chkRemoveAndSortUsingsOnSave.UseVisualStyleBackColor = true;
            this.chkRemoveAndSortUsingsOnSave.CheckedChanged += new System.EventHandler(this.RemoveAndSortUsingsOnSave_CheckedChanged);
            // 
            // chkFormatOnSave
            // 
            this.chkFormatOnSave.AutoSize = true;
            this.chkFormatOnSave.Location = new System.Drawing.Point(16, 27);
            this.chkFormatOnSave.Name = "chkFormatOnSave";
            this.chkFormatOnSave.Size = new System.Drawing.Size(149, 17);
            this.chkFormatOnSave.TabIndex = 0;
            this.chkFormatOnSave.Text = "Format document on save";
            this.chkFormatOnSave.UseVisualStyleBackColor = true;
            this.chkFormatOnSave.CheckedChanged += new System.EventHandler(this.chkFormatOnSave_CheckedChanged);
            // 
            // GeneralControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpGeneral);
            this.Name = "GeneralControl";
            this.Size = new System.Drawing.Size(385, 97);
            this.Load += new System.EventHandler(this.GeneralControl_Load);
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpGeneral;
        private System.Windows.Forms.CheckBox chkRemoveAndSortUsingsOnSave;
        private System.Windows.Forms.CheckBox chkFormatOnSave;
    }
}
