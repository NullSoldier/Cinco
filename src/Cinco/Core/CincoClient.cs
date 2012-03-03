using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco.Messages;
using SampleGame;
using Tempest;
using Tempest.Providers.Network;

namespace Cinco.Core
{
	public class CincoClient
		: ClientBase
	{
		public CincoClient(IClientConnection connection)
			: base (connection, MessageTypes.Reliable)
		{
			CincoProtocol.Protocol.Discover (typeof (CincoMessageBase).Assembly);
			((NetworkConnection)connection).AddProtocol (CincoProtocol.Protocol);

			this.entities = new List<NetworkEntity> ();
			this.entityMap = new Dictionary<uint, NetworkEntity>();
			
			this.RegisterMessageHandler<EntitySnapshotMessage> (OnEntitySnapshotMessage);
		}

		public void Register (NetworkEntity networkEntity)
		{
			if (networkEntity.EntityType != EntityType.Client)
				throw new ArgumentException ("You can only register client entities.");

			entities.Add (networkEntity);
			entityMap.Add (networkEntity.NetworkID, networkEntity);
		}

		public void OnEntitySnapshotMessage (MessageEventArgs<EntitySnapshotMessage> ev)
		{
			var entityMessage = ev.Message;

			foreach (SnapshotEntity entity in entityMessage.Entities)
				SyncEntity (entity.Entity);
		}

		private List<NetworkEntity> entities;
		private Dictionary<uint, NetworkEntity> entityMap; 

		private void SyncEntity (NetworkEntity entity)
		{
			var localEntity = entityMap [entity.NetworkID];
			
			foreach (var kvp in entity.Fields)
				localEntity.Fields[kvp.Key] = kvp.Value;
		}
	}
}
