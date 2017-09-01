using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
using System.Windows.Media;


namespace Net1
{
	public partial class netForm1 : Form
	{
		public Network Net { get; private set; }
		//private ScreenUpdateData screenUpdateData { get; set; }

		//screen refresh region checkbox values
		private bool refreshArea1 { get; set; }
		private bool refreshArea2 { get; set; }
		private bool refreshArea3 { get; set; }

		//last file opened in previous session
		private string lastFileOpen;


		public netForm1()
		{
			InitializeComponent();
		}

		private void form1_Load(object sender, EventArgs e)
		{
			//structure for screen updates
			ScreenUpdateData.DataChanged += new ScreenUpdateChangedEventHandler(ScreenDataChanged);
			NetConfigData.DataChanged += new NetConfigData.NetConfigDataChangedEventHandler(NetConfigDataChanged); // needs item

			loadSettings();

			//string filename = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\..\..\TestData.csv";

			//attempt to open last file
			if ( lastFileOpen.Length > 2 )
			{
				Net = CreateNetworkFromFile(lastFileOpen);
			}
			else
			{
				//or show open file dialog
				openToolStripMenuItem_Click(null, EventArgs.Empty);
			}

			//network loaded - configure GUI
			if ( Net != null )
			{
				configureControls();
				ScreenDataChanged();
				enableControls();

				//show network configuration window
				showNetConfigDlg();

				//show 3D display window
				Viewer3D.Start();
			}
		}

		private void form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			shutdown();

		}

		private void form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Application.Exit();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			shutdown();
			Application.Exit();
		}

		private void shutdown()
		{
			//abort training task
			while (Net.TrainingInProgress)
				Thread.Sleep(10);

			//unsubscribe from data update events
			ScreenUpdateData.DataChanged -= ScreenDataChanged;
			NetConfigData.DataChanged -= NetConfigDataChanged;
			saveSettings();
		}
	

		private void btnNextCase_Click(object sender, EventArgs e)
		{
			Net.LoadNextCase();
		}

		private void btnTrainCase_Click(object sender, EventArgs e)
		{
			Net.TrainingInProgress = true;
			Net.TrainCase();
			Net.TrainingInProgress = false;
		}

		private void btnNextCaseTrain_Click(object sender, EventArgs e)
		{
			btnNextCase.PerformClick();
			btnTrainCase.PerformClick();
		}

		private void btnTrainEpochs_Click(object sender, EventArgs e)
		{
			for (int epoch = 0; epoch < NetConfigData.NumEpochsToTrain; epoch++)
			{
				Net.TrainEpoch();
				Application.DoEvents();
			}

			enableControls();
		}

		private void btnTrain_Click(object sender, EventArgs e)
		{
			Net.TrainRequest = true;
			Net.TrainingInProgress = true;
			btnStop.Focus();

			while (Net.TrainRequest)
			{
				Net.TrainEpoch();
				Application.DoEvents();
			};

			Net.TrainingInProgress = false;
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			Net.TrainRequest = false;
		}

		private void btnOpenFile_Click(object sender, EventArgs e)
		{
			string fname = "";
			OpenFileDialog dlg = new OpenFileDialog()
			{
				InitialDirectory = @"C:\Projects",
				Multiselect = false,
				Filter = "CSV files (*.csv)|*.csv| All files (*.*)|*.*"
			};
			if (dlg.ShowDialog() == DialogResult.OK)
				fname = dlg.FileName;

			CreateNetworkFromFile(fname);

			ScreenDataChanged();
			enableControls();
		}

		public Network CreateNetworkFromFile(string filename)
		{
			Network net = null;

			if (File.Exists(filename))
			{
				net = new Network(filename);
			}
			else
				MessageBox.Show($"Network file not found:\n\n{ filename }");

			return net;
		}

		//respond to ScreenData change events
		public void ScreenDataChanged()
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(ScreenDataChanged));
			}
			else
			{
				refreshScreenData();
				refreshNetDisplayTabs();
				refreshStatisticsTabs();
			}
		}
		
		//respond to NetConfigData change events
		public void NetConfigDataChanged()
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(NetConfigDataChanged));
			}
			else
			{
				refreshNetDisplayTabs();
			}
		}

		private void refreshScreenData()
		{
			if (refreshArea1)
			{
				txtNumEpochsToTrain.Text = NetConfigData.NumEpochsToTrain.ToString();
				txtFilename.Text = Path.GetFileName(ScreenUpdateData.Filename);
				txtFileNumColumnsX.Text = ScreenUpdateData.FileNumColumnsX.ToString();
				txtFileNumColumnsY.Text = ScreenUpdateData.FileNumColumnsY.ToString();

				if (Net != null && Net.Lr != null)
				{
					txtCurrCaseNum.Text = Net.Trainer.CurrCaseNum.ToString();
					txtCurrEpochNum.Text = Net.Trainer.CurrEpochNum.ToString();

					txtSparsenessActual.Text = Net.Lr.SparsenessActual.ToString("N4");
					txtSparsenessTarget.Text = Net.Lr.SparsenessTarget.ToString("N4");
					txtSparsenessInterval.Text = Net.Lr.SparsenessInterval.ToString();
				}
			}
		}
				
		private void refreshNetDisplayTabs()
		{
			if (refreshArea2)
			{
				if (tabsNetDisplay.SelectedTab.Name == "tabInputPlane")
					tabInputPlane.Refresh();
				else if (tabsNetDisplay.SelectedTab.Name == "tabProximalSynapses")
					tabProximalSynapses.Refresh();
				else if (tabsNetDisplay.SelectedTab.Name == "tabInputOverlap")
					tabInputOverlap.Refresh();
				else if (tabsNetDisplay.SelectedTab.Name == "tabBoost")
					tabBoost.Refresh();
				else if (tabsNetDisplay.SelectedTab.Name == "tabActivation")
					tabActivation.Refresh();
			}
		}

		private void refreshStatisticsTabs()
		{
			if (refreshArea3)
			{
				if (Net != null && Net.Lr != null)
				{
					//Sparseness tab - refresh
					if (tabsStatistics.SelectedTab.Name == "tabSparsenessChart")
					{
						txtSparsenessVal.Text = Net.Lr.SparsenessActual.ToString("N4");
						txtSparsenessAve.Text = Net.Lr.SparsenessStat.Mean().ToString("N4");
						txtSparsenessStdDev.Text = Net.Lr.SparsenessStat.StdDev().ToString("N4");

						//update Sparseness chart
						ScreenUpdateData.ChartSparsenessLineSeries.Values.Add(Net.Lr.SparsenessStat.Value);
						if (ScreenUpdateData.ChartSparsenessLineSeries.Values.Count > Global.CHART_NUM_POINTS)
							ScreenUpdateData.ChartSparsenessLineSeries.Values.RemoveAt(0);
						chartSparseness.Refresh();
					}
					//Entropy tab - refresh
					else if (tabsStatistics.SelectedTab.Name == "tabEntropyChart")
					{
						txtEntropyVal.Text = Net.Lr.EntropyStat.Value.ToString("N4");
						txtEntropyAve.Text = Net.Lr.EntropyStat.Mean().ToString("N4");
						txtEntropyStdDev.Text = Net.Lr.EntropyStat.StdDev().ToString("N4");

						//update Entropy chart
						ScreenUpdateData.ChartEntropyLineSeries.Values.Add(Net.Lr.EntropyStat.Value);
						if (ScreenUpdateData.ChartEntropyLineSeries.Values.Count > Global.CHART_NUM_POINTS)
							ScreenUpdateData.ChartEntropyLineSeries.Values.RemoveAt(0);
						chartEntropy.Refresh();
					}
					//Param tab - refresh
					else if (tabsStatistics.SelectedTab.Name == "tabPrms")
					{
						txtTabPrmsSparsenessSetpoint.Text = Net.Lr.ParamSearch_Sparseness_ZoneCoveragePercProximal.PV_Target.ToString("N4");
						txtTabPrmsSparsenessInput.Text = Net.Lr.ParamSearch_Sparseness_ZoneCoveragePercProximal.PV.ToString("N4");
						txtTabPrmsSparsenessOutput.Text = Net.Lr.ParamSearch_Sparseness_ZoneCoveragePercProximal.CV.ToString("N4");
					}
				}
			}
		}


		private void enableControls()
		{
			if(Net != null && Net.Trainer != null && Net.Trainer.DataReady)
			{
				txtNumEpochsToTrain.Enabled = true;
				btnTrainEpochs.Enabled = true;
				btnTrainCase.Enabled = true;
				btnNextCase.Enabled = true;
				btnNetConfig.Enabled = true;
			}
			else
			{
				txtNumEpochsToTrain.Enabled = false;
				btnTrainEpochs.Enabled = false;
				btnTrainCase.Enabled = false;
				btnNextCase.Enabled = false;
				btnNetConfig.Enabled = false;
			}
		}

		private void configureControls()
		{
			cbRefreshArea1.Checked = refreshArea1;
			cbRefreshArea2.Checked = refreshArea2;
			cbRefreshArea3.Checked = refreshArea3;

			//configure tabStatistics Sparseness chart
			//ScreenUpdateData.chartSparsenessLineSeries = new LineSeries();    //Sparsness chart data series
			ScreenUpdateData.ChartSparsenessLineSeries.Values = new ChartValues<double> { 0 };
			ScreenUpdateData.ChartSparsenessLineSeries.LineSmoothness = 0;
			ScreenUpdateData.ChartSparsenessLineSeries.PointGeometry = DefaultGeometries.Circle;
			ScreenUpdateData.ChartSparsenessLineSeries.PointGeometrySize = 3;
			ScreenUpdateData.ChartSparsenessLineSeries.PointForeground = System.Windows.Media.Brushes.DarkGray;
			ScreenUpdateData.ChartSparsenessLineSeries.StrokeThickness = 1;
			chartSparseness.Series.Add(ScreenUpdateData.ChartSparsenessLineSeries);

			chartSparseness.AxisX.Add(new Axis
			{
				Title = "",
				//Labels = new[] { }
			});
			chartSparseness.AxisY.Add(new Axis
			{
				Title = "Sparseness",
				//Labels = new[] { }
				LabelFormatter = value => value.ToString("N2"),
				MaxValue = 0.05,
				MinValue = 0.0
				
			});
			chartSparseness.LegendLocation = LegendLocation.None;

			//configure tabStatistics Entropy chart
			//ScreenUpdateData.chartEntropyLineSeries = new LineSeries();       //Entropy chart data series
			ScreenUpdateData.ChartEntropyLineSeries.Values = new ChartValues<double> { 0 };
			ScreenUpdateData.ChartEntropyLineSeries.LineSmoothness = 0;
			ScreenUpdateData.ChartEntropyLineSeries.PointGeometry = DefaultGeometries.Triangle;
			ScreenUpdateData.ChartEntropyLineSeries.PointGeometrySize = 3;
			ScreenUpdateData.ChartEntropyLineSeries.PointForeground = System.Windows.Media.Brushes.Red;
			ScreenUpdateData.ChartEntropyLineSeries.StrokeThickness = 1;
			chartEntropy.Series.Add(ScreenUpdateData.ChartEntropyLineSeries);

			chartEntropy.AxisX.Add(new Axis
			{
				Title = "",
				//Labels = new[] { }
			});
			chartEntropy.AxisY.Add(new Axis
			{
				Title = "Entropy",
				//Labels = new[] { }
				LabelFormatter = value => value.ToString("N2"),
				//MaxValue = 1.0,
				MinValue = 0.0
			});
			chartEntropy.LegendLocation = LegendLocation.None;

			//parameter search enable checkboxes
			cbSparsenessParamSearchEnable.Checked = ScreenUpdateData.SparsenessParamSearchEnable;

		}

		private void tabInputPlane_Paint(object sender, PaintEventArgs e)
		{
			//pattern offset in XY
			int xOffs = Global.DISPLAY_X_OFFSET;
			int yOffs = Global.DISPLAY_Y_OFFSET;

			//row/col separation (pitch)
			int xPitch = Global.DISPLAY_X_PITCH;
			int yPitch = Global.DISPLAY_Y_PITCH;

			//current location in XY
			int xLoc = xOffs;
			int yLoc = yOffs;

			if (Net != null && Net.Ip != null)
			{
				int numColumnsX = Net.Ip.NumColumnsX;
				int numColumnsY = Net.Ip.NumColumnsY;

				//using (Font font = new Font("Arial", 8))
				using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Black))
				{
					for (int y = 0; y < numColumnsY; y++)
					{
						for (int x = 0; x < numColumnsX; x++)
						{
							if(Net.Ip.Columns[y][x].IsActive)
								e.Graphics.DrawString("*", this.Font, brush, xLoc, yLoc);
							else
								e.Graphics.DrawString(".", this.Font, brush, xLoc, yLoc);

							xLoc += xPitch;
						}
						yLoc += yPitch;
						xLoc = xOffs;
					}
				}
			}
		}

		private void tabActivation_Paint(object sender, PaintEventArgs e)
		{
			//pattern offset in XY
			int xOffs = Global.DISPLAY_X_OFFSET;
			int yOffs = Global.DISPLAY_Y_OFFSET;

			//row/col separation (pitch)
			int xPitch = Global.DISPLAY_X_PITCH;
			int yPitch = Global.DISPLAY_Y_PITCH;

			//current location in XY
			int xLoc = xOffs;
			int yLoc = yOffs;

			if (Net != null && Net.Lr != null)
			{
				int numColumnsX = Net.Lr.NumColumnsX;
				int numColumnsY = Net.Lr.NumColumnsY;

				//using (Font font = new Font("Arial", 8))
				using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Black))
				{
					for (int y = 0; y < numColumnsY; y++)
					{
						for (int x = 0; x < numColumnsX; x++)
						{
							if (Net.Lr.Columns[y][x].IsActive)
								e.Graphics.DrawString("A", this.Font, brush, xLoc, yLoc);
							else
								e.Graphics.DrawString(".", this.Font, brush, xLoc, yLoc);

								xLoc += xPitch;
						}
						yLoc += yPitch;
						xLoc = xOffs;
					}
				}
			}
		}

		private void tabInputOverlap_Paint(object sender, PaintEventArgs e)
		{
			//pattern offset in XY
			int xOffs = Global.DISPLAY_X_OFFSET;
			int yOffs = Global.DISPLAY_Y_OFFSET;

			//row/col separation (pitch)
			int xPitch = Global.DISPLAY_X_PITCH;
			int yPitch = Global.DISPLAY_Y_PITCH;

			//current location in XY
			int xLoc = xOffs;
			int yLoc = yOffs;

			if (Net != null && Net.Lr != null)
			{
				int numColumnsX = Net.Lr.NumColumnsX;
				int numColumnsY = Net.Lr.NumColumnsY;

				using (Font font = new Font("Arial", 6))
				{
					using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Black))
					{
						for (int y = 0; y < numColumnsY; y++)
						{
							for (int x = 0; x < numColumnsX; x++)
							{
								e.Graphics.DrawString($"{Net.Lr.Columns[y][x].InputOverlap:N0}", font, brush, xLoc, yLoc);
								xLoc += xPitch;
							}
							yLoc += yPitch;
							xLoc = xOffs;
						}
					}
				}
			}
		}

		private void tabProximalSynapses_Paint(object sender, PaintEventArgs e)
		{
			//pattern offset in XY
			int xOffs = Global.DISPLAY_X_OFFSET;
			int yOffs = Global.DISPLAY_Y_OFFSET;

			//row/col separation (pitch)
			int xPitch = Global.DISPLAY_X_PITCH;
			int yPitch = Global.DISPLAY_Y_PITCH;

			//current location in XY
			int xLoc = xOffs;
			int yLoc = yOffs;

			if (Net != null && Net.Lr != null)
			{
				int numColumnsX = Net.Lr.NumColumnsX;
				int numColumnsY = Net.Lr.NumColumnsY;

				using (Font font = new Font("Arial", 6))
				{
					using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Black))
					{
						for (int y = 0; y < numColumnsY; y++)
						{
							for (int x = 0; x < numColumnsX; x++)
							{
								e.Graphics.DrawString($"{Net.Lr.Columns[y][x].ProximalDendrite.Synapses.Count }", font, brush, xLoc, yLoc);
								xLoc += xPitch;
							}
							yLoc += yPitch;
							xLoc = xOffs;
						}
					}
				}
			}
		}

		private void tabBoost_Paint(object sender, PaintEventArgs e)
		{
			//pattern offset in XY
			int xOffs = Global.DISPLAY_X_OFFSET;
			int yOffs = Global.DISPLAY_Y_OFFSET;

			//row/col separation (pitch)
			int xPitch = Global.DISPLAY_X_PITCH;
			int yPitch = Global.DISPLAY_Y_PITCH;

			//current location in XY
			int xLoc = xOffs;
			int yLoc = yOffs;

			if (Net != null && Net.Lr != null)
			{
				int numColumnsX = Net.Lr.NumColumnsX;
				int numColumnsY = Net.Lr.NumColumnsY;

				using (Font font = new Font("Arial", 6))
				{
					using (SolidBrush brush = new SolidBrush(System.Drawing.Color.Black))
					{
						for (int y = 0; y < numColumnsY; y++)
						{
							for (int x = 0; x < numColumnsX; x++)
							{
								e.Graphics.DrawString($"{Net.Lr.Columns[y][x].Boost:N4}", font, brush, xLoc, yLoc);
								xLoc += 3 * xPitch;
							}
							yLoc += yPitch;
							xLoc = xOffs;
						}
					}
				}
			}
		}
		private void loadSettings()
		{
			//load network settings
			NetConfigData.SynapsePermanenceThreshold = Properties.Settings.Default.NetConfig_SynapsePermanenceThreshold;
			NetConfigData.SynapsePermanenceIncrease = Properties.Settings.Default.NetConfig_SynapsePermanenceIncrease;
			NetConfigData.SynapsePermanenceDecrease = Properties.Settings.Default.NetConfig_SynapsePermanenceDecrease;
			NetConfigData.DendriteActivationThresholdProximal = Properties.Settings.Default.NetConfig_DendriteActivationThresholdProximal;
			NetConfigData.DendriteActivationThresholdBasal = Properties.Settings.Default.NetConfig_DendriteActivationThresholdBasal;
			NetConfigData.DendriteActivationThresholdApical = Properties.Settings.Default.NetConfig_DendriteActivationThresholdApical;
			NetConfigData.ColumnStimulusThreshold = Properties.Settings.Default.NetConfig_ColumnStimulusThreshold;
			NetConfigData.ZoneSizePercProximal = Properties.Settings.Default.NetConfig_ZoneSizePercProximal;
			NetConfigData.ZoneCoveragePercProximal = Properties.Settings.Default.NetConfig_ZoneCoveragePercProximal;
			NetConfigData.ZoneSizePercBasal = Properties.Settings.Default.NetConfig_ZoneSizePercBasal;
			NetConfigData.ZoneCoveragePercBasal = Properties.Settings.Default.NetConfig_ZoneCoveragePercBasal;
			NetConfigData.ZoneSizePercApical = Properties.Settings.Default.NetConfig_ZoneSizePercApical;
			NetConfigData.ZoneCoveragePercApical = Properties.Settings.Default.NetConfig_ZoneCoveragePercApical;
			NetConfigData.NumCellsInColumn = Properties.Settings.Default.NetConfig_NumCellsInColumn;
			NetConfigData.ColumnsTopPercentile = Properties.Settings.Default.NetConfig_ColumnsTopPercentile;
			NetConfigData.NumEpochsToTrain = Properties.Settings.Default.NetConfig_NumEpochsToTrain;
			NetConfigData.ActivationDensityInterval = Properties.Settings.Default.NetConfig_ActivationDensityInterval;
			NetConfigData.ActivationDensityTarget = Properties.Settings.Default.NetConfig_ActivationDensityTarget;

			refreshArea1 = Properties.Settings.Default.RefreshArea1;
			refreshArea2 = Properties.Settings.Default.RefreshArea2;
			refreshArea3 = Properties.Settings.Default.RefreshArea3;

			ScreenUpdateData.SparsenessParamSearchEnable = Properties.Settings.Default.SparsenessParamSearchEnable;

			lastFileOpen = Properties.Settings.Default.LastFileOpen;
		}

		private void saveSettings()
		{
			//save settings
			Properties.Settings.Default.NetConfig_SynapsePermanenceThreshold = NetConfigData.SynapsePermanenceThreshold;
			Properties.Settings.Default.NetConfig_SynapsePermanenceIncrease = NetConfigData.SynapsePermanenceIncrease;
			Properties.Settings.Default.NetConfig_SynapsePermanenceDecrease = NetConfigData.SynapsePermanenceDecrease;
			Properties.Settings.Default.NetConfig_DendriteActivationThresholdProximal = NetConfigData.DendriteActivationThresholdProximal;
			Properties.Settings.Default.NetConfig_DendriteActivationThresholdBasal = NetConfigData.DendriteActivationThresholdBasal;
			Properties.Settings.Default.NetConfig_DendriteActivationThresholdApical = NetConfigData.DendriteActivationThresholdApical;
			Properties.Settings.Default.NetConfig_ColumnStimulusThreshold = NetConfigData.ColumnStimulusThreshold;
			Properties.Settings.Default.NetConfig_ZoneSizePercProximal = NetConfigData.ZoneSizePercProximal;
			Properties.Settings.Default.NetConfig_ZoneCoveragePercProximal = NetConfigData.ZoneCoveragePercProximal;
			Properties.Settings.Default.NetConfig_ZoneSizePercBasal = NetConfigData.ZoneSizePercBasal;
			Properties.Settings.Default.NetConfig_ZoneCoveragePercBasal = NetConfigData.ZoneCoveragePercBasal;
			Properties.Settings.Default.NetConfig_ZoneSizePercApical = NetConfigData.ZoneSizePercApical;
			Properties.Settings.Default.NetConfig_ZoneCoveragePercApical = NetConfigData.ZoneCoveragePercApical;
			Properties.Settings.Default.NetConfig_NumCellsInColumn = NetConfigData.NumCellsInColumn;
			Properties.Settings.Default.NetConfig_ColumnsTopPercentile = NetConfigData.ColumnsTopPercentile;
			Properties.Settings.Default.NetConfig_NumEpochsToTrain = NetConfigData.NumEpochsToTrain;
			Properties.Settings.Default.NetConfig_ActivationDensityInterval = NetConfigData.ActivationDensityInterval;
			Properties.Settings.Default.NetConfig_ActivationDensityTarget = NetConfigData.ActivationDensityTarget;
	
			Properties.Settings.Default.RefreshArea1 = refreshArea1; 
			Properties.Settings.Default.RefreshArea2 = refreshArea2;
			Properties.Settings.Default.RefreshArea3 = refreshArea3;

			Properties.Settings.Default.SparsenessParamSearchEnable = ScreenUpdateData.SparsenessParamSearchEnable;

			Properties.Settings.Default.LastFileOpen = lastFileOpen;

			Properties.Settings.Default.Save();
		}

		private void btnNetConfig_Click(object sender, EventArgs e)
		{
			showNetConfigDlg();
		}

		private void showNetConfigDlg()
		{
			NetConfigForm form = new NetConfigForm(Net.Lr.NumColumns);  //need to change when multiple layers implemented
			form.Show();
		}

		private void txtSparsenessTarget_TextChanged(object sender, EventArgs e)
		{
			double val;
			if(double.TryParse(txtSparsenessTarget.Text, out val))
			{
				Net.Lr.SparsenessTarget = val;
			}
		}

		private void txtSparsenessInterval_TextChanged(object sender, EventArgs e)
		{
			int val;
			if (Int32.TryParse(txtSparsenessInterval.Text, out val))
			{
				Net.Lr.SparsenessInterval = val;
			}
		}

		private void txtNumEpochsToTrain_TextChanged(object sender, EventArgs e)
		{
			int val;
			if (Int32.TryParse(txtNumEpochsToTrain.Text, out val))
			{
				NetConfigData.NumEpochsToTrain = val;
			}

		}

		private void txtNumCasesToTrain_TextChanged(object sender, EventArgs e)
		{

		}

		private void cbSparsenessParamSearchEnable_CheckedChanged(object sender, EventArgs e)
		{
			ScreenUpdateData.SparsenessParamSearchEnable = cbSparsenessParamSearchEnable.Checked;
			Net.Lr.ParamSearch_Sparseness_ZoneCoveragePercProximal.Enabled = ScreenUpdateData.SparsenessParamSearchEnable;
		}

		private void cbRefreshArea1_CheckedChanged(object sender, EventArgs e)
		{
			refreshArea1 = cbRefreshArea1.Checked;
		}

		private void cbRefreshArea2_CheckedChanged(object sender, EventArgs e)
		{
			refreshArea2 = cbRefreshArea2.Checked;
		}

		private void cbRefreshArea3_CheckedChanged(object sender, EventArgs e)
		{
			refreshArea3 = cbRefreshArea3.Checked;
		}

		private void btnViewer3D_Click(object sender, EventArgs e)
		{
			Viewer3D.Start();

			//wire Engine events
			//used in WatchForm window TODO
			//Viewer3D.EngineStarted += WatchForm.Instance.Handler_SimEngineStarted;
			//Viewer3D.EngineShutdown += WatchForm.Instance.Handler_SimEngineShutdown;
		}

		private void CleanUp ()
		{
			//this.buttonInitHTM.Enabled = true;
			//this.EnableSteeringButtons ( false );
			//this.menuView3DSimulation.Enabled = false;
			//this.menuProjectProperties.Enabled = true;

			if ( Viewer3D.IsActive )
			{
				Viewer3D.End ();
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
			dlg.CheckFileExists = true;
			dlg.CheckPathExists = true;
			dlg.DefaultExt = "csv";
			dlg.InitialDirectory = Environment.CurrentDirectory;

			string filename = "";
			if ( dlg.ShowDialog() == DialogResult.OK )
			{
				filename = dlg.FileName;
				if ( File.Exists(filename) )
				{
					Net = new Network(filename);
					lastFileOpen = filename;

					//save last open file in Settings
					Properties.Settings.Default.LastFileOpen = lastFileOpen;
					Properties.Settings.Default.Save();
				}
			}


		}
	}
}
