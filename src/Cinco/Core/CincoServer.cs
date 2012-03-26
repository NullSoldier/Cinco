using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cinco.Core;
using Cinco.Messages;
using SampleGame;
using Tempest;
using Tempest.InternalProtocol;
using Tempest.Providers.Network;

namespace Cinco
{
	public class CincoServer
		: ServerBase
	{
		public CincoServer(IConnectionProvider connectionProvider, ServerOptions serverOptions)
			: base (connectionProvider, MessageTypes.Reliable)
		{
			CincoProtocol.Protocol.Discover (typeof (CincoMessageBase).Assembly);

			this.options = serverOptions;
			this.users = new Dictionary<IConnection, CincoUser> (options.MaxUsers);
			this.pingMap = new Dictionary<IConnection, PingContainer>();
			this.pingQueue = new Queue<PingContainer>();
			this.pongMessageQueue = new Queue<MessageEventArgs<CincoPongMessage>>();
			this.entities = new Dictionary<uint, NetworkEntity> ();
			this.userLock = new object ();
			this.entityLock = new object ();

			// TODO: Recalculate on server options changing
			tickDelay = (1 / options.TickRate) * 1000;
			snapshotDelay = (1 / options.HistoryRate) * 1000;
			pingDelay = 5f;
			
			this.RegisterMessageHandler<CincoPongMessage> (OnPongMessageReceived);

			StartRunnerThreads ();
		}

		public void RegisterEntity(NetworkEntity entity)
		{
			lock (entityLock)
			{
				entity.NetworkID = ++lastEntityID;
				entities.Add (entity.NetworkID, entity);
			}
		}

		public void UnregisterEntity(uint entityID)
		{
			lock (entityLock)
				entities.Remove (entityID);
		}

		public virtual void Tick(DateTime dateTime)
		{
		}

		public virtual void Update()
		{
			DateTime currentTime = DateTime.Now;
			TimeSpan difference = currentTime - lastTickTime;

			if (difference.TotalMilliseconds >= tickDelay)
			{
				lostTime = difference.TotalMilliseconds - tickDelay;

				while (lostTime >= tickDelay)
				{
					lostTime -= tickDelay;
					catchupTicks++;
				}

				Tick (currentTime);
				lastTickTime = currentTime;
			}

			if (catchupTicks > 0)
			{
				while (catchupTicks > 0)
				{
					Tick (currentTime);
					catchupTicks--;
				}
			}

			// Send snapshots to the clients who need them
			Snapshot snapshot = null;
			lock (userLock)
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

		public CincoUser RegisterUser(IConnection connection)
		{
			var cincoUser = new CincoUser
			{
				Connection = connection,
				TickRate = options.TickRate,
				UpdateRate = options.UpdateRate,
				IsActive = true,
			};

			lock (userLock)
			{
				users.Add (connection, cincoUser);

				// Add ping tracking info
				pingMap.Add (connection, new PingContainer (cincoUser));
				pingQueue.Enqueue (pingMap[connection]);
			}

			SendServerInformation (cincoUser);

			return cincoUser;
		}

		protected override void OnConnectionDisconnected(object sender, DisconnectedEventArgs e)
		{
			lock (userLock)
			{
				users.Remove (e.Connection);
				pingMap.Remove (e.Connection);
			}

			base.OnConnectionDisconnected (sender, e);
		}

		private Dictionary<IConnection, CincoUser> users;
		private Dictionary<uint, NetworkEntity> entities;
		private Dictionary<IConnection, PingContainer> pingMap;
		private Thread updateThread;
		private Thread pingThread;
		private ServerOptions options;
		private Queue<Snapshot> history;
		private Queue<PingContainer> pingQueue;
		private Queue<MessageEventArgs<CincoPongMessage>> pongMessageQueue;
		private uint lastEntityID;
		private int userCount;
		private object userLock;
		private object entityLock;

		private float tickDelay;
		private float snapshotDelay;
		private float pingDelay;
		private DateTime lastTickTime;

		private double lostTime;
		private int catchupTicks;

		private void StartRunnerThreads()
		{
			updateThread = new Thread (UpdateRunner);
			updateThread.Name = "Update Thread";
			updateThread.Start ();

			pingThread = new Thread (PingRunner);
			pingThread.Name = "Ping Thread";
			pingThread.Start ();
		}

		private void UpdateRunner()
		{
			lastTickTime = DateTime.Now;

			while (true)
				Update();
		}

		private void PingRunner()
		{
			int index = 0;
			int maxCheck = 5;
			DateTime currentTime;

			while (true)
			{
				// No pings to process
				if (pingQueue.Count == 0)
				{
					Thread.Sleep (1);
					continue;
				}

				currentTime = DateTime.Now;

				for (index = 0; index < maxCheck; index++)
				{
					// If the user is no longer connected, disgard their ping data
					if (!users.ContainsKey (pingQueue.Peek ().User.Connection))
					{
						pingQueue.Dequeue ();
						continue;
					}

					// Is it time to ping yet?
					if (currentTime.Subtract (pingQueue.Peek ().PingTime).TotalSeconds < pingDelay)
						break; // Quit early, There's no more to do

					var pingTuple = pingQueue.Dequeue ();
					pingTuple.PingTime = currentTime;
					pingTuple.User.Connection.Send (new CincoPingMessage());
					pingQueue.Enqueue (pingTuple);
				}

				while (pongMessageQueue.Count > 0)
				{
					var messageEventArgs = pongMessageQueue.Dequeue();

					if (messageEventArgs == null)
						continue;

					var connection = messageEventArgs.Connection;
					double latency = currentTime.Subtract (pingMap[connection].PingTime).TotalMilliseconds;
					pingMap[connection].User.LatencySamples.Enqueue (latency);
				}
			}
		}

		private Snapshot GetSnapshot(bool fullSnapshot)
		{
			Snapshot snapshot = new Snapshot ();

			lock (entityLock)
			{
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
			}

			return snapshot;
		}

		private void SendServerInformation(CincoUser user)
		{
			user.Connection.Send (new ServerInformationMessage (tickDelay));
		}

		private void OnPongMessageReceived (MessageEventArgs<CincoPongMessage> ev)
		{
			if (ev == null)
				ev = ev;

			pongMessageQueue.Enqueue (ev);
		}
	}
}
