using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cinco;
using Cinco.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SampleGame.Core;
using Tempest;
using Tempest.Providers.Network;

namespace SampleGame
{
	public class SampleGame
		: Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private SimpleClient client;
		private List<CPlayer> players;
		private SpriteFont font;
		private Texture2D playerTexture;

		public SampleGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			players = new List<CPlayer>();
			spriteBatch = new SpriteBatch(GraphicsDevice);
			font = Content.Load<SpriteFont> ("font");
			playerTexture = Content.Load<Texture2D> ("circle");

			IPAddress host = IPAddress.Parse ("127.0.0.1");
			int port = Convert.ToInt32 (42900);

			client = new SimpleClient (this, new NetworkClientConnection (new [] { P.Protocol, CincoProtocol.Protocol }));
			client.Register("Player", typeof(CPlayer));

			client.ConnectAsync (new IPEndPoint (host, port))
				.ContinueWith (t =>
				{
					client.Authenticate ("Player");
					Console.WriteLine ("We're connected!");
				});
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			
			foreach (CPlayer player in players)
				spriteBatch.Draw (playerTexture, player.Postion, Color.Red);

			spriteBatch.DrawString (font, "Connected: " + client.IsConnected.ToString(), Vector2.Zero, Color.Black);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		public void OnPlayerCreated (CPlayer player)
		{
			players.Add (player);
		}
	}
}
