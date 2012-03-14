using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Tempest;

namespace SampleGame.Messages
{
	public class MoveMessage
		: SampleBaseMessage
	{
		public MoveMessage()
			: base (SampleMessageTypes.MoveMessage)
		{
		}

		public MoveMessage (Vector2 direction)
			: base (SampleMessageTypes.MoveMessage)
		{
			this.Direction = direction;
		}

		public Vector2 Direction
		{
			get;
			private set;
		}

		public override void WritePayload(ISerializationContext context, IValueWriter writer)
		{
			writer.Write (context, Direction);
		}

		public override void ReadPayload(ISerializationContext context, IValueReader reader)
		{
			Direction = reader.Read<Vector2> (context);
		}
	}
}
