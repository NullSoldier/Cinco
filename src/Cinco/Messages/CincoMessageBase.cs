using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace Cinco.Messages
{
	public abstract class CincoMessageBase
		: Message
	{
		public CincoMessageBase(CincoMessageTypes messageType)
			: base (CincoProtocol.Protocol, (ushort)messageType)
		{
		}
	}

	public enum CincoMessageTypes
	{
		EntitySnapshotMessage = 0,
		ServerInformationMessage = 1,
		DestroyEntityMessage = 2,
		PingMessage = 3,
		PongMessage = 4,
		TimeSyncMessage = 5
	}
}
