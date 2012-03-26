using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinco.Core
{
	public class PingContainer
	{
		public PingContainer(CincoUser user)
		{
			this.User = user;
		}

		public DateTime PingTime
		{
			get;
			set;
		}

		public CincoUser User
		{
			get;
			set;
		}
	}
}
