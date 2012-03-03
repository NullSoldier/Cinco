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
			writer.WriteInt32 (entity.Fields.Count);

			foreach (var field in entity.Fields)
			{
				writer.WriteString (field.Key);

				// Write the field type
				ushort typeID;
				context.TypeMap.GetTypeId (field.Value.Value.GetType (), out typeID);
				writer.WriteUInt16 (typeID);

				if (field.Value.Value is Vector2)
					writer.Write (context, (Vector2)field.Value.Value, new Vector2Serializer());
				else
					writer.Write (context, field.Value.Value, field.Value.Type);
			}
		}

		private NetworkEntity ReadEntity (ISerializationContext context, IValueReader reader)
		{
			var entity = new NetworkEntity (reader.ReadString (), EntityType.Client);

			int fieldCount = reader.ReadInt32 ();

			for (int f = 0; f < fieldCount; f++)
			{
				string name = reader.ReadString ();
				ushort typeID = reader.ReadUInt16();

				Type type;
				context.TypeMap.TryGetType (typeID, out type);
				
				object value;

				if (type == typeof (Vector2))
					value = reader.Read (context, new Vector2Serializer ());
				else
					value = reader.Read (context);

				entity.Fields.Add (name, new PropertyGroup (value, type));
			}

			return entity;
		}
	}
}
