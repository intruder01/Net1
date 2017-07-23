using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net1
{
	internal static class RandomSampleExtensions
	{

		//usage:
		//IEnumerable<Point> inputSpaceRandomPositions = inputSpacePositions.RandomSample(synapsesPerSegment, randomSeed, false);
		public static IEnumerable<T> RandomSample<T>(this IEnumerable<T> source, int count, bool allowDuplicates)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return RandomSampleIterator<T>(source, count, allowDuplicates);
		}

		public static IEnumerable<T> RandomSampleIterator<T>(IEnumerable<T> source, int count, bool allowDuplicates)
		{
			// take a copy of the current list
			var buffer = new List<T>(source);

			Random random = Global.rnd;

			count = count <= buffer.Count ? count : buffer.Count;

			if (count > 0)
			{
				// iterate count times and "randomly" return one of the elements
				for (int i = 1; i <= count; i++)
				{
					// maximum index actually buffer.Count -1 because 
					// Random.Next will only return values LESS than specified.
					int randomIndex = random.Next(buffer.Count);
					yield return buffer[randomIndex];
					if (!allowDuplicates)
					{
						// remove the element so it can't be selected a second time
						buffer.RemoveAt(randomIndex);
					}
				}
			}
		}
		
		
	}

}
