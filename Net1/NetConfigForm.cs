using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Net1
{

	public partial class NetConfigForm : Form
	{
		private int NumColumns;
		public NetConfigForm(int numColumns)
		{
			InitializeComponent();
			NumColumns = numColumns;
		}


		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		public static extern int FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll", EntryPoint = "MoveWindow")]
		internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		//event trigger for data Save button
		//notifies main form to save Settings
		public delegate void NetConfigDataSaveEventHandler();
		public static event NetConfigDataSaveEventHandler DataSave;
		public static void OnDataSave()
		{
			DataSave?.Invoke();
		}

		private void NetConfigForm_Load(object sender, EventArgs e)
		{
			int posX = Application.OpenForms[0].Location.X;
			int posY = Application.OpenForms[0].Location.Y;
			int width = Application.OpenForms[0].Width;
			int height = Application.OpenForms[0].Height;

			MoveWindow(this.Handle, posX + width, posY, this.Width, this.Height, true);
			

			//NetConfigData.DataChanged += new NetConfigData.NetConfigDataChangedEventHandler(UpdateControls);
			UpdateControls();
		}

		private void UpdateControls()
		{
			if (InvokeRequired)
			{
				Invoke(new MethodInvoker(UpdateControls));
			}
			else
			{
				tbrSynapsePermanenceThreshold.Value = (int)(NetConfigData.SynapsePermanenceThreshold * 1000);
				txtSynapsePermanenceThreshold.Text = NetConfigData.SynapsePermanenceThreshold.ToString("N4");
				tbrSynapsePermanenceIncrease.Value = (int)(NetConfigData.SynapsePermanenceIncrease * 10000);
				txtSynapsePermanenceIncrease.Text = NetConfigData.SynapsePermanenceIncrease.ToString("N4");
				tbrSynapsePermanenceDecrease.Value = (int)(NetConfigData.SynapsePermanenceDecrease * 100000);
				txtSynapsePermanenceDecrease.Text = NetConfigData.SynapsePermanenceDecrease.ToString("N4");
				tbrDendriteActivationThresholdProximal.Maximum = NumColumns;
				tbrDendriteActivationThresholdProximal.Value = (int)(NetConfigData.DendriteActivationThresholdProximal);
				txtDendriteActivationThresholdProximal.Text = NetConfigData.DendriteActivationThresholdProximal.ToString();
				tbrDendriteActivationThresholdBasal.Maximum = NumColumns;
				tbrDendriteActivationThresholdBasal.Value = (int)(NetConfigData.DendriteActivationThresholdBasal);
				txtDendriteActivationThresholdBasal.Text = NetConfigData.DendriteActivationThresholdBasal.ToString();
				tbrDendriteActivationThresholdApical.Maximum = NumColumns;
				tbrDendriteActivationThresholdApical.Value = (int)(NetConfigData.DendriteActivationThresholdApical);
				txtDendriteActivationThresholdApical.Text = NetConfigData.DendriteActivationThresholdApical.ToString();
				tbrColumnStimulusThreshold.Maximum = NumColumns;
				tbrColumnStimulusThreshold.Value = (int)(NetConfigData.ColumnStimulusThreshold);
				txtColumnStimulusThreshold.Text = NetConfigData.ColumnStimulusThreshold.ToString();
				tbrZoneSizePercProximal.Value = (int)(NetConfigData.ZoneSizePercProximal * 1000);
				txtZoneSizePercProximal.Text = NetConfigData.ZoneSizePercProximal.ToString("N4");
				tbrZoneCoveragePercProximal.Value = (int)(NetConfigData.ZoneCoveragePercProximal * 1000);
				txtZoneCoveragePercProximal.Text = NetConfigData.ZoneCoveragePercProximal.ToString("N4");
				tbrZoneSizePercBasal.Value = (int)(NetConfigData.ZoneSizePercBasal * 1000);
				txtZoneSizePercBasal.Text = NetConfigData.ZoneSizePercBasal.ToString("N4");
				tbrZoneCoveragePercBasal.Value = (int)(NetConfigData.ZoneCoveragePercBasal * 1000);
				txtZoneCoveragePercBasal.Text = NetConfigData.ZoneCoveragePercBasal.ToString("N4");
				tbrNumCellsInColumn.Value = (int)(NetConfigData.NumCellsInColumn * 100);
				txtNumCellsInColumn.Text = NetConfigData.NumCellsInColumn.ToString();
				tbrColumnsTopPercentile.Value = (int)(NetConfigData.ColumnsTopPercentile * 1000);
				txtColumnsTopPercentile.Text = NetConfigData.ColumnsTopPercentile.ToString("N4");
			}
		}

		private void tbrSynapsePermanenceThreshold_Scroll(object sender, EventArgs e)
		{
			int scrollValue = tbrSynapsePermanenceThreshold.Value;
			double value = Convert.ToDouble(scrollValue / 1000.0);
			NetConfigData.SynapsePermanenceThreshold = value;
			txtSynapsePermanenceThreshold.Text = NetConfigData.SynapsePermanenceThreshold.ToString("N4");
		}
		private void txtSynapsePermanenceThreshold_TextChanged(object sender, EventArgs e)
		{
			double val;
			if (double.TryParse(txtSynapsePermanenceThreshold.Text, out val))
			{
				NetConfigData.SynapsePermanenceThreshold = val;
				tbrSynapsePermanenceThreshold.Value = (int)(NetConfigData.SynapsePermanenceThreshold * 1000);
			}
		}

		private void tbrSynapsePermanenceIncrease_Scroll(object sender, EventArgs e)	//range 0 - 0.1
		{
			int scrollValue = tbrSynapsePermanenceIncrease.Value;
			double value = Convert.ToDouble(scrollValue / 10000.0);
			NetConfigData.SynapsePermanenceIncrease = value;
			txtSynapsePermanenceIncrease.Text = NetConfigData.SynapsePermanenceIncrease.ToString("N4");
		}
		private void txtSynapsePermanenceIncrease_TextChanged(object sender, EventArgs e)
		{
			double val;
			if (double.TryParse(txtSynapsePermanenceIncrease.Text, out val))
			{
				NetConfigData.SynapsePermanenceIncrease = val;
				tbrSynapsePermanenceIncrease.Value = (int)(NetConfigData.SynapsePermanenceIncrease * 10000);
			}
		}

		private void tbrSynapsePermanenceDecrease_Scroll(object sender, EventArgs e)
		{
			int scrollValue = tbrSynapsePermanenceDecrease.Value;
			double value = Convert.ToDouble(scrollValue / 100000.0);
			NetConfigData.SynapsePermanenceDecrease = value;
			txtSynapsePermanenceDecrease.Text = NetConfigData.SynapsePermanenceDecrease.ToString("N4");
		}
		private void txtSynapsePermanenceDecrease_TextChanged(object sender, EventArgs e)
		{
			double val;
			if (double.TryParse(txtSynapsePermanenceDecrease.Text, out val))
			{
				NetConfigData.SynapsePermanenceDecrease = val;
				tbrSynapsePermanenceDecrease.Value = (int)(NetConfigData.SynapsePermanenceDecrease * 100000);
			}
		}

		private void tbrDendriteActivationThresholdProximal_Scroll(object sender, EventArgs e)
		{
			int value = tbrDendriteActivationThresholdProximal.Value;
			NetConfigData.DendriteActivationThresholdProximal = value;
			txtDendriteActivationThresholdProximal.Text = NetConfigData.DendriteActivationThresholdProximal.ToString();
		}
		private void txtDendriteActivationThresholdProximal_TextChanged(object sender, EventArgs e)
		{
			int val;
			if (Int32.TryParse(txtDendriteActivationThresholdProximal.Text, out val))
			{
				NetConfigData.DendriteActivationThresholdProximal = val;
				tbrDendriteActivationThresholdProximal.Value = (int)(NetConfigData.DendriteActivationThresholdProximal);
			}
		}

		private void tbrDendriteActivationThresholdBasal_Scroll(object sender, EventArgs e)
		{
			int value = tbrDendriteActivationThresholdBasal.Value;
			NetConfigData.DendriteActivationThresholdBasal = value;
			txtDendriteActivationThresholdBasal.Text = NetConfigData.DendriteActivationThresholdBasal.ToString();
		}
		private void txtDendriteActivationThresholdBasal_TextChanged(object sender, EventArgs e)
		{
			int val;
			if (Int32.TryParse(txtDendriteActivationThresholdBasal.Text, out val))
			{
				NetConfigData.DendriteActivationThresholdBasal = val;
				tbrDendriteActivationThresholdBasal.Value = (int)(NetConfigData.DendriteActivationThresholdBasal);
			}
		}

		private void tbrDendriteActivationThresholdApical_Scroll(object sender, EventArgs e)
		{
			int value = tbrDendriteActivationThresholdApical.Value;
			NetConfigData.DendriteActivationThresholdApical = value;
			txtDendriteActivationThresholdApical.Text = NetConfigData.DendriteActivationThresholdApical.ToString();
		}
		private void txtDendriteActivationThresholdApical_TextChanged(object sender, EventArgs e)
		{
			int val;
			if (Int32.TryParse(txtDendriteActivationThresholdApical.Text, out val))
			{
				NetConfigData.DendriteActivationThresholdApical = val;
				tbrDendriteActivationThresholdApical.Value = (int)(NetConfigData.DendriteActivationThresholdApical);
			}
		}

		private void tbrColumnStimulusThreshold_Scroll(object sender, EventArgs e)
		{
			int value = tbrColumnStimulusThreshold.Value;
			NetConfigData.ColumnStimulusThreshold = value;
			txtColumnStimulusThreshold.Text = NetConfigData.ColumnStimulusThreshold.ToString();
		}
		private void txtColumnStimulusThreshold_TextChanged(object sender, EventArgs e)
		{
			int val;
			if (Int32.TryParse(txtColumnStimulusThreshold.Text, out val))
			{
				NetConfigData.ColumnStimulusThreshold = val;
				tbrColumnStimulusThreshold.Value = (int)(NetConfigData.ColumnStimulusThreshold);
			}
		}

		private void tbrZoneSizePercProximal_Scroll(object sender, EventArgs e)
		{
			int scrollValue = tbrZoneSizePercProximal.Value;
			double value = Convert.ToDouble(scrollValue / 1000.0);
			NetConfigData.ZoneSizePercProximal = value;
			txtZoneSizePercProximal.Text = NetConfigData.ZoneSizePercProximal.ToString("N4");
		}
		private void txtZoneSizePercProximal_TextChanged(object sender, EventArgs e)
		{
			double val;
			if (double.TryParse(txtZoneSizePercProximal.Text, out val))
			{
				NetConfigData.ZoneSizePercProximal = val;
				tbrZoneSizePercProximal.Value = (int)(NetConfigData.ZoneSizePercProximal * 1000);
			}
		}

		private void tbrZoneCoveragePercProximal_Scroll(object sender, EventArgs e)
		{
			int scrollValue = tbrZoneCoveragePercProximal.Value;
			double value = Convert.ToDouble(scrollValue / 1000.0);
			NetConfigData.ZoneCoveragePercProximal = value;
			txtZoneCoveragePercProximal.Text = NetConfigData.ZoneCoveragePercProximal.ToString("N4");
		}
		private void txtZoneCoveragePercProximal_TextChanged(object sender, EventArgs e)
		{
			double val;
			if (double.TryParse(txtZoneCoveragePercProximal.Text, out val))
			{
				NetConfigData.ZoneCoveragePercProximal = val;
				tbrZoneCoveragePercProximal.Value = (int)(NetConfigData.ZoneCoveragePercProximal * 1000);
			}
		}
		private void tbrZoneSizePercBasal_Scroll(object sender, EventArgs e)
		{
			int scrollValue = tbrZoneSizePercBasal.Value;
			double value = Convert.ToDouble(scrollValue / 1000.0);
			NetConfigData.ZoneSizePercBasal = value;
			txtZoneSizePercBasal.Text = NetConfigData.ZoneSizePercBasal.ToString("N4");
		}
		private void txtZoneSizePercBasal_TextChanged(object sender, EventArgs e)
		{
			double val;
			if (double.TryParse(txtZoneSizePercBasal.Text, out val))
			{
				NetConfigData.ZoneSizePercBasal = val;
				tbrZoneSizePercBasal.Value = (int)(NetConfigData.ZoneSizePercBasal * 1000);
			}
		}

		private void tbrZoneCoveragePercBasal_Scroll(object sender, EventArgs e)
		{
			int scrollValue = tbrZoneCoveragePercBasal.Value;
			double value = Convert.ToDouble(scrollValue / 1000.0);
			NetConfigData.ZoneCoveragePercBasal = value;
			txtZoneCoveragePercBasal.Text = NetConfigData.ZoneCoveragePercBasal.ToString("N4");
		}
		private void txtZoneCoveragePercBasal_TextChanged(object sender, EventArgs e)
		{
			double val;
			if (double.TryParse(txtZoneCoveragePercBasal.Text, out val))
			{
				NetConfigData.ZoneCoveragePercBasal = val;
				tbrZoneCoveragePercBasal.Value = (int)(NetConfigData.ZoneCoveragePercBasal * 1000);
			}
		}

		private void tbrNumCellsPerColumn_Scroll(object sender, EventArgs e)
		{
			int value = tbrNumCellsInColumn.Value;
			NetConfigData.NumCellsInColumn = value / 100;
			txtNumCellsInColumn.Text = NetConfigData.NumCellsInColumn.ToString();
		}
		private void txtNumCellsInColumn_TextChanged(object sender, EventArgs e)
		{
			int val;
			if (Int32.TryParse(txtNumCellsInColumn.Text, out val))
			{
				NetConfigData.NumCellsInColumn = val;
				tbrNumCellsInColumn.Value = (int)(NetConfigData.NumCellsInColumn * 100);
			}
		}

		private void tbrColumnTopPercentile_Scroll(object sender, EventArgs e)
		{
			int scrollValue = tbrColumnsTopPercentile.Value;
			double value = Convert.ToDouble(scrollValue / 1000.0);
			NetConfigData.ColumnsTopPercentile = value;
			txtColumnsTopPercentile.Text = NetConfigData.ColumnsTopPercentile.ToString("N4");
		}
		private void txtColumnsTopPercentile_TextChanged(object sender, EventArgs e)
		{
			double val;
			if (double.TryParse(txtColumnsTopPercentile.Text, out val))
			{
				NetConfigData.ColumnsTopPercentile = val;
				tbrColumnsTopPercentile.Value = (int)(NetConfigData.ColumnsTopPercentile * 1000);
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			//notify lsteners Save button pressed
			//main screen saves Settings data
			OnDataSave();
		}
	}
}
