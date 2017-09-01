using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;

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
				int planeX = rnd.Next(1, 20);
				int planeY = rnd.Next(1, 20);
				int centreX = rnd.Next(0, planeX);
				int centreY = rnd.Next(0, planeY);
				double zoneSizePerc = rnd.NextDouble();

				bool includeCentre = true;


				InputPlane ip = new InputPlane(planeX, planeY);
				
				for (int x = 0; x < ip.NumColumnsX; x++)
				{
					for (int y = 0; y < ip.NumColumnsY; y++)
					{
						List<Column> columns = ip.GetColumnsFromCentre(x, y, zoneSizePerc, includeCentre );

						//********************************************************************
						//this logic just mimicks the function code
						//for test - run it here and compare results
						int numToCreate = (int)( (double)ip.NumColumnsX * (double)ip.NumColumnsY * zoneSizePerc - ( includeCentre ? 0 : 1 ) );

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
								if ( zoneWidth < ip.NumColumnsX )
									zoneWidth++;
							}
							else
							{
								if ( zoneHeight < ip.NumColumnsY )
									zoneHeight++;
							}
						}

						int zoneLeft = Math.Max ( Math.Min ( ip.NumColumnsX - zoneWidth, x - zoneWidth / 2 ), 0 );
						int zoneRight = Math.Min ( zoneLeft + zoneWidth - 1, ip.NumColumnsX - 1 );
						int zoneTop = Math.Max ( Math.Min ( ip.NumColumnsY - zoneHeight, y - zoneHeight / 2 ), 0 );
						int zoneBottom = Math.Min ( zoneTop + zoneHeight - 1, ip.NumColumnsY - 1 );
						//**********************************************************************

						Assert.AreEqual ( numToCreate, columns.Count );
						foreach ( Column col in columns )
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
	}
}

#endif

