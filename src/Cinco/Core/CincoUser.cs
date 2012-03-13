using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tempest;

namespace Cinco.Core
{
	public class CincoUser
	{
		public CincoUser()
		{
			TickRate = 66;
			UpdateRate = 66;
		}
	
		public IConnection Connection
		{
			get;
			set;
		}

		public bool IsActive
		{
			get;
			set;
		}

		#region Client settings

		/// <summary>
		/// Absolute ticks per second
		/// </summary>
		public float TickRate
		{
			get;
			set;
		}

		/// <summary>
		/// Absolute number of client snapshots to send this client a second
		/// </summary>
		public float UpdateRate
		{
			get;
			set;
		}

		#endregion

		public bool NeedsSnapshot (DateTime currentTime)
		{
			return (currentTime - lastSendTime).TotalSeconds >= (1 / UpdateRate);
		}

		public void SendSnapshot (Snapshot snapshot, DateTime currentTime)
		{
			if (!NeedsSnapshot (currentTime))
				throw new Exception ("Sending more snapshots than the clients UpdateRate.");

			lastSendTime = currentTime;
			Connection.Send (new EntitySnapshotMessage (snapshot));
		}

		private DateTime lastSendTime;
	}
}
