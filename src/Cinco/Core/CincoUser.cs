using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco.Messages;
using Tempest;

namespace Cinco.Core
{
	public class CincoUser
	{
		public CincoUser()
		{
			TickRate = 66;
			UpdateRate = 66;
			TimeSyncRate = 0.5f;

			latencySamples = new IndexedQueue<LatencySample> (10);
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

		public bool IsClockSynchronized
		{
			get;
			set;
		}

		public DateTime PingTime
		{
			get;
			set;
		}

		public IndexedQueue<LatencySample> LatencySamples
		{
			get { return latencySamples; }
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

		/// <summary>
		/// How often to syncronize the users time a second
		/// </summary>
		public float TimeSyncRate
		{
			get;
			set;
		}

		#endregion

		public bool NeedsSnapshot (DateTime currentTime)
		{
			return (currentTime - lastSendTime).TotalSeconds >= (1 / UpdateRate);
		}

		public bool NeedsTimeSync(DateTime currentTime)
		{
			return (currentTime - lastTimeSync).TotalSeconds >= (1 / TimeSyncRate);
		}

		public void SendTimeSync (DateTime currentTime)
		{
			latency = MarzulloCalculater.CalculateLatency (ref latencySamples);

			Connection.Send (new TimeSyncMessage (currentTime, latency));
			lastTimeSync = currentTime;
		}

		public void SendSnapshot (Snapshot snapshot, DateTime currentTime)
		{
			if (!NeedsSnapshot (currentTime))
				throw new Exception ("Sending more snapshots than the clients UpdateRate.");

			Connection.Send (new EntitySnapshotMessage (snapshot));
			lastSendTime = currentTime;
		}

		private DateTime lastSendTime;
		private DateTime lastTimeSync;
		private double latency;
		private IndexedQueue<LatencySample> latencySamples;
	}
}
