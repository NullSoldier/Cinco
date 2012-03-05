using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SampleGame
{
	public static class EnumHelpers
	{
		public static bool Contains(this Enum keys, Enum flag)
		{
			ulong keysVal = Convert.ToUInt64(keys);
			ulong flagVal = Convert.ToUInt64(flag);

			return (keysVal & flagVal) == flagVal;
		}
	}

	public static class Helpers
	{
		public static int LowerClamp(int value, int minimum)
		{
			if (value < minimum)
				return minimum;

			return value;
		}

		public static string ToOnOff (this bool self)
		{
			return self ? "On" : "Off";
		}
	}

	public static class VectorHelpers
	{
		public static Vector2 CreateLook(float radians)
		{
			return new Vector2 ((float)Math.Cos (radians), (float)Math.Sin (radians));
		}
	}

	public static class DateTimeHelpers
	{
		public static DateTime SubtractMilliseconds(this DateTime self, int milliseconds)
		{
			return self.Subtract (new TimeSpan (0, 0, 0, 0, milliseconds));
		}
	}
}
