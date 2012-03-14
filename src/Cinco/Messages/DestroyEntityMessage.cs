using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace Cinco.Messages
{
	public class DestroyEntityMessage
		: CincoMessageBase
	{
		public DestroyEntityMessage()
			: base (CincoMessageTypes.DestroyEntityMessage)
		{
		}

		public DestroyEntityMessage (uint entityID)
			: base (CincoMessageTypes.DestroyEntityMessage)
		{
			this.EntityID = entityID;
		}

		public uint EntityID;

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteUInt32 (EntityID);
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			EntityID = reader.ReadUInt32 ();
		}
	}
}
