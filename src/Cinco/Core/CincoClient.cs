using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

			this.snapshotManager = new SnapshotManager(10);
			this.entities = new List<NetworkEntity> ();
			this.entityMap = new Dictionary<uint, NetworkEntity>();
			this.entityTypeInformation = new Dictionary<string, EntityInformation>();
			
			Predict = false;
			Extrapolate = false;
			Interpolation = 0.3f;
			ExtrapolateAmount = 0.25f;

			this.RegisterMessageHandler<EntitySnapshotMessage> (OnEntitySnapshotMessage);
		}

		public SnapshotManager Snapshots
		{
			get { return snapshotManager; }
		}

		public bool Predict
		{
			get;
			set;
		}

		public bool Extrapolate
		{
			get;
			set;
		}

		public float Interpolation
		{
			get;
			set;
		}

		public float ExtrapolateAmount
		{
			get;
			set;
		}

		public void Register (string name, Type entityType)
		{
			if (entityType.IsAssignableFrom (typeof(NetworkEntity)))
				throw new Exception("Must be an object that inherits from Network Entity");

			ConstructorInfo constructorInfo = entityType.GetConstructors().FirstOrDefault();
			if (constructorInfo == null)
				throw new Exception ("Entity must contain a parameterless constructor");

			var entityInfo = new EntityInformation (constructorInfo);
			entityTypeInformation.Add (name, entityInfo);
		}

		public void OnEntitySnapshotMessage (MessageEventArgs<EntitySnapshotMessage> ev)
		{
			var entityMessage = ev.Message;

			foreach (SnapshotEntity entity in entityMessage.Entities)
			{
				if (!entityMap.ContainsKey(entity.Entity.NetworkID))
					CreateEntity(entity.Entity);
				else
					SyncEntity (entity.Entity);
			}
		}

		public virtual void OnEntityCreated (NetworkEntity entity)
		{
		}

		private SnapshotManager snapshotManager;
		private List<NetworkEntity> entities;
		private Dictionary<uint, NetworkEntity> entityMap;
		private Dictionary<string, EntityInformation> entityTypeInformation;

		private void CreateEntity (NetworkEntity entity)
		{
			if (!entityTypeInformation.ContainsKey(entity.EntityName))
				throw new Exception ("Entity has not been registered with the network system");

			EntityInformation info = entityTypeInformation[entity.EntityName];
			var newEntity = (NetworkEntity)info.Create();
			newEntity.NetworkID = entity.NetworkID;

			entityMap.Add(newEntity.NetworkID, newEntity);
			entities.Add(newEntity);

			SyncEntity (entity);
			OnEntityCreated (newEntity);
		}

		private void SyncEntity (NetworkEntity entity)
		{
			var localEntity = entityMap [entity.NetworkID];
			
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
		
		public object Create ()
		{
			return constructorInfo.Invoke(null);
		}

		private readonly ConstructorInfo constructorInfo;
	}
}
