using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Math;

namespace Net1.Tests
{
	/// <summary>
	/// Summary description for ParamSearch functions
	/// </summary>
	[TestClass]
	public class ParamSearchTests
	{
		[TestMethod]
		public void ParamSearch_FindOptimalCVTest()
		{
			//attempt to find longest range where PV=2 with CV range 0.0 - 1.0 step 0.1
			ParamSearch ps = new ParamSearch(2, 0.0, 1.0, 0.1, 1, ParamSearchMode.Exact, true);
			Assert.IsTrue(Math.Abs(ps.Update(1) - 0.1) < 0.00001);	//note: ret value is always 1 step higher than stored
			Assert.IsTrue(Math.Abs(ps.Update(3) - 0.2) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(2) - 0.3) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(2) - 0.4) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(4) - 0.5) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(2) - 0.6) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(2) - 0.7) < 0.00001);  //<- this should be the middle range for best stability: CV=0.6
			Assert.IsTrue(Math.Abs(ps.Update(2) - 0.8) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(0) - 0.9) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(5) - 1.0) < 0.00001);

			// at this point, ParamSearch should have built the response curve and be ready to return best CV

			double bestCV = ps.Update(1.0);     //here actual search result should be returned
			Assert.IsTrue(Math.Abs(bestCV - 0.6) < 0.00001);
		}

		[TestMethod]
		public void ParamSearch_FindOptimalCV_ClosestHigher_Test()
		{
			//attempt to find longest range where PV=10 with CV range 0.0 - 1.0 step 0.1
			//test for exact range not found and closest higher selected
			ParamSearch ps = new ParamSearch(10, 0.0, 1.0, 0.1, 1, ParamSearchMode.Higher, true);
			Assert.IsTrue(Math.Abs(ps.Update(20) - 0.1) < 0.00001); //note: ret value is always 1 step higher than stored
			Assert.IsTrue(Math.Abs(ps.Update(1) - 0.2) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(12) - 0.3) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(11) - 0.4) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(15) - 0.5) < 0.00001); //<- this should be the closest higher than 10: CV=0.4
			Assert.IsTrue(Math.Abs(ps.Update(12) - 0.6) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(9) - 0.7) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(20) - 0.8) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(20) - 0.9) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(5) - 1.0) < 0.00001);

			// at this point, ParamSearch should have built the response curve and be ready to return best CV

			double bestCV = ps.Update(1.0);     //here actual search result should be returned
			Assert.IsTrue(Math.Abs(bestCV - 0.4) < 0.00001);
		}

		[TestMethod]
		public void ParamSearch_FindOptimalCV_ClosestLower_Test()
		{
			//attempt to find longest range where PV=5 with CV range 0.0 - 1.0 step 0.1
			//test for exact range not found and closest higher selected
			ParamSearch ps = new ParamSearch(5, 0.0, 1.0, 0.1, 1, ParamSearchMode.Lower, true);
			Assert.IsTrue(Math.Abs(ps.Update(20) - 0.1) < 0.00001); //note: ret value is always 1 step higher than stored
			Assert.IsTrue(Math.Abs(ps.Update(1) - 0.2) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(1) - 0.3) < 0.00001);  
			Assert.IsTrue(Math.Abs(ps.Update(3) - 0.4) < 0.00001);  //<- this should be the closest lower than 5: CV=0.3
			Assert.IsTrue(Math.Abs(ps.Update(4) - 0.5) < 0.00001); 
			Assert.IsTrue(Math.Abs(ps.Update(6) - 0.6) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(9) - 0.7) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(20) - 0.8) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(20) - 0.9) < 0.00001);
			Assert.IsTrue(Math.Abs(ps.Update(6) - 1.0) < 0.00001);

			// at this point, ParamSearch should have built the response curve and be ready to return best CV

			double bestCV = ps.Update(1.0);     //here actual search result should be returned
			Assert.IsTrue(Math.Abs(bestCV - 0.3) < 0.00001);
		}
	}
}
