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

		List<LayeredSprite> _cloudSprites = new List<LayeredSprite>();
		List<LayeredSprite> _cloudShadowSprites = new List<LayeredSprite>();
		List<LayeredSprite> _distantCloudSprites = new List<LayeredSprite>();
		Texture2D clouds;
		Texture2D tower;
		Texture2D walls;
		Texture2D mountains;
		Texture2D bgIce;
		Texture2D landWithPerspective;
		Texture2D cloudShadow;
		float counter = 0;
		public MainMenuBackgroundRenderingSystem(NamelessGame game)
		{
			_spriteBatch = new SpriteBatch(game.GraphicsDevice, 6400);
			clouds = game.Content.Load<Texture2D>("Sprites\\Clouds");
			tower = game.Content.Load<Texture2D>("Sprites\\tower");
			walls = game.Content.Load<Texture2D>("Sprites\\townwalls");
			mountains = game.Content.Load<Texture2D>("Sprites\\mountains");
			landWithPerspective = game.Content.Load<Texture2D>("Sprites\\landwithperspective");

			bgIce = game.Content.Load<Texture2D>("Sprites\\ice");

			cloudShadow = game.Content.Load<Texture2D>("Sprites\\Shadow");

			cloud1 = new Rectangle(0, 0, 128, 32);
			cloud2 = new Rectangle(0, 33, 128, 32);
			cloud3 = new Rectangle(0, 65, 128, 32);
			cloud4 = new Rectangle(0, 97, 64, 32);
			cloud5 = new Rectangle(64, 97, 64, 32);
			cloudRectangles = new List<Rectangle>() { cloud1, cloud2, cloud3, cloud4, cloud5 };

			_distantCloudSprites = new List<LayeredSprite>();

			distantCloudsWidth = bgIce.Width * 1.2f;
			distantCloudsHeight = bgIce.Height * 1.2f;
			var distantCloudsSpeed = -0.02f;
			_distantCloudSprites.Add(new LayeredSprite(bgIce, Rectangle.Empty, 9, new Vector2(0, 0), distantCloudsSpeed));
			_distantCloudSprites.Add(new LayeredSprite(bgIce, Rectangle.Empty, 9, new Vector2(distantCloudsWidth, 0), distantCloudsSpeed));
			_distantCloudSprites.Add(new LayeredSprite(bgIce, Rectangle.Empty, 9, new Vector2(distantCloudsWidth * 2, 0), distantCloudsSpeed));
			_distantCloudSprites.Add(new LayeredSprite(bgIce, Rectangle.Empty, 9, new Vector2(distantCloudsWidth * 3, 0), distantCloudsSpeed));
			_distantCloudSprites.Add(new LayeredSprite(bgIce, Rectangle.Empty, 9, new Vector2(distantCloudsWidth * 4, 0), distantCloudsSpeed));

		}
		float cloudScale = 1.2f;
		float distantCloudsWidth;
		float distantCloudsHeight;

	

		//basically, clouds far away move slower, flouds in front move faster, clounds far away are smaller and lower to create perspective
		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			int gh = namelessGame.GetActualHeight();
			counter += gameTime.ElapsedGameTime.Milliseconds;
			if (counter > 250)
			{
				counter = 0;


				var randomIndex = random.Next(0, 5);
				var randomDepth = 1f - random.Next(2, 11) / 10f;
				var cloudRectangle = cloudRectangles[randomIndex];
				var speed = (1f - randomDepth) / 10f;
				//if (speed < 0.04f) { speed = 0.04f; }
				_cloudSprites.Add(new LayeredSprite(clouds, cloudRectangle, randomDepth, new Vector2(-clouds.Width, 300 * randomDepth), speed));

				_cloudShadowSprites.Add(new LayeredSprite(cloudShadow, Rectangle.Empty, randomDepth, new Vector2(-clouds.Width, distantCloudsHeight + (distantCloudsHeight - ((distantCloudsHeight) * randomDepth))), speed));
			}
			var mountainsScale = 1.5f;
			var mountainShiftX = 150;

			var mountainShiftY = distantCloudsHeight - (mountains.Height* mountainsScale);

			var towerScale = 0.4f;
			var towerHalfSize = new Vector2((tower.Width * towerScale), (tower.Height * towerScale));
		
			var townshift = 100;


			_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			_spriteBatch.Draw(landWithPerspective, new Vector2(0, distantCloudsHeight), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

			foreach (LayeredSprite distantCloud in _distantCloudSprites)
			{
				distantCloud.Update(gameTime);
				float difference = 0;
				if (distantCloud.Position.X < -distantCloudsWidth)
				{
					difference = -distantCloudsWidth - distantCloud.Position.X;
					distantCloud.Position = new Vector2((distantCloudsWidth * 4) - difference, distantCloud.Position.Y);
				}
				_spriteBatch.Draw(distantCloud.Texture, distantCloud.Position, null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, cloudScale, SpriteEffects.None, 0);
			}

			var clearList = new List<LayeredSprite>(_cloudSprites.Count);
			foreach (LayeredSprite layeredSprite in _cloudSprites)
			{
				layeredSprite.Update(gameTime);
				_spriteBatch.Draw(layeredSprite.Texture, layeredSprite.Position, layeredSprite.SpritePositon, Microsoft.Xna.Framework.Color.SlateGray, 0, Vector2.Zero, 1 - layeredSprite.Depth, SpriteEffects.None, layeredSprite.Depth);
				if (layeredSprite.Position.X < namelessGame.GetActualWidth())
				{
					clearList.Add(layeredSprite);
				}
			}

		


			_spriteBatch.Draw(mountains, new Vector2(0 - mountainShiftX, mountainShiftY), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, mountainsScale, SpriteEffects.None, 0);
			_spriteBatch.Draw(mountains, new Vector2(mountains.Width - mountainShiftX, mountainShiftY), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, mountainsScale, SpriteEffects.None, 0);
			_spriteBatch.Draw(mountains, new Vector2((mountains.Width - mountainShiftX) * 2, mountainShiftY), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, mountainsScale, SpriteEffects.None, 0);
			_spriteBatch.Draw(mountains, new Vector2((mountains.Width - mountainShiftX) * 3, mountainShiftY), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, mountainsScale, SpriteEffects.None, 0);

			_spriteBatch.Draw(walls, new Vector2(namelessGame.GetActualWidth() / 2 - walls.Width / 2, namelessGame.GetActualHeight() / 2 - (walls.Height / 2) + townshift), null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

			_spriteBatch.Draw(tower, new Vector2(namelessGame.GetActualWidth() / 2 - towerHalfSize.X, namelessGame.GetActualHeight() / 2 - towerHalfSize.Y + townshift), null, new Microsoft.Xna.Framework.Color(70, 70, 70, 255), 0, Vector2.Zero, towerScale, SpriteEffects.None, 0);

			_spriteBatch.Draw(tower, new Vector2(namelessGame.GetActualWidth() / 2 - towerHalfSize.X, namelessGame.GetActualHeight() / 2 - towerHalfSize.Y + townshift), null, new Microsoft.Xna.Framework.Color(70, 70, 70, 255), 0, Vector2.Zero, towerScale, SpriteEffects.None, 0);



			var clearShadowList = new List<LayeredSprite>(_cloudShadowSprites.Count);
			foreach (LayeredSprite layeredSprite in _cloudShadowSprites)
			{
				layeredSprite.Update(gameTime);
				_spriteBatch.Draw(layeredSprite.Texture, layeredSprite.Position, null, new Microsoft.Xna.Framework.Color(1, 1, 1, 1f), 0, Vector2.Zero, 1 - layeredSprite.Depth, SpriteEffects.None, layeredSprite.Depth);
				if (layeredSprite.Position.X < namelessGame.GetActualWidth())
				{
					clearShadowList.Add(layeredSprite);
				}
			}



			_spriteBatch.End();

			_cloudSprites.Clear();
			_cloudSprites = clearList;

			_cloudShadowSprites.Clear();
			_cloudShadowSprites = clearShadowList;
		}
	}
}
