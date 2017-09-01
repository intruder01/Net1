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
using Net1.Stats;
using System.IO;

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


		public RunningStat EntropyStat;                 //running measure of Entropy

		//Prediction reconstruction - commented out for this commit //20170818-1

		/// <param name="numColumnsX"></param>
		/// <param name="numColumnsY"></param>
		/// <param name="numCellsInColumn"></param>
		////// 20160109-1
		////// declare as property since it may be accessed by other viewer classes
		////private float[,] predictionReconstruction;
		////public float[,] PredictionReconstruction
		////{
		////	get { return predictionReconstruction; }
		////	set { predictionReconstruction = value; }
		////}



		#region Constructors

		public Layer (int numColumnsX, int numColumnsY, int numCellsInColumn)
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

			//// 20160109-1
			//// Allocate input reconstruction array
			//this.PredictionReconstruction = new float[0, 0]; //20170818-1

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
			//Update InputOverlap for all Columns
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[y][x];

					//reset column state and get ready for another pass
					col.SetInhibited(false);

					//process proximal input
					col.Update_Proximal();
				}

			//Update IsActive for all Columns
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[y][x];
					List<Column> neighbours = GetColumnsFromCentre_WithThreshold(
						col.X, col.Y, ZoneSizePercProximal, true, NetConfigData.ColumnStimulusThreshold);
					col.Update_Activation(neighbours, InhibitionEnabled);
				}

			//Update IsPredicting for all Columns
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[y][x];
					col.Update_Basal();
				}
		}

		public void AdjustSparseness(double desired)
		{
			SparsenessActual = SparsenessStat.MinVal();

			//ParameterSearch control
			if( ParamSearch_Sparseness_ZoneCoveragePercProximal.Enabled )
				ZoneCoveragePercProximal = ParamSearch_Sparseness_ZoneCoveragePercProximal.Update(SparsenessActual);

			NetConfigData.ZoneCoveragePercProximal = ZoneCoveragePercProximal;
		}

		public void AdjustBoostFactors()
		{
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[y][x];

					//get neighbourhood Columns
					List<Column> colList = GetColumnsFromCentre(x, y, ZoneSizePercBasal, true);

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


		//Create Column grid according to internal parameters NumColumnsX, NumColumnsY, NumCellsInColumn
		//Layer row/column structure:
		//
		//NumColumnsX = 10
		//NumColumnsY = 5
		//
		// 0 1 2 3 4 5 6 7 8 9
		// 0 1 2 3 4 5 6 7 8 9
		// 0 1 2 3 4 5 6 7 8 9
		// 0 1 2 3 4 5 6 7 8 9
		// 0 1 2 3 4 5 6 7 8 9
		//


		/// <summary>
		/// Dynamically adjust number of Columns in Layer
		/// Add and remove Columns as necessary.
		/// When removing - remove from the end of list to preserve indexes.
		/// </summary> 
		public void CreateColumns ()
		{
			//Y dimension - add rows (of Column)
			while ( Columns.Count < NumColumnsY ) // add rows 
			{
				List<Column> row = new List<Column> ();
				Columns.Add ( row );
			}
			while ( Columns.Count > 0 && Columns.Count > NumColumnsY ) // subtract rows 
			{
				//remove last row (List<Column>) - to preserve column numbering
				Columns.RemoveAt ( Columns.Count - 1 );
			}

			
			//X dimension - add columns (of Column)
			int xIdx= 0;
			foreach ( List<Column> row in Columns )	//for each row 
			{
				while ( row.Count < NumColumnsX ) // add Columns 
				{
					Column column = new Column ( row.Count, xIdx, NumCellsInColumn );
					row.Add ( column );
				}
				while ( row.Count > 0 && row.Count > NumColumnsX ) // subtract Columns 
				{
					//remove last Column - to preserve column numbering
					row.RemoveAt ( row.Count - 1 );
				}
				xIdx++;
			}

			//adjust number of Cells in Columns
			foreach ( List<Column> row in Columns )
			{
				foreach ( Column column in row )
				{
					column.CreateCells ( NumCellsInColumn );
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
				for (int x = 0; x < NumColumnsX; x++)
					for (int y = 0; y < NumColumnsY; y++)
					{
						Column col = Columns[y][x];
						col.CreateProximalSynapses(this, ip, ZoneSizePercProximal, ZoneCoveragePercProximal);
					}

				//// Allocate input prediction reconstruction 
				//PredictionReconstruction = new float[ip.NumColumnsX, ip.NumColumnsY]; //20170818-1
			}
			else	//input layer empty, remove all Synapses
			{
				for (int x = 0; x < NumColumnsX; x++)
					for (int y = 0; y < NumColumnsY; y++)
					{
						Column col = Columns[y][x];
						col.RemoveAllProximalSynapses();
					}
			}
		}

		//create Basal Synapses to other Columns in Layer
		//receptive area is this Layer
		private void connectBasal()
		{
			for (int x = 0; x < NumColumnsX; x++)
				for (int y = 0; y < NumColumnsY; y++)
				{
					Column col = Columns[y][x];
					col.CreateBasalSynapses(this, ZoneSizePercBasal, ZoneCoveragePercBasal);
				}
		}

		//return list of Columns within given zone
		//includeCenter - true includes the centre column (for connecting to InputPlane)
		//				  false does NOT include the centre column (for connecting Columns in the same Layer)
		public List<Column> GetColumnsFromCentre(int centreX, int centreY, double zoneSizePerc, bool includeCentre)
		{
			//centreX = 0;
			//centreY = 0;
			//zoneSizePerc = 0.20;
			//includeCentre = true;

			//subtract 1 when centre not included
			int numToCreate = (int)((double)NumColumnsX * (double)NumColumnsY * zoneSizePerc - ( includeCentre ? 0 : 1 ) );

			//StreamWriter file = new StreamWriter ( "AAA log.txt", true );
			//using ( file )
			//{
			//	file.WriteLine ( "GetColumnsFromCentre() includeCentre {0}", includeCentre );
			//	file.WriteLine ( "   NumColumnsX  {0}    NumColumnsY   {1}", NumColumnsX, NumColumnsY );
			//	file.WriteLine ( "   centreX	     {0}    centreY       {1}", centreX, centreY );
			//	file.WriteLine ( "   zoneSizePerc {0:F2} numToCreate   {1}", zoneSizePerc, numToCreate );
			//	file.Close ();
			//}

			//find rectangular zone dimensions that gives minimum that many elements
			int zoneWidth = 0;
			int zoneHeight = 0;
			bool alternate = false;
			List<Column> result = new List<Column>();
						
			//subtract 1 when centre not included
			//this will result in a larger rect zone if necessary
			while ( zoneWidth * zoneHeight - ( includeCentre ? 0 : 1 ) < numToCreate )
			{
				alternate = !alternate;
				if ( alternate )
				{
					if ( zoneWidth < NumColumnsX )
						zoneWidth++;
				}
				else
				{
					if ( zoneHeight < NumColumnsY )
						zoneHeight++;
				}
			}

			int zoneLeft = Math.Max ( Math.Min(NumColumnsX - zoneWidth, centreX - zoneWidth / 2), 0 );
			int zoneRight = Math.Min ( zoneLeft + zoneWidth - 1, NumColumnsX - 1 );
			int zoneTop = Math.Max(Math.Min(NumColumnsY - zoneHeight, centreY - zoneHeight / 2), 0);
			int zoneBottom = Math.Min ( zoneTop + zoneHeight - 1, NumColumnsY - 1 );

			for ( int x = zoneLeft; x <= zoneRight; x++ )
			{
				for ( int y = zoneTop; y <= zoneBottom; y++ )
				{
					Column col = Columns[y][x];

					if ( includeCentre || !( col.X == centreX && col.Y == centreY ) ) //include? centre Column
					{
						result.Add ( col );
					}

					//exit once correct number created
					if ( result.Count >= numToCreate )
						return result;
				}
			}

			return result;
		}

		//return list of Columns within given zone and InputOverlap above given threshold
		//includeCenter - true includes the centre column (for connecting to InputPlane)
		//				  false does NOT include the centre column (for connecting Columns in the same Layer)
		public List<Column> GetColumnsFromCentre_WithThreshold(int centreX, int centreY, double zoneSizePerc, bool includeCentre, int threshold)
		{
			List<Column> result = GetColumnsFromCentre(centreX, centreY, zoneSizePerc, includeCentre);
			result = (from col in result
						where col.InputOverlap >= threshold
						select col).ToList();

			return result;
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
					Columns[y][x].SetCellsActiveState(false);
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
					Debug.WriteLine("  " + x + ", " + y + " IsActive=" + Columns[y][x].IsActive.ToString());
		}

#endif



		#endregion
	}
}
