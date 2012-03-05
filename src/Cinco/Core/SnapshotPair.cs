using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cinco.Core
{
	public class SnapshotPair
	{
		public SnapshotPair(Snapshot older, Snapshot newer)
		{
			this.Older = older;
			this.Newer = newer;
		}

		public Snapshot Older;
		public Snapshot Newer;
		
		public NetworkEntity GetMerged (NetworkEntity source, DateTime time)
		{
			uint networkID = source.NetworkID;

			// Extrapolation is disabled so just use the latest version
			if (Newer == null || Older == null)
				return source;

			double renderTime = (time - Older.Taken).TotalSeconds;
			double timeRange = (Newer.Taken - Older.Taken).TotalSeconds;

			NetworkEntity one = Older.GetEntity (networkID).Entity;
			NetworkEntity two = Newer.GetEntity (networkID).Entity;

			// If the last snapshot doesn't have the character return the most recent one
			if (one == null)
				return source;

			return LerpEntity (source, one, two, (float)(renderTime / timeRange));
		}

		public bool ShouldExtrapolate (DateTime currentTime, float extrapolateLimit)
		{
			return Newer == null && Older != null && (currentTime - Older.Taken).TotalSeconds <= extrapolateLimit;
		}

		private NetworkEntity LerpEntity (NetworkEntity source, NetworkEntity one, NetworkEntity two, float lerp)
		{
			return source.Lerp(source, one, two, lerp);
		}
	}
}
