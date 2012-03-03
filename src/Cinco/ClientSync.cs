using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco;
using Cinco.Core;
using Tempest;

namespace SampleGame
{
	public class ClientSync
	{
		public ClientSync (CincoClient client)
		{
			this.entities = new List<NetworkEntity> ();
			this.entityMap = new Dictionary<uint, NetworkEntity>();
			
			client.RegisterMessageHandler<EntitySnapshotMessage> (OnEntitySnapshotMessage);
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

/*
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
				Entities = sendEntities
			};
			client.Connection.Send (message);
		}
*/