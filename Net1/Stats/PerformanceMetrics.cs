using System;
using System.Collections.Generic;
//Network performance metrics
//
//Spareseness	- number of active Columns after each training case
//				- target activation density is 2%-5%
//Entropy		- high entropy indicates that max number of Cells participate


namespace Net1.Stats
{
	public class PerformanceMetrics
	{


		#region Constructors

		public PerformanceMetrics()
		{

		}

		#endregion



		#region Public Methods

		//clear vars in preparation for next epoch
		public void InitializeEpoch(Layer lr, InputPlane ip, int numCases)
		{
			clearColumnActivationCounts(lr);
		}

		//calculate sparseness (activation density)
		public void CalcSparseness(Layer lr, int numCases)
		{
			//Eq 10
			lr.SparsenessStat.Value = (double)lr.CountActiveColumns() / lr.NumColumns;
		}



		//calculate entropy 
		public void CalcEntropy(Layer lr, int numCases)
		{
			double entropy = 0.0;

			//Eq 11
			List<List<double>> ai = new List<List<double>>();
			//for each Column, calculate activation frequency
			for (int x = 0; x < lr.Columns.Count; x++)
			{
				List<double> yCol = new List<double>();
				ai.Add(yCol);
				for (int y = 0; y < lr.Columns[x].Count; y++)
				{
					yCol.Add(Math.Max(0.0000001, lr.Columns[x][y].ColumnActivationCount / (double)numCases));
				}
			}

			//Eq 12
			double SumActCounts = 0;
			for (int x = 0; x < lr.Columns.Count; x++)
			{
				for (int y = 0; y < lr.Columns[x].Count; y++)
				{
					SumActCounts += ai[x][y];
				}
			}

			if (SumActCounts > 0)
			{
				List<List<double>> Pai = new List<List<double>>();
				for (int x = 0; x < lr.Columns.Count; x++)
				{
					List<double> yCol = new List<double>();
					Pai.Add(yCol);
					for (int y = 0; y < lr.Columns[x].Count; y++)
					{
						yCol.Add(ai[x][y] / SumActCounts);
					}
				}

				//Eq 13
				for (int x = 0; x < lr.Columns.Count; x++)
				{
					for (int y = 0; y < lr.Columns[x].Count; y++)
					{
						double P = Pai[x][y];
						entropy += P * Math.Log(P, 2) + (1.0 - P) * Math.Log(1.0 - P, 2);
					}
				}
			}

			lr.EntropyStat.Value = -entropy;
		}

		#endregion




		#region Maintenance Methods 

		//clear column activation counters
		private void clearColumnActivationCounts(Layer lr)
		{
			for (int x = 0; x < lr.Columns.Count; x++)
			{
				for (int y = 0; y < lr.Columns[x].Count; y++)
				{
					lr.Columns[x][y].ColumnActivationCount = 0;
				}
			}
		}

		#endregion

	}
}
