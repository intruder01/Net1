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
	public class ColumnTests
	{
		[TestMethod()]
		public void ColumnTest()
		{
			Column c = new Column(1, 100000, 5);
			Assert.IsNotNull(c.Cells);
			Assert.AreEqual(c.X, 1);
			Assert.AreEqual(c.Y, 100000);
			Assert.AreEqual(c.Cells.Count, 5);
			Assert.IsNotNull(c.ProximalDendrite);
			Assert.IsNotNull(c.ApicalDendrite);

			c = new Column(0, 200000, 5000);
			Assert.IsNotNull(c.Cells);
			Assert.AreEqual(c.X, 0);
			Assert.AreEqual(c.Y, 200000);
			Assert.AreEqual(c.Cells.Count, 5000);
			Assert.IsNotNull(c.ProximalDendrite);
			Assert.IsNotNull(c.ApicalDendrite);

			c = new Column(300000, 0, 5);
			Assert.IsNotNull(c.Cells);
			Assert.AreEqual(c.X, 300000);
			Assert.AreEqual(c.Y, 0);
			Assert.AreEqual(c.Cells.Count, 5);
			Assert.IsNotNull(c.ProximalDendrite);
			Assert.IsNotNull(c.ApicalDendrite);

			c = new Column(10000, 10000, 50);
			Assert.IsNotNull(c.Cells);
			Assert.AreEqual(c.X, 10000);
			Assert.AreEqual(c.Y, 10000);
			Assert.AreEqual(c.Cells.Count, 50);
			Assert.IsNotNull(c.ProximalDendrite);
			Assert.IsNotNull(c.ApicalDendrite);
		}

	
		[TestMethod()]
		public void ColumnUpdate_ColumnLevelTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Debug.WriteLine("ColumnUpdateBasalTest testNum=" + testNum.ToString());

				//Layer
				int layerColumnsX = rnd.Next(1, 10);
				int layerColumnsY = layerColumnsX > 1 ? rnd.Next(1, 10) : rnd.Next(2, 10);
				int layerNumCellsInColumn = rnd.Next(1, 5);
				//layerColumnsX = 2;
				//layerColumnsY = 4;
				//layerNumCellsInColumn = 2;
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);

				//InputPlane
				int ipColumnsX = rnd.Next(1, 20);
				int ipColumnsY = rnd.Next(1, 20);
				//ipColumnsX = 4;
				//ipColumnsY = 12;
				InputPlane ip = new InputPlane(ipColumnsX, ipColumnsY);

				// create synapses
				//lr.ConnectProximal(ip, 1, 1);
				//lr.ConnectBasal(1, 1);
				lr.ZoneSizePercProximal = 1.0;
				lr.ZoneCoveragePercProximal = 1.0;
				lr.ZoneSizePercBasal = 1.0;
				lr.ZoneCoveragePercBasal = 1.0;
				lr.ConnectColumns(ip);  //uses Leyer.ZoneSizePercProximal, ZoneCoveragePercProximal, Basal

				lr.OverrideBasalPermanences(1.0);
				lr.OverrideProximalPermanences(1.0);
				lr.OverrideProximalDendriteActivationThreshold(Global.DENDRITE_INITIAL_ACTIVATION_THRESHOLD);
				lr.OverrideBasalDendriteActivationThreshold(Global.DENDRITE_INITIAL_ACTIVATION_THRESHOLD);
				lr.InhibitionEnabled = false;

				int lrNumColumns = lr.NumColumnsX * lr.NumColumnsY;
				int ipNumColumns = ip.NumColumnsX * ip.NumColumnsY;

				foreach (List<Column> colRow in lr.Columns)
				{
					foreach (Column col in colRow)
					{
						col.CreateProximalSynapses(lr, ip, 1, 1);
						col.CreateBasalSynapses(lr, 1, 1);
						Assert.AreEqual(col.ProximalDendrite.Synapses.Count, ipNumColumns);

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
						// Active		- InputPlane Activations 0
						// Predicting	- own Layer Predicting 0 
						//
						//b. due to INTERNAL factors
						//
						// Active		- ProximalDendrite
						//					- due number of ProximalSynapses to InputPlane columns
						//					- due to ProximalSynapses Permanence values
						//
						// Predicting	- BasalDendrite
						//					- due to number of BasalSynapses to own Layer columns
						//					- due to BasalSynapses Permanence values

						lr.Override(false, false);
						lr.OverrideProximalInputOverlap(0);
						Assert.IsFalse(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 0);
						col.Update_Basal();
						Assert.IsFalse(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 0);

						lr.Override(true, false);
						lr.OverrideProximalInputOverlap(1);
						Assert.IsTrue(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 1);
						col.Update_Proximal();
						List<Column> neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 1);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsTrue(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, col.ProximalDendrite.Synapses.Count);

						lr.Override(false, true);
						lr.OverrideProximalInputOverlap(1);
						Assert.IsFalse(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 1);
						col.Update_Proximal();
						neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 1);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsFalse(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 0);

						lr.Override(true, true);
						lr.OverrideProximalInputOverlap(1);
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 1);
						col.Update_Proximal();
						neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 1);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, col.ProximalDendrite.Synapses.Count);

						//IsActive - Proximal Synapses Active OFF
						lr.Override(true, true);
						lr.OverrideProximalInputOverlap(1);
						foreach (SynapseProximal syn in col.ProximalDendrite.Synapses)
							syn.ColumnConnected.OverrideActive(false, 2);
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 1);
						col.Update_Proximal();
						neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 1);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsFalse(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 0);

						//IsPredicting - Basal ConnectedCells Active OFF
						lr.Override(true, true);
						lr.OverrideProximalInputOverlap(1);
						foreach (Cell cell in col.Cells)
							foreach (SynapseBasal syn in cell.BasalDendrite.Synapses)
								syn.ColumnConnected.OverridePredicting(false, 2);
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 1);
						col.Update_Proximal();
						neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 1);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsTrue(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, col.ProximalDendrite.Synapses.Count);

						// Active		- ProximalDendrite
						//				- due number of ProximalSynapses to InputPlane
						lr.Override(true, true);
						lr.OverrideProximalInputOverlap(1);
						col.ProximalDendrite.OverrideActivationThreshold(col.ProximalDendrite.Synapses.Count - rnd.Next(0, 5));
						while (col.ProximalDendrite.Synapses.Count >= col.ProximalDendrite.ActivationThreshold &&
								col.ProximalDendrite.Synapses.Count > 1) //leave at least 1 synapse
						{
							SynapseProximal syn = col.ProximalDendrite.Synapses[0];
							col.ProximalDendrite.Synapses.Remove(syn);
						}
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 1);
						col.Update_Proximal();
						neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 1);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, col.ProximalDendrite.Synapses.Count); //all remaining synapses

						//lr.ConnectProximal(ip, 1, 1);
						//lr.ConnectBasal(1, 1);
						lr.ZoneSizePercProximal = 1.0;
						lr.ZoneCoveragePercProximal = 1.0;
						lr.ZoneSizePercBasal = 1.0;
						lr.ZoneCoveragePercBasal = 1.0;
						lr.ConnectColumns(ip);
						lr.OverrideProximalPermanences(NetConfigData.SynapsePermanenceThreshold);
						lr.OverrideProximalDendriteActivationThreshold(Global.DENDRITE_INITIAL_ACTIVATION_THRESHOLD);

						// Active		- ProximalDendrite
						//				- due to ProximalSynapses Permanence values
						lr.Override(true, true);
						lr.OverrideProximalInputOverlap(0);
						lr.InhibitionEnabled = false;
						col.ProximalDendrite.OverridePermanence(NetConfigData.SynapsePermanenceThreshold 
														- 2 * NetConfigData.SynapsePermanenceIncrease);
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 0);

						col.Update_Proximal();
						neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 0);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsFalse(col.IsActive);           //first update - false           
						Assert.IsFalse(col.IsPredicting);       
						Assert.AreEqual(col.InputOverlap, 0);

						col.Update_Proximal();
						neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 0);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsTrue(col.IsActive);           //second update - true //*********************** FAILS HERE
						Assert.IsTrue(col.IsPredicting);        
						Assert.AreEqual(col.InputOverlap, col.ProximalDendrite.Synapses.Count);

						lr.OverrideProximalPermanences(NetConfigData.SynapsePermanenceThreshold);

						// Predicting	- BasalDendrite
						//				- due to number of BasalSynapses to own Layer columns
						lr.Override(true, true);
						lr.OverrideProximalInputOverlap(0);
						foreach (Cell cell in col.Cells)
						{
							cell.BasalDendrite.OverrideActivationThreshold(cell.BasalDendrite.Synapses.Count 
																			- rnd.Next(0, 5));
							while (cell.BasalDendrite.Synapses.Count >= cell.BasalDendrite.ActivationThreshold)
							{
								SynapseBasal syn = cell.BasalDendrite.Synapses[0];
								cell.BasalDendrite.Synapses.Remove(syn);
							}
						}
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 0);
						col.Update_Proximal();
						neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 0);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsTrue(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, col.ProximalDendrite.Synapses.Count);

						//lr.ConnectBasal(1, 1);
						//lr.ConnectProximal(ip, 1, 1);
						lr.ZoneSizePercProximal = 1.0;
						lr.ZoneCoveragePercProximal = 1.0;
						lr.ZoneSizePercBasal = 1.0;
						lr.ZoneCoveragePercBasal = 1.0;
						lr.ConnectColumns(ip);
						lr.OverrideBasalPermanences(NetConfigData.SynapsePermanenceThreshold);
						lr.OverrideBasalDendriteActivationThreshold(Global.DENDRITE_INITIAL_ACTIVATION_THRESHOLD);

						// Predicting	- BasalDendrite
						//				- due to BasalSynapses Permanence values
						lr.Override(true, true);
						lr.OverrideProximalInputOverlap(0);
						foreach (Cell cell in col.Cells)
							cell.BasalDendrite.OverridePermanence(NetConfigData.SynapsePermanenceThreshold - 2 * NetConfigData.SynapsePermanenceIncrease);
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.AreEqual(col.InputOverlap, 0);
						col.Update_Proximal();
						neighbours = lr.GetColumnsFromCentre_WithThreshold(col.X, col.Y, 1, true, 0);
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsTrue(col.IsActive);
						Assert.IsFalse(col.IsPredicting);       //first update - false
						Assert.AreEqual(col.InputOverlap, col.ProximalDendrite.Synapses.Count);
						col.Update_Proximal();
						col.Update_Activation(neighbours, false);
						col.Update_Basal();
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);       //second update - true
						Assert.AreEqual(col.InputOverlap, col.ProximalDendrite.Synapses.Count);
					}
				}
			}
		}

		[TestMethod()]
		public void ColumnUpdate_LayerLevelTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			//override config params
			NetConfigData.ColumnsTopPercentile = 0;
			NetConfigData.ColumnStimulusThreshold = 1;
			NetConfigData.SynapsePermanenceIncrease = 0.0;
			NetConfigData.SynapsePermanenceDecrease = 0.0;

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Debug.WriteLine("ColumnUpdateProximalTest testNum=" + testNum.ToString());

				//Layer
				int layerColumnsX = rnd.Next(1, 10);
				int layerColumnsY = layerColumnsX > 1 ? rnd.Next(1, 10) : rnd.Next(2, 10);
				int layerNumCellsInColumn = rnd.Next(1, 5);
				//layerColumnsX = 1;	//debug
				//layerColumnsY = 8;	//debug
				//layerNumCellsInColumn = 3;//debug
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);

				//InputPlane
				int ipColumnsX = rnd.Next(1, 20);
				int ipColumnsY = rnd.Next(1, 20);
				//ipColumnsX = 11;    //debug
				//ipColumnsY = 19;    //debug
				InputPlane ip = new InputPlane(ipColumnsX, ipColumnsY);

				int numActive;	//count active
				int sbActive;	//should be active

				// create synapses
				lr.ZoneSizePercProximal = 1;
				lr.ZoneCoveragePercProximal = 1;
				lr.ZoneSizePercBasal = 1;
				lr.ZoneCoveragePercBasal = 1;
				lr.ConnectColumns(ip);

				lr.OverrideBasalPermanences(1.0);
				lr.OverrideProximalPermanences(1.0);
				lr.OverrideProximalDendriteActivationThreshold(1);
				lr.OverrideBasalDendriteActivationThreshold(1);

				lr.InhibitionEnabled = false;

				//set InputOverlap > threshold
				lr.Override(true, true);
				
				//test Activate() function by progressively zeroing Proximal connections
				NetConfigData.ColumnStimulusThreshold = 1;
				NetConfigData.SynapsePermanenceIncrease = 0.0;
				NetConfigData.SynapsePermanenceDecrease = 0.0;
				for (int y = 0; y < lr.NumColumnsY; y++)
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						//this will prevent cell from activating
						lr.Columns[y][x].OverrideProximalPermanence(0);

						lr.Update();
						//lr.PrintActive();
						numActive = lr.CountActiveColumns();
						sbActive = lr.NumColumns - (y * lr.NumColumnsX + x) - 1;
						Assert.AreEqual(sbActive, numActive);
					}

				//restore global params
				NetConfigData.SetDefaults();

				lr.ZoneSizePercProximal = 1;
				lr.ZoneCoveragePercProximal = 1;
				lr.ZoneSizePercBasal = 1;
				lr.ZoneCoveragePercBasal = 1;
				//lr.ConnectProximal(ip, 1, 1);
				//lr.ConnectBasal(1, 1);
				lr.ConnectColumns(ip);
				lr.OverrideBasalPermanences(1.0);
				lr.OverrideProximalPermanences(1.0);
				lr.OverrideProximalDendriteActivationThreshold(1);
				lr.OverrideBasalDendriteActivationThreshold(1);

				//test UpdateProximal() function by rising stimulus threshold
				NetConfigData.ColumnsTopPercentile = 0;
				NetConfigData.ColumnStimulusThreshold = ip.NumColumns;
				lr.Update();
				numActive = lr.CountActiveColumns();
				Assert.AreEqual(numActive, lr.NumColumns);
				NetConfigData.ColumnStimulusThreshold++;
				lr.Update();
				numActive = lr.CountActiveColumns();
				Assert.AreEqual(numActive, 0);	// ************************ FAILS HERE

				//restore global params
				NetConfigData.SetDefaults();

				//lr.ConnectProximal(ip, 1, 1);
				//lr.ConnectBasal(1, 1);
				lr.ConnectColumns(ip);
				lr.OverrideBasalPermanences(1.0);
				lr.OverrideProximalPermanences(1.0);
				lr.OverrideProximalDendriteActivationThreshold(1);
				lr.OverrideBasalDendriteActivationThreshold(1);
				lr.ZoneSizePercProximal = 1;
				lr.ZoneCoveragePercProximal = 1;
				lr.ZoneSizePercBasal = 1;
				lr.ZoneCoveragePercBasal = 1;

				//test UpdateProximal() function by rising neighbour top percentile
				NetConfigData.ColumnsTopPercentile = 0;
				lr.Update();
				numActive = lr.CountActiveColumns();
				Assert.AreEqual(numActive, lr.NumColumns);
				NetConfigData.ColumnsTopPercentile = 1;
				lr.Update();
				numActive = lr.CountActiveColumns();
				Assert.AreEqual(numActive, lr.NumColumns);

				//restore global params
				NetConfigData.SetDefaults();

				//lr.ConnectProximal(ip, 1, 1);
				//lr.ConnectBasal(1, 1);
				lr.ConnectColumns(ip);
				lr.OverrideBasalPermanences(1.0);
				lr.OverrideProximalPermanences(1.0);
				lr.OverrideProximalDendriteActivationThreshold(1);
				lr.OverrideBasalDendriteActivationThreshold(1);
				lr.ZoneSizePercProximal = 1;
				lr.ZoneCoveragePercProximal = 1;
				lr.ZoneSizePercBasal = 1;
				lr.ZoneCoveragePercBasal = 1;

				//test UpdateProximal() function by deactivating columns in InputPlane
				ip.Override(false, false);
				lr.Update();
				numActive = lr.CountActiveColumns();
				Assert.AreEqual(0, numActive);	//all inputs 0 - no active columns
				ip.Override(true, false);
				lr.Update();
				numActive = lr.CountActiveColumns();
				Assert.AreEqual(lr.NumColumns, numActive);	//all inputs 1 - all columns active
				ip.Override(false, false);
				ip.Columns[0][0].OverrideActive(true, 2);
				lr.Update();
				numActive = lr.CountActiveColumns();
				Assert.AreEqual(lr.NumColumns, numActive);	//one input 1 - all columns active
				NetConfigData.ColumnStimulusThreshold = 2;   
				lr.Update();
				numActive = lr.CountActiveColumns();
				Assert.AreEqual(0, numActive);  //increase COLUMN_STIMULUS_THRESHOLD = 2 - no columns active
			}

			//restore global params
			NetConfigData.SetDefaults();
		}

		[TestMethod()]
		public void ColumnOverrideTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				//Layer
				int layerColumnsX = rnd.Next(1, 10);    
				int layerColumnsY;
				//minimum 2 columns neeed for testing basal overrides
				layerColumnsY = layerColumnsX != 1 ? layerColumnsY = rnd.Next(1, 10) : layerColumnsY = rnd.Next(2, 10);
				int layerNumCellsInColumn = rnd.Next(1, 5);
				//layerColumnsX = 8;
				//layerColumnsY = 8;
				//layerNumCellsInColumn = 2;
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);

				//InputPlane
				int ipColumnsX = rnd.Next(1, 10);
				int ipColumnsY = rnd.Next(1, 10);
				//ipColumnsX = 1;
				//ipColumnsY = 6;
				InputPlane ip = new InputPlane(ipColumnsX, ipColumnsY);
				

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						Column col = lr.Columns[y][x];
						col.CreateProximalSynapses(lr, ip, 1, 1);
						col.CreateBasalSynapses(lr, 1, 1);
						col.OverrideProximalPermanence(1.0);
						col.OverrideBasalPermanences(1.0);

						col.OverrideActive(false, 0);
						col.OverridePredicting(false, 0);
						Assert.IsFalse(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.IsFalse(col.ProximalDendrite.Synapses[0].ColumnConnected.IsActive);
						Assert.IsFalse(col.ProximalDendrite.Synapses[0].ColumnConnected.IsPredicting);
						Assert.AreEqual(ip.CountActiveColumns(), 0);
						Assert.AreEqual(lr.CountActiveColumns(), 0);
						Assert.AreEqual(ip.CountPredictingColumns(), 0);
						Assert.AreEqual(lr.CountPredictingColumns(), 0);


						col.OverrideActive(false, 0);
						col.OverridePredicting(true, 0);
						Assert.IsFalse(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.IsFalse(col.ProximalDendrite.Synapses[0].ColumnConnected.IsActive);
						Assert.IsTrue(col.Cells[0].BasalDendrite.Synapses[0].ColumnConnected.IsPredicting);
						Assert.AreEqual(ip.CountActiveColumns(), 0);
						Assert.AreEqual(lr.CountActiveColumns(), 0);
						Assert.AreEqual(ip.CountPredictingColumns(), 0);
						Assert.AreEqual(lr.CountPredictingColumns(), col.CountBasalSynapses() / col.Cells.Count + 1); 

						col.OverrideActive(true, 0);
						col.OverridePredicting(false, 0);
						Assert.IsTrue(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.IsTrue(col.ProximalDendrite.Synapses[0].ColumnConnected.IsActive);
						Assert.IsFalse(col.Cells[0].BasalDendrite.Synapses[0].ColumnConnected.IsPredicting);
						Assert.AreEqual(ip.CountActiveColumns(), col.CountProximalSynapses());
						Assert.AreEqual(lr.CountActiveColumns(), 1); //this col
						Assert.AreEqual(ip.CountPredictingColumns(), 0);
						Assert.AreEqual(lr.CountPredictingColumns(), 0);

						col.OverrideActive(true, 0);
						col.OverridePredicting(true, 0);
						Assert.IsTrue(col.IsActive);
						Assert.IsTrue(col.IsPredicting);
						Assert.IsTrue(col.ProximalDendrite.Synapses[0].ColumnConnected.IsActive);
						Assert.IsTrue(col.Cells[0].BasalDendrite.Synapses[0].ColumnConnected.IsPredicting);
						Assert.AreEqual(ip.CountActiveColumns(), col.CountProximalSynapses());
						Assert.AreEqual(lr.CountActiveColumns(), 1);
						Assert.AreEqual(ip.CountPredictingColumns(), 0);
						Assert.AreEqual(lr.CountPredictingColumns(), col.CountBasalSynapses() / col.Cells.Count + 1);

						col.OverrideActive(false, 0);
						col.OverridePredicting(false, 0);
						Assert.IsFalse(col.IsActive);
						Assert.IsFalse(col.IsPredicting);
						Assert.IsFalse(col.ProximalDendrite.Synapses[0].ColumnConnected.IsActive);
						Assert.IsFalse(col.Cells[0].BasalDendrite.Synapses[0].ColumnConnected.IsPredicting);
						Assert.AreEqual(ip.CountActiveColumns(), 0);
						Assert.AreEqual(lr.CountActiveColumns(), 0);
						Assert.AreEqual(ip.CountPredictingColumns(), 0);
						Assert.AreEqual(lr.CountPredictingColumns(), 0);
					}
				}
			}
		}

		[TestMethod()]
		public void ColumnCreateProximalConnectionsTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				//Layer
				int layerColumnsX = rnd.Next(1, 20);
				int layerColumnsY = rnd.Next(1, 20);
				int layerNumCellsInColumn = rnd.Next(1, 10);
				layerNumCellsInColumn = 1;
				Layer lr = new Layer(layerColumnsX, layerColumnsY, layerNumCellsInColumn);

				//Inputlane
				int inputPlaneX = rnd.Next(1, 20);
				int inputPlaneY = rnd.Next(1, 20);
				InputPlane ip = new InputPlane(inputPlaneX, inputPlaneY);

				//random % coverage
				double zoneSizePerc = rnd.NextDouble();
				double zoneCoveragePerc = rnd.NextDouble();

				lr.ZoneSizePercProximal = zoneSizePerc;
				lr.ZoneCoveragePercProximal = zoneCoveragePerc;
				lr.ZoneSizePercBasal = zoneSizePerc;
				lr.ZoneCoveragePercBasal = zoneCoveragePerc;

				List<Column> PotentialColumnsList = new List<Column>(); //list of columns within zone (potential connections)
				List<Column> ConnectedColumnsList = new List<Column>(); //list of columns connected to (actual connections)

				for (int y = 0; y < lr.NumColumnsY; y++)
				{
					for (int x = 0; x < lr.NumColumnsX; x++)
					{
						Column column = lr.Columns[y][x];

						//scale between InputPlane and Layer location positions
						int scaledX, scaledY;
						lr.MapPoint(column.X, column.Y, ip, out scaledX, out scaledY);

						//calculate number of connections that will be created
						PotentialColumnsList = ip.GetColumnsFromCentre(scaledX, scaledY, zoneSizePerc, true);
						int numToConnect = column.CalcNumProximalSynapsesToCreate(lr, ip, zoneSizePerc, zoneCoveragePerc);

						column.CreateProximalSynapses(lr, ip, zoneSizePerc, zoneCoveragePerc);
						int numConnected = column.CountProximalSynapses();

						//build check-list of connected columns
						ConnectedColumnsList.Clear();
						foreach (Synapse syn in column.ProximalDendrite.Synapses)
						{
							Column cc = syn.ColumnConnected;
							Assert.IsNotNull(cc);
							Assert.AreNotSame(column, cc); //check column not connected to itself (here column is stand-alone...)
													
							double distance = Algebra.EuclideanDistance2D(scaledX, scaledY, cc.X, cc.Y); 
							Assert.IsTrue( PotentialColumnsList.Contains( cc ) );

							//add unique connected columns to list to obtain 
							//connected columns counter for each synapse
							bool columnOnList = false;
							foreach (Column connectedColumn in ConnectedColumnsList)
								if (connectedColumn == cc)
									columnOnList = true;
							if (!columnOnList)
								ConnectedColumnsList.Add(cc);
						}
						//check correct # columns connected to each Column
						Assert.AreEqual(ConnectedColumnsList.Count, numToConnect);
						Assert.AreEqual(ConnectedColumnsList.Count, numConnected);
						Assert.AreEqual(numConnected, numToConnect);
					}
				}
			}
		}


	}
}

#endif