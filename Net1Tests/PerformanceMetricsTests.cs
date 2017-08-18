using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Net1.Tests
{

	[TestClass]
	public class PerformanceMetricsTests
	{
		[TestMethod]
		public void PerformanceMetrics_CalcSparsenessTests ()
		{
			//note: this network and layer are separate
			//but that is ok for testing purposes
			Network net = new Network ();
			Layer lr = new Layer ( 10, 10, 3 );				//10x10 = 100 columns
			lr.Columns[0][0].OverrideActive ( true, 0 );	//activate 1 column
			double sparseness = net.Metrics.CalcSparseness ( lr );
			Assert.AreEqual ( sparseness, 0.01 );			//sparseness should be 0.01
			lr.Columns[1][2].OverrideActive ( true, 0 );
			lr.Columns[2][2].OverrideActive ( true, 0 );
			lr.Columns[3][2].OverrideActive ( true, 0 );
			lr.Columns[4][2].OverrideActive ( true, 0 );
			sparseness = net.Metrics.CalcSparseness ( lr );
			Assert.AreEqual ( sparseness, 0.05 );           //5 columns active, sparseness should be 0.05
			lr.Columns[1][5].OverrideActive ( true, 0 );
			lr.Columns[2][5].OverrideActive ( true, 0 );
			lr.Columns[3][5].OverrideActive ( true, 0 );
			lr.Columns[4][5].OverrideActive ( true, 0 );
			lr.Columns[9][9].OverrideActive ( true, 0 );
			sparseness = net.Metrics.CalcSparseness ( lr );
			Assert.AreEqual ( sparseness, 0.1 );            //10 columns active, sparseness should be 0.1
		}
	}
}
