using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco;
using Cinco.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SampleGame.Messages;
using Tempest;

namespace SampleGame.Core
{
	public class SimpleServer
		: CincoServer
	{
		public SimpleServer(IConnectionProvider provider)
			: base (provider, new ServerOptions())
		{
			this.players = new Dictionary<long, SPlayer>();
			this.bots = new List<SPlayerAI> ();
			this.botLock = new object();
			this.random = new Random();

			this.RegisterMessageHandler<ConnectMessage> (OnConnectMessageReceived);

			// Spawn 5 bots
			for (int i = 0; i < 5; i++)
				SpawnBot ();
		}

		private Dictionary<long, SPlayer> players;
		private List<SPlayerAI> bots;
		private object botLock;
		private Random random;

		private void OnConnectMessageReceived(MessageEventArgs<ConnectMessage> ev)
		{
			var player = new SPlayer();
			player.Name = ev.Message.PlayerName;

			players.Add (ev.Connection.ConnectionId, player);
			RegisterEntity (player);

			Console.Write (player.Name + " has connected!");
		}

		public override void Tick (DateTime dateTime)
		{
			if (botLock == null)
				return;

			lock (botLock)
			{
				// Move all of the bots
				foreach (SPlayerAI bot in bots)
					bot.Update();
			}
		}

		private void SpawnBot()
		{
			SPlayerAI bot = new SPlayerAI();
			RegisterEntity (bot);

			bot.Name = "Bot " + bot.NetworkID; 
			bot.Postion = new Vector2 (random.Next(0, 500), random.Next(0, 600));

			lock (botLock)
				bots.Add (bot);

			Console.WriteLine ("Spawned bot: " + bot.Name);
		}
	}
}
