using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace Cinco
{
	public class EntitySnapshotMessage
		: Message
	{
		public EntitySnapshotMessage ()
			: base (P.Protocol, 0)
		{
		}

		public List<NetworkEntity> NetworkEntities; 

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
