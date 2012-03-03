using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tempest;

namespace Cinco
{
	public class Vector2Serializer
		: ISerializer<Vector2>
	{
		public void Serialize(ISerializationContext context, IValueWriter writer, Vector2 element)
		{
			writer.WriteSingle (element.X);
			writer.WriteSingle (element.Y);
		}

		public Vector2 Deserialize(ISerializationContext context, IValueReader reader)
		{
			return new Vector2 (reader.ReadSingle (), reader.ReadSingle ());
		}
	}
}
