namespace DeepDiver.VSXtraCommands
{
    public partial class CommandsControl
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
            this.gridVisibility = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridVisibility)).BeginInit();
            this.SuspendLayout();
            // 
            // gridVisibility
            // 
            this.gridVisibility.AllowUserToAddRows = false;
            this.gridVisibility.AllowUserToDeleteRows = false;
            this.gridVisibility.AllowUserToResizeRows = false;
            this.gridVisibility.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridVisibility.Location = new System.Drawing.Point(14, 15);
            this.gridVisibility.MultiSelect = false;
            this.gridVisibility.Name = "gridVisibility";
            this.gridVisibility.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridVisibility.ShowEditingIcon = false;
            this.gridVisibility.Size = new System.Drawing.Size(359, 254);
            this.gridVisibility.TabIndex = 0;
            this.gridVisibility.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridVisibility_CellValueChanged);
            this.gridVisibility.MouseLeave += new System.EventHandler(this.gridVisibility_MouseLeave);
            // 
            // CommandsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.gridVisibility);
            this.Name = "CommandsControl";
            this.Size = new System.Drawing.Size(388, 285);
            this.Load += new System.EventHandler(this.CommandsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridVisibility)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridVisibility;


    }
}
