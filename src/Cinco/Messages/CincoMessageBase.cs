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
		EntitySnapshotMessage=0
	}
}
