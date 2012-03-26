using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinco.Core
{
	public static class MarzulloCalculater
	{
		public static double CalculateLatency(ref IndexedQueue<LatencySample> samples)
		{
			// There can only be marzullo!
			LatencyTuple[] table;

			GetLatencyTable (5, ref samples, out table);
			SortLatencyTable (ref table);

			int best = 0; // largest number of overlapping intervals found
			int count = 0; // Current number of overlapping intervals
			LatencyTuple bestStart = table[0]; // The beginning of the best interval
			LatencyTuple bestEnd = table[0]; // The end of the best interval

			for (int i = 0; i < table.Length; i++)
			{
				count -= table[i].Type;

				if (count > best)
				{
					best = count;
					bestStart = table[i];
					bestEnd = table[i + 1];
				}
			}

			return (bestStart.Offset + bestEnd.Offset) / 2;
		}

		private static void GetLatencyTable(int sampleCount, ref IndexedQueue<LatencySample> samples, out LatencyTuple[] table)
		{
			table = new LatencyTuple[sampleCount * 2];

			for (int i = 0; i < 5; i+=2)
			{
				LatencySample sample = samples[i];
				table[i] = new LatencyTuple (sample.LowValue, -1);
				table[i+1] = new LatencyTuple (sample.HighValue, +1);
			}
		}

		private static void SortLatencyTable (ref LatencyTuple[] table)
		{
			table = table.OrderBy (i => i.Offset).ToArray();
		}

		private struct LatencyTuple
		{
			public LatencyTuple(double offset, int type)
			{
				Offset = offset;
				Type = type;
			}

			public readonly double Offset;
			public readonly int Type;
		}
	}
}
