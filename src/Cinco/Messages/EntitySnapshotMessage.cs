using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadenza.Collections;
using Cinco.Core;
using Cinco.Messages;
using Tempest;
using Microsoft.Xna.Framework;

namespace Cinco
{
	public class EntitySnapshotMessage
		: CincoMessageBase
	{
		public EntitySnapshotMessage()
			: base (CincoMessageTypes.EntitySnapshotMessage)
		{
		}

		public EntitySnapshotMessage (Snapshot snapshot)
			: base (CincoMessageTypes.EntitySnapshotMessage)
		{
			Entities = snapshot.Entities.Values.ToList();
		}

		public List<SnapshotEntity> Entities;

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteInt32 (Entities.Count);

			foreach (SnapshotEntity sentity in Entities)
			{
				WriteEntity (sentity.Entity, context, writer);
			}
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			int entityCount = reader.ReadInt32 ();
			Entities = new List<SnapshotEntity>(entityCount);

			// Read in the entities
			for (int i = 0; i < entityCount; i++)
			{
				NetworkEntity entity = ReadEntity(context, reader);
				Entities.Add (new SnapshotEntity (entity, true));
			}
		}

		private void WriteEntity (NetworkEntity entity, ISerializationContext context, IValueWriter writer)
		{
			writer.WriteString (entity.EntityName);
			writer.WriteUInt16 ((UInt16)entity.NetworkID);
			writer.WriteUInt16 ((UInt16)entity.Fields.Count);

			foreach (var kvp in entity.Fields)
			{
				object fieldValue = kvp.Value.Value;

				writer.WriteString (kvp.Key);

				// Write the field type
				ushort typeID;
				context.TypeMap.GetTypeId (fieldValue.GetType (), out typeID);
				writer.WriteUInt16 (typeID);

				if (fieldValue is Vector2)
					writer.Write (context, (Vector2)fieldValue, Vector2Serializer.Instance);
				else if (fieldValue is Vector3)
					writer.Write (context, (Vector3)fieldValue, Vector3Serializer.Instance);
				else
					writer.Write (context, fieldValue, kvp.Value.Type);
			}
		}

		private NetworkEntity ReadEntity (ISerializationContext context, IValueReader reader)
		{
			var entity = new NetworkEntity (reader.ReadString (), EntityType.Client);

			entity.NetworkID = reader.ReadUInt16 ();
			UInt16 fieldCount = reader.ReadUInt16 ();

			for (int f = 0; f < fieldCount; f++)
			{
				string name = reader.ReadString ();
				ushort typeID = reader.ReadUInt16 ();

				Type type;
				context.TypeMap.TryGetType (typeID, out type);

				object value;

				if (type == typeof (Vector2))
					value = reader.Read (context, Vector2Serializer.Instance);
				else if (type == typeof (Vector3))
					value = reader.Read (context, Vector3Serializer.Instance);
				else
					value = reader.Read (context, type);

				entity.Fields.Add (name, new PropertyGroup (value, type));
			}

			return entity;
		}
	}
}
