using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadenza.Collections;

namespace Cinco.Core
{
	public class Snapshot
	{
		public Snapshot()
		{
			entities = new Dictionary<uint, SnapshotEntity> ();
			Entities = new ReadOnlyDictionary<uint, SnapshotEntity> (this.entities);
		}

		public DateTime Taken;
		public ReadOnlyDictionary<uint, SnapshotEntity> Entities;

		public void AddEntity (NetworkEntity entity, bool changed)
		{
			entities.Add (entity.NetworkID, new SnapshotEntity (entity.DeepCopy(), changed));
		}

		public SnapshotEntity GetEntity (uint networkID)
		{
			if (!entities.ContainsKey (networkID))
				return null;

			return entities[networkID];
		}

		private Dictionary<uint, SnapshotEntity> entities;
	}

	public class SnapshotEntity
	{
		public SnapshotEntity (NetworkEntity entity, bool changed)
		{
			Entity = entity;
			Changed = changed;
		}

		public NetworkEntity Entity;
		public bool Changed;
	}
}
