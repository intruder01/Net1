using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net1.Stats;
using System.IO;


//Network contains 
//One InputPlane
//One Layer - to be extended later
//Network creates all neccessary inter-layer bindings
//Contains Trainer for presenting input data
//Train() function performs network training
namespace Net1
{
	public class Network
	{
		public InputPlane Ip { get; private set; }
		public Layer Lr { get; private set; }
		public Trainer Trainer { get; private set; }
		public string Filename { get; set; }

		////performance metrics
		public PerformanceMetrics Metrics { get; private set; }

		//training handshake signals
		public bool TrainRequest { get; set; }          //request to start training 
		public bool TrainingInProgress { get; set; }    //network is training indicator

		public Network()
		{
			Ip = null;
			Lr = null;
			Filename = "";

			//subscribe to configuration data changed
			NetConfigData.DataItemChanged += NetConfigDataChanged;

			Metrics = new PerformanceMetrics();
			TrainRequest = false;
			TrainingInProgress = false;
		}

		~Network()
		{
			//unsubscrible configuration data changed
			NetConfigData.DataItemChanged -= NetConfigDataChanged;
		}

		public Network(string filename)
			: this()
		{
			Filename = filename;

			Trainer = new Trainer(Filename);

			CreateLayers();
			ConnectLayers();

			Trainer.LoadCase(Ip, 0);

			ScreenUpdateData.DataChanged();
		}

	

		//create network layer structures to correspond to Trainer parameters
		public void CreateLayers()
		{
			Ip = new InputPlane(Trainer.NumColumnsX, Trainer.NumColumnsY);
			//Lr = new Layer ( Ip, Trainer.NumColumnsX, Trainer.NumColumnsY, NetConfigData.NumCellsInColumn );
			Lr = new Layer ( Ip, 2, 2, NetConfigData.NumCellsInColumn );
		}

		//create network synaptic connections per NetConfigData
		//this method is called when synapses are changing
		public void ConnectLayers()
		{
			Lr.ConnectColumns(Ip);
		}

		public void TrainEpoch()
		{
			if (Trainer.DataReady)
			{
				TrainingInProgress = true;
				for (int caseNum = 0; caseNum < Trainer.NumCases; caseNum++)
				{
					LoadNextCase();
					TrainCase();
				}
				TrainingInProgress = false;
			}
		}

		public void TrainCase()
		{
			if (Trainer.CurrCaseNum == 0 && Trainer.CurrCaseNum != Trainer.LastCaseNum)   //new epoch
			{
				statsInitialiseEpoch();
			}

			Lr.Update();
			statsUpdate_Case();

			//when back at case 0 - perform epoch end tasks
			if (Trainer.CurrCaseNum == Trainer.NumCases - 1)
			{
				statsUpdate_Epoch();
				adjustNetworkParams();
			}

			ScreenUpdateData.DataChanged();
		}

		public void LoadNextCase()
		{
			Trainer.LoadNextCase(Ip);
		}

		//stats update after completing 1 case  
		private void statsUpdate_Case()
		{
			Lr.SparsenessStat.Value = Metrics.CalcSparseness(Lr);

		}

		//stats update after completing epoch
		private void statsUpdate_Epoch()
		{
			Lr.EntropyStat.Value = Metrics.CalcEntropy(Lr, Trainer.NumCases);
			
		}

		//reset stats in preparation for another epoch
		private void statsInitialiseEpoch()
		{
			Metrics.InitializeEpoch(Lr);
		}

		//network parameter automatic adjustments at end of epoch
		private void adjustNetworkParams()
		{
			
			Lr.AdjustSparseness(Lr.SparsenessTarget);
			Lr.ConnectColumns(Ip);
			Lr.AdjustBoostFactors();
		}

		//event handler for configuration data changes.
		//this may be extended later and split up for specific parts of the network
		//eg. change Dendrite params only, without affecting others
		public void NetConfigDataChanged(NetConfigDataItem changedItem)
		{
			switch (changedItem)
			{
				//Synapse parameters
				//-SynapsePermanenceIncrease - used directly 
				case NetConfigDataItem.SynapsePermanenceThreshold:
					break;
				//-SynapsePermanenceDecrease - used directly
				case NetConfigDataItem.SynapsePermanenceIncrease:
					break;
				//-SynapsePermanenceThreshold - used directly
				case NetConfigDataItem.SynapsePermanenceDecrease:
					break;

				//Dendrite parameters
				//-DendriteActivationThreshold_Proximal - DendriteProximal property
				case NetConfigDataItem.DendriteActivationThreshold_Proximal:
					for (int x = 0; x < Lr.NumColumnsX; x++)
						for (int y = 0; y < Lr.NumColumnsY; y++)
						{
							Column col = Lr.Columns[y][x];
							col.ProximalDendrite.ActivationThreshold = NetConfigData.DendriteActivationThresholdProximal;
						}
					break;
				//-DendriteActivationThreshold_Basal - DendriteBasal property
				case NetConfigDataItem.DendriteActivationThreshold_Basal:
					for (int x = 0; x < Lr.NumColumnsX; x++)
						for (int y = 0; y < Lr.NumColumnsY; y++)
						{
							Column col = Lr.Columns[y][x];
							foreach (Cell cell in col.Cells)
							{
								cell.BasalDendrite.ActivationThreshold = NetConfigData.DendriteActivationThresholdBasal;
							}
						}
					break;
				//-DendriteActivationThreshold_Apical - DendriteApical property
				case NetConfigDataItem.DendriteActivationThreshold_Apical:
					for (int x = 0; x < Lr.NumColumnsX; x++)
						for (int y = 0; y < Lr.NumColumnsY; y++)
						{
							Column col = Lr.Columns[y][x];
							col.ApicalDendrite.ActivationThreshold = NetConfigData.DendriteActivationThresholdApical;
						}
					break;

				//Column parameters
				//-ColumnStimulusThreshold - used directly
				case NetConfigDataItem.ColumnStimulusThreshold:
					break;

				//Layer parameters
				//-ZoneSizePercProximal - used in Layer
				case NetConfigDataItem.ZoneSizePercProximal:
					Lr.ZoneSizePercProximal = NetConfigData.ZoneSizePercProximal;
					Lr.ConnectColumns(Ip); 
					break;
				//-ZoneCoveragePercProximal - used in Layer
				case NetConfigDataItem.ZoneCoveragePercProximal:
					Lr.ZoneCoveragePercProximal = NetConfigData.ZoneCoveragePercProximal;
					Lr.ConnectColumns(Ip);
					break;
				//-ZoneSizePercBasal - used in Layer
				case NetConfigDataItem.ZoneSizePercBasal:
					Lr.ZoneSizePercBasal = NetConfigData.ZoneSizePercBasal;
					Lr.ConnectColumns(Ip);
					break;
				//-ZoneCoveragePercBasal - used in Layer
				case NetConfigDataItem.ZoneCoveragePercBasal:
					Lr.ZoneCoveragePercBasal = NetConfigData.ZoneCoveragePercBasal;
					Lr.ConnectColumns(Ip);
					break;


				//Network parameters
				//-NumCellsPerColumn - used in Layer
				case NetConfigDataItem.NumCellsPerColumn:
					Lr.NumCellsInColumn = NetConfigData.NumCellsInColumn;
					Lr.CreateColumns();
					Lr.ConnectColumns(Ip);
					break;
				//-ColumnsTopPercentile - used directly
				case NetConfigDataItem.ColumnsTopPercentile:
					Lr.ZoneCoveragePercBasal = NetConfigData.ColumnsTopPercentile;
					break;



			}
		}
	}
}
