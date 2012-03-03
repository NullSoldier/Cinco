using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cinco;
using Cinco.Core;
using Tempest;
using Tempest.InternalProtocol;

namespace SampleGame
{
	public class ServerSync
	{
		public ServerSync ()
		{
			entities = new List<NetworkEntity> ();
		}

		public void RegisterEntity(NetworkEntity entity)
		{
		}

		public void Syncronize(IEnumerable<CincoClient> clients)
		{
			
		}

		private List<NetworkEntity> entities; 
	}
}
