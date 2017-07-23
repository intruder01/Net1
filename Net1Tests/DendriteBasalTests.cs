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
	public class DendriteBasalCtorTests
	{
		[TestMethod()]
		public void DendriteBasalCtorTest()
		{
			DendriteBasal den = new DendriteBasal(5);
			Assert.AreEqual(den.ActivationThreshold, 5);
			Assert.IsNotNull(den.Synapses);
			Assert.IsTrue(den.Synapses.Count == 0);
		}

		[TestMethod()]
		public void DendriteBasalUpdateTest()
		{
			DendriteBasal den = new DendriteBasal(3);
			List<Column> columns = new List<Column>();
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));

			//test connecting to Cells
			foreach (Column col in columns)
			{
				Assert.IsTrue(den.CreateSynapse(col));
			}
			//test number of connections created
			Assert.AreEqual(den.Synapses.Count, 5);

			//test no Cells are not active
			foreach (Column col in columns)
			{
				col.Update_Basal();
				Assert.IsFalse(col.IsPredicting);
			}

			den.OverridePermanence(1.0);

			//activate below threshold (2)
			columns[0].OverridePredicting(true, 0);
			columns[4].OverridePredicting(true, 0);
			den.Update();
			Assert.IsFalse(den.IsActive);

			//activate = threshold (3)
			columns[1].OverridePredicting(true, 0);
			den.Update();
			Assert.IsTrue(den.IsActive);

			//activate < threshold (2)
			columns[0].OverridePredicting(false, 0);
			den.Update();
			Assert.IsFalse(den.IsActive);

			//activate > threshold (5)
			columns[0].OverridePredicting(true, 0);
			columns[1].OverridePredicting(true, 0);
			columns[2].OverridePredicting(true, 0);
			columns[3].OverridePredicting(true, 0);
			columns[4].OverridePredicting(true, 0);
			den.Update();
			Assert.IsTrue(den.IsActive);

			//activate < threshold (2)
			columns[2].OverridePredicting(false, 0);
			columns[3].OverridePredicting(false, 0);
			columns[4].OverridePredicting(false, 0);
			den.Update();
			Assert.IsFalse(den.IsActive);
		}

		[TestMethod()]
		public void DendriteBasalMakeConnectionTest()
		{
			DendriteBasal den = new DendriteBasal(0);
			List<Column> columns = new List<Column>();
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));

			//test no connecions are present
			Assert.AreEqual(den.Synapses.Count, 0);

			//test connecting to a different Cell
			foreach (Column col in columns)
				Assert.IsTrue(den.CreateSynapse(col));

			//test all DendriteBasals are fully connected
			Assert.AreEqual(den.Synapses.Count, 10);

			//test connecting to a same Cells
			foreach (Column col in columns)
				Assert.IsFalse(den.CreateSynapse(col));

			//test ColumnConnected 
			for (int i = 0; i < columns.Count; i++)
			{
				Assert.AreSame(den.Synapses[i].ColumnConnected, columns[i]);
			}
		}

		[TestMethod()]
		public void DendriteBasalConnectionExistsTest()
		{
			DendriteBasal den = new DendriteBasal(3);
			List<Column> columns = new List<Column>();
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));
			columns.Add(new Column(0, 0, 2));


			//connect to all Cells
			foreach (Column col in columns)
			{
				Assert.IsTrue(den.CreateSynapse(col));
				Assert.IsFalse(den.CreateSynapse(col));
			}

			//test connections exist to all Cells
			foreach (Column col in columns)
			{
				Assert.IsTrue(den.ConnectionExists(col));
			}

			//connect AGAIN to all Cells
			foreach (Column col in columns)
			{
				Assert.IsFalse(den.CreateSynapse(col));
			}

			//test NO connections exist to same Cells
			for (int i = 0; i < den.Synapses.Count; i++)
				for (int j = 0; j < den.Synapses.Count; j++)
					if (i != j)
						Assert.AreNotSame(den.Synapses[i], den.Synapses[j]);
		}

		[TestMethod()]
		public void DendriteBasalOverrideTest()
		{
			//dendrite
			DendriteBasal den = new DendriteBasal(3);

			//InputPlane
			InputPlane ip = new InputPlane(5, 5);

			//create synapses
			foreach (List<Column> listCol in ip.Columns)
				foreach (Column col in listCol)
					den.CreateSynapse(col);

			//InputPlane cells not active
			ip.Override(false, false);
			den.Override(true, 0);
			foreach (List<Column> listCol in ip.Columns)
				foreach (Column col in listCol)
					Assert.IsTrue(col.IsPredicting);
			foreach (SynapseBasal syn in den.Synapses)
			{
				Assert.IsTrue(syn.IsActive);
				Assert.IsTrue(syn.ColumnConnected.IsPredicting);
			}

			//InputPlane cells active
			ip.Override(false, true);
			den.Override(false, 0);
			foreach (List<Column> listCol in ip.Columns)
				foreach (Column col in listCol)
					Assert.IsFalse(col.IsPredicting);
			foreach (SynapseBasal syn in den.Synapses)
			{
				Assert.IsFalse(syn.IsActive);
				Assert.IsFalse(syn.ColumnConnected.IsPredicting);
			}

		}


		[TestMethod()]
		public void DendriteBasalOverridePermanenceTest()
		{
			//dendrite
			DendriteBasal den = new DendriteBasal(3);

			//InputPlane
			InputPlane ip = new InputPlane(5, 5);

			//create synapses
			foreach (List<Column> listCol in ip.Columns)
				foreach (Column col in listCol)
					den.CreateSynapse(col);

			den.OverridePermanence(-5.0);
			foreach (SynapseBasal syn in den.Synapses)
				Assert.AreEqual(syn.Permanence, 0.0);

			den.OverridePermanence(0.0);
			foreach (SynapseBasal syn in den.Synapses)
				Assert.AreEqual(syn.Permanence, 0.0);

			den.OverridePermanence(0.112);
			foreach (SynapseBasal syn in den.Synapses)
				Assert.AreEqual(syn.Permanence, 0.112);

			den.OverridePermanence(9999.99);
			foreach (SynapseBasal syn in den.Synapses)
				Assert.AreEqual(syn.Permanence, 1.0);
		}


	}
}

#endif
