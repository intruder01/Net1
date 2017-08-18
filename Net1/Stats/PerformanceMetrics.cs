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
		public void InitializeEpoch(Layer lr)
		{
			clearColumnCounters(lr);
		}

		//calculate sparseness (activation density)
		public double CalcSparseness (Layer lr)
		{
			//Eq 10
			return (double)lr.CountActiveColumns () / lr.NumColumns;
		}



		//calculate entropy 
		public double CalcEntropy(Layer lr, int numCases)
		{
			double entropy = 0.0;

			//Eq 11
			List<List<double>> ai = new List<List<double>>();
			//for each Column, calculate activation frequency
			for (int x = 0; x < lr.NumColumnsX; x++)
			{
				List<double> yCol = new List<double>();
				ai.Add(yCol);
				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					yCol.Add(Math.Max(0.0000001, lr.Columns[y][x].ColumnActivationCount / (double)numCases));
				}
			}

			//Eq 12
			double SumActCounts = 0;
			for (int x = 0; x < lr.NumColumnsX; x++)
			{
				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					SumActCounts += ai[x][y];
				}
			}

			if (SumActCounts > 0)
			{
				List<List<double>> Pai = new List<List<double>>();
				for (int x = 0; x < lr.NumColumnsX; x++)
				{
					List<double> yCol = new List<double>();
					Pai.Add(yCol);
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						yCol.Add(ai[x][y] / SumActCounts);
					}
				}

				//Eq 13
				for (int x = 0; x < lr.NumColumnsX; x++)
				{
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						double P = Pai[x][y];
						entropy += P * Math.Log(P, 2) + (1.0 - P) * Math.Log(1.0 - P, 2);
					}
				}
			}
			return - entropy;
		}

		#endregion




		#region Maintenance Methods 

		//clear column activation counters
		private void clearColumnCounters(Layer lr)
		{
			for (int x = 0; x < lr.NumColumnsX; x++)
			{
				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					lr.Columns[y][x].ColumnActivationCount = 0;
					lr.Columns[y][x].ColumnInhibitedCount = 0;
				}
			}
		}

		#endregion

	}
}
