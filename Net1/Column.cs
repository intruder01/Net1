using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using static System.Math;



namespace Net1
{
	public class Column
	{
		//Location poroperties 
		//X location in Layer
		public int X { get; protected set; }
		//Y location in Layer
		public int Y { get; protected set; }
		//List of Cells in this COlumn
		public List<Cell> Cells { get; private set; }
		//Proximal segment (generic for now)
		public DendriteProximal ProximalDendrite { get; private set; }
		public DendriteApical ApicalDendrite { get; private set; }
		//input overlap value
		public double InputOverlap { get; private set; }
		//time average of InputOverlap
		public double InputOverlap_TimeAve { get; private set; }
		//True if Column is active.
		public bool IsActive { get; private set; }
		//True if Column is predicting.
		public bool IsPredicting { get; private set; }
		//Column boost value
		public double Boost { get; private set; }   //TODO

		//Column performance and statistical information
		public int ColumnActivationCount { get; set; }		//count Column activation every epoch

		#region Constructors

		//generic constructor
		public Column()
		{
			X = -1;
			Y = -1;
			Cells = new List<Cell>();
			ProximalDendrite = new DendriteProximal(NetConfigData.DendriteActivationThresholdProximal);
			ApicalDendrite = new DendriteApical(NetConfigData.DendriteActivationThresholdApical);
			InputOverlap = 0;
			InputOverlap_TimeAve = 0.0;
			Boost = Global.COLUMN_INITIAL_BOOST_VALUE;
			ColumnActivationCount = 0;
		}

		//added Layer reference for Cells construction to auto-create basal connections
		public Column(int x, int y, int numCellsInColumn) : this()
		{
			//store Cell's location in Layer
			X = x;
			Y = y;

			CreateCells(numCellsInColumn);
		}

		#endregion



		#region Public methods


		// Process all element functions.
		public void Update_Basal()
		{
			IsPredicting = false;

			foreach (Cell cell in Cells)
			{
				cell.Update();
				if (this.IsActive && cell.IsPredicting)
					IsPredicting = true;
		}
	}

	//update Column InputOverlap
	public void Update_Proximal()
		{
			InputOverlap = Boost * ProximalDendrite.Update();
			InputOverlap_TimeAve = Algebra.RunningAverage(InputOverlap_TimeAve, InputOverlap, Global.DEFAULT_TIME_AVERAGE_PERIOD);
		}

		//update Column IsActive
		//call after Update_Proximal
		//activates Column based on InputOverlap value and 
		//activity rank within the neighbourhood
		public int Update_Activation(List<Column> columns, bool inhibitionEnabled)
		{
			IsActive = false;

			if (columns.Count > 0)
			{
				//find neighbour Columns in top percentile
				List<Column> topPercentileList = FindWinningColumns(
					columns,
					NetConfigData.ColumnsTopPercentile,
					NetConfigData.ColumnStimulusThreshold);

				//Activate column only if it is on the list of top percentile columns
				//inhibition occurs here
				if (topPercentileList.Contains(this))
				{
					IsActive = true;
					ColumnActivationCount++;
				}

				if (IsActive && inhibitionEnabled)
				{
					//disable other Columns
					foreach (Column col in topPercentileList)
					{
						if(col != this)
							col.setInputOverlap(0);
					}
				}
			}
			return IsActive ? 1 : 0;
		}
		
		public bool CreateCells(int numCellsInColumn)
		{
			int numNewCells = numCellsInColumn - this.Cells.Count;

			while(this.Cells.Count < numCellsInColumn) //add cells
			{
					CreateCell(this.Cells.Count);
			}
			while (Cells.Count > 0 && Cells.Count > numCellsInColumn) //remove cells
			{
					RemoveCellAt(Global.rnd.Next(0, Cells.Count));
			}
			return true;
		}
		
		//creation wrappers handle any connection adjustments that may be neccessary
		//when adding/deleting elements
		public bool CreateCell(int index)
		{
			Cell cell = new Cell(index);
			Cells.Add(cell);
			return true;
		}

		public bool RemoveCell(Cell cell)
		{
			Cells.Remove(cell);
			return true;
		}

		public bool RemoveCellAt(int idx)
		{
			Cells.RemoveAt(idx);
			return true;
		}

		public List<Column> FindWinningColumns(List<Column> columns, double percentile, double stimulusThreshold)
		{
			columns.Sort((x, y) => x.InputOverlap.CompareTo(y.InputOverlap));
			int idx = Min((int)(Ceiling(columns.Count - (columns.Count * percentile))), columns.Count - 1);
			double percentileValue = columns[idx].InputOverlap;

			List<Column> result = (from col in columns
					  where col.InputOverlap >= percentileValue
					  && col.InputOverlap >=stimulusThreshold
					  select col).ToList();

			return result;
		}

		//create Proximal connections from this Column to Columns in Input Layer/Plane
		public void CreateProximalSynapses(Layer lr, Layer ip, double radius, double zoneCoveragePerc)
		{
			//scale layer locations	to InputPlane
			lr.MapPoint(X, Y, ip, out int scaledX, out int scaledY);

			//create random list of columns to connect - Inclusive of centre
			List<Column> potentialColumns = ip.GetColumnsFromCentre(scaledX, scaledY, radius, true);

			ProximalDendrite.CreateSynapses(potentialColumns, zoneCoveragePerc);
		}

		//create Basal connections from each Cell to other Columns in same Layer
		public void CreateBasalSynapses(Layer lr, double radius, double zoneCoveragePerc)
		{
			//create random list of columns to connect - Exclusive of centre
			List<Column> potentialColumns = lr.GetColumnsFromCentre(this.X, this.Y, radius, false);

			foreach (Cell cell in Cells)
				cell.CreateBasalSynapses(potentialColumns, zoneCoveragePerc);
		}

		internal void RemoveAllProximalSynapses()
		{
			ProximalDendrite.RemoveAllSynapses();
		}

		internal void RemoveBasalSynapses()
		{
			foreach (Cell cell in Cells)
				cell.RemoveAllBasalSynapses();
		}

		internal void RemoveApicalSynapses()
		{
			ApicalDendrite.RemoveAllSynapses();
		}

		public int CalcNumProximalSynapsesToCreate(Layer lr, InputPlane ip, double zoneSizePerc, double zoneCoveragePerc)
		{
			//calculate number of connections that will be created - Proximal
			double radius = ip.CalcRadius(zoneSizePerc);

			//scale between InputPlane and Layer location positions
			int scaledX, scaledY;
			lr.MapPoint(X, Y, ip, out scaledX, out scaledY);

			List<Column> potentialColumns = ip.GetColumnsFromCentre(scaledX, scaledY, radius, true);
			int numToConnect = (int)(potentialColumns.Count * zoneCoveragePerc);
			
			return numToConnect;
		}
		
		//calculate number of Basal connections to create 
		public int CalcNumBasalSynapsesToCreate(Layer lr, double zoneSizePerc, double zoneCoveragePerc)
		{
			double radius = lr.CalcRadius(zoneSizePerc);
			List<Column> potentialColumns = lr.GetColumnsFromCentre(this.X, this.Y, radius, false);
			int numToConnect = (int)(potentialColumns.Count * zoneCoveragePerc);
			return numToConnect;
		}
		
		public int CountBasalSynapses()
		{
			int cnt = 0;
			foreach (Cell cell in Cells)
				cnt += cell.CountBasalSynapses();

			return cnt;
		}

		public int CountProximalSynapses()
		{
			return ProximalDendrite.CountSynapses();
		}

		//get actual number of active Proximal connections to this column.
		public int CountActiveProximalSynapses()
		{
			return ProximalDendrite.CountActiveSynapses();
		}

		//get actual number of active Basal connections to this column.
		public int CountActiveBasalSynapses()
		{
			int cnt = 0;
			foreach (Cell cell in this.Cells)
				cnt += cell.CountActiveBasalSynapses();
			return cnt;
		}

		public void SetActive(bool active)
		{
			IsActive = active;
		}

		//Set cell IsActive - used when setting up training case in InputPlane
		public void SetCellsActiveState(bool active)
		{
			foreach (Cell cell in Cells)
				cell.SetActiveState(active);

			//override own elements
			IsActive = active;
		}

		public void SetBoost(double boost)
		{
			Boost = boost;
		}

		private void setInputOverlap(int inputOverlap)
		{
			InputOverlap = inputOverlap;
		}


#if TESTING

		//General rule for Override functions:
		//1. Override() method sets IsActive, IsPredicting etc.
		//2. DEEP Override() - alters all sub-elements so that Update() will give required results.


		public void OverrideActive(bool active, int depth)
		{
			IsActive = active;
			InputOverlap = NetConfigData.ColumnStimulusThreshold;
			if (depth < Global.OVERRIDE_DEPTH)
			{
				depth++;
				ProximalDendrite.Override(active, depth);

				foreach (Cell cell in Cells)
					cell.OverrideActive(active);
			}

		}

		public void OverridePredicting(bool predicting, int depth)
		{
			IsPredicting = predicting;
			if (depth < Global.OVERRIDE_DEPTH)
			{
				depth++;
				foreach (Cell cell in Cells)
					cell.OverridePredicting(predicting, depth);
			}

		}

		public void OverrideProximalPermanence(double permanence)
		{
			ProximalDendrite.OverridePermanence(permanence);
		}

		public void OverrideApicalPermanences(double permanence)
		{
			ApicalDendrite.OverridePermanence(permanence);
		}

		public void OverrideBasalPermanences(double permanence)
		{
			foreach (Cell cell in Cells)
				cell.BasalDendrite.OverridePermanence(permanence);
		}

		public void OverrideProximalDendriteActivationThreshold(int threshold)
		{
			ProximalDendrite.OverrideActivationThreshold(threshold);
		}

		public void OverrideApicalDendriteActivationThreshold(int threshold)
		{
			ApicalDendrite.OverrideActivationThreshold(threshold);
		}

		public void OverrideProximalInputOverlap(int inputOverlap)
		{
			InputOverlap = inputOverlap;
		}

#endif


		#endregion
	}
}


