using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;





//From: How do neurons operate on sparse distributed representations.pdf
//2.1. Model Dendrite
//Figure 2 illustrates our segment model and how a segment detects patterns.We model the
//instantaneous activity of presynaptic cells as being either on or off.The effect of an individualSparse Distributed Representations
//This	is	a provisional file, not the final typeset article 4
//synapse is similarly binary. As shown in Figure 2A, a dendritic segment would typically be
//connected to a very small subset of all possible neurons in an input region.Although this paper
//focuses on a static analysis(i.e.we do not model learning), the existence of plasticity rules reliant on
//synapse clustering and NMDA spikes has been shown experimentally(Takahashi et al., 2012;
//Losonczy and Magee, 2006; Makino and Malinow, 2011; Makara et al., 2009). The specific subset of
//synapses within a segment will change over time as a result of structural plasticity(Chklovskii et al.,
//2004). Therefore the number of potential connections to a segment via plasticity is much larger than
//the number of actual connections.The actual connections are a subset of prototype activity patterns
//to be recognized.

//[Figure 2 goes about here, please see end of manuscript]
//Formally, we denote the number of potential connections as �. A dendritic segment is represented as
//a binary vector � = �!, ⋯ , �!!! where a non-zero value �! indicates a synaptic connection to
//presynaptic cell � and � = � indicates the number of synapses on that segment.Experimental
//findings suggest that typical numbers for � are between 20 and 300 (Major et al., 2013). �, the
//number of potential connections is assumed to be much larger(numbering in the thousands) leading
//to very sparse �


namespace Net1
{
	public abstract class Dendrite
	{
		//Number of active Synapses
		public int ActivationSum { get; protected set; }

		//Number of active Synapses to activate
		private int _activationThreshold;
		public int ActivationThreshold
		{
			get { return _activationThreshold; }
			set { _activationThreshold = Max(1, value); }
		}


		//True if Dendrite is active.
		public bool IsActive { get; protected set; }




		#region  Constructors


		public Dendrite(int threshold)
		{
			ActivationThreshold = threshold;
		}


		#endregion




		#region Public methods




		// Process Synapses
		public abstract int Update();

		//create all Synaptic connections given list of potential columns and percentage coverage
		public abstract bool CreateSynapses(List<Column> potentialColumns, double zoneCoveragePerc);

		//Make new Synaptic connection to a Cell
		//(duplicate connections not allowed)
		//return true if new connection made
		public abstract bool CreateSynapse(Column col);
		//Remove Synaptic connection to specified Column
		public abstract bool RemoveSynapse(Column col);
		//Remove Synaptic connection at specified index
		public abstract bool RemoveSynapseAt(int idx);
		public abstract void RemoveAllSynapses();

		//Find connection index to specified Column
		public abstract int FindConnectionIndex(Column col);

		//Check if this Dendrite is connected to another Dendrite
		//return true if already connected
		public abstract bool ConnectionExists(Column col);

		//count number of Synapses on Dendrite
		public abstract int CountSynapses();

		//count number of active Synapses on Dendrite
		public abstract int CountActiveSynapses();

		//SetActivationThreshold
		public void SetActivationThreshold(int threshold)
		{
			ActivationThreshold = threshold;
		}

		//get list of connected columns
		public abstract List<Column> GetConnectedColumnsList();



#if TESTING

		//General rule for Override functions:
		//1. Override() method sets IsActive, IsPredicting only
		//2. Update() method does not alter sub-elements in TESTING

		public abstract void Override(bool state, int depth);
		public abstract void OverridePermanence(double permanence);

		public abstract void OverrideActivationThreshold(int threshold);

		

#endif


		#endregion

	}


}
