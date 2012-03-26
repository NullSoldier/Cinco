using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Cinco.Messages;
using Microsoft.Xna.Framework;
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

			this.snapshotManager = new SnapshotManager (10);
			this.entities = new Dictionary<uint, NetworkEntity> ();
			this.entityTypeInformation = new Dictionary<string, EntityInformation> ();
			this.entityLock = new object ();

			TickRate = new TimeSpan (0, 0, 0, 0, 15);

			this.RegisterMessageHandler<EntitySnapshotMessage> (OnEntitySnapshotMessage);
			this.RegisterMessageHandler<ServerInformationMessage> (OnServerInformationMessage);
			this.RegisterMessageHandler<DestroyEntityMessage> (OnDestroyEntityMessage);
			this.RegisterMessageHandler<CincoPingMessage> (OnPingMessageReceived);
			this.RegisterMessageHandler<TimeSyncMessage> (OnTimeSyncMessage);
		}

		public SnapshotManager Snapshots
		{
			get { return snapshotManager; }
		}

		public TimeSpan TickRate
		{
			get;
			private set;
		}

		public double Latency
		{
			get;
			private set;
		}

		public DateTime GetCurrentTime()
		{
			return DateTime.UtcNow + clockOffset;
		}

		public void Register (string name, Type entityType)
		{
			if (entityType.IsAssignableFrom (typeof (NetworkEntity)))
				throw new Exception ("Must be an object that inherits from Network Entity");

			ConstructorInfo constructorInfo = entityType.GetConstructor (Type.EmptyTypes);
			if (constructorInfo == null)
				throw new Exception ("Entity must contain a parameterless constructor");

			var entityInfo = new EntityInformation (constructorInfo);
			entityTypeInformation.Add (name, entityInfo);
		}

		private void OnEntitySnapshotMessage (MessageEventArgs<EntitySnapshotMessage> ev)
		{
			var entityMessage = ev.Message;

			foreach (SnapshotEntity entity in entityMessage.Entities)
			{
				if (!this.entities.ContainsKey (entity.Entity.NetworkID))
					CreateEntity (entity.Entity);
				else
					SyncEntity (entity.Entity);
			}
			
			OnSnapshotProcessed();
		}

		private void OnServerInformationMessage (MessageEventArgs<ServerInformationMessage> ev)
		{
			TickRate = new TimeSpan (0, 0, 0, 0, (int)ev.Message.TickRate);
		}

		private void OnPingMessageReceived (MessageEventArgs<CincoPingMessage> ev)
		{
			connection.Send (new CincoPongMessage());
		}

		private void OnTimeSyncMessage(MessageEventArgs<TimeSyncMessage> ev)
		{
			var serverTime = ev.Message.ServerClockTime;
			var adjusted = serverTime.AddMilliseconds (ev.Message.Latency);

			clockOffset = DateTime.UtcNow.Subtract(adjusted);

			Latency = ev.Message.Latency;
		}

		public void OnDestroyEntityMessage (MessageEventArgs<DestroyEntityMessage> ev)
		{
			NetworkEntity destroyedEntity;
			lock (entityLock)
			{
				destroyedEntity = entities[ev.Message.EntityID];
				entities.Remove (ev.Message.EntityID);
			}

			OnEntityDestroyed (destroyedEntity);
		}

		protected virtual void OnEntityCreated (NetworkEntity entity)
		{
		}

		protected virtual void OnEntityDestroyed (NetworkEntity entity)
		{
		}

		protected virtual void OnSnapshotProcessed ()
		{
		}

		private SnapshotManager snapshotManager;
		private Dictionary<uint, NetworkEntity> entities;
		private Dictionary<string, EntityInformation> entityTypeInformation;
		private object entityLock;
		private TimeSpan clockOffset;

		private void CreateEntity (NetworkEntity entity)
		{
			if (!entityTypeInformation.ContainsKey (entity.EntityName))
				throw new Exception ("Entity has not been registered with the network system");

			EntityInformation info = entityTypeInformation[entity.EntityName];
			var newEntity = (NetworkEntity)info.Create ();
			newEntity.NetworkID = entity.NetworkID;

			lock (entityLock)
				entities.Add (newEntity.NetworkID, newEntity);

			SyncEntity (entity);
			OnEntityCreated (newEntity);
		}

		private void SyncEntity (NetworkEntity entity)
		{
			var localEntity = entities[entity.NetworkID];

			foreach (var kvp in entity.Fields)
				localEntity.Fields[kvp.Key] = kvp.Value;
		}
	}

	public class EntityInformation
	{
		public EntityInformation (ConstructorInfo constructorInfo)
		{
			this.constructorInfo = constructorInfo;
		}

		public object Create()
		{
			return constructorInfo.Invoke (null);
		}

		private readonly ConstructorInfo constructorInfo;
	}
}
