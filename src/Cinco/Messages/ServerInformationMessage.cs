using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace Cinco.Messages
{
	public class ServerInformationMessage
		: CincoMessageBase
	{
		public ServerInformationMessage()
			: base (CincoMessageTypes.ServerInformationMessage)
		{
		}

		public ServerInformationMessage (float millisecondTickRate)
			: base (CincoMessageTypes.ServerInformationMessage)
		{
			this.TickRate = millisecondTickRate;
		}

		public float TickRate;

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteSingle (TickRate);
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			TickRate = reader.ReadSingle();
		}
	}
}
