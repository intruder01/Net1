using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;


namespace Net1
{
	public static class Algebra
	{
		public static double EuclideanDistance2D(int x1, int y1, int x2, int y2)
		{
			return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
		}

		public static double EuclideanDistance3D(int x1, int y1, int z1, int x2, int y2, int z2)
		{
			return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
		}

		public static double RunningAverage(double prevAverage, double newSample, int numSamples)
		{
			return ((numSamples - 1) * prevAverage + newSample) / numSamples;
		}

		public static double RunningStdDev(double prevStdDev, double newSample, int numSamples)
		{
			return ((numSamples - 1) * prevStdDev + newSample) / numSamples;
		}
	}
}
