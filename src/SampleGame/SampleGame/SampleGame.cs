using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cinco.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SampleGame.Core;
using Tempest.Providers.Network;

namespace SampleGame
{
	public class SampleGame : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		CincoClient client;
		SpriteFont font;

		public SampleGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			font = Content.Load<SpriteFont> ("font");

			IPAddress host = IPAddress.Parse ("127.0.0.1");
			int port = Convert.ToInt32 (42900);

			client = new SimpleClient (new NetworkClientConnection (P.Protocol));
			client.ConnectAsync (new IPEndPoint (host, port))
				.ContinueWith ((t) => Console.WriteLine("We're connected!"));
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			spriteBatch.DrawString (font, "Connected: " + client.IsConnected.ToString(), Vector2.Zero, Color.Black);
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
