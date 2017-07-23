using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Net1
{
	public abstract class Synapse
	{
		//List of Columns connected to this Synapse
		public Column ColumnConnected { get; protected set; }

		//Active when IsConnected and ColumnConnected IsActive
		public bool IsActive { get; protected set; }
		//Synapse Permanence value (scalar 0-1)
		private double _permanence;
		public double Permanence
		{
			get
			{
				return _permanence;
			}
			protected set
			{
				_permanence = Max(0, Min(value, 1.0));
			}
		}



		#region Constructors

		public Synapse(Column col)
		{
			ColumnConnected = col;
			IsActive = false;

			//Permanence randomly distributed between min-max
			Permanence = Global.rnd.NextDouble(
				Global.SYNAPSE_INITIAL_PERMANENCE_MIN,
				Global.SYNAPSE_INITIAL_PERMANENCE_MAX);
		}


		#endregion


		#region Public Methods

		//Update active state
		public abstract int Update();

#if TESTING


		//General rule for Override functions:
		//1. Override() method sets IsActive, IsPredicting etc.
		//2. DEEP Override() - alters all sub-elements so that Update() will give required results.

		public abstract void Override(bool state, int depth);
		public abstract void OverridePermanence(double permanence);

#endif



		#endregion


	}


}
