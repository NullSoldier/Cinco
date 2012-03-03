using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco.Core;
using SampleGame.Messages;
using Tempest;

namespace SampleGame.Core
{
	public class SimpleClient
		: CincoClient
	{
		public SimpleClient (IClientConnection connection)
			: base (connection)
		{
			
		}

		public void Authenticate (string playerName)
		{
			ConnectMessage message = new ConnectMessage(playerName);
			connection.Send (message);
		}
	}
}
