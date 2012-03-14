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
using SampleGame.Messages;
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
		private SpriteFont font;
		private bool isloaded;

		private CPlayer localPlayer;
		private List<CPlayer> players;
		private Texture2D playerTexture;
		private string playerName = "TestPlayer";
		private float Interpolation = 0.1f;

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

			// Start connecting to the server at 127.0.0.1:42900
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

		protected override void Update(GameTime gameTime)
		{
			if (localPlayer != null)
			{
				var keyState = Keyboard.GetState ();
				var moveVector = Vector2.Zero;

				if (keyState.IsKeyDown (Keys.Left)) { moveVector.X -= 1; }
				if (keyState.IsKeyDown (Keys.Right)) { moveVector.X += 1; }
				if (keyState.IsKeyDown (Keys.Down)) { moveVector.Y += 1; }
				if (keyState.IsKeyDown (Keys.Up)) { moveVector.Y -= 1; }

				if (moveVector != Vector2.Zero)
					moveVector.Normalize ();

				client.Connection.Send (new MoveMessage (moveVector));
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin();

			if (isloaded)
				RenderPlayers();

			string renderText = "Connected: " + client.IsConnected.ToOnOff();

			spriteBatch.DrawString (font, renderText, Vector2.Zero, Color.Red);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void RenderPlayers()
		{
			// Get the time to render at by first calculating the LERP value
			int lerp = (int)(Interpolation * 1000);
			DateTime currentTime = DateTime.Now.ToUniversalTime();
			DateTime renderTime = currentTime.SubtractMilliseconds (lerp);

			// Get the two snapshots to interpolate between
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
				isloaded = true;
			}
			else
				players.Add (player);
		}
	}
}
