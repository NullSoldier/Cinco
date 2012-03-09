using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tempest;

namespace Cinco
{
	public class Vector3Serializer
		: ISerializer<Vector3>
	{
		public void Serialize(ISerializationContext context, IValueWriter writer, Vector3 element)
		{
			writer.WriteSingle (element.X);
			writer.WriteSingle (element.Y);
			writer.WriteSingle (element.Z);
		}

		public Vector3 Deserialize(ISerializationContext context, IValueReader reader)
		{
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			float z = reader.ReadSingle();

			return new Vector3 (x, y, z);
		}

		public static readonly Vector3Serializer Instance = new Vector3Serializer();
	}
}
