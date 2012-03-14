using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace SampleGame.Messages
{
	public abstract class SampleBaseMessage
		: Message
	{
		public SampleBaseMessage (SampleMessageTypes messageType)
			: base (P.Protocol, (ushort)messageType)
		{
		}
	}

	public enum SampleMessageTypes
	{
		ConnectMessage = 1,
		MoveMessage = 2
	}
}
