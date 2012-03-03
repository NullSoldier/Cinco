using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cinco.Core;
using SampleGame;
using Tempest;
using Tempest.InternalProtocol;

namespace Cinco
{
	public class CincoServer
		: ServerBase
	{
		public CincoServer (IConnectionProvider connectionProvider, ServerOptions serverOptions)
			: base (connectionProvider, MessageTypes.All) 
		{
			this.options = serverOptions;
			this.users = new Dictionary<IConnection, CincoUser> (options.MaxUsers);
			this.userLock = new object();
			this.syncLock = new object();

			// TODO: Recalculate on server options changing
			tickDelay = (1 / options.TickRate) * 1000;
			snapshotDelay = (1 / options.HistoryRate) * 1000;

			StartUpdateThread();
		}

		public void RegisterEntity (NetworkEntity entity)
		{
			entity.NetworkID = ++lastEntityID;

			entities.Add (entity.NetworkID, entity);
		}

		public void UnregisterEntity (uint entityID)
		{
			entities.Remove (entityID);
		}

		public virtual void Tick (DateTime dateTime)
		{
		}

		public virtual void Update()
		{
			// Tick the game if it needs it
			// Send snapshots toc lients
			// Enqueue history with snapshot
			
			while (true)
			{
				DateTime currentTime = DateTime.Now;

				if ((currentTime - lastTickTime).TotalMilliseconds >= tickDelay)
				{
					Tick (currentTime);
					lastTickTime = currentTime;
				}

				// Send snapshots to the clients who need them
				Snapshot snapshot = null;
				lock (syncLock)
				{
					foreach (CincoUser user in users.Values)
					{
						if (!user.IsActive)
							continue;

						if ((user.NeedsSnapshot (currentTime)))
						{
							if (snapshot == null)
								snapshot = GetSnapshot (false);

							snapshot.Taken = currentTime;
							user.SendSnapshot (snapshot, currentTime);
						}
					}
				}

				// Enqueue the snapshot in history if we need it
				if ((currentTime - lastTickTime).TotalMilliseconds >= snapshotDelay)
				{
					if (snapshot == null)
						snapshot = GetSnapshot (true);

					history.Enqueue (snapshot);
				}

				Thread.Sleep (1);
			}
		}

		protected override void OnConnectionMade (object sender, ConnectionMadeEventArgs e)
		{
			var cincoUser = new CincoUser { Connection = e.Connection };

			lock (userLock)
			{
				this.users.Add (e.Connection, cincoUser);
			}

			base.OnConnectionMade (sender, e);
		}

		protected override void OnConnectionDisconnected(object sender, DisconnectedEventArgs e)
		{
			lock (userLock)
			{
				this.users.Remove (e.Connection);
			}

			base.OnConnectionDisconnected (sender, e);
		}

		private Dictionary<IConnection, CincoUser> users;
		private Dictionary<uint, NetworkEntity> entities;
		private Thread updateThread;
		private ServerOptions options;
		private Queue<Snapshot> history;
		private uint lastEntityID;
		private int userCount;
		private object userLock;
		private object syncLock;

		private float tickDelay;
		private float snapshotDelay;
		private DateTime lastTickTime;

		private void StartUpdateThread()
		{
			updateThread = new Thread (UpdateRunner);
			updateThread.Name = "Update Thread";
			updateThread.Start ();
		}

		private void UpdateRunner()
		{
			Update ();
		}

		private Snapshot GetSnapshot (bool fullSnapshot)
		{
			Snapshot snapshot = new Snapshot();

			foreach (NetworkEntity entity in entities.Values)
			{
				switch (entity.ChangedState)
				{
					case ChangedState.Changed:
						snapshot.AddEntity (entity, true);
						break;

					case ChangedState.None:
						if (!fullSnapshot)
							snapshot.AddEntity (entity, false);

						break;
				}
			}

			return snapshot;
		}
	}
}
