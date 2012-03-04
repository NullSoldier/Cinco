using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinco
{
	public class IndexedQueue<T>
	{
		public IndexedQueue (int size)
		{
			items = new T[size];
			Lock = new object ();
		}

		public object Lock;

		public int Count
		{
			get { return items.Length; }
		}

		public T this [int index]
		{
			get
			{
				lock (Lock)
				{
					if (index < 0 || index >= items.Length)
						throw new ArgumentOutOfRangeException("index");

					int realIndex = startIndex + index;
					if (realIndex >= items.Length)
						realIndex = realIndex - items.Length;

					return items[realIndex];
				}
			}
		}

		public void Enqueue (T item)
		{
			lock (Lock)
			{
				items[startIndex % items.Length] = item;
				startIndex = (++startIndex % items.Length);
			}
		}

		private readonly T[] items;
		private int startIndex;
	}
}
