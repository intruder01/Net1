using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Math;

namespace Net1.Tests
{
	/// <summary>
	/// Summary description for AlgebraTests1
	/// </summary>
	[TestClass]
	public class AlgebraTests1
	{
		
		[TestMethod]
		public void AlgebraEuclideanDistance2D()
		{
			double r1 = Algebra.EuclideanDistance2D(0, 4, 4, 3);
			Assert.AreEqual(r1, 4.1231056256176606);
			r1 = Algebra.EuclideanDistance2D(-55, 23, 14, -33);
			Assert.AreEqual(r1, 88.86506625215557);
			r1 = Algebra.EuclideanDistance2D(130, -780, -260, 111);
			Assert.AreEqual(r1, 972.6155458350437);
		}

		[TestMethod]
		public void AlgebraEuclideanDistance3D()
		{
			double r1 = Algebra.EuclideanDistance3D(1, 2, 3, 4, 5, 6);
			Assert.AreEqual(r1, 5.196152422706632);
			r1 = Algebra.EuclideanDistance3D(123, 234, 345, 456, 567, 678);
			Assert.AreEqual(r1, 576.77291892043615);
			r1 = Algebra.EuclideanDistance3D(-1231, 234, -345, 456, -567, 678);
			Assert.AreEqual(r1, 2129.3423867476081);
			r1 = Algebra.EuclideanDistance3D(12315, -2342, 345, -4565, 5672, -6723);
			Assert.AreEqual(r1, 19977.86825464619);
		}

	}
}
