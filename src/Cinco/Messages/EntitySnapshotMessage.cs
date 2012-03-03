using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadenza.Collections;
using Cinco.Core;
using Cinco.Messages;
using Tempest;

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
			throw new NotImplementedException();
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			throw new NotImplementedException();
		}
	}
}
