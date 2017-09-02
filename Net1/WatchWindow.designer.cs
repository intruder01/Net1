namespace Net1
{
	partial class WatchWindow
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
			if ( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.propertyGrid1 = new AdamsLair.WinForms.PropertyEditing.PropertyGrid();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(284, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.AllowDrop = true;
			this.propertyGrid1.AutoScroll = true;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 25);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.ReadOnly = false;
			this.propertyGrid1.ShowNonPublic = false;
			this.propertyGrid1.Size = new System.Drawing.Size(284, 327);
			this.propertyGrid1.SplitterPosition = 114;
			this.propertyGrid1.SplitterRatio = 0.4F;
			this.propertyGrid1.TabIndex = 1;
			// 
			// WatchWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 352);
			this.Controls.Add(this.propertyGrid1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "WatchWindow";
			this.Text = "WatchWindow";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WatchWindow_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private AdamsLair.WinForms.PropertyEditing.PropertyGrid propertyGrid1;
	}
}