using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinco;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SampleGame.Core
{
	public class CPlayer
		: NetworkEntity
	{
		public CPlayer()
			: base ("Player", EntityType.Client)
		{
			base.Register ("Name", string.Empty);
			base.Register ("Position", Vector2.Zero);
		}

		public string Name
		{
			get { return (string)base["Name"]; }
			set { base["Name"] = value; }
		}

		public Vector2 Position
		{
			get { return (Vector2)base["Position"]; }
			set { base["Position"] = value; }
		}

		public Texture2D Texture
		{
			get;
			set;
		}

		public void Draw (SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (Texture, Position, Color.Red);
		}

		public override NetworkEntity Lerp (NetworkEntity source, NetworkEntity one, NetworkEntity two, float lerp)
		{
			return new CPlayer
			{
				Name = Name,
				Position = Vector2.Lerp (((CPlayer)one).Position, ((CPlayer)two).Position, lerp)
			};
		}
	}
}
