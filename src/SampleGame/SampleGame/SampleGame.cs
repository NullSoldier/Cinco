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

		private CPlayer localPlayer;
		private Texture2D playerTexture;
		private string playerName = "TestPlayer";
		private bool isLoaded;

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
					client.Authenticate (playerName);
					Console.WriteLine ("We're connected!");
				});
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin();

			if (isLoaded)
				RenderPlayers();

			string renderText = "Connected: " + client.IsConnected.ToOnOff();

			spriteBatch.DrawString (font, renderText, Vector2.Zero, Color.Red);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void RenderPlayers()
		{
			int lerp = (int)(client.Interpolation * 1000);

			DateTime currentTime = DateTime.Now.ToUniversalTime();
			DateTime renderTime = currentTime.SubtractMilliseconds (lerp);

			SnapshotPair pair = client.Snapshots.GetRenderSnapshots (renderTime);
			
			// Draw the local predicted version of our player
			localPlayer.Draw (spriteBatch);

			lock (players)
			{
				// Draw all of the interpolated snapshots of non local players
				foreach (CPlayer player in players)
				{
					((CPlayer)pair.GetMerged (player, renderTime)).Draw (spriteBatch);
				}
			}
		}

		public void OnPlayerCreated (CPlayer player)
		{
			player.Texture = playerTexture;

			// Is it our localPlayer, or someone else?
			if (player.Name == playerName)
			{
				localPlayer = player;
				isLoaded = true;
			}
			else
				players.Add (player);
		}
	}
}
