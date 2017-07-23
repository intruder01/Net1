using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if TESTING

namespace Net1.Tests
{
	[TestClass]
	public class SynapseBasalTests
	{
		[TestMethod]
		public void SynapseBasalCtorTest()
		{
			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Column col = new Column(0, 0, 1);
				SynapseBasal syn = new SynapseBasal(col);
				Assert.AreSame(syn.ColumnConnected, col);
				Assert.IsTrue(syn.Permanence >= 0.0);
				Assert.IsTrue(syn.Permanence <= 1.0);
				Assert.IsFalse(syn.IsActive);
			}
		}

		[TestMethod]
		public void SynapseBasalOverrideTest()
		{
			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Column col = new Column(0, 0, 3);
				SynapseBasal syn = new SynapseBasal(col);
				syn.OverridePermanence(1.0);

				//test override active 
				syn.Override(true, 0);
				Assert.IsTrue(syn.IsActive);
				Assert.IsTrue(col.Cells[0].IsPredicting);
				Assert.IsTrue(col.Cells[1].IsPredicting);
				Assert.IsTrue(col.Cells[2].IsPredicting);

				syn.Override(false, 0);
				Assert.IsFalse(syn.IsActive);
				Assert.IsFalse(col.Cells[0].IsPredicting);
				Assert.IsFalse(col.Cells[1].IsPredicting);
				Assert.IsFalse(col.Cells[2].IsPredicting);

				//test override permanence
				syn.OverridePermanence(-0.5);
				Assert.IsTrue(syn.Permanence == 0);
				syn.OverridePermanence(9999.99);
				Assert.IsTrue(syn.Permanence == 1.0);
				syn.OverridePermanence(NetConfigData.SynapsePermanenceThreshold + 0.01); 
				Assert.IsTrue(syn.Permanence > NetConfigData.SynapsePermanenceThreshold);
				Assert.IsTrue(syn.Permanence <= 1.0);

				//test override of connected column
				syn.Override(true, 0);
				Assert.IsTrue(col.IsPredicting);
				syn.Override(false, 0);
				Assert.IsFalse(col.IsPredicting);
			}
		}

		[TestMethod]
		public void SynapseBasalUpdateTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Column col = new Column(0, 0, rnd.Next(1, 10));
				SynapseBasal syn = new SynapseBasal(col);

				//predicting column
				col.OverridePredicting(true, 0);
				syn.OverridePermanence(NetConfigData.SynapsePermanenceThreshold + NetConfigData.SynapsePermanenceIncrease);
				syn.Update();
				Assert.IsTrue(syn.IsActive); 

				//drop permanence
				col.OverridePredicting(true, 0);
				syn.OverridePermanence(NetConfigData.SynapsePermanenceThreshold - 2 * NetConfigData.SynapsePermanenceIncrease);
				syn.Update();
				Assert.IsFalse(syn.IsActive);   //false on first update
				syn.Update();
				Assert.IsTrue(syn.IsActive);    //true on second update

				//column not predicting
				col.OverridePredicting(false, 0);
				syn.OverridePermanence(NetConfigData.SynapsePermanenceThreshold + NetConfigData.SynapsePermanenceIncrease);
				syn.Update();
				Assert.IsFalse(syn.IsActive);

				//column predicting
				col.OverridePredicting(true, 0);
				syn.OverridePermanence(NetConfigData.SynapsePermanenceThreshold + NetConfigData.SynapsePermanenceIncrease);
				syn.Update();
				Assert.IsTrue(syn.IsActive);

				//random permanence, column active
				col.OverridePredicting(true, 0);
				syn.OverridePermanence(rnd.NextDouble());
				syn.Update();
				if (syn.Permanence >= NetConfigData.SynapsePermanenceThreshold)
					Assert.IsTrue(syn.IsActive);
				else
					Assert.IsFalse(syn.IsActive);

				//random permanence, column not active
				col.OverridePredicting(false, 0);
				syn.OverridePermanence(rnd.NextDouble());
				syn.Update();
				if (syn.Permanence >= NetConfigData.SynapsePermanenceThreshold)
					Assert.IsFalse(syn.IsActive);
				else
					Assert.IsFalse(syn.IsActive);

			}
		}
	}
}

#endif

