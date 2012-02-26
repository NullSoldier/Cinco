using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tempest;
using Tempest.InternalProtocol;

namespace Cinco
{
	public class CincoServer
		: ServerBase
	{
		public CincoServer (IConnectionProvider connectionProvider)
			: base (connectionProvider, MessageTypes.All) 
		{
			clients = new Dictionary<IConnection, Client> (maxPlayers);
			random = new Random();
			options = new ServerOptions ();
			clientLock = new object ();

			this.RegisterMessageHandler<ConnectMessage> (OnConnectMessage);

			StartUpdateThread();
		}

		private Dictionary<IConnection, Client> clients;
		private ServerOptions options;
		private GenericManager<UserChatMessage> chatManager;
		private Thread updateThread;
		private Random random;
		private int userCount;
		private int maxPlayers = 32;
		private object clientLock;

		private void OnConnectMessage(MessageEventArgs<ConnectMessage> e)
		{
			e.con
			if (playerCount >= maxPlayers)
			{
				e.Connection.Send (new ConnectReplyMessage (ConnectResponse.ServerFull));
				DisconnectWithReason (e.Connection, "The server is full.");
				return;
			}
			else if (clients.ContainsKey (e.Connection))
			{
				e.Connection.Disconnect();
			}

			uint networkID = ++lastPlayerID;

			Character character = new Character (copyCommands:false)
			{
				NetworkID = networkID,
				Connection = e.Connection,
				Name = e.Message.PlayerName
			};
			Client client = new Client
			{
				NetworkID = networkID,
				Connection = e.Connection,
				Character = character
			};

			lock (playerLock)
			{
				this.clients.Add (e.Connection, client);
				this.serverProvider.CharacterManager.Add (networkID, character);
			}

			e.Connection.SendResponse (e.Message, new ConnectReplyMessage (ConnectResponse.Success, client.NetworkID));
			playerCount++;

			Console.WriteLine ("{0} has connected.", character.Name);
		}

		protected override void OnConnectionDisconnected(object sender, DisconnectedEventArgs e)
		{
			lock (playerLock)
			{
				if (this.clients.ContainsKey (e.Connection))
				{
					Character character = this.clients[e.Connection].Character;

					RemoveCharacter (character);
					Console.WriteLine ("{0} has disconnected.", character.Name);
				}
				else
					Console.WriteLine ("{0} has disconnected.", e.Connection.RemoteEndPoint);
			}

			base.OnConnectionDisconnected (sender, e);
		}

		private void StartUpdateThread ()
		{
			updateThread = new Thread (Update);
			updateThread.Name = "Update Thread";
			updateThread.Start();
		}
	}
}
