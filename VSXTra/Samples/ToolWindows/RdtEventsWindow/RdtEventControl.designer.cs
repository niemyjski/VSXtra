/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace DeepDiver.RdtEventsWindow
{
    partial class RdtEventControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }


        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
          this.components = new System.ComponentModel.Container();
          this.eventGrid = new System.Windows.Forms.DataGridView();
          this.detailsColumn = new System.Windows.Forms.DataGridViewButtonColumn();
          this.myControlBindingSource = new System.Windows.Forms.BindingSource(this.components);
          ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.myControlBindingSource)).BeginInit();
          this.SuspendLayout();
          // 
          // eventGrid
          // 
          this.eventGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
          this.eventGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
          this.eventGrid.Dock = System.Windows.Forms.DockStyle.Fill;
          this.eventGrid.Location = new System.Drawing.Point(0, 0);
          this.eventGrid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
          this.eventGrid.Name = "eventGrid";
          this.eventGrid.RowTemplate.Height = 24;
          this.eventGrid.Size = new System.Drawing.Size(519, 361);
          this.eventGrid.TabIndex = 1;
          this.eventGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.eventGrid_CellClick);
          // 
          // detailsColumn
          // 
          this.detailsColumn.Name = "detailsColumn";
          // 
          // myControlBindingSource
          // 
          this.myControlBindingSource.DataSource = typeof(DeepDiver.RdtEventsWindow.RdtEventControl);
          // 
          // RdtEventControl
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.BackColor = System.Drawing.SystemColors.Window;
          this.Controls.Add(this.eventGrid);
          this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
          this.Name = "RdtEventControl";
          this.Size = new System.Drawing.Size(519, 361);
          ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.myControlBindingSource)).EndInit();
          this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.BindingSource myControlBindingSource;
        private System.Windows.Forms.DataGridView eventGrid;
        private System.Windows.Forms.DataGridViewButtonColumn detailsColumn;
    }
}
