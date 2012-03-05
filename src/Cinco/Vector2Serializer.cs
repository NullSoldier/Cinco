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
			writer.WriteInt32 ((int)element.X);
			writer.WriteInt32 ((int)element.Y);
		}

		public Vector2 Deserialize(ISerializationContext context, IValueReader reader)
		{
			float x = reader.ReadInt32();
			float y = reader.ReadInt32();

			return new Vector2 (x, y);
		}

		public static readonly Vector2Serializer Instance = new Vector2Serializer();
	}
}
