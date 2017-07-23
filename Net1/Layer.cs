using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;
using System.Collections;
using Net1.RunningStats;


namespace Net1
{
	public class Layer
	{
		//Layer width - X
		private int _NumColumnsX;
		public int NumColumnsX
		{
			get { return _NumColumnsX; }
			set { _NumColumnsX = Max(1, value); }
		}
		//Layer height - Y
		private int _NumColumnsY;
		public int NumColumnsY
		{
			get { return _NumColumnsY; }
			set { _NumColumnsY = Max(1, value); }
		}
		//Layer Columns list array
		public List<List<Column>> Columns { get; private set; }
		//Number of cells in each Column in this Layer - adjustable property
		private int _NumCellsPerColumn;
		public int NumCellsInColumn
		{
			get { return _NumCellsPerColumn; }
			set { _NumCellsPerColumn = Max(1, Min(10, value)); }
		}

		//Layer total number of Columns 
		public int NumColumns { get; private set; }

		public double ZoneSizePercProximal { get; set; }       //zoneSizePerc as applied to Proximal connections to InputPlane
		public double ZoneCoveragePercProximal { get; set; }   //zoneCoveragePerc as applied to Proximal connections to InputPlane
		public double ZoneSizePercBasal { get; set; }          //zoneSizePerc as applied to Basal connections in Layer
		public double ZoneCoveragePercBasal { get; set; }      //zoneCoveragePerc as applied to Proximal connections inLayer	


		public bool InhibitionEnabled { get; set; }            //Used in testing only. Allows disable inhibition to obtain consistent training results.



		//Layer performance stats
		public RunningStat SparsenessStat { get; set; } //running measure of Activation Density (or Sparseness)
		public int SparsenessInterval { get; set; }		//adjust Activation Density every N epochs during training
		public double SparsenessTarget { get; set; }    //Activation Density target value for PID
		public double SparsenessActual { get; set; }    //Activation Density actual value for PID
		public ParamSearch ParamSearch_Sparseness_ZoneCoveragePercProximal { get; private set; } // Parameter Search control


		public RunningStat EntropyStat;					//running measure of Entropy



		#region Constructors

		public Layer(int numColumnsX, int numColumnsY, int numCellsInColumn)
		{
			NumColumnsX = numColumnsX;
			NumColumnsY = numColumnsY;
			NumCellsInColumn = numCellsInColumn;
			NumColumns = numColumnsX * numColumnsY;

			//default values from NetConfigData
			ZoneSizePercProximal = NetConfigData.ZoneSizePercProximal;
			ZoneCoveragePercProximal = NetConfigData.ZoneCoveragePercProximal;
			ZoneSizePercBasal = NetConfigData.ZoneSizePercBasal;
			ZoneCoveragePercBasal = NetConfigData.ZoneCoveragePercBasal;
			SparsenessInterval = NetConfigData.ActivationDensityInterval;
			SparsenessTarget = NetConfigData.ActivationDensityTarget;

			InhibitionEnabled = true;

			Columns = new List<List<Column>>();
			CreateColumns();

			//Layer running stats
			SparsenessStat = new RunningStat();
			EntropyStat = new RunningStat();

			//ZoneCoveragePercProximal parameter search
			//find optimal zone coverage to obtain minimum 2% Input Overlap
			ParamSearch_Sparseness_ZoneCoveragePercProximal = new ParamSearch(0.02, 0.0, 1.0, 0.001, 3, ParamSearchMode.Exact, ScreenUpdateData.SparsenessParamSearchEnable);

		}

		public Layer(Layer inputLayer, int numColumnsX, int numColumnsY, int numCellsInColumn)
			: this(numColumnsX, numColumnsY, numCellsInColumn)
		{
			CreateColumns();

		}

		#endregion


		#region Public methods

		public void Update()
		{
			double radius = CalcRadius(ZoneSizePercProximal);

			//Update InputOverlap for all Columns
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[x][y];
					col.Update_Proximal();
				}

			//Update IsActive for all Columns
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[x][y];
					List<Column> neighbours = GetColumnsFromCentre_WithThreshold(
						col.X, col.Y, radius, true, NetConfigData.ColumnStimulusThreshold);
					col.Update_Activation(neighbours, InhibitionEnabled);
				}

			//Update IsPredicting for all Columns
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[x][y];
					col.Update_Basal();
				}
		}

		public void AdjustSparseness(double desired)
		{
			SparsenessActual = SparsenessStat.MinVal();

			//ParameterSearch control
			ZoneCoveragePercProximal = ParamSearch_Sparseness_ZoneCoveragePercProximal.Update(SparsenessActual);

			NetConfigData.ZoneCoveragePercProximal = ZoneCoveragePercProximal;
		}

		public void AdjustBoostFactors()
		{
			double radius = CalcRadius(ZoneSizePercBasal);

			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[x][y];

					//get neighbourhood Columns
					List<Column> colList = GetColumnsFromCentre(x, y, radius, true);

					//neighbourhood average input overlap
					double InputOverlapSum = 0.0;
					foreach (Column c in colList)
						InputOverlapSum += c.InputOverlap_TimeAve;
					double InputOverlapAve = InputOverlapSum / colList.Count;

					//calc new Column Boost 
					double boost = Math.Pow(Math.E, -(Global.COLUMN_BOOST_ADJ_FACTOR * (col.InputOverlap_TimeAve - InputOverlapAve)));
					col.SetBoost(boost); 
				}

		}

		//create Column grid according to internal parameters NumColumnsX, NumColumnsY, NumCellsInColumn
		public void CreateColumns()
		{
			//X dimension
			while (Columns.Count < NumColumnsX) // add Columns in X dimension
			{
				List<Column> colList = new List<Column>();
				Columns.Add(colList);
			}
			while (Columns.Count > 0 && Columns.Count > NumColumnsX) // subtract Columns from X dimension
			{
				//remove last Column[] - to preserve column numbering
				Columns.RemoveAt(Columns.Count - 1);
			}

			int xIdx = 0;
			//Y dimension
			foreach (List<Column> colList in Columns)
			{
				while (colList.Count < NumColumnsY) // add Columns in Y dimension
				{
					Column col = new Column(xIdx, colList.Count, NumCellsInColumn);
					colList.Add(col);
				}
				while (colList.Count > 0 && colList.Count > NumColumnsY) // subtract Columns from Y dimension
				{
					//remove last Column - to preserve column numbering
					colList.RemoveAt(colList.Count - 1);
				}
				xIdx++;
			}

			//adjust number of Cells in Columns
			foreach (List<Column> colX in Columns)
			{
				foreach (Column col in colX)
				{
					col.CreateCells(NumCellsInColumn);
				}
			}

		}

		public void ConnectColumns(Layer ip)
		{
			connectProximal(ip);
			connectBasal();
		}

		//create Proximal Synapses to Columns in InputPlane
		//receptive area is InputPlane
		private void connectProximal(Layer ip)
		{
			if (ip != null)	//may be null during testing
			{
				//calculate radius from INPUT Layer/Plane diagonal dimension (to allow 1-dimensional layers) 
				double radius = ip.CalcRadius(ZoneSizePercProximal);

				for (int x = 0; x < NumColumnsX; x++)
					for (int y = 0; y < NumColumnsY; y++)
					{
						Column col = Columns[x][y];
						col.CreateProximalSynapses(this, ip, radius, ZoneCoveragePercProximal);
					}
			}
			else	//imput layer empty, remove all Synapses
			{
				for (int x = 0; x < NumColumnsX; x++)
					for (int y = 0; y < NumColumnsY; y++)
					{
						Column col = Columns[x][y];
						col.RemoveAllProximalSynapses();
					}
			}
		}

		//create Basal Synapses to other Columns in Layer
		//receptive area is this Layer
		private void connectBasal()
		{
			//calculate radius from THIS LAYER diagonal dimension (to allow 1-dimensional layers) 
			double radius = this.CalcRadius(ZoneSizePercBasal);

			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[x][y];
					col.CreateBasalSynapses(this, radius, ZoneCoveragePercBasal);
				}
		}

		//return list of Columns within given radius
		//radius is euclidean distance from centre point.
		//includeCenter - true includes the centre column (for connecting to InputPlane)
		//				  false does NOT include the centre column (for connecting Columns in the same Layer)
		public List<Column> GetColumnsFromCentre(int centreX, int centreY, double radius, bool includeCentre)
		{
			List<Column> result = new List<Column>();
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[x][y];
					double distance = Algebra.EuclideanDistance2D(centreX, centreY, col.X, col.Y);

					if (distance <= radius) 
						if (distance > 0 || includeCentre == true) //include? centre Column
							result.Add(col);
				}

			return result;
		}

		//return list of Columns within given radius and InputOverlap above given threshold
		//radius is euclidean distance from centre point.
		//includeCenter - true includes the centre column (for connecting to InputPlane)
		//				  false does NOT include the centre column (for connecting Columns in the same Layer)
		public List<Column> GetColumnsFromCentre_WithThreshold(int centreX, int centreY, double radius, bool includeCentre, int threshold)
		{
			List<Column> result = GetColumnsFromCentre(centreX, centreY, radius, includeCentre);
			result = (from col in result
						where col.InputOverlap >= threshold
						select col).ToList();

			return result;
		}
		
		/// <summary>
		/// Calculate radius based on Layer dimensions
		/// </summary>
		/// <param name="zoneSizePerc">0.0-1.0 percentage of layer larger dimension to use as radius.
		/// -1 means use infinite radius</param>
		/// <returns>int radius as % or Layer's larger dimension</returns>
		public double CalcRadius(double zoneSizePerc)
		{
			double radius = Max(Global.NEIGHBOURHOOD_RADIUS_MIN, 
								Sqrt(this.NumColumnsX * this.NumColumnsX + this.NumColumnsY * this.NumColumnsY) * zoneSizePerc);
			return radius;
		}

		private void calcMapping(Layer lr, out double scaleX, out double scaleY)
		{
			//scale between this and another Layer
			scaleX = lr == null ? 1.0 : ((double)lr.NumColumnsX / this.NumColumnsX);
			scaleY = lr == null ? 1.0 : ((double)lr.NumColumnsY / this.NumColumnsY);
		}

		public void MapPoint(int X, int Y, Layer lr, out int scaledX, out int scaledY)
		{
			double scaleX, scaleY;
			calcMapping(lr, out scaleX, out scaleY);
			//scale between this and the other Layer
			scaledX = (int)(X * scaleX);
			scaledY = (int)(Y * scaleY);

			//ensure point is inside target layer
			if (scaledX >= lr.NumColumnsX) scaledX = lr.NumColumnsX - 1;
			if (scaledY >= lr.NumColumnsY) scaledY = lr.NumColumnsY - 1;
		}

		//Clear all Columns states
		public void ClearColumns()
		{
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
					Columns[x][y].SetCellsActiveState(false);
		}

		//count number of active Columns
		public int CountActiveColumns()
		{
			var actCtr = 0;
			foreach(List<Column> colRow in Columns)
			{
				foreach (Column col in colRow)
					if (col.IsActive)
						actCtr++;
			}
			return actCtr;
		}

		public int CountActiveColumnsP()
		{
			int activeCtr = 0;
			Parallel.For(0, Columns.Count, i =>
			{
				List<Column> colRow = Columns[i];
				foreach (Column col in colRow)
					if (col.IsActive)
						Interlocked.Increment(ref activeCtr);
			});
			return activeCtr;
		}


		//count number of predicting Columns
		public int CountPredictingColumnsP()
		{
			int predictingCtr = 0;
			Parallel.For(0, Columns.Count, i =>
			{
				List<Column> colRow = Columns[i];
				foreach (Column col in colRow)
					if (col.IsPredicting)
						Interlocked.Increment(ref predictingCtr);
			});
			return predictingCtr;
		}

		public int CountPredictingColumns()
		{
			var actCtr = 0;
			foreach (List<Column> colRow in Columns)
			{
				foreach (Column col in colRow)
					if (col.IsPredicting)
						actCtr++;
			}
			return actCtr;
		}

		




#if TESTING


		//General rule for Override functions:
		//1. Override() method sets IsActive, IsPredicting etc.
		//2. DEEP Override() - alters all sub-elements so that Update() will give required results.

		public void Override(bool active, bool predicting)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
				{
					col.OverrideActive(active, 0);
					col.OverridePredicting(predicting, 0);
				}
		}

		public void OverrideActive(bool active)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
					col.OverrideActive(active, 0);
		}

		public void OverridePredicting(bool predicting)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
					col.OverridePredicting(predicting, 0);
		}

		public void OverrideProximalPermanences(double permanence)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
					col.OverrideProximalPermanence(permanence);
		}

		public void OverrideApicalPermanences(double permanence)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
					col.OverrideApicalPermanences(permanence);
		}

		public void OverrideBasalPermanences(double permanence)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
					col.OverrideBasalPermanences(permanence);
		}

		public void OverrideProximalDendriteActivationThreshold(int threshold)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
					col.OverrideProximalDendriteActivationThreshold(threshold);
		}

		public void OverrideApicalDendriteActivationThreshold(int threshold)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
					col.OverrideApicalDendriteActivationThreshold(threshold);
		}

		public void OverrideBasalDendriteActivationThreshold(int threshold)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
					foreach (Cell cell in col.Cells)
						cell.OverrideBasalDendriteActivationThreshold(threshold);
		}

		public void OverrideProximalInputOverlap(int inputOverlap)
		{
			foreach (List<Column> colRow in Columns)
				foreach (Column col in colRow)
					col.OverrideProximalInputOverlap(inputOverlap);
		}

		public void PrintActive()
		{
			Debug.WriteLine("Layer " + NumColumnsX + ", " + NumColumnsY);
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
					Debug.WriteLine("  " + x + ", " + y + " IsActive=" + Columns[x][y].IsActive.ToString());
		}

#endif



		#endregion
	}
}
