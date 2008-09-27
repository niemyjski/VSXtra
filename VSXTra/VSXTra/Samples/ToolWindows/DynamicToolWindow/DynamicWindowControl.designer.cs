/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace DeepDiver.DynamicToolWindow
{
	partial class DynamicWindowControl
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2204:LiteralsShouldBeSpelledCorrectly")]
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.xText = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.yText = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.widthText = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.heightText = new System.Windows.Forms.TextBox();
			this.dockedCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(17, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "&X:";
			// 
			// xText
			// 
			this.xText.Location = new System.Drawing.Point(66, 11);
			this.xText.Name = "xText";
			this.xText.ReadOnly = true;
			this.xText.Size = new System.Drawing.Size(83, 20);
			this.xText.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(17, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "&Y:";
			// 
			// yText
			// 
			this.yText.Location = new System.Drawing.Point(66, 36);
			this.yText.Name = "yText";
			this.yText.ReadOnly = true;
			this.yText.Size = new System.Drawing.Size(83, 20);
			this.yText.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "&Width:";
			// 
			// widthText
			// 
			this.widthText.Location = new System.Drawing.Point(66, 61);
			this.widthText.Name = "widthText";
			this.widthText.ReadOnly = true;
			this.widthText.Size = new System.Drawing.Size(83, 20);
			this.widthText.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 89);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "&Height:";
			// 
			// heightText
			// 
			this.heightText.Location = new System.Drawing.Point(66, 86);
			this.heightText.Name = "heightText";
			this.heightText.ReadOnly = true;
			this.heightText.Size = new System.Drawing.Size(83, 20);
			this.heightText.TabIndex = 7;
			// 
			// dockedCheckBox
			// 
			this.dockedCheckBox.AutoCheck = false;
			this.dockedCheckBox.AutoSize = true;
			this.dockedCheckBox.Enabled = false;
			this.dockedCheckBox.Location = new System.Drawing.Point(10, 128);
			this.dockedCheckBox.Name = "dockedCheckBox";
			this.dockedCheckBox.Size = new System.Drawing.Size(72, 17);
			this.dockedCheckBox.TabIndex = 8;
			this.dockedCheckBox.Text = "Dockable";
			this.dockedCheckBox.UseVisualStyleBackColor = true;
			// 
			// DynamicWindowControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Controls.Add(this.dockedCheckBox);
			this.Controls.Add(this.heightText);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.widthText);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.yText);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.xText);
			this.Controls.Add(this.label1);
			this.Name = "DynamicWindowControl";
			this.Size = new System.Drawing.Size(157, 167);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox xText;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox yText;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox widthText;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox heightText;
		private System.Windows.Forms.CheckBox dockedCheckBox;
	}
}
