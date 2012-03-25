using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace Cinco.Messages
{
	public class PingMessage
		: CincoMessageBase
	{
		public PingMessage()
			: base (CincoMessageTypes.PingMessage)
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
