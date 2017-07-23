using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if TESTING

namespace Net1.Tests
{
	[TestClass]
	public class SynapseProximalTests
	{
		[TestMethod]
		public void SynapseProximalCtorTest()
		{
			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Column col = new Column(0, 0, 1);
				SynapseProximal syn = new SynapseProximal(col);
				Assert.AreSame(syn.ColumnConnected, col);
				Assert.IsTrue(syn.Permanence >= 0.0);
				Assert.IsTrue(syn.Permanence <= 1.0);
				Assert.IsFalse(syn.IsActive);
			}
		}

		[TestMethod]
		public void SynapseProximalOverrideTest()
		{
			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Column col = new Column(0, 0, 3);
				SynapseProximal syn = new SynapseProximal(col);
				syn.OverridePermanence(1.0);

				//test override active 
				syn.Override(true, 0);
				Assert.IsTrue(syn.IsActive);
				Assert.IsTrue(col.Cells[0].IsActive);
				Assert.IsTrue(col.Cells[1].IsActive);
				Assert.IsTrue(col.Cells[2].IsActive);

				syn.Override(false, 0);
				Assert.IsFalse(syn.IsActive);
				Assert.IsFalse(col.Cells[0].IsActive);
				Assert.IsFalse(col.Cells[1].IsActive);
				Assert.IsFalse(col.Cells[2].IsActive);

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
				Assert.IsTrue(col.IsActive);
				syn.Override(false, 0);
				Assert.IsFalse(col.IsActive);

			}
		}

		[TestMethod]
		public void SynapseProximalUpdateTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				Column col = new Column(0, 0, rnd.Next(1, 10));
				SynapseProximal syn = new SynapseProximal(col);

				//active column
				col.OverrideActive(true, 0);
				syn.OverridePermanence(NetConfigData.SynapsePermanenceThreshold + NetConfigData.SynapsePermanenceIncrease);
				syn.Update();
				Assert.IsTrue(syn.IsActive);

				//drop permanence
				col.OverrideActive(true, 0);
				syn.OverridePermanence(NetConfigData.SynapsePermanenceThreshold - 2 * NetConfigData.SynapsePermanenceIncrease);
				syn.Update();
				Assert.IsFalse(syn.IsActive);	//false on first update
				syn.Update();
				Assert.IsTrue(syn.IsActive);	//true on second update

				//column not active
				col.OverrideActive(false, 0);
				syn.OverridePermanence(NetConfigData.SynapsePermanenceThreshold + NetConfigData.SynapsePermanenceIncrease);
				syn.Update();
				Assert.IsFalse(syn.IsActive);

				//column active
				col.OverrideActive(true, 0);
				syn.OverridePermanence(NetConfigData.SynapsePermanenceThreshold + NetConfigData.SynapsePermanenceIncrease);
				syn.Update();
				Assert.IsTrue(syn.IsActive);

				//random permanence, column active
				col.OverrideActive(true, 0);
				syn.OverridePermanence(rnd.NextDouble());
				syn.Update();
				if(syn.Permanence >= NetConfigData.SynapsePermanenceThreshold)
					Assert.IsTrue(syn.IsActive);
				else
					Assert.IsFalse(syn.IsActive);

				//random permanence, column not active
				col.OverrideActive(false, 0);
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

