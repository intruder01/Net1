using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

#if TESTING

namespace Net1.Tests
{
	[TestClass]
	public class InputPlaneTests
	{
		[TestMethod]
		public void InputPlaneGetColumnsFromCentreTest()
		{
			Random rnd = Global.rnd;
			NetConfigData.SetDefaults();

			for (int testNum = 0; testNum < Global.Tests.TestNumLoops; testNum++)
			{
				int planeX = rnd.Next(0, 50);
				int planeY = rnd.Next(0, 50);
				int centreX = rnd.Next(0, planeX);
				int centreY = rnd.Next(0, planeY);
				int radius = rnd.Next(0, 50 / 2);

				InputPlane ip = new InputPlane(planeX, planeY);

				List<Column> desiredResult = new List<Column>();    //correct result list for compare
				List<Column> result = ip.GetColumnsFromCentre(centreX, centreY, radius, true);

				for (int x = 0; x < ip.NumColumnsX; x++)
				{
					for (int y = 0; y < ip.NumColumnsY; y++)
					{
						Column col = ip.Columns[y][x];
						if (Algebra.EuclideanDistance2D(centreX, centreY, col.X, col.Y) <= radius)
						{
							desiredResult.Add(col);
						}
					}
				}

				//compare desired and actual lists
				Assert.AreEqual(result.Count, desiredResult.Count);
				for (int i = 0; i < desiredResult.Count; i++)
				{
					Assert.AreSame(desiredResult[i], result[i]);
				}
			}
		}
	}
}

#endif

