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
	public class LayerTests
	{
		[TestMethod()]
		public void LayerCtorTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				int numColumnsX = rnd.Next(1, 20);
				int numColumnsY = rnd.Next(1, 20);
				int numCellsInColumn = rnd.Next(1, 10);

				Layer lr = new Layer(numColumnsX, numColumnsY, numCellsInColumn);
				Assert.AreEqual(lr.NumColumnsX, numColumnsX);
				Assert.AreEqual(lr.NumColumnsY, numColumnsY);
				Assert.AreEqual(lr.NumCellsInColumn, numCellsInColumn);

				Assert.IsNotNull(lr.Columns);
				Assert.AreEqual(lr.Columns.Count, numColumnsY);

				for (int x = 0; x < lr.NumColumnsX; x++)
				{
					for (int y = 0; y < lr.NumColumnsY; y++)
					{
						Assert.AreEqual(lr.Columns[y].Count, numColumnsX);
						Column col = lr.Columns[y][x];
						Assert.AreEqual(col.Cells.Count, numCellsInColumn);

						Assert.AreEqual(col.X, x);
						Assert.AreEqual(col.Y, y);
					}
				}
			}
		}

		[TestMethod()]
		public void LayerUpdateTest()
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
				Debug.WriteLine("LayerUpdateTest testNum=" + testNum.ToString());

				//Layer
				int layerColumnsX = rnd.Next(1, 10);
				//layerColumnsX = 2;////////////////////
				//minimum 2 columns neeed for testing basal updates
				int layerColumnsY = layerColumnsX != 1 ? layerColumnsY = rnd.Next(1, 10) 
														: layerColumnsY = rnd.Next(1, 10);
				//layerColumnsY = 4;////////////////////
				int layerNumCellsInColumn = rnd.Next(1, 5);
				layerNumCellsInColumn = 2;
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);

				//InputPlane
				int ipColumnsX = rnd.Next(1, 20);
				//ipColumnsX = 1;///////////////////////
				int ipColumnsY = rnd.Next(1, 20);
				//ipColumnsY = 2;/////////////////////
				InputPlane ip = new InputPlane(ipColumnsX, ipColumnsY);
				ip.OverrideActive(true);

				// create synapses
				double zoneSizePerc = rnd.NextDouble();
				//zoneSizePerc = 0.625;/////////////////////
				double zoneCoveragePerc = rnd.NextDouble();
				//zoneCoveragePerc = 0.600;///////////////////

				lr.ZoneSizePercProximal = zoneSizePerc;
				lr.ZoneCoveragePercProximal = zoneCoveragePerc;
				lr.ZoneSizePercBasal = zoneSizePerc;
				lr.ZoneCoveragePercBasal = zoneCoveragePerc;

				//create connections
				lr.ConnectColumns(ip);

				//override synapses
				lr.OverrideBasalPermanences(1.0);
				lr.OverrideProximalPermanences(1.0);
				
				//override activation thresholds
				lr.OverrideProximalDendriteActivationThreshold(1);
				lr.OverrideBasalDendriteActivationThreshold(1);

				int numProximalSynapses = 0;
				int numBasalSynapses = 0;

				
				//with permanences = 1.0
				lr.Override(false, false);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, true);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);	//can't be predictive without being active
					}

				lr.Override(true, false);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						//numProximalSynapses can be 0 depending on zoneSizePerc and zoneCoveragePerc
						numProximalSynapses = column.CountActiveProximalSynapses();
						//if (numProximalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsActive);
						//}
						//else
						//	Assert.IsFalse(column.IsActive);

						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(true, true);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						////numProximalSynapses can be 0 depending on zoneSizePerc and zoneCoveragePerc
						//numProximalSynapses = column.CountActiveProximalSynapses();
						//if (numProximalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsActive);
						//}
						//else
						//	Assert.IsFalse(column.IsActive);

						numBasalSynapses = column.CountActiveBasalSynapses();
						if (numBasalSynapses > 0 && column.IsActive)
							Assert.IsTrue(column.IsPredicting);
						else
							Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, false);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				//change Proximal permanences
				lr.Override(false, false);
				lr.OverrideProximalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, true);
				lr.OverrideProximalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);

						//numBasalSynapses = column.CountActiveBasalSynapses();
						//if (numBasalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsPredicting);
						//}
						//else
						//	Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(true, false);
				lr.OverrideProximalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						//numProximalSynapses can be 0 depending on zoneSizePerc and zoneCoveragePerc
						//numProximalSynapses = column.CountActiveProximalSynapses();
						//if (numProximalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsActive);
						//}
						//else
						//	Assert.IsFalse(column.IsActive);

						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(true, true);
				lr.OverrideProximalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);

						//numBasalSynapses = column.CountActiveBasalSynapses();
						//if (numBasalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsPredicting);
						//}
						//else
						//	Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, false);
				lr.OverrideProximalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				//override Basal permanences
				lr.Override(false, false);
				lr.OverrideBasalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, true);
				lr.OverrideBasalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(true, false);
				lr.OverrideBasalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						//numProximalSynapses can be 0 depending on zoneSizePerc and zoneCoveragePerc
						//numProximalSynapses = column.CountActiveProximalSynapses();
						//if (numProximalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsActive);
						//}
						//else
						//	Assert.IsFalse(column.IsActive);

						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(true, true);
				lr.OverrideBasalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						//numProximalSynapses can be 0 depending on zoneSizePerc and zoneCoveragePerc
						//numProximalSynapses = column.CountActiveProximalSynapses();
						//if (numProximalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsActive);
						//}
						//else
						//	Assert.IsFalse(column.IsActive);

						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, false);
				lr.OverrideBasalPermanences(0.0);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				//change InputPlane active = false
				lr.Override(false, false);
				ip.OverrideActive(false);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, true);
				ip.OverrideActive(false);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);

						Assert.IsFalse(column.IsPredicting);

						//numBasalSynapses = column.CountActiveBasalSynapses();
						//if (numBasalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsPredicting);
						//}
						//else
						//	Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(true, false);
				ip.OverrideActive(false);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(true, true);
				ip.OverrideActive(false);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);

						//numBasalSynapses = column.CountActiveBasalSynapses();
						//if (numBasalSynapses > 0)
						//	Assert.IsTrue(column.IsPredicting);
						//else
						//	Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, false);
				ip.OverrideActive(false);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				//override InputPlane active = true
				lr.Override(false, false);
				ip.OverrideActive(true);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, true);
				ip.OverrideActive(true);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);

						//numBasalSynapses = column.CountActiveBasalSynapses();
						//if (numBasalSynapses > 0)
						//	Assert.IsTrue(column.IsPredicting);
						//else
						//	Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(true, false);
				ip.OverrideActive(true);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						//numProximalSynapses = column.CountActiveProximalSynapses();
						//if (numProximalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsActive);
						//}
						//else
						//	Assert.IsFalse(column.IsActive);

						Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(true, true);
				ip.OverrideActive(true);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						//numProximalSynapses can be 0 depending on zoneSizePerc and zoneCoveragePerc
						//numProximalSynapses = column.CountActiveProximalSynapses();
						//if (numProximalSynapses > 0)
						//{
						//	Assert.IsTrue(column.IsActive);
						//}
						//else
						//	Assert.IsFalse(column.IsActive);

						//numBasalSynapses = column.CountActiveBasalSynapses();
						//if (numBasalSynapses > 0 && column.IsActive)
						//	Assert.IsTrue(column.IsPredicting);
						//else
						//	Assert.IsFalse(column.IsPredicting);
					}

				lr.Override(false, false);
				ip.OverrideActive(true);
				lr.Update();
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						numProximalSynapses = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);
						Assert.AreEqual(column.ProximalDendrite.Synapses.Count, numProximalSynapses);
						numBasalSynapses = column.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						foreach (Cell cell in column.Cells)
							Assert.AreEqual(cell.BasalDendrite.Synapses.Count, numBasalSynapses);

						Assert.IsFalse(column.IsActive);
						Assert.IsFalse(column.IsPredicting);
					}
			}

			//restore global params
			NetConfigData.SetDefaults();
		}

		[TestMethod ()]
		public void LayerOverrideTest ()
		{
			Random rnd = Global.rnd;

			for ( int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++ )
			{
				//Layer
				int numColumnsX = rnd.Next ( 1, 10 );
				int numColumnsY = rnd.Next ( 1, 10 );
				int numCellsInCol = rnd.Next ( 1, 5 );

				Layer lr = new Layer ( numColumnsX, numColumnsY, numCellsInCol );
				Assert.IsNotNull ( lr.Columns );
				Assert.AreEqual ( lr.Columns.Count, numColumnsY );
				Assert.AreEqual ( lr.Columns[0].Count, numColumnsX );

				//InputPlane
				int ipNumColumnsX = rnd.Next ( 1, 20 );
				int ipNumColumnsY = rnd.Next ( 1, 20 );
				InputPlane ip = new InputPlane ( ipNumColumnsX, ipNumColumnsY );
				ip.OverrideActive ( true );
				
				//create synapses
				//lr.ConnectProximal(ip);
				//lr.ConnectBasal(1, 1);
				lr.ConnectColumns ( ip );

				//with Permanence = 1.0
				lr.OverrideBasalPermanences ( 1.0 );
				lr.OverrideProximalPermanences ( 1.0 );

				lr.Override ( false, false );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsFalse ( column.IsActive );
						//test deep override - active
						Assert.IsFalse ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsFalse ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 0.0 );
						}
						Assert.IsFalse ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsFalse ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsFalse ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 0.0 );
							}
						}
					}

				lr.Override ( false, true );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsFalse ( column.IsActive );
						//test deep override - active
						Assert.IsFalse ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsFalse ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 0.0 );
						}
						Assert.IsTrue ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsTrue ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsTrue ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 1.0 );
							}
						}
					}

				lr.Override ( true, false );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsTrue ( column.IsActive );
						//test deep override - active
						Assert.IsTrue ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsTrue ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 1.0 );
						}
						Assert.IsFalse ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsFalse ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsFalse ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 0.0 );
							}
						}
					}

				lr.Override ( true, true );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsTrue ( column.IsActive );
						//test deep override - active
						Assert.IsTrue ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsTrue ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 1.0 );
						}
						Assert.IsTrue ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsTrue ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsTrue ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 1.0 );
							}
						}
					}

				lr.Override ( false, false );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsFalse ( column.IsActive );
						//test deep override - active
						Assert.IsFalse ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsFalse ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 0.0 );
						}
						Assert.IsFalse ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsFalse ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsFalse ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 0.0 );
							}
						}
					}

				//with Permanence = 0.0
				lr.OverrideBasalPermanences ( 0.0 );
				lr.OverrideProximalPermanences ( 0.0 );

				lr.Override ( false, false );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsFalse ( column.IsActive );
						//test deep override - active
						Assert.IsFalse ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsFalse ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 0.0 );
						}
						Assert.IsFalse ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsFalse ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsFalse ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 0.0 );
							}
						}
					}

				lr.Override ( false, true );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsFalse ( column.IsActive );
						//test deep override - active
						Assert.IsFalse ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsFalse ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 0.0 );
						}
						Assert.IsTrue ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsTrue ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsTrue ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 1.0 );
							}
						}
					}

				lr.Override ( true, false );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsTrue ( column.IsActive );
						//test deep override - active
						Assert.IsTrue ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsTrue ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 1.0 );
						}
						Assert.IsFalse ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsFalse ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsFalse ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 0.0 );
							}
						}
					}

				lr.Override ( true, true );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsTrue ( column.IsActive );
						//test deep override - active
						Assert.IsTrue ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsTrue ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 1.0 );
						}
						Assert.IsTrue ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsTrue ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsTrue ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 1.0 );
							}
						}
					}

				lr.Override ( false, false );
				foreach ( List<Column> colRow in lr.Columns )
					foreach ( Column column in colRow )
					{
						Assert.IsFalse ( column.IsActive );
						//test deep override - active
						Assert.IsFalse ( column.ProximalDendrite.IsActive );
						foreach ( SynapseProximal syn in column.ProximalDendrite.Synapses )
						{
							Assert.IsFalse ( syn.IsActive );
							Assert.IsTrue ( syn.Permanence == 0.0 );
						}
						Assert.IsFalse ( column.IsPredicting );
						//test deep override
						foreach ( Cell cell in column.Cells )
						{

							Assert.IsFalse ( cell.BasalDendrite.IsActive );
							foreach ( SynapseBasal syn in cell.BasalDendrite.Synapses )
							{
								Assert.IsFalse ( syn.IsActive );
								Assert.IsTrue ( syn.Permanence == 0.0 );
							}
						}
					}
			}
		}




		[TestMethod()]
		public void LayerOverrideActiveTest()
		{
			Random rnd = Global.rnd;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				//Layer
				int numColumnsX = rnd.Next(1, 10);
				int numColumnsY = rnd.Next(1, 10);
				int numCellsInCol = rnd.Next(1, 5);

				Layer lr = new Layer(numColumnsX, numColumnsY, numCellsInCol);
				Assert.IsNotNull(lr.Columns);
				Assert.AreEqual(lr.Columns.Count, numColumnsY);
				Assert.AreEqual(lr.Columns[0].Count, numColumnsX);

				//InputPlane
				int ipNnumColumnsX = rnd.Next(1, 20);
				int ipNumColumnsY = rnd.Next(1, 20);
				InputPlane ip = new InputPlane(ipNnumColumnsX, ipNumColumnsY);
				ip.OverrideActive(true);

				//create synapses
				//lr.ConnectProximal(ip, 1, 1);
				lr.ConnectColumns(ip);

				//with Permanence = 1.0
				lr.OverrideProximalPermanences(1.0);

				lr.OverrideActive(false);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsFalse(column.IsActive);
						//test deep override
						Assert.IsFalse(column.ProximalDendrite.IsActive);
						foreach (SynapseProximal syn in column.ProximalDendrite.Synapses)
						{
							Assert.IsFalse(syn.IsActive);
							Assert.IsTrue(syn.Permanence == 0.0);
						}
					}

				lr.OverrideActive(true);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsTrue(column.IsActive);
						//test deep override
						Assert.IsTrue(column.ProximalDendrite.IsActive);
						foreach (SynapseProximal syn in column.ProximalDendrite.Synapses)
						{
							Assert.IsTrue(syn.IsActive);
							Assert.IsTrue(syn.Permanence == 1.0);
						}
					}

				lr.OverrideActive(false);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsFalse(column.IsActive);
						//test deep override
						Assert.IsFalse(column.ProximalDendrite.IsActive);
						foreach (SynapseProximal syn in column.ProximalDendrite.Synapses)
						{
							Assert.IsFalse(syn.IsActive);
							Assert.IsTrue(syn.Permanence == 0.0);
						}
					}

				lr.OverrideActive(true);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsTrue(column.IsActive);
						//test deep override
						Assert.IsTrue(column.ProximalDendrite.IsActive);
						foreach (SynapseProximal syn in column.ProximalDendrite.Synapses)
						{
							Assert.IsTrue(syn.IsActive);
							Assert.IsTrue(syn.Permanence == 1.0);
						}
					}

				//with Permanence = 0.0
				lr.OverrideProximalPermanences(0.0);

				lr.OverrideActive(false);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsFalse(column.IsActive);
						//test deep override
						Assert.IsFalse(column.ProximalDendrite.IsActive);
						foreach (SynapseProximal syn in column.ProximalDendrite.Synapses)
						{
							Assert.IsFalse(syn.IsActive);
							Assert.IsTrue(syn.Permanence == 0.0);
						}
					}

				lr.OverrideActive(true);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsTrue(column.IsActive);
						//test deep override
						Assert.IsTrue(column.ProximalDendrite.IsActive);
						foreach (SynapseProximal syn in column.ProximalDendrite.Synapses)
						{
							Assert.IsTrue(syn.IsActive);
							Assert.IsTrue(syn.Permanence == 1.0);
						}
					}

				lr.OverrideActive(false);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsFalse(column.IsActive);
						//test deep override
						Assert.IsFalse(column.ProximalDendrite.IsActive);
						foreach (SynapseProximal syn in column.ProximalDendrite.Synapses)
						{
							Assert.IsFalse(syn.IsActive);
							Assert.IsTrue(syn.Permanence == 0.0);
						}
					}

				lr.OverrideActive(true);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsTrue(column.IsActive);
						//test deep override
						Assert.IsTrue(column.ProximalDendrite.IsActive);
						foreach (SynapseProximal syn in column.ProximalDendrite.Synapses)
						{
							Assert.IsTrue(syn.IsActive);
							Assert.IsTrue(syn.Permanence == 1.0);
						}
					}
			}
		}

		[TestMethod()]
		public void LayerOverridePredictingTest()
		{
			Random rnd = Global.rnd;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				//Layer
				int numColumnsX = rnd.Next(1, 20);
				int numColumnsY = rnd.Next(1, 20);
				int numCellsInCol = rnd.Next(1, 5);

				Layer lr = new Layer(numColumnsX, numColumnsY, numCellsInCol);
				Assert.IsNotNull(lr.Columns);
				Assert.AreEqual(lr.Columns.Count, numColumnsY);
				Assert.AreEqual(lr.Columns[0].Count, numColumnsX);

				//create synapses
				//lr.ConnectBasal(1, 1);
				lr.ConnectColumns(null);

				//with Permanence = 0.0
				lr.OverrideBasalPermanences(0.0);

				lr.OverridePredicting(false);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsFalse(column.IsPredicting);
						//test deep override
						foreach (Cell cell in column.Cells)
						{

							Assert.IsFalse(cell.BasalDendrite.IsActive);
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
							{
								Assert.IsFalse(syn.IsActive);
								Assert.IsTrue(syn.Permanence == 0.0);
							}
						}
					}

				lr.OverridePredicting(true);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsTrue(column.IsPredicting);
						//test deep override
						foreach (Cell cell in column.Cells)
						{

							Assert.IsTrue(cell.BasalDendrite.IsActive);
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
							{
								Assert.IsTrue(syn.IsActive);
								Assert.IsTrue(syn.Permanence == 1.0);
							}
						}
					}

				lr.OverridePredicting(false);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsFalse(column.IsPredicting);
						//test deep override
						foreach (Cell cell in column.Cells)
						{

							Assert.IsFalse(cell.BasalDendrite.IsActive);
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
							{
								Assert.IsFalse(syn.IsActive);
								Assert.IsTrue(syn.Permanence == 0.0);
							}
						}
					}

				lr.OverridePredicting(true);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsTrue(column.IsPredicting);
						//test deep override
						foreach (Cell cell in column.Cells)
						{

							Assert.IsTrue(cell.BasalDendrite.IsActive);
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
							{
								Assert.IsTrue(syn.IsActive);
								Assert.IsTrue(syn.Permanence == 1.0);
							}
						}
					}

				//with Permanence = 1.0
				lr.OverrideBasalPermanences(1.0);

				lr.OverridePredicting(false);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsFalse(column.IsPredicting);
						//test deep override
						foreach (Cell cell in column.Cells)
						{

							Assert.IsFalse(cell.BasalDendrite.IsActive);
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
							{
								Assert.IsFalse(syn.IsActive);
								Assert.IsTrue(syn.Permanence == 0.0);
							}
						}
					}

				lr.OverridePredicting(true);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsTrue(column.IsPredicting);
						//test deep override
						foreach (Cell cell in column.Cells)
						{

							Assert.IsTrue(cell.BasalDendrite.IsActive);
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
							{
								Assert.IsTrue(syn.IsActive);
								Assert.IsTrue(syn.Permanence == 1.0);
							}
						}
					}

				lr.OverridePredicting(false);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsFalse(column.IsPredicting);
						//test deep override
						foreach (Cell cell in column.Cells)
						{

							Assert.IsFalse(cell.BasalDendrite.IsActive);
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
							{
								Assert.IsFalse(syn.IsActive);
								Assert.IsTrue(syn.Permanence == 0.0);
							}
						}
					}

				lr.OverridePredicting(true);
				foreach (List<Column> colRow in lr.Columns)
					foreach (Column column in colRow)
					{
						Assert.IsTrue(column.IsPredicting);
						//test deep override
						foreach (Cell cell in column.Cells)
						{

							Assert.IsTrue(cell.BasalDendrite.IsActive);
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
							{
								Assert.IsTrue(syn.IsActive);
								Assert.IsTrue(syn.Permanence == 1.0);
							}
						}
					}
			}
		}

		[TestMethod()]
		public void LayerCountActiveColumnsTest()
		{
			Random rnd = Global.rnd;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				//Layer
				int layerColumnsX = rnd.Next(1, 10);
				//minimum 2 columns neeed for testing basal overrides
				int layerColumnsY = layerColumnsX != 1 ? layerColumnsY = rnd.Next(1, 10) : layerColumnsY = rnd.Next(2, 10);
				int layerNumCellsInColumn = rnd.Next(1, 10);
				//layerColumnsX = 1;
				//layerColumnsY = 1;
				//layerNumCellsInColumn = 2;
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);
				int lrNumColumns = lr.NumColumnsX * lr.NumColumnsY;

				//InputPlane
				int ipColumnsX = rnd.Next(1, 20);
				int ipColumnsY = rnd.Next(1, 20);
				//ipColumnsX = 6;
				//ipColumnsY = 8;
				InputPlane ip = new InputPlane(ipColumnsX, ipColumnsY);
				int ipNumColumns = ip.NumColumnsX * ip.NumColumnsY;

				//create synapses
				//lr.ConnectProximal(ip, 1, 1);
				//lr.ConnectBasal(1, 1);
				lr.ConnectColumns(ip);
				lr.OverrideBasalPermanences(1.0);
				lr.OverrideProximalPermanences(1.0);


				lr.Override(false, false);
				//lr.Update();
				int numActive = lr.CountActiveColumns();
				int numPredicting = lr.CountPredictingColumns();
				Assert.AreEqual(0, numActive);
				Assert.AreEqual(0, numPredicting);

				lr.Override(false, true);
				//lr.Update();
				numActive = lr.CountActiveColumns();
				numPredicting = lr.CountPredictingColumns();
				Assert.AreEqual(0, numActive);
				Assert.AreEqual(lrNumColumns, numPredicting);

				lr.Override(true, false);
				//lr.Update();
				numActive = lr.CountActiveColumns();
				numPredicting = lr.CountPredictingColumns();
				Assert.AreEqual(lrNumColumns, numActive);
				Assert.AreEqual(0, numPredicting);

				lr.Override(true, true);
				//lr.Update();
				numActive = lr.CountActiveColumns();
				numPredicting = lr.CountPredictingColumns();
				Assert.AreEqual(lrNumColumns, numActive);
				Assert.AreEqual(lrNumColumns, numPredicting);

				lr.Override(false, false);
				//lr.Update();
				numActive = lr.CountActiveColumns();
				numPredicting = lr.CountPredictingColumns();
				Assert.AreEqual(0, numActive);
				Assert.AreEqual(0, numPredicting);
			}
		}

		[TestMethod()]
		public void LayerCountActiveColumnsTestP()
		{
			Random rnd = Global.rnd;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				//Layer
				int layerColumnsX = rnd.Next(1, 20);
				int layerColumnsY = rnd.Next(1, 20);
				int layerNumCellsInColumn = rnd.Next(1, 10);
				//layerColumnsX = 1;
				//layerColumnsY = 1;
				//layerNumCellsInColumn = 1;
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);
				int lrNumColumns = lr.NumColumnsX * lr.NumColumnsY;

				//InputPlane
				int ipColumnsX = rnd.Next(1, 20);
				int ipColumnsY = rnd.Next(1, 20);
				InputPlane ip = new InputPlane(ipColumnsX, ipColumnsY);
				int ipNumColumns = ip.NumColumnsX * ip.NumColumnsY;
				ip.OverrideActive(true);

				//create synapses
				//lr.ConnectProximal(ip, 1, 1);
				//lr.ConnectBasal(1, 1);
				lr.ConnectColumns(ip);

				lr.Override(false, false);
				int numActive = lr.CountActiveColumnsP();
				int numPredicting = lr.CountPredictingColumnsP();
				Assert.AreEqual(0, numActive);
				Assert.AreEqual(0, numPredicting);

				lr.Override(false, true);
				numActive = lr.CountActiveColumnsP();
				numPredicting = lr.CountPredictingColumnsP();
				Assert.AreEqual(0, numActive);
				Assert.AreEqual(lrNumColumns, numPredicting);

				lr.Override(true, false);
				numActive = lr.CountActiveColumnsP();
				numPredicting = lr.CountPredictingColumnsP();
				Assert.AreEqual(lrNumColumns, numActive);
				Assert.AreEqual(0, numPredicting);

				lr.Override(true, true);
				numActive = lr.CountActiveColumnsP();
				numPredicting = lr.CountPredictingColumnsP();
				Assert.AreEqual(lrNumColumns, numActive);
				Assert.AreEqual(lrNumColumns, numPredicting);

				lr.Override(false, false);
				numActive = lr.CountActiveColumnsP();
				numPredicting = lr.CountPredictingColumnsP();
				Assert.AreEqual(0, numActive);
				Assert.AreEqual(0, numPredicting);
			}
		}

		
		[TestMethod()]
		public void LayerGetColumnsAroundCentre_InclusiveTest()
		{
			Random rnd = Global.rnd;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				//Layer
				int layerColumnsX = rnd.Next(1, 20);
				int layerColumnsY = rnd.Next(1, 20);
				int layerNumCellsInColumn = rnd.Next(1, 10);
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);
				
				//random zone size
				double zoneSizePerc = rnd.NextDouble();
				lr.ZoneSizePercProximal = zoneSizePerc;
				lr.ZoneSizePercBasal = zoneSizePerc;

				bool includeCentre = true;

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						List<Column> columns = lr.GetColumnsFromCentre(x, y, zoneSizePerc, includeCentre );
						
						//********************************************************************
						//this logic just mimicks the function code
						//for test - run it here and compare results
						int numToCreate = (int)( (double)lr.NumColumnsX * (double)lr.NumColumnsY * zoneSizePerc - ( includeCentre ? 0 : 1 ) );

						//find rectangular zone dimensions that gives minimum that many elements
						int zoneWidth = 0;
						int zoneHeight = 0;
						bool alternate = false;

						//subtract 1 when centre not included
						//this will result in a larger rect zone if necessary
						//includeCentre = true in this test
						while ( zoneWidth * zoneHeight - ( includeCentre ? 0 : 1 ) < numToCreate )
						{
							alternate = !alternate;
							if ( alternate )
							{
								if ( zoneWidth < lr.NumColumnsX )
									zoneWidth++;
							}
							else
							{
								if ( zoneHeight < lr.NumColumnsY )
									zoneHeight++;
							}
						}

						int zoneLeft = Math.Max ( Math.Min ( lr.NumColumnsX - zoneWidth, x - zoneWidth / 2 ), 0 );
						int zoneRight = Math.Min ( zoneLeft + zoneWidth - 1, lr.NumColumnsX - 1 );
						int zoneTop = Math.Max ( Math.Min ( lr.NumColumnsY - zoneHeight, y - zoneHeight / 2 ), 0 );
						int zoneBottom = Math.Min ( zoneTop + zoneHeight - 1, lr.NumColumnsY - 1 );
						//**********************************************************************

						
						foreach (Column col in columns)
						{
							Assert.IsTrue ( col.X >= zoneLeft );
							Assert.IsTrue ( col.X <= zoneRight );
							Assert.IsTrue ( col.Y >= zoneTop );
							Assert.IsTrue ( col.Y <= zoneBottom );
						}
					}
				}
			}
		}

		[TestMethod()]
		public void LayerGetColumnsAroundCentre_Inclusive_WithThresholdTest()
		{
			Random rnd = Global.rnd;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				int stimulusThreshold = rnd.Next(2, 5);

				//Layer
				int layerColumnsX = rnd.Next(1, 20);
				int layerColumnsY = rnd.Next(1, 20);
				int layerNumCellsInColumn = rnd.Next(1, 5);
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);


				//random zone
				double zoneSizePerc = rnd.NextDouble();
				lr.ZoneSizePercProximal = zoneSizePerc;
				lr.ZoneSizePercBasal = zoneSizePerc;

				//set InputOverlap > threshold
				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						lr.Columns[y][x].OverrideProximalInputOverlap(stimulusThreshold + 1);
					}
				}

				int ctr = 0;

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						List<Column> columnsAll = lr.GetColumnsFromCentre(x, y, zoneSizePerc, true);
						List<Column> columnsThreshold = lr.GetColumnsFromCentre_WithThreshold(x, y, zoneSizePerc, true, stimulusThreshold);

						ctr = 0;
						foreach (Column col in columnsThreshold)
						{
							if (col.InputOverlap >= stimulusThreshold)
								ctr++;
						}
						//Test all columns InputOverlap > thresholdstimulusThreshold
						Assert.AreEqual(columnsThreshold.Count, columnsAll.Count);
					}
				}


				//set InputOverlap < threshold
				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						lr.Columns[y][x].OverrideProximalInputOverlap(stimulusThreshold - 1);
					}
				}

				ctr = 0;

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						List<Column> columnsAll = lr.GetColumnsFromCentre(x, y, zoneSizePerc, true);
						List<Column> columnsThreshold = lr.GetColumnsFromCentre_WithThreshold(x, y, zoneSizePerc, true, stimulusThreshold);

						ctr = 0;
						foreach (Column col in columnsThreshold)
						{
							if (col.InputOverlap >= stimulusThreshold)
								ctr++;
						}
						//Test all columns InputOverlap < thresholdstimulusThreshold
						Assert.AreEqual(0, columnsThreshold.Count);
					}
				}

				//set random InputOverlap
				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						if (rnd.NextDouble() > 0.5)
							lr.Columns[y][x].OverrideProximalInputOverlap(stimulusThreshold + 1);
						else
							lr.Columns[y][x].OverrideProximalInputOverlap(stimulusThreshold - 1);
					}
				}

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						List<Column> columnsAll = lr.GetColumnsFromCentre(x, y, zoneSizePerc, true);
						List<Column> columnsThreshold = lr.GetColumnsFromCentre_WithThreshold(x, y, zoneSizePerc, true, stimulusThreshold);

						ctr = 0;
						foreach (Column col in columnsAll)
						{
							if (col.InputOverlap >= stimulusThreshold)
								ctr++;
						}
						//Test all columns InputOverlap counts
						Assert.AreEqual(columnsThreshold.Count, ctr);
					}
				}

			}
		}

		[TestMethod()]
		public void LayerGetColumnsAroundCentre_ExclusiveTest()
		{
			Random rnd = Global.rnd;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				//Layer
				int layerColumnsX = rnd.Next(1, 20);
				int layerColumnsY = rnd.Next(1, 20);
				int layerNumCellsInColumn = rnd.Next(1, 10);
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);

				//random zone
				double zoneSizePerc = rnd.NextDouble();
				lr.ZoneSizePercProximal = zoneSizePerc;
				lr.ZoneSizePercBasal = zoneSizePerc;

				bool includeCentre = false;

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						List<Column> columns = lr.GetColumnsFromCentre(x, y, zoneSizePerc, includeCentre );

						//********************************************************************
						//this logic just mimicks the function code
						//for test - run it here and compare results
						int numToCreate = (int)( (double)lr.NumColumnsX * (double)lr.NumColumnsY * zoneSizePerc - (includeCentre ? 0 : 1) );

						//find rectangular zone dimensions that gives minimum that many elements
						int zoneWidth = 0;
						int zoneHeight = 0;
						bool alternate = false;

						//subtract 1 when centre not included
						//this will result in a larger rect zone if necessary
						//includeCentre = false in this test
						while ( zoneWidth * zoneHeight - ( includeCentre ? 0 : 1 ) < numToCreate )
						{
							alternate = !alternate;
							if ( alternate )
							{
								if ( zoneWidth < lr.NumColumnsX )
									zoneWidth++;
							}
							else
							{
								if ( zoneHeight < lr.NumColumnsY )
									zoneHeight++;
							}
						}

						int zoneLeft = Math.Max ( Math.Min ( lr.NumColumnsX - zoneWidth, x - zoneWidth / 2 ), 0 );
						int zoneRight = Math.Min ( zoneLeft + zoneWidth - 1, lr.NumColumnsX - 1 );
						int zoneTop = Math.Max ( Math.Min ( lr.NumColumnsY - zoneHeight, y - zoneHeight / 2 ), 0 );
						int zoneBottom = Math.Min ( zoneTop + zoneHeight - 1, lr.NumColumnsY - 1 );
						//**********************************************************************

						Assert.AreEqual ( numToCreate, columns.Count );
						foreach ( Column col in columns )
						{
							Assert.IsTrue ( col.X >= zoneLeft );
							Assert.IsTrue ( col.X <= zoneRight );
							Assert.IsTrue ( col.Y >= zoneTop );
							Assert.IsTrue ( col.Y <= zoneBottom );

							//assert exclusive
							Assert.IsFalse ( col.X == x && col.Y == y );
						}
					}
				}
			}
		}

		[TestMethod()]
		public void LayerGetColumnsAroundCentre_Exclusive_WithThresholdTest()
		{
			Random rnd = Global.rnd;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				int stimulusThreshold = rnd.Next(2, 5);

				//Layer
				int layerColumnsX = rnd.Next(1, 20);
				int layerColumnsY = rnd.Next(1, 20);
				int layerNumCellsInColumn = rnd.Next(1, 5);
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);


				//random zone
				double zoneSizePerc = rnd.NextDouble();
				lr.ZoneSizePercProximal = zoneSizePerc;
				lr.ZoneSizePercBasal = zoneSizePerc;

				//set InputOverlap > threshold
				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						lr.Columns[y][x].OverrideProximalInputOverlap(stimulusThreshold + 1);
					}
				}

				int ctr = 0;

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						List<Column> columnsAll = lr.GetColumnsFromCentre(x, y, zoneSizePerc, false);
						List<Column> columnsThreshold = lr.GetColumnsFromCentre_WithThreshold(x, y, zoneSizePerc, false, stimulusThreshold);

						ctr = 0;
						foreach (Column col in columnsThreshold)
						{
							if (col.InputOverlap >= stimulusThreshold)
								ctr++;
						}
						//Test all columns InputOverlap > thresholdstimulusThreshold
						Assert.AreEqual(columnsThreshold.Count, columnsAll.Count);
					}
				}


				//set InputOverlap < threshold
				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						lr.Columns[y][x].OverrideProximalInputOverlap(stimulusThreshold - 1);
					}
				}

				ctr = 0;

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						List<Column> columnsAll = lr.GetColumnsFromCentre(x, y, zoneSizePerc, false);
						List<Column> columnsThreshold = lr.GetColumnsFromCentre_WithThreshold(x, y, zoneSizePerc, false, stimulusThreshold);

						ctr = 0;
						foreach (Column col in columnsThreshold)
						{
							if (col.InputOverlap >= stimulusThreshold)
								ctr++;
						}
						//Test all columns InputOverlap < thresholdstimulusThreshold
						Assert.AreEqual(0, columnsThreshold.Count);
					}
				}

				//set random InputOverlap
				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						if (rnd.NextDouble() > 0.5)
							lr.Columns[y][x].OverrideProximalInputOverlap(stimulusThreshold + 1);
						else
							lr.Columns[y][x].OverrideProximalInputOverlap(stimulusThreshold - 1);
					}
				}				

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						List<Column> columnsAll = lr.GetColumnsFromCentre(x, y, zoneSizePerc, false);
						List<Column> columnsThreshold = lr.GetColumnsFromCentre_WithThreshold(x, y, zoneSizePerc, false, stimulusThreshold);

						ctr = 0;
						foreach (Column col in columnsAll)
						{
							if (col.InputOverlap >= stimulusThreshold)
								ctr++;
						}
						//Test all columns InputOverlap counts
						Assert.AreEqual(columnsThreshold.Count, ctr);
					}
				}

			}
		}

		[TestMethod()]
		public void LayerCreateBasalConnectionsTest()
		{
			Random rnd = Global.rnd;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				//Layer
				int layerColumnsX = rnd.Next(1, 10);
				int layerColumnsY = rnd.Next(1, 10);
				int layerNumCellsInColumn = rnd.Next(1, 10);

				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);

				int numColumns = lr.NumColumnsX * lr.NumColumnsY;

				double zoneSizePerc = rnd.NextDouble();
				double zoneCoveragePerc = rnd.NextDouble();
				lr.ZoneSizePercProximal = zoneSizePerc;
				lr.ZoneCoveragePercProximal = zoneCoveragePerc;
				lr.ZoneSizePercBasal = zoneSizePerc;
				lr.ZoneCoveragePercBasal = zoneCoveragePerc;

				lr.ConnectColumns(null);

				//for each column, count unique connected columns
				List<Column> ConnectedColumnsList = new List<Column>(); //list of columns connected to (actual connections)
								
				foreach (List<Column> colRow in lr.Columns)
				{
					foreach (Column col in colRow)
					{
						int numToConnect = col.CalcNumBasalSynapsesToCreate(lr, zoneSizePerc, zoneCoveragePerc);
						int numConnected = col.CountBasalSynapses();
						Assert.AreEqual(numToConnect * col.Cells.Count, numConnected);

						//test ConnectedColumns to each Cell
						foreach (Cell cell in col.Cells)
						{
							ConnectedColumnsList.Clear();
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
							{
								Column cc = syn.ColumnConnected;
								Assert.IsNotNull(cc);
								Assert.AreNotSame(col, cc);	//check column not connected to itself

								//add unique connected columns to list to obtain 
								//connected columns counter for each synapse
								bool columnOnList = false;
								foreach (Column connectedColumn in ConnectedColumnsList)
									if (connectedColumn == cc)
										columnOnList = true;
								if (!columnOnList)
									ConnectedColumnsList.Add(cc);
							}
							//check correct % columns connected to each synapse
							Assert.AreEqual(ConnectedColumnsList.Count, numToConnect);
						}
					}
				}
			}
		}
	}
}

#endif


