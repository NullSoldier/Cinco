using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace SampleGame.Messages
{
	public abstract class BaseMessage
		: Message
	{
		public BaseMessage(NetworkMessageTypes messageType)
			: base (P.Protocol, (ushort)messageType)
		{
		}
	}

	public enum NetworkMessageTypes
	{
		ConnectMessage = 1,
		ConnectReplyMessage = 2,
		PlayerInfoMessage = 3,
		PlayersSnapshotMessage = 4,
		RequestAllPlayersMessage = 5,
		RequestCharacterMessage = 6,
		UserCommandMessage = 7,
		UserChatMessage = 8
	}
}
