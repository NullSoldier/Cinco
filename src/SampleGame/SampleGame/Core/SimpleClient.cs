using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco;
using Cinco.Core;
using SampleGame.Messages;
using Tempest;

namespace SampleGame.Core
{
	public class SimpleClient
		: CincoClient
	{
		public SimpleClient (SampleGame game, IClientConnection connection)
			: base (connection)
		{
			this.game = game;
		}

		public void Authenticate (string playerName)
		{
			ConnectMessage message = new ConnectMessage(playerName);
			connection.Send (message);
		}

		protected override void OnEntityCreated(NetworkEntity entity)
		{
			game.OnPlayerCreated ((CPlayer)entity);
		}

		private SampleGame game;
	}
}
