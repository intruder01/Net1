using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net1
{
	internal static class RandomExtensions
	{
		public static double NextDouble(this Random rnd, double minValue, double maxValue)
		{
			return rnd.NextDouble() * (maxValue - minValue) + minValue;
		}
	}
}
