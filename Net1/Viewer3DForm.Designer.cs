namespace Net1
{
	partial class Viewer3DForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Viewer3DForm));
			this.pictureBoxSurface = new System.Windows.Forms.PictureBox();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.menuShow = new System.Windows.Forms.ToolStripDropDownButton();
			this.spatialLearningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.temporalLearningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.coordinateSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.activeColumnGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.regionPredictionsGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.regionPredictionReconstructionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showCorrectButton = new System.Windows.Forms.ToolStripButton();
			this.showSeqPredictingButton = new System.Windows.Forms.ToolStripButton();
			this.showPredictingButton = new System.Windows.Forms.ToolStripButton();
			this.showLearningButton = new System.Windows.Forms.ToolStripButton();
			this.showActiveButton = new System.Windows.Forms.ToolStripButton();
			this.showFalsePredictedButton = new System.Windows.Forms.ToolStripButton();
			this.btnResetCamera = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxSurface)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBoxSurface
			// 
			this.pictureBoxSurface.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBoxSurface.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxSurface.Name = "pictureBoxSurface";
			this.pictureBoxSurface.Size = new System.Drawing.Size(1064, 615);
			this.pictureBoxSurface.TabIndex = 1;
			this.pictureBoxSurface.TabStop = false;
			this.pictureBoxSurface.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxSurface_MouseClick);
			this.pictureBoxSurface.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxSurface_MouseDoubleClick);
			this.pictureBoxSurface.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxSurface_MouseMove);
			this.pictureBoxSurface.Resize += new System.EventHandler(this.pictureBoxSurface_Resize);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuShow,
            this.showCorrectButton,
            this.showSeqPredictingButton,
            this.showPredictingButton,
            this.showLearningButton,
            this.showActiveButton,
            this.showFalsePredictedButton,
            this.btnResetCamera});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1064, 25);
			this.toolStrip1.TabIndex = 2;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// menuShow
			// 
			this.menuShow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.menuShow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spatialLearningToolStripMenuItem,
            this.temporalLearningToolStripMenuItem,
            this.coordinateSystemToolStripMenuItem,
            this.activeColumnGridToolStripMenuItem,
            this.regionPredictionsGridToolStripMenuItem,
            this.regionPredictionReconstructionToolStripMenuItem});
			this.menuShow.Image = ((System.Drawing.Image)(resources.GetObject("menuShow.Image")));
			this.menuShow.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuShow.Name = "menuShow";
			this.menuShow.Size = new System.Drawing.Size(49, 22);
			this.menuShow.Text = "Show";
			// 
			// spatialLearningToolStripMenuItem
			// 
			this.spatialLearningToolStripMenuItem.Name = "spatialLearningToolStripMenuItem";
			this.spatialLearningToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
			this.spatialLearningToolStripMenuItem.Text = "Spatial Learning";
			this.spatialLearningToolStripMenuItem.Click += new System.EventHandler(this.spatialLearningToolStripMenuItem_Click);
			// 
			// temporalLearningToolStripMenuItem
			// 
			this.temporalLearningToolStripMenuItem.Name = "temporalLearningToolStripMenuItem";
			this.temporalLearningToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
			this.temporalLearningToolStripMenuItem.Text = "Temporal Learning";
			this.temporalLearningToolStripMenuItem.Click += new System.EventHandler(this.temporalLearningToolStripMenuItem_Click);
			// 
			// coordinateSystemToolStripMenuItem
			// 
			this.coordinateSystemToolStripMenuItem.Name = "coordinateSystemToolStripMenuItem";
			this.coordinateSystemToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
			this.coordinateSystemToolStripMenuItem.Text = "Coordinate System";
			this.coordinateSystemToolStripMenuItem.Click += new System.EventHandler(this.coordinateSystemToolStripMenuItem_Click);
			// 
			// activeColumnGridToolStripMenuItem
			// 
			this.activeColumnGridToolStripMenuItem.Name = "activeColumnGridToolStripMenuItem";
			this.activeColumnGridToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
			this.activeColumnGridToolStripMenuItem.Text = "Active Column Grid";
			this.activeColumnGridToolStripMenuItem.Click += new System.EventHandler(this.activeColumnGridToolStripMenuItem_Click);
			// 
			// regionPredictionsGridToolStripMenuItem
			// 
			this.regionPredictionsGridToolStripMenuItem.Name = "regionPredictionsGridToolStripMenuItem";
			this.regionPredictionsGridToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
			this.regionPredictionsGridToolStripMenuItem.Text = "Region Predictions Grid";
			this.regionPredictionsGridToolStripMenuItem.Click += new System.EventHandler(this.regionPredictionsGridToolStripMenuItem_Click);
			// 
			// regionPredictionReconstructionToolStripMenuItem
			// 
			this.regionPredictionReconstructionToolStripMenuItem.Name = "regionPredictionReconstructionToolStripMenuItem";
			this.regionPredictionReconstructionToolStripMenuItem.Size = new System.Drawing.Size(251, 22);
			this.regionPredictionReconstructionToolStripMenuItem.Text = "Region Prediction Reconstruction";
			this.regionPredictionReconstructionToolStripMenuItem.Click += new System.EventHandler(this.regionPredictionReconstructionToolStripMenuItem_Click);
			// 
			// showCorrectButton
			// 
			this.showCorrectButton.BackColor = System.Drawing.Color.Green;
			this.showCorrectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.showCorrectButton.ForeColor = System.Drawing.Color.Black;
			this.showCorrectButton.Image = ((System.Drawing.Image)(resources.GetObject("showCorrectButton.Image")));
			this.showCorrectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.showCorrectButton.Name = "showCorrectButton";
			this.showCorrectButton.Size = new System.Drawing.Size(23, 22);
			this.showCorrectButton.ToolTipText = "Show Correct";
			this.showCorrectButton.Click += new System.EventHandler(this.showCorrectButton_Click);
			// 
			// showSeqPredictingButton
			// 
			this.showSeqPredictingButton.BackColor = System.Drawing.Color.Aqua;
			this.showSeqPredictingButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.showSeqPredictingButton.ForeColor = System.Drawing.Color.Black;
			this.showSeqPredictingButton.Image = ((System.Drawing.Image)(resources.GetObject("showSeqPredictingButton.Image")));
			this.showSeqPredictingButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.showSeqPredictingButton.Name = "showSeqPredictingButton";
			this.showSeqPredictingButton.Size = new System.Drawing.Size(23, 22);
			this.showSeqPredictingButton.ToolTipText = "Show Seq Predicting";
			this.showSeqPredictingButton.Click += new System.EventHandler(this.showSeqPredictingButton_Click);
			// 
			// showPredictingButton
			// 
			this.showPredictingButton.BackColor = System.Drawing.Color.Chocolate;
			this.showPredictingButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.showPredictingButton.ForeColor = System.Drawing.Color.Black;
			this.showPredictingButton.Image = ((System.Drawing.Image)(resources.GetObject("showPredictingButton.Image")));
			this.showPredictingButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.showPredictingButton.Name = "showPredictingButton";
			this.showPredictingButton.Size = new System.Drawing.Size(23, 22);
			this.showPredictingButton.ToolTipText = "Show Predicting";
			this.showPredictingButton.Click += new System.EventHandler(this.showPredictingButton_Click);
			// 
			// showLearningButton
			// 
			this.showLearningButton.BackColor = System.Drawing.Color.Gray;
			this.showLearningButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.showLearningButton.ForeColor = System.Drawing.Color.Black;
			this.showLearningButton.Image = ((System.Drawing.Image)(resources.GetObject("showLearningButton.Image")));
			this.showLearningButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.showLearningButton.Name = "showLearningButton";
			this.showLearningButton.Size = new System.Drawing.Size(23, 22);
			this.showLearningButton.ToolTipText = "Show Learning";
			this.showLearningButton.Click += new System.EventHandler(this.showLearningButton_Click);
			// 
			// showActiveButton
			// 
			this.showActiveButton.BackColor = System.Drawing.Color.Black;
			this.showActiveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.showActiveButton.ForeColor = System.Drawing.Color.White;
			this.showActiveButton.Image = ((System.Drawing.Image)(resources.GetObject("showActiveButton.Image")));
			this.showActiveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.showActiveButton.Name = "showActiveButton";
			this.showActiveButton.Size = new System.Drawing.Size(23, 22);
			this.showActiveButton.ToolTipText = "Show Active";
			this.showActiveButton.Click += new System.EventHandler(this.showActiveButton_Click);
			// 
			// showFalsePredictedButton
			// 
			this.showFalsePredictedButton.BackColor = System.Drawing.Color.Red;
			this.showFalsePredictedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.showFalsePredictedButton.ForeColor = System.Drawing.Color.Black;
			this.showFalsePredictedButton.Image = ((System.Drawing.Image)(resources.GetObject("showFalsePredictedButton.Image")));
			this.showFalsePredictedButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.showFalsePredictedButton.Name = "showFalsePredictedButton";
			this.showFalsePredictedButton.Size = new System.Drawing.Size(23, 22);
			this.showFalsePredictedButton.ToolTipText = "Show False Predicted";
			this.showFalsePredictedButton.Click += new System.EventHandler(this.showFalsePredictedButton_Click);
			// 
			// btnResetCamera
			// 
			this.btnResetCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.btnResetCamera.Image = ((System.Drawing.Image)(resources.GetObject("btnResetCamera.Image")));
			this.btnResetCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnResetCamera.Name = "btnResetCamera";
			this.btnResetCamera.Size = new System.Drawing.Size(67, 22);
			this.btnResetCamera.Text = "Reset Cam";
			this.btnResetCamera.ToolTipText = "Reset Camera";
			this.btnResetCamera.Click += new System.EventHandler(this.btnResetCamera_Click);
			// 
			// Viewer3DForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1064, 615);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.pictureBoxSurface);
			this.KeyPreview = true;
			this.Name = "Viewer3DForm";
			this.TabText = "Viewer3DForm";
			this.Text = "Viewer3DForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Viewer3DForm_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Viewer3DForm_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxSurface)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		public System.Windows.Forms.PictureBox pictureBoxSurface;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripDropDownButton menuShow;
		private System.Windows.Forms.ToolStripMenuItem spatialLearningToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem temporalLearningToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem coordinateSystemToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem activeColumnGridToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem regionPredictionsGridToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem regionPredictionReconstructionToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton showCorrectButton;
		private System.Windows.Forms.ToolStripButton showSeqPredictingButton;
		private System.Windows.Forms.ToolStripButton showPredictingButton;
		private System.Windows.Forms.ToolStripButton showLearningButton;
		private System.Windows.Forms.ToolStripButton showActiveButton;
		private System.Windows.Forms.ToolStripButton showFalsePredictedButton;
		private System.Windows.Forms.ToolStripButton btnResetCamera;
	}
}