using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using SampleGame;
using SampleGame.Core;
using SampleGame.Messages;
using Tempest.Providers.Network;

namespace SampleServer
{
	class Program
	{
		private static SimpleServer server;
		private static bool isRunning;
		private static int port = 42900;

		static void Main(string[] args)
		{
			P.Protocol.Discover (typeof (BaseMessage).Assembly);

			server = new SimpleServer (new NetworkConnectionProvider (P.Protocol, new IPEndPoint (IPAddress.Any, port), 32));
			server.Start ();

			isRunning = true;
			Console.WriteLine ("Server started on port {0}", port);

			while (isRunning)
			{
				Thread.Sleep (10000);
			}
		}
	}
}
