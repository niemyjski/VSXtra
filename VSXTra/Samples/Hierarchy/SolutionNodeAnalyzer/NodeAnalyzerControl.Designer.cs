// ================================================================================================
// NodeAnalyzerControl.Designer.cs
//
// Created: 2008.09.28, by Istvan Novak (DeepDiver)
// ================================================================================================
namespace DeepDiver.SolutionNodeAnalyzer
{
    partial class NodeAnalyzerControl
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NodeAnalyzerControl));
          this.NodeListView = new System.Windows.Forms.ListView();
          this.IDHeader = new System.Windows.Forms.ColumnHeader();
          this.DepthHeader = new System.Windows.Forms.ColumnHeader();
          this.NameHeader = new System.Windows.Forms.ColumnHeader();
          this.ParentIDHeader = new System.Windows.Forms.ColumnHeader();
          this.ParentHierHeader = new System.Windows.Forms.ColumnHeader();
          this.NodeImages = new System.Windows.Forms.ImageList(this.components);
          this.SuspendLayout();
          // 
          // NodeListView
          // 
          this.NodeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IDHeader,
            this.DepthHeader,
            this.NameHeader,
            this.ParentIDHeader,
            this.ParentHierHeader});
          this.NodeListView.Dock = System.Windows.Forms.DockStyle.Fill;
          this.NodeListView.FullRowSelect = true;
          this.NodeListView.Location = new System.Drawing.Point(0, 0);
          this.NodeListView.Name = "NodeListView";
          this.NodeListView.Size = new System.Drawing.Size(1113, 468);
          this.NodeListView.SmallImageList = this.NodeImages;
          this.NodeListView.TabIndex = 0;
          this.NodeListView.UseCompatibleStateImageBehavior = false;
          this.NodeListView.View = System.Windows.Forms.View.Details;
          // 
          // IDHeader
          // 
          this.IDHeader.Text = "ID";
          this.IDHeader.Width = 140;
          // 
          // DepthHeader
          // 
          this.DepthHeader.Text = "Depth";
          this.DepthHeader.Width = 80;
          // 
          // NameHeader
          // 
          this.NameHeader.Text = "Name";
          this.NameHeader.Width = 400;
          // 
          // ParentIDHeader
          // 
          this.ParentIDHeader.Text = "Parent ID";
          this.ParentIDHeader.Width = 120;
          // 
          // ParentHierHeader
          // 
          this.ParentHierHeader.Text = "Parent Hierarchy";
          this.ParentHierHeader.Width = 359;
          // 
          // NodeImages
          // 
          this.NodeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("NodeImages.ImageStream")));
          this.NodeImages.TransparentColor = System.Drawing.Color.Transparent;
          this.NodeImages.Images.SetKeyName(0, "Hierarchy");
          this.NodeImages.Images.SetKeyName(1, "Item");
          // 
          // NodeAnalyzerControl
          // 
          this.BackColor = System.Drawing.SystemColors.Window;
          this.Controls.Add(this.NodeListView);
          this.Name = "NodeAnalyzerControl";
          this.Size = new System.Drawing.Size(1113, 468);
          this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.ListView NodeListView;
        private System.Windows.Forms.ImageList NodeImages;
        private System.Windows.Forms.ColumnHeader IDHeader;
        private System.Windows.Forms.ColumnHeader NameHeader;
        private System.Windows.Forms.ColumnHeader ParentIDHeader;
        private System.Windows.Forms.ColumnHeader DepthHeader;
        private System.Windows.Forms.ColumnHeader ParentHierHeader;

    }
}
