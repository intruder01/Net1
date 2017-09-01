using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

//Cell implements functionality of a neuron.
//Main components:
//	activation level
//		Active		- cell has been activated by proximal inputs
//		Predicting	- cell has been depolarized by basal synapses
//	dendrite segments
//		proximal	- 1 only, basic input space for the neuron
//		basal		- multiple, context for activation changes
//		apical		- ???, feedback 


namespace Net1
{
	public class Cell : Selectable3DObject
	{
		//Cell index in Column
		public int Index { get; private set; }
		
		//Basal Dendrite segment
		public DendriteBasal BasalDendrite { get; private set; }
		
		//Status properties
		public bool IsActive { get; private set; }
		//cell is predicting when cell's distal dendrite is active
		//indicating that cell is anticipating activation
		public bool IsPredicting { get; private set; }
		//cell is sequence predicting when cell's apical dendrite is active
		//indicating that cell is anticipating activation
		public bool IsSeqPredicting { get; private set; }
		//cell is learning when it's column is activated via proximal connection
		//and no cell in the column predicted the activation.
		//All cells turn on in 'Learning' mode indicating that any proximal input will be
		//learned by one of the cells.
		public bool IsLearning { get; private set; }    //TODO: need logic for this property
		
		public Column Column { get; private set; }		//TODO: reference to parent column. May need to remove later
		//Column location reference for debug
		//TODO: remove later
		public int ColumnX;
		public int ColumnY;
		

		#region Constructors
		//generic constructor
		//no synaptic connections
		public Cell(int index)
		{
			Index = index;
			IsActive = false;
			IsPredicting = false;
			IsSeqPredicting = false;
			IsLearning = false;
			BasalDendrite = new DendriteBasal(NetConfigData.DendriteActivationThresholdBasal);
		}

		//debug constructor with Column location reference
		//TODO: remove this constructor 
		public Cell(int index, Column column) : this(index)
		{
			Column = column;
			ColumnX = column.X;
			ColumnY = column.Y;
		}


		#endregion


		#region Public methods

		//Main function to update all cell states
		public void Update()
		{
			BasalDendrite.Update();
			IsPredicting = BasalDendrite.IsActive;

			//ApicalDendrite.Update ();
			//IsSeqPredicting = ApicalDendrite.IsActive;
		}
		
		//public void CreateBasalSynapses(List<Column> potentialColumns, double zoneCoveragePerc)
		//{
		//	zoneCoveragePerc = zoneCoveragePerc < 0 ? 1.0 : zoneCoveragePerc;

		//	//create random list of columns to connect
		//	int numSynapsesRequired = (int)(potentialColumns.Count * zoneCoveragePerc);
		//	int numNewSynapses = numSynapsesRequired - BasalDendrite.Synapses.Count;

		//	if (numNewSynapses > 0) //add synapses
		//	{
		//		//remove columns already connected from the potential list
		//		List<Column> alreadyConnected = BasalDendrite.GetConnectedColumnsList();
		//		potentialColumns.RemoveAll(x => alreadyConnected.Contains(x));

		//		IEnumerable<Column> connectColumns = potentialColumns.RandomSample(numNewSynapses, false);
		//		foreach (Column col in connectColumns)
		//			BasalDendrite.CreateSynapse(col);
		//	}

		//	if (numNewSynapses < 0)  //remove synapses
		//	{
		//		while (BasalDendrite.Synapses.Count > 0 && BasalDendrite.Synapses.Count > numSynapsesRequired)
		//		{
		//			//remove random synapse
		//			BasalDendrite.RemoveSynapseAt(Global.rnd.Next(BasalDendrite.Synapses.Count));
		//		}
		//	}
		//}

		public void CreateBasalSynapses (List<Column> potentialColumns, double zoneCoveragePerc)
		{
			BasalDendrite.CreateSynapses ( potentialColumns, zoneCoveragePerc );
		}

		internal void RemoveAllBasalSynapses()
		{
			BasalDendrite.Synapses.Clear();
		}

		public int CountBasalSynapses()
		{
			return BasalDendrite.CountSynapses();
		}

		public int CountActiveBasalSynapses()
		{
			return BasalDendrite.CountActiveSynapses();

		}

		//Set cell IsActive state - used when setting up training case in InputPlane
		public void SetActiveState(bool active)
		{
			IsActive = active;
		}

		//Set cell IsLearning state
		public void SetLearningState (bool learning)
		{
			IsLearning = learning;
		}

		//TODO: verify if HTM theory requires multiple Basal dendrites (segments) in a cell
		public DendriteBasal GetSequencePredictingBasalSegment ()
		{
			
			//TODO: this logic different from openHTM due to only single basal dendrite (currently)

			DendriteBasal predictingDendrite = null;

			if ( this.IsPredicting)
			{
				if ( BasalDendrite.IsActive )
					predictingDendrite = BasalDendrite;
				else
					predictingDendrite = null;
			}
			return predictingDendrite;
		}


#if TESTING

		//General rule for Override functions:
		//1. Override() method sets IsActive, IsPredicting etc.
		//2. DEEP Override() - alters all sub-elements so that Update() will give required results.

		public void OverrideActive(bool active)
		{
			IsActive = active;
		}

		public void OverridePredicting(bool predicting, int depth)
		{
			IsPredicting = predicting;
			if (depth < Global.OVERRIDE_DEPTH)
			{
				BasalDendrite.Override(predicting, depth);
			}
		}

		public void OverrideBasalDendriteActivationThreshold(int threshold)
		{
			BasalDendrite.OverrideActivationThreshold(threshold);
		}




#endif


#endregion
	}
}
