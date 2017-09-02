namespace Net1
{
	partial class WatchForm
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
			//if (disposing)
			//{
			//	if (components != null)
			//	{
			//		components.Dispose ();
			//	}
			//	if (_instance != null)
			//	{
			//		_instance.Dispose ();
			//		_instance = null;
			//	}
			//}
			//base.Dispose ( disposing );


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
			this.watchPropertyGrid = new AdamsLair.WinForms.PropertyEditing.PropertyGrid();
			this.SuspendLayout();
			// 
			// watchPropertyGrid
			// 
			this.watchPropertyGrid.AllowDrop = true;
			this.watchPropertyGrid.AutoScroll = true;
			this.watchPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.watchPropertyGrid.Location = new System.Drawing.Point(0, 0);
			this.watchPropertyGrid.Name = "watchPropertyGrid";
			this.watchPropertyGrid.ReadOnly = false;
			this.watchPropertyGrid.ShowNonPublic = false;
			this.watchPropertyGrid.Size = new System.Drawing.Size(307, 409);
			this.watchPropertyGrid.SplitterPosition = 123;
			this.watchPropertyGrid.SplitterRatio = 0.4F;
			this.watchPropertyGrid.TabIndex = 0;
			// 
			// WatchForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(307, 409);
			this.Controls.Add(this.watchPropertyGrid);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "WatchForm";
			this.Text = "Watch";
			this.ResumeLayout(false);

		}

		#endregion

		private AdamsLair.WinForms.PropertyEditing.PropertyGrid watchPropertyGrid;





	}
}