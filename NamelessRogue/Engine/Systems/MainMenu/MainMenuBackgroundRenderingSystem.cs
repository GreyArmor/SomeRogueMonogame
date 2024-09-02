using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace NamelessRogue.Engine.Systems.MainMenu
{
	public class PosScale
	{
		public Vector2 Position;
		public Vector2 Scale;
        public bool StartFrame;
		public float Speed;
	}
	internal class MainMenuBackgroundRenderingSystem : BaseSystem
	{
		private SpriteBatch _spriteBatch;

		public override HashSet<Type> Signature => new HashSet<Type>();

		List<Rectangle> cloudRectangles;
		Random random = new Random();
		float numberspeed = 1.5f;

		List<PosScale> positions = new List<PosScale>();
        float counter = 0;
		float frequencyOfNewLinesMiliseconds = 250;
		AnimatedSpriteNR ZeroAndOne = null;
        AnimatedSpriteNR ZeroAndOne2 = null;
        private int screenWidth;

        public MainMenuBackgroundRenderingSystem(NamelessGame game)
		{
			_spriteBatch = new SpriteBatch(game.GraphicsDevice, 6400);
			ZeroAndOne = SpriteLibrary.SpritesAnimatedIdle["ZeroAndOne"];
            ZeroAndOne2 = SpriteLibrary.SpritesAnimatedIdle["ZeroAndOne2"];
            screenWidth = game.GetActualWidth();
            for (int i = 0; i < 10; i++)
			{
				AddNewChain(random.Next(screenWidth), random.Next(300), new Vector2(random.NextFloat(0.1f, 0.5f)));
            }
		}

		public void AddNewChain(float positionX, float positionY, Vector2 scale)
		{
			for (int i = 0; i < 8; i++)
            {
				positions.Add(new PosScale() { Position = new Vector2(positionX, -positionY - i * (64 * scale.Y)), Scale = scale, StartFrame = random.Next(2) == 1, Speed = random.NextFloat(1f, 3f) });
            }
		}
		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			counter += gameTime.ElapsedGameTime.Milliseconds;

			if(counter> frequencyOfNewLinesMiliseconds)
			{
                AddNewChain(random.Next(screenWidth), random.Next(300), new Vector2(random.NextFloat(0.1f, 0.5f)));
                counter = 0;
            }
			ZeroAndOne.Update(gameTime);
			ZeroAndOne2.Update(gameTime);
            namelessGame.Batch.Begin();

            for (int i = 0; i < positions.Count(); i++)
			{
				var sprite = positions[i].StartFrame? ZeroAndOne: ZeroAndOne2;

                sprite.Draw(namelessGame, gameTime, positions[i].Position, positions[i].Scale, Microsoft.Xna.Framework.Color.DarkGreen);
				positions[i].Position.Y += positions[i].Speed;
            }
			foreach (var position in positions.ToList())
			{
				if (position.Position.Y > namelessGame.GetActualHeight())
				{
					positions.Remove(position);
                }
			}
            namelessGame.Batch.End();
        }
	}
}
