using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;


//contains network configuration parameters
//stored centrally in the Network
//reference is accessible by all Layers
namespace Net1
{
	public static class NetConfigData
	{
		//event trigger for data changes where Item is sent with message
		//used for automatic network re-configuration
		public delegate void NetConfigDataItemChangedEventHandler(NetConfigDataItem changedItem);
		public static event NetConfigDataItemChangedEventHandler DataItemChanged;

		public static void OnDataItemChanged(NetConfigDataItem changedItem)
		{
			//specific data changed event
			DataItemChanged?.Invoke(changedItem);

			//also trigger generic data changed event
			DataChanged?.Invoke();
		}


		//event trigger for data changes without Item
		//used for GUI updates
		public delegate void NetConfigDataChangedEventHandler();
		public static event NetConfigDataChangedEventHandler DataChanged;
		public static void OnDataChanged()
		{
			DataChanged?.Invoke();
		}

		//Synapse parameters

		//Numenta: set to 0.5 for all experiments
		private static double _SynapsePermanenceThreshold;
		public static double SynapsePermanenceThreshold
		{
			get { return _SynapsePermanenceThreshold; }
			set { _SynapsePermanenceThreshold = Max(0, Min(1.0, value)); OnDataItemChanged(NetConfigDataItem.SynapsePermanenceThreshold); }
		}
		//Increase Permanence when connectedColumn is active
		private static double _SynapsePermanenceIncrease;
		public static double SynapsePermanenceIncrease
		{
			get { return _SynapsePermanenceIncrease; }
			set { _SynapsePermanenceIncrease = Max(0, Min(0.1, value)); OnDataItemChanged(NetConfigDataItem.SynapsePermanenceIncrease); }
		}
		//Decrease Permanence when connectedColumn is not active
		private static double _SynapsePermanenceDecrease;
		public static double SynapsePermanenceDecrease
		{
			get { return _SynapsePermanenceDecrease; }
			set { _SynapsePermanenceDecrease = Max(0, Min(0.01, value)); OnDataItemChanged(NetConfigDataItem.SynapsePermanenceDecrease); }
		}

		//Dendrite parameters

		//Number of active Synapses to turn on Dendrite
		private static int _DendriteActivationThresholdProximal;
		public static int DendriteActivationThresholdProximal
		{
			get { return _DendriteActivationThresholdProximal; }
			set { _DendriteActivationThresholdProximal = Max(0, value); OnDataItemChanged(NetConfigDataItem.DendriteActivationThreshold_Proximal); }
		}
		private static int _DendriteActivationThresholdBasal;
		public static int DendriteActivationThresholdBasal
		{
			get { return _DendriteActivationThresholdBasal; }
			set { _DendriteActivationThresholdBasal = Max(0, value); OnDataItemChanged(NetConfigDataItem.DendriteActivationThreshold_Basal); }
		}
		private static int _DendriteActivationThresholdApical;
		public static int DendriteActivationThresholdApical
		{
			get { return _DendriteActivationThresholdApical; }
			set { _DendriteActivationThresholdApical = Max(0, value); OnDataItemChanged(NetConfigDataItem.DendriteActivationThreshold_Apical); }
		}


		//Column parameters

		//minimum Column InputOverlap for activation
		private static int _ColumnStimulusThreshold;
		public static int ColumnStimulusThreshold
		{
			get { return _ColumnStimulusThreshold; }
			set { _ColumnStimulusThreshold = Max(0, value); OnDataItemChanged(NetConfigDataItem.ColumnStimulusThreshold); }
		}
		
		//Layer parameters

		//zoneSizePerc as applied to Proximal connections to InputPlane
		private static double _ZoneSizePercProximal;
		public static double ZoneSizePercProximal
		{
			get { return _ZoneSizePercProximal; }
			set { _ZoneSizePercProximal = Max(0, Min(1, value)); OnDataItemChanged(NetConfigDataItem.ZoneSizePercProximal); }
		}
		//zoneCoveragePerc as applied to Proximal connections to InputPlane
		private static double _ZoneCoveragePercProximal;
		public static double ZoneCoveragePercProximal
		{
			get { return _ZoneCoveragePercProximal; }
			set { _ZoneCoveragePercProximal = Max(0, Min(1, value)); OnDataItemChanged(NetConfigDataItem.ZoneCoveragePercProximal); }
		}
		//zoneSizePerc as applied to Basal connections in Layer
		private static double _ZoneSizePercBasal;
		public static double ZoneSizePercBasal
		{
			get { return _ZoneSizePercBasal; }
			set { _ZoneSizePercBasal = Max(0, Min(1, value)); OnDataItemChanged(NetConfigDataItem.ZoneSizePercBasal); }
		}
		//zoneCoveragePerc as applied to Proximal connections inLayer	
		private static double _ZoneCoveragePercBasal;
		public static double ZoneCoveragePercBasal
		{
			get { return _ZoneCoveragePercBasal; }
			set { _ZoneCoveragePercBasal = Max(0, Min(1, value)); OnDataItemChanged(NetConfigDataItem.ZoneCoveragePercBasal); }
		}
		//zoneSizePerc as applied to Apical connections in Layer
		private static double _ZoneSizePercApical;
		public static double ZoneSizePercApical
		{
			get { return _ZoneSizePercApical; }
			set { _ZoneSizePercApical = Max(0, Min(1, value)); OnDataItemChanged(NetConfigDataItem.ZoneSizePercApical); }
		}
		//zoneCoveragePerc as applied to Proximal connections inLayer	
		private static double _ZoneCoveragePercApical;
		public static double ZoneCoveragePercApical
		{
			get { return _ZoneCoveragePercApical; }
			set { _ZoneCoveragePercApical = Max(0, Min(1, value)); OnDataItemChanged(NetConfigDataItem.ZoneCoveragePercApical); }
		}



		//Network parameters

		//number of Cells per Column
		private static int _NumCellsInColumn;
		public static int NumCellsInColumn
		{
			get { return _NumCellsInColumn; }
			set { _NumCellsInColumn = Max(1, Min(10, value)); OnDataItemChanged(NetConfigDataItem.NumCellsPerColumn); }
		}

		//2% is the usual ratio to obtain SDR
		private static double _ColumnsTopPercentile;
		public static double ColumnsTopPercentile
		{
			get { return _ColumnsTopPercentile; }
			set { _ColumnsTopPercentile = Max(0, Min(1, value)); OnDataItemChanged(NetConfigDataItem.ColumnsTopPercentile); }
		}

		//number of Epochs to train with Train button
		private static int _NumEpochsToTrain;
		public static int NumEpochsToTrain
		{
			get { return _NumEpochsToTrain; }
			set { _NumEpochsToTrain = value; OnDataItemChanged(NetConfigDataItem.NumEpochsToTrain); }
		}

		//ActivationDensity control parameters

		//activation density adjustment interval
		private static int _ActivationDensityInterval;
		public static int ActivationDensityInterval
		{
			get { return _ActivationDensityInterval; }
			set { _ActivationDensityInterval = value; OnDataItemChanged(NetConfigDataItem.ActivationDensityInterval); }
		}

		//target activation density from Spatial Pooler
		private static double _ActivationDensityTarget;
		public static double ActivationDensityTarget
		{
			get { return _ActivationDensityTarget; }
			set { _ActivationDensityTarget = value; OnDataItemChanged(NetConfigDataItem.ActivationDensityTarget); }
		}


		//used only for testing
		public static void SetDefaults()
		{
			//Synapse parameters
			SynapsePermanenceThreshold = 0.5;       //Minimum permanence to activate Synapse
			SynapsePermanenceIncrease = 0.01;		//Increase Permanence when connectedColumn is active
			SynapsePermanenceDecrease = 0.001;      //Decrease Permanence when connectedColumn is not active

			//Dendrite parameters
			DendriteActivationThresholdProximal = 1;     //Number of active Synapses to turn on Dendrite
			DendriteActivationThresholdBasal = 1;        //Number of active Synapses to turn on Dendrite
			DendriteActivationThresholdApical = 1;       //Number of active Synapses to turn on Dendrite

			//Column parameters
			ColumnStimulusThreshold = 1;            //minimum Column InputOverlap for activation

			//Layer parameters
			ZoneSizePercProximal = 0.3;			//zoneSizePerc as applied to Proximal connections to InputPlane
			ZoneCoveragePercProximal = 0.01;		//zoneCoveragePerc as applied to Proximal connections to InputPlane
			ZoneSizePercBasal = 1;					//zoneSizePerc as applied to Basal connections in Layer
			ZoneCoveragePercBasal = 1;              //zoneCoveragePerc as applied to Proximal connections inLayer	
			ZoneSizePercApical = 1;                  //zoneSizePerc as applied to Apical connections in Layer
			ZoneCoveragePercApical = 1;              //zoneCoveragePerc as applied to Apical connections inLayer	
													//Network parameters	
			NumCellsInColumn = 3;					//number of Cells per Column in Layer
			ColumnsTopPercentile = 0.02;            // 2% is the usual ratio to obtain SDR

			NumEpochsToTrain = 10;

			ActivationDensityInterval = 1;
			ActivationDensityTarget = 0.02;

		}




	}

	//event parameters - used to pecify which configuration item changed
	//so network adjustments can be made
	public enum NetConfigDataItem
	{
		None,
		//Synapse parameters
		SynapsePermanenceThreshold,       //Minimum permanence to activate Synapse
		SynapsePermanenceIncrease,       //Increase Permanence when connectedColumn is active
		SynapsePermanenceDecrease,      //Decrease Permanence when connectedColumn is not active

		//Dendrite parameters
		DendriteActivationThreshold_Proximal,        //Number of active Synapses to turn on Dendrite
		DendriteActivationThreshold_Basal,        //Number of active Synapses to turn on Dendrite
		DendriteActivationThreshold_Apical,        //Number of active Synapses to turn on Dendrite

		//Column parameters
		ColumnStimulusThreshold,            //minimum Column InputOverlap for activation

		//Layer parameters
		ZoneSizePercProximal,            //zoneSizePerc as applied to Proximal connections to InputPlane
		ZoneCoveragePercProximal,        //zoneCoveragePerc as applied to Proximal connections to InputPlane
		ZoneSizePercBasal,               //zoneSizePerc as applied to Basal connections in Layer
		ZoneCoveragePercBasal,           //zoneCoveragePerc as applied to Proximal connections inLayer	
		ZoneSizePercApical,              //zoneSizePerc as applied to Apical connections in Layer
		ZoneCoveragePercApical,          //zoneCoveragePerc as applied to Proximal connections inLayer	


		//Network parameters	
		NumCellsPerColumn,                  //number of Cells per Column in Layer
		ColumnsTopPercentile,
		NumEpochsToTrain,
		ActivationDensityInterval,
		ActivationDensityTarget
	}


}

