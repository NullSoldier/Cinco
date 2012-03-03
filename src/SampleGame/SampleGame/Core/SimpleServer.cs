using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco;
using Cinco.Core;
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

			this.RegisterMessageHandler<ConnectMessage> (OnConnectMessageReceived);
		}

		private Dictionary<long, SPlayer> players;

		public void OnConnectMessageReceived (MessageEventArgs<ConnectMessage> ev)
		{
			// When a player connects
			// Create a character for them
			// Register the character

			var player = new SPlayer();
			player.Name = ev.Message.PlayerName;

			players.Add (ev.Connection.ConnectionId, player);

			Console.Write (player.Name + " has connected!");
		}
	}
}
