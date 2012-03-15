using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinco
{
	public static class NetworkEntityHelper
	{
		public static void CopyFrom (this NetworkEntity self, NetworkEntity networkEntity)
		{
			var deepCopy = networkEntity.DeepCopy ();

			foreach (var kvp in deepCopy.Fields)
				self.Fields[kvp.Key] = kvp.Value;
		}
	}
}
