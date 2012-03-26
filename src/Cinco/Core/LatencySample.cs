using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinco.Core
{
	public struct LatencySample
	{
		public LatencySample(double latency, double range)
		{
			this.Latency = latency;
			this.Range = range;
			this.HighValue = Latency + Range;
			this.LowValue = Latency - Range;
		}

		public readonly double Latency;
		public readonly double Range;
		public readonly double HighValue;
		public readonly double LowValue;
	}
}
