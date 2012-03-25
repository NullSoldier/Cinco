using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace Cinco.Messages
{
	public class PongMessage
		: CincoMessageBase
	{
		public PongMessage()
			: base (CincoMessageTypes.PongMessage)
		{
		}

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
		}
	}
}
