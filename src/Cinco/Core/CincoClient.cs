using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace Cinco.Core
{
	public abstract class CincoClient
		: ClientBase
	{
		public CincoClient(IClientConnection connection)
			: base (connection, MessageTypes.Reliable)
		{
			this.RegisterMessageHandler<EntitySnapshotMessage> (OnEntitySnapshotMessage);
		}

		private void OnEntitySnapshotMessage(MessageEventArgs<EntitySnapshotMessage> ev)
		{
			throw new NotImplementedException("Client receiving snapshot not implemented");
		}
	}
}
