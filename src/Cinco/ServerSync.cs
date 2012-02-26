using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cinco;
using Tempest;
using Tempest.InternalProtocol;

namespace SampleGame
{
	public partial class ServerSync
		: ServerBase
	{
		public ServerSync (IConnectionProvider provider)
			: base (provider, MessageTypes.Reliable)
		{
			connections = new HashSet<IConnection>();
			userLock = new object ();

			this.RegisterMessageHandler<EntitySnapshotMessage> (OnEntitySnapshotMessage);
		}

		private HashSet<IConnection> connections;
		private int lastEntityID;
		private object userLock;

		private void OnEntitySnapshotMessage(MessageEventArgs<EntitySnapshotMessage> e)
		{
		
		}
	}
}
