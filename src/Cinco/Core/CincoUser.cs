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
			Interpolation = 0.3f;
			CmdRate = 20;
			Extrapolate = true;
			Predict = false;
			Smooth = true;
			SmoothTime = 1f;
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
		/// // Absolute ticks per second
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
		/// // Delay in time in seconds to use in rendering
		/// </summary>
		public float Interpolation
		{
			get;
			set;
		}

		/// <summary>
		/// Absolute UserCommand packets sent per second
		/// </summary>
		public float CmdRate
		{
			get;
			set;
		}

		/// <summary>
		/// When there aren't at least two packets in the Interpolation range, perform linear interpolation
		/// </summary>
		public bool Extrapolate
		{
			get;
			set;
		}

		public float ExtrapolateAmount
		{
			get;
			set;
		}

		public bool Predict
		{
			get;
			set;
		}

		public bool Smooth
		{
			get;
			set;
		}

		public float SmoothTime
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
