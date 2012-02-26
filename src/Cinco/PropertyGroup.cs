using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinco
{
	public class PropertyGroup
	{
		public PropertyGroup (Object value, Type type)
		{
			this.Type = type;
			this.Value = value;
		}

		public Object Value;
		public Type Type;
	}
}
