using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net1
{
	public class DendriteApical : Dendrite
	{
		//List of Synapses on this Dendrite.
		public List<SynapseApical> Synapses { get; private set; }


		#region Constructors

		public DendriteApical(int threshold) : base(threshold)
		{
			Synapses = new List<SynapseApical>();
		}

		#endregion




		#region Public Methods

		public override int Update()
		{
			ActivationSum = 0;
			foreach (SynapseApical syn in Synapses)
			{
				syn.Update();
				if (syn.IsActive)
					ActivationSum++;
			}
			IsActive = Synapses.Count > 0 ? ActivationSum >= ActivationThreshold : false;
			return ActivationSum;
		}

		//create all Synaptic connections given list of potential columns and percentage coverage
		public override bool CreateSynapses(List<Column> potentialColumns, double zoneCoveragePerc)
		{
#if !DEBUG
			//calculate how many new synapses to create
			int numSynapsesRequired = (int)(potentialColumns.Count * zoneCoveragePerc);
			int numNewSynapses = numSynapsesRequired - this.Synapses.Count;

			//add random synapses from list
			if (numNewSynapses > 0) //add synapses
			{
				//remove columns already connected from the potential list
				List<Column> alreadyConnected = GetConnectedColumnsList();
				potentialColumns.RemoveAll(x => alreadyConnected.Contains(x));

				IEnumerable<Column> connectColumns = potentialColumns.RandomSample(numNewSynapses, false);
				foreach (Column col in connectColumns)
					CreateSynapse(col);
			}

			//Live - remove random synapses
			if (numNewSynapses < 0) //remove synapses
			{
				while (Synapses.Count > 0 && Synapses.Count > numSynapsesRequired)
					RemoveSynapseAt(Global.rnd.Next(0, Synapses.Count));
			}
			return true;
#else //DEBUG
			// remove existing synapses which are NOT in potential list (outside of zone)
			List<Column> connectedList = GetConnectedColumnsList ();
			List<Column> removeList = connectedList.Where ( x => !( potentialColumns.Contains ( x ) ) ).ToList ();
			RemoveSpecificSynapses ( removeList );

			//calculate how many new synapses to create
			int numSynapsesRequired = (int)( potentialColumns.Count * zoneCoveragePerc );
			int numNewSynapses = numSynapsesRequired - this.Synapses.Count;

			//add first x synapses from the list
			if ( numNewSynapses > 0 )
			{
				//remove columns already connected from the potential list
				List<Column> alreadyConnected = GetConnectedColumnsList ();
				potentialColumns.RemoveAll ( x => alreadyConnected.Contains ( x ) );

				for ( int i = 0; i < Math.Min ( numNewSynapses, potentialColumns.Count ); i++ )
				{
					Column col = potentialColumns[i];
					CreateSynapse ( col );
				}
			}

			//DEBUG - remove synapses from end
			if ( numNewSynapses < 0 )
			{
				while ( Synapses.Count > 0 && Synapses.Count > numSynapsesRequired )
					RemoveSynapseAt ( Synapses.Count - 1 );
			}
			return true;

#endif
		}

		protected void RemoveSpecificSynapses (List<Column> columnList)
		{
			foreach ( Column column in columnList )
			{
				for ( int i = 0; i < Synapses.Count; i++ )
				{
					SynapseApical syn = Synapses[i];
					if ( syn.ColumnConnected == column )
						Synapses.Remove ( syn );
				}
			}
		}

		//Make new Synaptic connection to a Cell
		//(duplicate connections not allowed)
		//return true if new connection made
		public override bool CreateSynapse(Column col)
		{
			if (!ConnectionExists(col))
			{
				SynapseApical syn = new SynapseApical(col);
				Synapses.Add(syn);
				return true;
			}
			return false;
		}
		public override bool RemoveSynapse(Column col)
		{
			int idx = FindConnectionIndex(col);
			if (idx >= 0)
			{
				Synapses.RemoveAt(idx);
				return true;
			}
			return false;
		}

		public override bool RemoveSynapseAt(int idx)
		{
			if (idx < Synapses.Count)
			{
				Synapses.RemoveAt(idx);
				return true;
			}
			return false;
		}

		public override void RemoveAllSynapses()
		{
			Synapses.Clear();
		}

		//Check if this Dendrite is connected to another Dendrite
		//return true if already connected
		public override bool ConnectionExists(Column col)
		{
			foreach (SynapseApical syn in Synapses)
				if (syn.ColumnConnected == col)
					return true;
			return false;
		}

		public override int FindConnectionIndex(Column col)
		{
			for (int i = 0; i < Synapses.Count; i++)
			{
				if (Synapses[i].ColumnConnected == col)
					return i;
			}
			return -1;
		}

		//count number of active Synapses on Dendrite
		public override int CountActiveSynapses()
		{
			int cnt = 0;
			foreach (SynapseApical syn in Synapses)
				if (syn.IsActive)
					cnt++;
			return cnt;
		}

		public override List<Column> GetConnectedColumnsList()
		{
			List<Column> res = new List<Column>();
			foreach (SynapseApical syn in Synapses)
			{
				res.Add(syn.ColumnConnected);
			}
			return res;
		}

		public override int CountSynapses()
		{
			return Synapses.Count;
		}


#if TESTING

		//General rule for Override functions:
		//1. Override() method sets IsActive, IsPredicting etc.
		//2. DEEP Override() - alters all sub-elements so that Update() will give required results.

		public override void Override(bool state, int depth)
		{
			foreach (SynapseApical syn in Synapses)
				syn.Override(state, depth);

			IsActive = state;
		}

		public override void OverridePermanence(double permanence)
		{
			foreach (SynapseApical syn in Synapses)
				syn.OverridePermanence(permanence);
		}

		public override void OverrideActivationThreshold(int threshold)
		{
			ActivationThreshold = threshold;
		}

		

#endif


#endregion
	}
}
