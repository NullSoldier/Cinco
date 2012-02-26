using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco;
using Tempest;

namespace SampleGame
{
	public class ClientSync
		: ClientBase
	{
		public ClientSync (IClientConnection connection)
 			: base (connection, MessageTypes.Reliable, false)
		{
			entities = new List<NetworkEntity> ();
			entityMap = new Dictionary<int, NetworkEntity>();
			
			this.RegisterMessageHandler<EntitySnapshotMessage> (OnEntitySnapshotMessage);
		}

		public void Register (NetworkEntity networkEntity)
		{
			if (networkEntity.EntityType != EntityType.Client)
				throw new ArgumentException ("You can only register client entities.");

			entities.Add (networkEntity);
			entityMap.Add (networkEntity.NetworkID, networkEntity);
		}
		
		public void Send()
		{
			var sendEntities = new List<NetworkEntity>();

			foreach (NetworkEntity entity in this.entities)
			{
				if (entity.SendState == SendState.Never
					|| entity.ChangedState == ChangedState.None)
					continue;

				sendEntities.Add (entity);
				entity.ChangedState = ChangedState.None;
			}

			// No entities to send? Return
			if (sendEntities.Count <= 0)
				return;

			var message = new EntitySnapshotMessage
			{
				NetworkEntities = sendEntities
			};
			connection.Send (message);
		}

		public void OnEntitySnapshotMessage (MessageEventArgs<EntitySnapshotMessage> ev)
		{
			var entityMessage = ev.Message;

			foreach (NetworkEntity entity in entityMessage.NetworkEntities)
				SyncEntity (entity);
		}

		private List<NetworkEntity> entities;
		private Dictionary<int, NetworkEntity> entityMap; 

		private void SyncEntity (NetworkEntity entity)
		{
			var localEntity = entityMap [entity.NetworkID];
			
			foreach (var kvp in entity.Fields)
				localEntity.Fields[kvp.Key] = kvp.Value;
		}
	}
}
