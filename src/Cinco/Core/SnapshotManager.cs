using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinco.Core
{
	public class SnapshotManager
	{
		public SnapshotManager (int maxHistory)
		{
			this.snapshotHistory = new IndexedQueue<Snapshot> (maxHistory);
			this.ExtrapolateAmount = ExtrapolateAmount;
		}

		public IndexedQueue<Snapshot> snapshotHistory
		{
			get;
			private set;
		}

		public float ExtrapolateAmount
		{
			get;
			set;
		}

		public SnapshotPair GetRenderSnapshots (DateTime renderTime)
		{
			Snapshot olderSnap = null;
			Snapshot newerSnap = null;

			lock (snapshotHistory.Lock)
			{
				for (int i = snapshotHistory.Count - 1; i >= 0; i--)
				{
					// No snapshots to look at
					if (snapshotHistory[i] == null)
						continue;

					if (renderTime.CompareTo (snapshotHistory[i].Taken) >= 0)
					{
						olderSnap = snapshotHistory[i];

						if (i < snapshotHistory.Count - 1)
							newerSnap = snapshotHistory[i + 1];

						break;
					}
				}

				// All the snapshots are newer than our Render time, use the most recent one
				if (olderSnap == null && newerSnap == null)
					olderSnap = newerSnap = snapshotHistory[snapshotHistory.Count - 1];
			}

			//GameConsole.Instance.Chat.Write ((snapshotHistory[9].Taken - renderTime).TotalSeconds.ToString(), Color.Green);
			return new SnapshotPair (olderSnap, newerSnap);
		}
	}
}
