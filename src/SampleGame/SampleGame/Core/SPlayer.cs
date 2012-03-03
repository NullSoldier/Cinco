using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco;
using Microsoft.Xna.Framework;

namespace SampleGame.Core
{
	public class SPlayer
		: NetworkEntity
	{
		public SPlayer()
			: base ("Player", EntityType.Server)
		{
			base.Register ("Position", Vector2.Zero);
			this.SendState = SendState.Always;
		}

		public string Name
		{
			get;
			set;
		}

		public Vector2 Postion
		{
			get { return (Vector2)base["Position"]; }
			set { base["Position"] = value; }
		}

		public int Health
		{
			get;
			set;
		}
	}
}
