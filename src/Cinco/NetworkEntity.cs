using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinco
{
	public class NetworkEntity
	{
		public NetworkEntity(string entityName, EntityType entityType)
		{
			this.EntityName = entityName;
			this.EntityType = entityType;
			this.Fields = new Dictionary<string, PropertyGroup>();
			this.Changed = new HashSet<string>();
			this.SyncLock = new object ();
		}

		public uint NetworkID;
		public string EntityName;
		public EntityType EntityType;
		public ChangedState ChangedState;
		public SendState SendState;
		public Dictionary<string, PropertyGroup> Fields;
		public HashSet<string> Changed;
		public object SyncLock;

		public void Register<T> (string name, T value)
		{
			lock (SyncLock)
				Fields.Add (name, new PropertyGroup (value, value.GetType()));
		}

		public NetworkEntity DeepCopy()
		{
			throw new NotImplementedException();

			NetworkEntity newCopy = new NetworkEntity (EntityName, EntityType);

			return newCopy;
		}

		public object this[string name]
		{
			get { return this.Fields[name].Value; }
			set
			{
				Fields[name].Value = value;
				MarkAsChanged (name);
			}
		}

		public void ClearChanged()
		{
			lock (SyncLock)
			{
				ChangedState = ChangedState.None;
				Changed.Clear();
			}
		}

		private void MarkAsChanged (string name)
		{
			lock (SyncLock)
			{
				if (Changed.Contains (name))
					return;

				Changed.Add (name);
				ChangedState = ChangedState.Changed;
			}
		}
	}

	public enum EntityType
	{
		Server,
		Client
	}

	public enum ChangedState
	{
		None,
		Changed
	}

	public enum SendState
	{
		Always,
		Never
	}
}
