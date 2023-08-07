using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace NamelessRogue.Engine.Systems.MainMenu
{
	internal class MainMenuBackgroundRenderingSystem : BaseSystem
	{
		private SpriteBatch _spriteBatch;

		public override HashSet<Type> Signature => new HashSet<Type>();

		Rectangle cloud1, cloud2, cloud3, cloud4, cloud5;
		List<Rectangle> cloudRectangles;
		Random random = new Random();
		float cloudspeed = 0.1f;
		public MainMenuBackgroundRenderingSystem(NamelessGame game)
		{
			_spriteBatch = new SpriteBatch(game.GraphicsDevice, 6400);
			clouds = game.Content.Load<Texture2D>("Sprites\\Clouds");
			cloud1 = new Rectangle(0, 0, 128, 32);
			cloud2 = new Rectangle(0, 33, 128, 32);
			cloud3 = new Rectangle(0, 65, 128, 32);
			cloud4 = new Rectangle(0, 97, 64, 32);
			cloud5 = new Rectangle(64, 97, 64, 32);
			cloudRectangles = new List<Rectangle>() { cloud1, cloud2, cloud3, cloud4, cloud5 };
		}
		List<LayeredSprite> _sprites = new List<LayeredSprite>();
		Texture2D clouds;
		float counter = 0;
		//basically, clouds far away move slower, flouds in front move faster, clounds far away are smaller and lower to create perspective
		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			counter += gameTime.ElapsedGameTime.Milliseconds;
			if (counter > 250)
			{
				counter = 0;

				var randomIndex = random.Next(0, 5);
				var randomDepth = random.Next(1, 11)/10f;
				var cloudRectangle = cloudRectangles[randomIndex];
				var speed = (1f - randomDepth)/ 10f;
				//if (speed < 0.04f) { speed = 0.04f; }
				_sprites.Add(new LayeredSprite(clouds, cloudRectangle, randomDepth, new Vector2(-clouds.Width, 250 * randomDepth),speed));
			}
			_spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			var clearList = new List<LayeredSprite>(_sprites.Count);
			foreach (LayeredSprite layeredSprite in _sprites)
			{
				layeredSprite.Update(gameTime);
				_spriteBatch.Draw(layeredSprite.Texture, layeredSprite.Position, layeredSprite.SpritePositon, Microsoft.Xna.Framework.Color.SlateGray, 0, Vector2.Zero, 1-layeredSprite.Depth, SpriteEffects.None, layeredSprite.Depth);
				if (layeredSprite.Position.X < namelessGame.GetActualWidth())
				{
					clearList.Add(layeredSprite);
				}
			}
			
			_spriteBatch.End();

			_sprites.Clear();
			_sprites = clearList;



		}
	}
}
