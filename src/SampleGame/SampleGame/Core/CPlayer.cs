using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco;
using Microsoft.Xna.Framework;

namespace SampleGame.Core
{
	public class CPlayer
		: NetworkEntity
	{
		public CPlayer()
			: base ("Player", EntityType.Client)
		{
			base.Register ("Position", Vector2.Zero);
		}

		public Vector2 Postion
		{
			get { return (Vector2)base["Position"]; }
			set { base["Position"] = value; }
		}
	}
}
