namespace Net1
{
	partial class netForm1
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.networkConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.btnViewer3D = new System.Windows.Forms.Button();
			this.cbRefreshArea1 = new System.Windows.Forms.CheckBox();
			this.label24 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.btnNextCaseTrain = new System.Windows.Forms.Button();
			this.cbSparsenessParamSearchEnable = new System.Windows.Forms.CheckBox();
			this.txtSparsenessInterval = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.txtSparsenessActual = new System.Windows.Forms.TextBox();
			this.txtSparsenessTarget = new System.Windows.Forms.TextBox();
			this.btnNextCase = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnTrain = new System.Windows.Forms.Button();
			this.btnNetConfig = new System.Windows.Forms.Button();
			this.txtFileNumColumnsY = new System.Windows.Forms.TextBox();
			this.txtFileNumColumnsX = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.btnOpenFile = new System.Windows.Forms.Button();
			this.txtFilename = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtNumEpochsToTrain = new System.Windows.Forms.TextBox();
			this.btnTrainEpochs = new System.Windows.Forms.Button();
			this.btnTrainCase = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.txtCurrCaseNum = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtCurrEpochNum = new System.Windows.Forms.TextBox();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.cbRefreshArea2 = new System.Windows.Forms.CheckBox();
			this.tabsNetDisplay = new System.Windows.Forms.TabControl();
			this.tabInputPlane = new System.Windows.Forms.TabPage();
			this.tabProximalSynapses = new System.Windows.Forms.TabPage();
			this.tabInputOverlap = new System.Windows.Forms.TabPage();
			this.tabBoost = new System.Windows.Forms.TabPage();
			this.tabActivation = new System.Windows.Forms.TabPage();
			this.tabPrediction = new System.Windows.Forms.TabPage();
			this.cbRefreshArea3 = new System.Windows.Forms.CheckBox();
			this.tabsStatistics = new System.Windows.Forms.TabControl();
			this.tabSparsenessChart = new System.Windows.Forms.TabPage();
			this.txtSparsenessStdDev = new System.Windows.Forms.TextBox();
			this.txtSparsenessAve = new System.Windows.Forms.TextBox();
			this.txtSparsenessVal = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.chartSparseness = new LiveCharts.WinForms.CartesianChart();
			this.label3 = new System.Windows.Forms.Label();
			this.tabEntropyChart = new System.Windows.Forms.TabPage();
			this.txtEntropyStdDev = new System.Windows.Forms.TextBox();
			this.txtEntropyAve = new System.Windows.Forms.TextBox();
			this.txtEntropyVal = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.chartEntropy = new LiveCharts.WinForms.CartesianChart();
			this.label17 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tabParams = new System.Windows.Forms.TabPage();
			this.txtTabPrmsSparsenessOutput = new System.Windows.Forms.TextBox();
			this.label22 = new System.Windows.Forms.Label();
			this.txtTabPrmsSparsenessSetpoint = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.txtTabPrmsSparsenessInput = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tabsNetDisplay.SuspendLayout();
			this.tabsStatistics.SuspendLayout();
			this.tabSparsenessChart.SuspendLayout();
			this.tabEntropyChart.SuspendLayout();
			this.tabParams.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.configurationToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(798, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// configurationToolStripMenuItem
			// 
			this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.networkConfigurationToolStripMenuItem});
			this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
			this.configurationToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
			this.configurationToolStripMenuItem.Text = "Configuration";
			// 
			// networkConfigurationToolStripMenuItem
			// 
			this.networkConfigurationToolStripMenuItem.Name = "networkConfigurationToolStripMenuItem";
			this.networkConfigurationToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
			this.networkConfigurationToolStripMenuItem.Text = "Network Configuration";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.btnViewer3D);
			this.splitContainer1.Panel1.Controls.Add(this.cbRefreshArea1);
			this.splitContainer1.Panel1.Controls.Add(this.label24);
			this.splitContainer1.Panel1.Controls.Add(this.label23);
			this.splitContainer1.Panel1.Controls.Add(this.label18);
			this.splitContainer1.Panel1.Controls.Add(this.btnNextCaseTrain);
			this.splitContainer1.Panel1.Controls.Add(this.cbSparsenessParamSearchEnable);
			this.splitContainer1.Panel1.Controls.Add(this.txtSparsenessInterval);
			this.splitContainer1.Panel1.Controls.Add(this.label14);
			this.splitContainer1.Panel1.Controls.Add(this.label13);
			this.splitContainer1.Panel1.Controls.Add(this.label11);
			this.splitContainer1.Panel1.Controls.Add(this.label12);
			this.splitContainer1.Panel1.Controls.Add(this.txtSparsenessActual);
			this.splitContainer1.Panel1.Controls.Add(this.txtSparsenessTarget);
			this.splitContainer1.Panel1.Controls.Add(this.btnNextCase);
			this.splitContainer1.Panel1.Controls.Add(this.btnStop);
			this.splitContainer1.Panel1.Controls.Add(this.btnTrain);
			this.splitContainer1.Panel1.Controls.Add(this.btnNetConfig);
			this.splitContainer1.Panel1.Controls.Add(this.txtFileNumColumnsY);
			this.splitContainer1.Panel1.Controls.Add(this.txtFileNumColumnsX);
			this.splitContainer1.Panel1.Controls.Add(this.label9);
			this.splitContainer1.Panel1.Controls.Add(this.label8);
			this.splitContainer1.Panel1.Controls.Add(this.label7);
			this.splitContainer1.Panel1.Controls.Add(this.btnOpenFile);
			this.splitContainer1.Panel1.Controls.Add(this.txtFilename);
			this.splitContainer1.Panel1.Controls.Add(this.label6);
			this.splitContainer1.Panel1.Controls.Add(this.label5);
			this.splitContainer1.Panel1.Controls.Add(this.txtNumEpochsToTrain);
			this.splitContainer1.Panel1.Controls.Add(this.btnTrainEpochs);
			this.splitContainer1.Panel1.Controls.Add(this.btnTrainCase);
			this.splitContainer1.Panel1.Controls.Add(this.label2);
			this.splitContainer1.Panel1.Controls.Add(this.txtCurrCaseNum);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.txtCurrEpochNum);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(798, 508);
			this.splitContainer1.SplitterDistance = 183;
			this.splitContainer1.TabIndex = 1;
			// 
			// btnViewer3D
			// 
			this.btnViewer3D.Location = new System.Drawing.Point(6, 309);
			this.btnViewer3D.Name = "btnViewer3D";
			this.btnViewer3D.Size = new System.Drawing.Size(70, 23);
			this.btnViewer3D.TabIndex = 38;
			this.btnViewer3D.Text = "3D View";
			this.btnViewer3D.UseVisualStyleBackColor = true;
			this.btnViewer3D.Click += new System.EventHandler(this.btnViewer3D_Click);
			// 
			// cbRefreshArea1
			// 
			this.cbRefreshArea1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbRefreshArea1.AutoSize = true;
			this.cbRefreshArea1.Location = new System.Drawing.Point(165, 3);
			this.cbRefreshArea1.Name = "cbRefreshArea1";
			this.cbRefreshArea1.Size = new System.Drawing.Size(15, 14);
			this.cbRefreshArea1.TabIndex = 37;
			this.cbRefreshArea1.UseVisualStyleBackColor = true;
			this.cbRefreshArea1.CheckedChanged += new System.EventHandler(this.cbRefreshArea1_CheckedChanged);
			// 
			// label24
			// 
			this.label24.AutoSize = true;
			this.label24.Location = new System.Drawing.Point(153, 250);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(25, 13);
			this.label24.TabIndex = 36;
			this.label24.Text = "Prm";
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.Location = new System.Drawing.Point(5, 237);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(62, 13);
			this.label23.TabIndex = 35;
			this.label23.Text = "Sparseness";
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(115, 62);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(31, 13);
			this.label18.TabIndex = 34;
			this.label18.Text = "Case";
			// 
			// btnNextCaseTrain
			// 
			this.btnNextCaseTrain.Location = new System.Drawing.Point(118, 104);
			this.btnNextCaseTrain.Name = "btnNextCaseTrain";
			this.btnNextCaseTrain.Size = new System.Drawing.Size(50, 23);
			this.btnNextCaseTrain.TabIndex = 33;
			this.btnNextCaseTrain.Text = "Trn Nxt";
			this.btnNextCaseTrain.UseVisualStyleBackColor = true;
			this.btnNextCaseTrain.Click += new System.EventHandler(this.btnNextCaseTrain_Click);
			// 
			// cbSparsenessParamSearchEnable
			// 
			this.cbSparsenessParamSearchEnable.AutoSize = true;
			this.cbSparsenessParamSearchEnable.Location = new System.Drawing.Point(159, 269);
			this.cbSparsenessParamSearchEnable.Name = "cbSparsenessParamSearchEnable";
			this.cbSparsenessParamSearchEnable.Size = new System.Drawing.Size(15, 14);
			this.cbSparsenessParamSearchEnable.TabIndex = 0;
			this.cbSparsenessParamSearchEnable.UseVisualStyleBackColor = true;
			this.cbSparsenessParamSearchEnable.CheckedChanged += new System.EventHandler(this.cbSparsenessParamSearchEnable_CheckedChanged);
			// 
			// txtSparsenessInterval
			// 
			this.txtSparsenessInterval.Location = new System.Drawing.Point(108, 266);
			this.txtSparsenessInterval.Name = "txtSparsenessInterval";
			this.txtSparsenessInterval.Size = new System.Drawing.Size(40, 20);
			this.txtSparsenessInterval.TabIndex = 32;
			this.txtSparsenessInterval.TextChanged += new System.EventHandler(this.txtSparsenessInterval_TextChanged);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(10, 269);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(0, 13);
			this.label14.TabIndex = 28;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(105, 250);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(42, 13);
			this.label13.TabIndex = 27;
			this.label13.Text = "Interval";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(54, 250);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(37, 13);
			this.label11.TabIndex = 26;
			this.label11.Text = "Actual";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(5, 250);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(38, 13);
			this.label12.TabIndex = 25;
			this.label12.Text = "Target";
			// 
			// txtSparsenessActual
			// 
			this.txtSparsenessActual.Enabled = false;
			this.txtSparsenessActual.Location = new System.Drawing.Point(57, 266);
			this.txtSparsenessActual.Name = "txtSparsenessActual";
			this.txtSparsenessActual.Size = new System.Drawing.Size(40, 20);
			this.txtSparsenessActual.TabIndex = 24;
			// 
			// txtSparsenessTarget
			// 
			this.txtSparsenessTarget.Location = new System.Drawing.Point(6, 266);
			this.txtSparsenessTarget.Name = "txtSparsenessTarget";
			this.txtSparsenessTarget.Size = new System.Drawing.Size(40, 20);
			this.txtSparsenessTarget.TabIndex = 23;
			this.txtSparsenessTarget.TextChanged += new System.EventHandler(this.txtSparsenessTarget_TextChanged);
			// 
			// btnNextCase
			// 
			this.btnNextCase.Location = new System.Drawing.Point(62, 78);
			this.btnNextCase.Name = "btnNextCase";
			this.btnNextCase.Size = new System.Drawing.Size(50, 23);
			this.btnNextCase.TabIndex = 22;
			this.btnNextCase.Text = "Nxt Cas";
			this.btnNextCase.UseVisualStyleBackColor = true;
			this.btnNextCase.Click += new System.EventHandler(this.btnNextCase_Click);
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(85, 162);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(67, 23);
			this.btnStop.TabIndex = 21;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnTrain
			// 
			this.btnTrain.Location = new System.Drawing.Point(6, 162);
			this.btnTrain.Name = "btnTrain";
			this.btnTrain.Size = new System.Drawing.Size(70, 23);
			this.btnTrain.TabIndex = 20;
			this.btnTrain.Text = "Trn";
			this.btnTrain.UseVisualStyleBackColor = true;
			this.btnTrain.Click += new System.EventHandler(this.btnTrain_Click);
			// 
			// btnNetConfig
			// 
			this.btnNetConfig.Location = new System.Drawing.Point(6, 433);
			this.btnNetConfig.Name = "btnNetConfig";
			this.btnNetConfig.Size = new System.Drawing.Size(172, 23);
			this.btnNetConfig.TabIndex = 19;
			this.btnNetConfig.Text = "Net Config";
			this.btnNetConfig.UseVisualStyleBackColor = true;
			this.btnNetConfig.Click += new System.EventHandler(this.btnNetConfig_Click);
			// 
			// txtFileNumColumnsY
			// 
			this.txtFileNumColumnsY.Enabled = false;
			this.txtFileNumColumnsY.Location = new System.Drawing.Point(113, 393);
			this.txtFileNumColumnsY.Name = "txtFileNumColumnsY";
			this.txtFileNumColumnsY.Size = new System.Drawing.Size(31, 20);
			this.txtFileNumColumnsY.TabIndex = 18;
			// 
			// txtFileNumColumnsX
			// 
			this.txtFileNumColumnsX.Enabled = false;
			this.txtFileNumColumnsX.Location = new System.Drawing.Point(41, 393);
			this.txtFileNumColumnsX.Name = "txtFileNumColumnsX";
			this.txtFileNumColumnsX.Size = new System.Drawing.Size(31, 20);
			this.txtFileNumColumnsX.TabIndex = 16;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(78, 396);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(37, 13);
			this.label9.TabIndex = 17;
			this.label9.Text = "ColsY:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(5, 396);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(37, 13);
			this.label8.TabIndex = 15;
			this.label8.Text = "ColsX:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(3, 351);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(26, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "File:";
			// 
			// btnOpenFile
			// 
			this.btnOpenFile.Location = new System.Drawing.Point(145, 365);
			this.btnOpenFile.Name = "btnOpenFile";
			this.btnOpenFile.Size = new System.Drawing.Size(33, 23);
			this.btnOpenFile.TabIndex = 13;
			this.btnOpenFile.Text = "...";
			this.btnOpenFile.UseVisualStyleBackColor = true;
			this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
			// 
			// txtFilename
			// 
			this.txtFilename.Enabled = false;
			this.txtFilename.Location = new System.Drawing.Point(6, 367);
			this.txtFilename.Name = "txtFilename";
			this.txtFilename.Size = new System.Drawing.Size(138, 20);
			this.txtFilename.TabIndex = 12;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(59, 62);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(31, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "Case";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(10, 62);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(38, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "Epoch";
			// 
			// txtNumEpochsToTrain
			// 
			this.txtNumEpochsToTrain.Location = new System.Drawing.Point(6, 78);
			this.txtNumEpochsToTrain.Name = "txtNumEpochsToTrain";
			this.txtNumEpochsToTrain.Size = new System.Drawing.Size(50, 20);
			this.txtNumEpochsToTrain.TabIndex = 8;
			this.txtNumEpochsToTrain.TextChanged += new System.EventHandler(this.txtNumEpochsToTrain_TextChanged);
			// 
			// btnTrainEpochs
			// 
			this.btnTrainEpochs.Location = new System.Drawing.Point(6, 104);
			this.btnTrainEpochs.Name = "btnTrainEpochs";
			this.btnTrainEpochs.Size = new System.Drawing.Size(50, 23);
			this.btnTrainEpochs.TabIndex = 7;
			this.btnTrainEpochs.Text = "TrnEp";
			this.btnTrainEpochs.UseVisualStyleBackColor = true;
			this.btnTrainEpochs.Click += new System.EventHandler(this.btnTrainEpochs_Click);
			// 
			// btnTrainCase
			// 
			this.btnTrainCase.Location = new System.Drawing.Point(62, 104);
			this.btnTrainCase.Name = "btnTrainCase";
			this.btnTrainCase.Size = new System.Drawing.Size(50, 23);
			this.btnTrainCase.TabIndex = 0;
			this.btnTrainCase.Text = "Trn Cas";
			this.btnTrainCase.UseVisualStyleBackColor = true;
			this.btnTrainCase.Click += new System.EventHandler(this.btnTrainCase_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(72, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Curr Case";
			// 
			// txtCurrCaseNum
			// 
			this.txtCurrCaseNum.Enabled = false;
			this.txtCurrCaseNum.Location = new System.Drawing.Point(72, 22);
			this.txtCurrCaseNum.Name = "txtCurrCaseNum";
			this.txtCurrCaseNum.Size = new System.Drawing.Size(60, 20);
			this.txtCurrCaseNum.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Curr Epoch";
			// 
			// txtCurrEpochNum
			// 
			this.txtCurrEpochNum.Enabled = false;
			this.txtCurrEpochNum.Location = new System.Drawing.Point(6, 22);
			this.txtCurrEpochNum.Name = "txtCurrEpochNum";
			this.txtCurrEpochNum.Size = new System.Drawing.Size(60, 20);
			this.txtCurrEpochNum.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.cbRefreshArea2);
			this.splitContainer2.Panel1.Controls.Add(this.tabsNetDisplay);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.cbRefreshArea3);
			this.splitContainer2.Panel2.Controls.Add(this.tabsStatistics);
			this.splitContainer2.Panel2.Controls.Add(this.splitter1);
			this.splitContainer2.Size = new System.Drawing.Size(611, 508);
			this.splitContainer2.SplitterDistance = 336;
			this.splitContainer2.TabIndex = 0;
			// 
			// cbRefreshArea2
			// 
			this.cbRefreshArea2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbRefreshArea2.AutoSize = true;
			this.cbRefreshArea2.Location = new System.Drawing.Point(525, 3);
			this.cbRefreshArea2.Name = "cbRefreshArea2";
			this.cbRefreshArea2.Size = new System.Drawing.Size(15, 14);
			this.cbRefreshArea2.TabIndex = 37;
			this.cbRefreshArea2.UseVisualStyleBackColor = true;
			this.cbRefreshArea2.CheckedChanged += new System.EventHandler(this.cbRefreshArea2_CheckedChanged);
			// 
			// tabsNetDisplay
			// 
			this.tabsNetDisplay.Controls.Add(this.tabInputPlane);
			this.tabsNetDisplay.Controls.Add(this.tabProximalSynapses);
			this.tabsNetDisplay.Controls.Add(this.tabInputOverlap);
			this.tabsNetDisplay.Controls.Add(this.tabBoost);
			this.tabsNetDisplay.Controls.Add(this.tabActivation);
			this.tabsNetDisplay.Controls.Add(this.tabPrediction);
			this.tabsNetDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabsNetDisplay.Location = new System.Drawing.Point(0, 0);
			this.tabsNetDisplay.Name = "tabsNetDisplay";
			this.tabsNetDisplay.SelectedIndex = 0;
			this.tabsNetDisplay.Size = new System.Drawing.Size(607, 332);
			this.tabsNetDisplay.TabIndex = 0;
			// 
			// tabInputPlane
			// 
			this.tabInputPlane.Location = new System.Drawing.Point(4, 22);
			this.tabInputPlane.Name = "tabInputPlane";
			this.tabInputPlane.Padding = new System.Windows.Forms.Padding(3);
			this.tabInputPlane.Size = new System.Drawing.Size(599, 306);
			this.tabInputPlane.TabIndex = 0;
			this.tabInputPlane.Text = "Inp";
			this.tabInputPlane.UseVisualStyleBackColor = true;
			this.tabInputPlane.Paint += new System.Windows.Forms.PaintEventHandler(this.tabInputPlane_Paint);
			// 
			// tabProximalSynapses
			// 
			this.tabProximalSynapses.Location = new System.Drawing.Point(4, 22);
			this.tabProximalSynapses.Name = "tabProximalSynapses";
			this.tabProximalSynapses.Size = new System.Drawing.Size(599, 306);
			this.tabProximalSynapses.TabIndex = 4;
			this.tabProximalSynapses.Text = "Prox";
			this.tabProximalSynapses.UseVisualStyleBackColor = true;
			this.tabProximalSynapses.Paint += new System.Windows.Forms.PaintEventHandler(this.tabProximalSynapses_Paint);
			// 
			// tabInputOverlap
			// 
			this.tabInputOverlap.Location = new System.Drawing.Point(4, 22);
			this.tabInputOverlap.Name = "tabInputOverlap";
			this.tabInputOverlap.Size = new System.Drawing.Size(599, 306);
			this.tabInputOverlap.TabIndex = 3;
			this.tabInputOverlap.Text = "InpOverlap";
			this.tabInputOverlap.UseVisualStyleBackColor = true;
			this.tabInputOverlap.Paint += new System.Windows.Forms.PaintEventHandler(this.tabInputOverlap_Paint);
			// 
			// tabBoost
			// 
			this.tabBoost.Location = new System.Drawing.Point(4, 22);
			this.tabBoost.Name = "tabBoost";
			this.tabBoost.Size = new System.Drawing.Size(599, 306);
			this.tabBoost.TabIndex = 5;
			this.tabBoost.Text = "Boost";
			this.tabBoost.UseVisualStyleBackColor = true;
			this.tabBoost.Paint += new System.Windows.Forms.PaintEventHandler(this.tabBoost_Paint);
			// 
			// tabActivation
			// 
			this.tabActivation.Location = new System.Drawing.Point(4, 22);
			this.tabActivation.Name = "tabActivation";
			this.tabActivation.Padding = new System.Windows.Forms.Padding(3);
			this.tabActivation.Size = new System.Drawing.Size(599, 306);
			this.tabActivation.TabIndex = 1;
			this.tabActivation.Text = "Act";
			this.tabActivation.UseVisualStyleBackColor = true;
			this.tabActivation.Paint += new System.Windows.Forms.PaintEventHandler(this.tabActivation_Paint);
			// 
			// tabPrediction
			// 
			this.tabPrediction.Location = new System.Drawing.Point(4, 22);
			this.tabPrediction.Name = "tabPrediction";
			this.tabPrediction.Padding = new System.Windows.Forms.Padding(3);
			this.tabPrediction.Size = new System.Drawing.Size(599, 306);
			this.tabPrediction.TabIndex = 2;
			this.tabPrediction.Text = "Pred";
			this.tabPrediction.UseVisualStyleBackColor = true;
			// 
			// cbRefreshArea3
			// 
			this.cbRefreshArea3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbRefreshArea3.AutoSize = true;
			this.cbRefreshArea3.Location = new System.Drawing.Point(527, 3);
			this.cbRefreshArea3.Name = "cbRefreshArea3";
			this.cbRefreshArea3.Size = new System.Drawing.Size(15, 14);
			this.cbRefreshArea3.TabIndex = 37;
			this.cbRefreshArea3.UseVisualStyleBackColor = true;
			this.cbRefreshArea3.CheckedChanged += new System.EventHandler(this.cbRefreshArea3_CheckedChanged);
			// 
			// tabsStatistics
			// 
			this.tabsStatistics.Controls.Add(this.tabSparsenessChart);
			this.tabsStatistics.Controls.Add(this.tabEntropyChart);
			this.tabsStatistics.Controls.Add(this.tabParams);
			this.tabsStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabsStatistics.Location = new System.Drawing.Point(3, 0);
			this.tabsStatistics.Name = "tabsStatistics";
			this.tabsStatistics.SelectedIndex = 0;
			this.tabsStatistics.Size = new System.Drawing.Size(604, 164);
			this.tabsStatistics.TabIndex = 1;
			// 
			// tabSparsenessChart
			// 
			this.tabSparsenessChart.Controls.Add(this.txtSparsenessStdDev);
			this.tabSparsenessChart.Controls.Add(this.txtSparsenessAve);
			this.tabSparsenessChart.Controls.Add(this.txtSparsenessVal);
			this.tabSparsenessChart.Controls.Add(this.label15);
			this.tabSparsenessChart.Controls.Add(this.label10);
			this.tabSparsenessChart.Controls.Add(this.chartSparseness);
			this.tabSparsenessChart.Controls.Add(this.label3);
			this.tabSparsenessChart.Location = new System.Drawing.Point(4, 22);
			this.tabSparsenessChart.Name = "tabSparsenessChart";
			this.tabSparsenessChart.Padding = new System.Windows.Forms.Padding(3);
			this.tabSparsenessChart.Size = new System.Drawing.Size(596, 138);
			this.tabSparsenessChart.TabIndex = 0;
			this.tabSparsenessChart.Text = "Spars";
			this.tabSparsenessChart.UseVisualStyleBackColor = true;
			// 
			// txtSparsenessStdDev
			// 
			this.txtSparsenessStdDev.Enabled = false;
			this.txtSparsenessStdDev.Location = new System.Drawing.Point(474, 97);
			this.txtSparsenessStdDev.Name = "txtSparsenessStdDev";
			this.txtSparsenessStdDev.Size = new System.Drawing.Size(50, 20);
			this.txtSparsenessStdDev.TabIndex = 8;
			// 
			// txtSparsenessAve
			// 
			this.txtSparsenessAve.Enabled = false;
			this.txtSparsenessAve.Location = new System.Drawing.Point(474, 60);
			this.txtSparsenessAve.Name = "txtSparsenessAve";
			this.txtSparsenessAve.Size = new System.Drawing.Size(50, 20);
			this.txtSparsenessAve.TabIndex = 6;
			// 
			// txtSparsenessVal
			// 
			this.txtSparsenessVal.Enabled = false;
			this.txtSparsenessVal.Location = new System.Drawing.Point(474, 23);
			this.txtSparsenessVal.Name = "txtSparsenessVal";
			this.txtSparsenessVal.Size = new System.Drawing.Size(50, 20);
			this.txtSparsenessVal.TabIndex = 4;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(474, 81);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(43, 13);
			this.label15.TabIndex = 7;
			this.label15.Text = "StdDev";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(476, 44);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(26, 13);
			this.label10.TabIndex = 5;
			this.label10.Text = "Ave";
			// 
			// chartSparseness
			// 
			this.chartSparseness.Dock = System.Windows.Forms.DockStyle.Left;
			this.chartSparseness.Location = new System.Drawing.Point(3, 3);
			this.chartSparseness.Name = "chartSparseness";
			this.chartSparseness.Size = new System.Drawing.Size(465, 132);
			this.chartSparseness.TabIndex = 0;
			this.chartSparseness.Text = "cartesianChart1";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(474, 7);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(34, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Spars";
			// 
			// tabEntropyChart
			// 
			this.tabEntropyChart.Controls.Add(this.txtEntropyStdDev);
			this.tabEntropyChart.Controls.Add(this.txtEntropyAve);
			this.tabEntropyChart.Controls.Add(this.txtEntropyVal);
			this.tabEntropyChart.Controls.Add(this.label16);
			this.tabEntropyChart.Controls.Add(this.chartEntropy);
			this.tabEntropyChart.Controls.Add(this.label17);
			this.tabEntropyChart.Controls.Add(this.label4);
			this.tabEntropyChart.Location = new System.Drawing.Point(4, 22);
			this.tabEntropyChart.Name = "tabEntropyChart";
			this.tabEntropyChart.Padding = new System.Windows.Forms.Padding(3);
			this.tabEntropyChart.Size = new System.Drawing.Size(596, 138);
			this.tabEntropyChart.TabIndex = 1;
			this.tabEntropyChart.Text = "Entr";
			this.tabEntropyChart.UseVisualStyleBackColor = true;
			// 
			// txtEntropyStdDev
			// 
			this.txtEntropyStdDev.Enabled = false;
			this.txtEntropyStdDev.Location = new System.Drawing.Point(474, 97);
			this.txtEntropyStdDev.Name = "txtEntropyStdDev";
			this.txtEntropyStdDev.Size = new System.Drawing.Size(50, 20);
			this.txtEntropyStdDev.TabIndex = 12;
			// 
			// txtEntropyAve
			// 
			this.txtEntropyAve.Enabled = false;
			this.txtEntropyAve.Location = new System.Drawing.Point(474, 60);
			this.txtEntropyAve.Name = "txtEntropyAve";
			this.txtEntropyAve.Size = new System.Drawing.Size(50, 20);
			this.txtEntropyAve.TabIndex = 10;
			// 
			// txtEntropyVal
			// 
			this.txtEntropyVal.Enabled = false;
			this.txtEntropyVal.Location = new System.Drawing.Point(474, 23);
			this.txtEntropyVal.Name = "txtEntropyVal";
			this.txtEntropyVal.Size = new System.Drawing.Size(50, 20);
			this.txtEntropyVal.TabIndex = 6;
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(474, 81);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(43, 13);
			this.label16.TabIndex = 11;
			this.label16.Text = "StdDev";
			// 
			// chartEntropy
			// 
			this.chartEntropy.Dock = System.Windows.Forms.DockStyle.Left;
			this.chartEntropy.Location = new System.Drawing.Point(3, 3);
			this.chartEntropy.Name = "chartEntropy";
			this.chartEntropy.Size = new System.Drawing.Size(465, 132);
			this.chartEntropy.TabIndex = 1;
			this.chartEntropy.Text = "cartesianChart1";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(476, 44);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(26, 13);
			this.label17.TabIndex = 9;
			this.label17.Text = "Ave";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(474, 7);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(43, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Entropy";
			// 
			// tabParams
			// 
			this.tabParams.Controls.Add(this.txtTabPrmsSparsenessOutput);
			this.tabParams.Controls.Add(this.label22);
			this.tabParams.Controls.Add(this.txtTabPrmsSparsenessSetpoint);
			this.tabParams.Controls.Add(this.label21);
			this.tabParams.Controls.Add(this.label20);
			this.tabParams.Controls.Add(this.txtTabPrmsSparsenessInput);
			this.tabParams.Controls.Add(this.label19);
			this.tabParams.Location = new System.Drawing.Point(4, 22);
			this.tabParams.Name = "tabParams";
			this.tabParams.Size = new System.Drawing.Size(596, 138);
			this.tabParams.TabIndex = 2;
			this.tabParams.Text = "Prms";
			this.tabParams.UseVisualStyleBackColor = true;
			// 
			// txtTabPrmsSparsenessOutput
			// 
			this.txtTabPrmsSparsenessOutput.Enabled = false;
			this.txtTabPrmsSparsenessOutput.Location = new System.Drawing.Point(55, 76);
			this.txtTabPrmsSparsenessOutput.Name = "txtTabPrmsSparsenessOutput";
			this.txtTabPrmsSparsenessOutput.Size = new System.Drawing.Size(50, 20);
			this.txtTabPrmsSparsenessOutput.TabIndex = 11;
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(3, 79);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(39, 13);
			this.label22.TabIndex = 10;
			this.label22.Text = "Output";
			// 
			// txtTabPrmsSparsenessSetpoint
			// 
			this.txtTabPrmsSparsenessSetpoint.Enabled = false;
			this.txtTabPrmsSparsenessSetpoint.Location = new System.Drawing.Point(55, 34);
			this.txtTabPrmsSparsenessSetpoint.Name = "txtTabPrmsSparsenessSetpoint";
			this.txtTabPrmsSparsenessSetpoint.Size = new System.Drawing.Size(50, 20);
			this.txtTabPrmsSparsenessSetpoint.TabIndex = 9;
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(3, 37);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(38, 13);
			this.label21.TabIndex = 8;
			this.label21.Text = "Traget";
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label20.Location = new System.Drawing.Point(3, 14);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(97, 13);
			this.label20.TabIndex = 7;
			this.label20.Text = "Prm Sparseness";
			// 
			// txtTabPrmsSparsenessInput
			// 
			this.txtTabPrmsSparsenessInput.Enabled = false;
			this.txtTabPrmsSparsenessInput.Location = new System.Drawing.Point(55, 55);
			this.txtTabPrmsSparsenessInput.Name = "txtTabPrmsSparsenessInput";
			this.txtTabPrmsSparsenessInput.Size = new System.Drawing.Size(50, 20);
			this.txtTabPrmsSparsenessInput.TabIndex = 6;
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(3, 58);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(31, 13);
			this.label19.TabIndex = 5;
			this.label19.Text = "Input";
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(0, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 164);
			this.splitter1.TabIndex = 0;
			this.splitter1.TabStop = false;
			// 
			// netForm1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(798, 532);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.Name = "netForm1";
			this.Text = "NetForm1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form1_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.form1_FormClosed);
			this.Load += new System.EventHandler(this.form1_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.tabsNetDisplay.ResumeLayout(false);
			this.tabsStatistics.ResumeLayout(false);
			this.tabSparsenessChart.ResumeLayout(false);
			this.tabSparsenessChart.PerformLayout();
			this.tabEntropyChart.ResumeLayout(false);
			this.tabEntropyChart.PerformLayout();
			this.tabParams.ResumeLayout(false);
			this.tabParams.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.TabControl tabsNetDisplay;
		private System.Windows.Forms.TabPage tabActivation;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtCurrEpochNum;
		private System.Windows.Forms.TabPage tabPrediction;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtCurrCaseNum;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtEntropyVal;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtSparsenessVal;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.Button btnTrainCase;
		private System.Windows.Forms.Button btnTrainEpochs;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtNumEpochsToTrain;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtFilename;
		private System.Windows.Forms.Button btnOpenFile;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtFileNumColumnsY;
		private System.Windows.Forms.TextBox txtFileNumColumnsX;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private LiveCharts.WinForms.CartesianChart chartSparseness;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TabControl tabsStatistics;
		private System.Windows.Forms.TabPage tabSparsenessChart;
		private System.Windows.Forms.TabPage tabEntropyChart;
		private LiveCharts.WinForms.CartesianChart chartEntropy;
		private System.Windows.Forms.TextBox txtSparsenessAve;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem networkConfigurationToolStripMenuItem;
		private System.Windows.Forms.Button btnNetConfig;
		private System.Windows.Forms.Button btnTrain;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnNextCase;
		private System.Windows.Forms.TabPage tabInputOverlap;
		private System.Windows.Forms.TabPage tabProximalSynapses;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox txtSparsenessActual;
		private System.Windows.Forms.TextBox txtSparsenessTarget;
		private System.Windows.Forms.TextBox txtSparsenessInterval;
		private System.Windows.Forms.CheckBox cbSparsenessParamSearchEnable;
		private System.Windows.Forms.TextBox txtSparsenessStdDev;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox txtEntropyStdDev;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox txtEntropyAve;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Button btnNextCaseTrain;
		private System.Windows.Forms.TabPage tabParams;
		private System.Windows.Forms.TextBox txtTabPrmsSparsenessOutput;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.TextBox txtTabPrmsSparsenessSetpoint;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox txtTabPrmsSparsenessInput;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.TabPage tabBoost;
		private System.Windows.Forms.CheckBox cbRefreshArea3;
		private System.Windows.Forms.CheckBox cbRefreshArea1;
		private System.Windows.Forms.CheckBox cbRefreshArea2;
		private System.Windows.Forms.TabPage tabInputPlane;
		private System.Windows.Forms.Button btnViewer3D;
	}
}

