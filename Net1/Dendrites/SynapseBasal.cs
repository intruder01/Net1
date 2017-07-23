using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net1
{
	public class SynapseBasal : Synapse
	{
		#region Constructors

		public SynapseBasal(Column col) : base(col)
		{
		}


		#endregion



		#region Public Methods


		public override int Update()
		{
			if (ColumnConnected.IsPredicting)
				Permanence += NetConfigData.SynapsePermanenceIncrease;
			else
				Permanence -= NetConfigData.SynapsePermanenceDecrease;

			IsActive = Permanence >= NetConfigData.SynapsePermanenceThreshold && ColumnConnected.IsPredicting;

			if (IsActive)
				return 1;
			else
				return 0;
		}





#if TESTING


		//General rule for Override functions:
		//1. Override() method sets IsActive, IsPredicting etc.
		//2. DEEP Override() - alters all sub-elements so that Update() will give required results.


		public override void Override(bool state, int depth)
		{
			ColumnConnected.OverridePredicting(state, depth);
			Permanence = state ? 1.0 : 0.0;
			IsActive = state;
		}

		public override void OverridePermanence(double permanence)
		{
			Permanence = permanence;
		}


#endif


		#endregion
	}
}
