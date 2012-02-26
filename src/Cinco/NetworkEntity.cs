using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinco
{
	public abstract class NetworkEntity
	{
		public NetworkEntity(string name, EntityType entityType)
		{
			this.Name = name;
			this.EntityType = entityType;
			this.Fields = new Dictionary<string, PropertyGroup>();
		}

		public string Name;
		public EntityType EntityType;
		public ChangedState ChangedState;
		public SendState SendState;
		public Dictionary<string, PropertyGroup> Fields;
		public HashSet<string> Changed;

		public void Register<T> (string name, T value)
		{
			Fields.Add (name, new PropertyGroup (value, value.GetType()));
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

		private void MarkAsChanged (string name)
		{
			if (Changed.Contains(name))
				return;

			Changed.Add(name);	
			ChangedState = ChangedState.Changed;
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
