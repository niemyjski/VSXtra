using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace DeepDiver.VSXtraCommands
{
    partial class UndoCloseControl
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
            this.lstDocuments = new System.Windows.Forms.ListView();
            this.Item = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lstDocuments
            // 
            this.lstDocuments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Item});
            this.lstDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstDocuments.FullRowSelect = true;
            this.lstDocuments.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstDocuments.HideSelection = false;
            this.lstDocuments.Location = new System.Drawing.Point(0, 0);
            this.lstDocuments.MultiSelect = false;
            this.lstDocuments.Name = "lstDocuments";
            this.lstDocuments.Size = new System.Drawing.Size(677, 196);
            this.lstDocuments.TabIndex = 0;
            this.lstDocuments.UseCompatibleStateImageBehavior = false;
            this.lstDocuments.View = System.Windows.Forms.View.Details;
            this.lstDocuments.DoubleClick += new System.EventHandler(this.lstDocuments_DoubleClick);
            this.lstDocuments.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstDocuments_KeyPress);
            // 
            // Item
            // 
            this.Item.Text = "Item";
            // 
            // UndoCloseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lstDocuments);
            this.Name = "UndoCloseControl";
            this.Size = new System.Drawing.Size(677, 196);
            this.Load += new System.EventHandler(this.UndoCloseControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ListView lstDocuments;
        private ColumnHeader Item;

    }
}
