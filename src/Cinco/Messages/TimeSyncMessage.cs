using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace Cinco.Messages
{
	public class TimeSyncMessage
		: CincoMessageBase
	{
		public TimeSyncMessage()
			: base(CincoMessageTypes.TimeSyncMessage)
		{
		}

		public TimeSyncMessage (DateTime currentTime, double latency)
			: base (CincoMessageTypes.TimeSyncMessage)
		{
			this.ServerClockTime = currentTime;
			this.Latency = latency;
		}

		public DateTime ServerClockTime;
		public double Latency;

		public override void WritePayload (ISerializationContext context, IValueWriter writer)
		{
			writer.WriteUniversalDate (ServerClockTime);
			writer.WriteDouble (Latency);
		}

		public override void ReadPayload (ISerializationContext context, IValueReader reader)
		{
			ServerClockTime = reader.ReadUniversalDate();
			Latency = reader.ReadDouble();
		}
	}
}
