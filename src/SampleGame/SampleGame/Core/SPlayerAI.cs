using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SampleGame.Core
{
	public class SPlayerAI
		: SPlayer
	{
		public override void Update()
		{
			var currentTime = DateTime.Now;
			
			if (currentTime.Subtract (lastMove).TotalSeconds > moveDelay)
			{
				Postion += new Vector2(0, speed);
				moveAmount += speed;

				if (moveAmount > maxMoveAmount)
				{
					speed *= -1;
					moveAmount = 0;
				}

				lastMove = currentTime;
			}
		}

		private DateTime lastMove;
		private float moveDelay = 2f;
		private float speed = 5f;
		private float moveAmount;
		private float maxMoveAmount;
	}
}
