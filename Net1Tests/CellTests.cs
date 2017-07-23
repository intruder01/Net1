using Microsoft.VisualStudio.TestTools.UnitTesting;
using Net1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

#if TESTING

namespace Net1.Tests
{
	[TestClass()]
	public class CellTests
	{
		[TestMethod()]
		public void CellCtorTest()
		{
			Cell c = new Cell(0);
			Assert.IsFalse(c.IsActive);
			Assert.IsFalse(c.IsPredicting);
			Assert.IsNotNull(c.BasalDendrite);
		}


		[TestMethod()]
		public void CellUpdateTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			//override global params
			NetConfigData.ColumnsTopPercentile = 0;
			NetConfigData.ColumnStimulusThreshold = 1;
			NetConfigData.SynapsePermanenceIncrease = 0.0;
			NetConfigData.SynapsePermanenceDecrease = 0.0;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Debug.WriteLine("CellUpdateTest testNum=" + testNum.ToString());

				//Layer
				int layerColumnsX = rnd.Next(1, 10);
				int layerColumnsY = layerColumnsX > 1 ? rnd.Next(1, 10) : rnd.Next(2, 10);
				int layerNumCellsInColumn = rnd.Next(1, 5);
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);
				int lrNumColumns = lr.NumColumnsX * lr.NumColumnsY;

				// create synapses
				//lr.ConnectBasal(1, 1);
				lr.ConnectColumns(null);

				lr.Override(false, false);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsFalse(col.IsPredicting);
						}
					}

				lr.Override(false, true);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsTrue(col.IsPredicting);
						}
					}

				lr.Override(true, false);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsFalse(col.IsPredicting);
						}
					}

				lr.Override(true, true);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsTrue(col.IsPredicting);
						}
					}

				//decrease Basal permanences
				lr.Override(false, false);
				lr.OverrideBasalPermanences(NetConfigData.SynapsePermanenceThreshold - 2* NetConfigData.SynapsePermanenceIncrease);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after first update
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after second update
						}
					}
				NetConfigData.SynapsePermanenceIncrease = 0.01;
				NetConfigData.SynapsePermanenceDecrease = 0.01;
				lr.Override(false, true);
				lr.OverrideBasalPermanences(NetConfigData.SynapsePermanenceThreshold - 2 * NetConfigData.SynapsePermanenceIncrease);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after first update
							cell.Update();
							Assert.IsTrue(cell.IsPredicting);  //true after second update
						}
					}
				lr.Override(true, false);
				lr.OverrideBasalPermanences(NetConfigData.SynapsePermanenceThreshold - 2 * NetConfigData.SynapsePermanenceIncrease);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after first update
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after second update
						}
					}

				lr.Override(true, true);
				lr.OverrideBasalPermanences(NetConfigData.SynapsePermanenceThreshold - 2 * NetConfigData.SynapsePermanenceIncrease);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after first update
							cell.Update();
							Assert.IsTrue(cell.IsPredicting);  //true after second update
						}
					}

				//increase Basal permanences
				lr.Override(false, false);
				lr.OverrideBasalPermanences(NetConfigData.SynapsePermanenceThreshold);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after first update
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after second update
						}
					}
				lr.Override(false, true);
				lr.OverrideBasalPermanences(NetConfigData.SynapsePermanenceThreshold);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsTrue(cell.IsPredicting);  //true after first update
							cell.Update();
							Assert.IsTrue(cell.IsPredicting);  //true after second update  
						}
					}
				lr.Override(true, false);
				lr.OverrideBasalPermanences(NetConfigData.SynapsePermanenceThreshold);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after first update
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after second update
						}
					}

				lr.Override(true, true);
				lr.OverrideBasalPermanences(NetConfigData.SynapsePermanenceThreshold);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.Update();
							Assert.IsTrue(cell.IsPredicting);  //true after first update
							cell.Update();
							Assert.IsTrue(cell.IsPredicting);  //true after second update
						}
					}

				//need to implement following tests:
				//
				//1. col Active 0  Predicting 0
				//2. col Active 0  Predicting 1
				//3. col Active 1  Predicting 0
				//4. col Active 1  Predicting 1
				//
				//with following variants:
				//
				//a. due to EXTERNAL factors
				//
				// Predicting	- own Layer Predicting 0 
				//
				//b. due to INTERNAL factors
				//
				//
				// Predicting	- BasalDendrite
				//					- due to number of BasalSynapses to own Layer columns
				//					- due to BasalSynapses Permanence values



				// Predicting	- own Layer Predicting 0 
				lr.OverrideActive(true);
				lr.OverridePredicting(true);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.BasalDendrite.OverrideActivationThreshold(Global.DENDRITE_INITIAL_ACTIVATION_THRESHOLD + 1);
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
								syn.ColumnConnected.OverridePredicting(false, 0);
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after first update
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after second update
						}
					}
				lr.OverrideBasalDendriteActivationThreshold(1);

				// Predicting	- BasalDendrite
				//					- due to number of BasalSynapses to own Layer columns

				lr.OverrideActive(true);
				lr.OverridePredicting(true);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							//require X synapses active to activate
							cell.BasalDendrite.OverrideActivationThreshold(cell.BasalDendrite.Synapses.Count - rnd.Next(1, 10));
							//remove snapses until less than threshold
							while (cell.BasalDendrite.Synapses.Count >= cell.BasalDendrite.ActivationThreshold)
							{
								SynapseBasal syn = cell.BasalDendrite.Synapses[0];
								cell.BasalDendrite.Synapses.Remove(syn);
							}
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after first update
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after second update
						}
					}

				//lr.ConnectBasal(1, 1);
				lr.ConnectColumns(null);
				lr.OverrideBasalDendriteActivationThreshold(1);

				// Predicting	- BasalDendrite
				//					- due to BasalSynapses Permanence values
				lr.OverrideActive(true);
				lr.OverridePredicting(true);
				for (int x = 0; x < lr.NumColumnsX; x++)
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Column col = lr.Columns[x][y];
						foreach (Cell cell in col.Cells)
						{
							cell.BasalDendrite.OverridePermanence(NetConfigData.SynapsePermanenceThreshold - 2 * NetConfigData.SynapsePermanenceIncrease);
							cell.Update();
							Assert.IsFalse(cell.IsPredicting);  //false after first update
							cell.Update();
							Assert.IsTrue(cell.IsPredicting);  //true after second update
						}
					}

			}

			//restore global params
			NetConfigData.SetDefaults();
		}

		[TestMethod()]
		public void CellCreateBasalConnectionsTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Debug.WriteLine("CellCreateBasalConnectionsTest testNum=" + testNum.ToString());

				//Layer
				int LayerColumnsX = rnd.Next(1, 20);
				int layerColumnsY = rnd.Next(1, 20);
				int layerNumCellsInColumn = rnd.Next(1, 5);
				Layer lr = new Layer(LayerColumnsX, layerColumnsY, layerNumCellsInColumn);
				int numColumns = lr.NumColumnsX * lr.NumColumnsY;


				//select random Column in Layer
				int columnX = rnd.Next(0, lr.NumColumnsX);
				int columnY = rnd.Next(0, lr.NumColumnsY);
				Column column = lr.Columns[columnX][columnY];

				//random % coverage
				double zoneSizePerc = rnd.NextDouble();
				double zoneCoveragePerc = rnd.NextDouble();

				lr.ZoneSizePercProximal = zoneSizePerc;
				lr.ZoneCoveragePercProximal = zoneCoveragePerc;
				lr.ZoneSizePercBasal = zoneSizePerc;
				lr.ZoneCoveragePercBasal = zoneCoveragePerc;

				double radius = lr.CalcRadius(zoneSizePerc);

				//for each column, count unique connected columns
				List<Column> PotentialColumnsList = new List<Column>(); //list of columns within radius (potential connections)
				List<Column> ConnectedColumnsList = new List<Column>(); //list of columns connected to (actual connections)

				//calculate number of connections that will be created
				PotentialColumnsList = lr.GetColumnsFromCentre(column.X, column.Y, radius, false);
				int numToConnect = (int)(PotentialColumnsList.Count * zoneCoveragePerc);
				
				foreach (Cell cell in column.Cells)
				{
					ConnectedColumnsList.Clear();
					cell.CreateBasalSynapses(PotentialColumnsList, zoneCoveragePerc);
					int numConnected = cell.CountBasalSynapses();

					foreach (Synapse syn in cell.BasalDendrite.Synapses)
					{
						Column cc = syn.ColumnConnected;
						Assert.IsNotNull(cc);
						Assert.AreNotSame(column, cc); //check column not connected to itself
						double distance = Algebra.EuclideanDistance2D(cc.X, cc.Y, column.X, column.Y);
						Assert.IsTrue(distance <= radius);
						Assert.IsTrue(distance > 0);

						//add unique connected columns to list to obtain 
						//connected columns counter for each synapse
						bool columnOnList = false;
						foreach (Column connectedColumn in ConnectedColumnsList)
							if (connectedColumn == cc)
								columnOnList = true;
						if (!columnOnList)
							ConnectedColumnsList.Add(cc);
					}
					//check correct # columns connected to each Cell
					Assert.AreEqual(ConnectedColumnsList.Count, numToConnect);
					Assert.AreEqual(ConnectedColumnsList.Count, numConnected);
					Assert.AreEqual(numConnected, numToConnect);


				}
			}
		}

		
	}
}

#endif