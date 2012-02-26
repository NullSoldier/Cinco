using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace SampleGame.Core
{
	public class SimpleClient
		: ClientBase
	{
		public SimpleClient(IClientConnection connection)
			: base (connection, MessageTypes.Reliable, false)
		{
			clientSync = new ClientSync (connection);
		}

		private ClientSync clientSync;
	}
}
