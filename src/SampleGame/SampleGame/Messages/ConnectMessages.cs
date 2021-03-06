﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace SampleGame.Messages
{

	public class ConnectMessage
		: SampleBaseMessage
	{
		public ConnectMessage()
			: base (SampleMessageTypes.ConnectMessage)
		{
		}

		public ConnectMessage(string playerName)
			: base (SampleMessageTypes.ConnectMessage)
		{
			this.PlayerName = playerName;
		}

		public string PlayerName
		{
			get;
			set;
		}

		public override void WritePayload(ISerializationContext context, IValueWriter writer)
		{
			writer.WriteString (this.PlayerName);
		}

		public override void ReadPayload(ISerializationContext context, IValueReader reader)
		{
			this.PlayerName = reader.ReadString ();
		}
	}
}
