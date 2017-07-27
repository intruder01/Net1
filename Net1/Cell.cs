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
		public bool IsPredicting { get; private set; }




		#region Constructors
		//generic constructor
		//no synaptic connections - mostly for testing
		public Cell(int index)
		{
			Index = index;
			IsActive = false;
			IsPredicting = false;
			BasalDendrite = new DendriteBasal(NetConfigData.DendriteActivationThresholdBasal);
		}

		//Cell in Column/Layer
		//Given cell location and Layer, make Basal synaptic connections
		public Cell(Layer lr, int colX, int colY, int index) : this(index)
		{
			//List<Column> potentialColumns = lr.GetColumnsFromCentre(colX, colY, lr.CalcRadius(lr.zoneSizePercBasal), false);
			//CreateBasalSynapses(potentialColumns, lr.zoneCoveragePercBasal);
		}


		#endregion


		#region Public methods

		//Main function to update all cell states
		public void Update()
		{
			BasalDendrite.Update();
			IsPredicting = BasalDendrite.IsActive;
		}
		
		public void CreateBasalSynapses(List<Column> potentialColumns, double zoneCoveragePerc)
		{
			zoneCoveragePerc = zoneCoveragePerc < 0 ? 1.0 : zoneCoveragePerc;

			//create random list of columns to connect
			int numSynapsesRequired = (int)(potentialColumns.Count * zoneCoveragePerc);
			int numNewSynapses = numSynapsesRequired - BasalDendrite.Synapses.Count;

			if (numNewSynapses > 0) //add synapses
			{
				//remove columns already connected from the potential list
				List<Column> alreadyConnected = BasalDendrite.GetConnectedColumnsList();
				potentialColumns.RemoveAll(x => alreadyConnected.Contains(x));

				IEnumerable<Column> connectColumns = potentialColumns.RandomSample(numNewSynapses, false);
				foreach (Column col in connectColumns)
					BasalDendrite.CreateSynapse(col);
			}

			if (numNewSynapses < 0)  //remove synapses
			{
				while (BasalDendrite.Synapses.Count > 0 && BasalDendrite.Synapses.Count > numSynapsesRequired)
				{
					//remove random synapse
					BasalDendrite.RemoveSynapseAt(Global.rnd.Next(BasalDendrite.Synapses.Count));
				}
			}
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

		//Set cell IsActive - used when setting up training case in InputPlane
		public void SetActiveState(bool active)
		{
			IsActive = active;
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
