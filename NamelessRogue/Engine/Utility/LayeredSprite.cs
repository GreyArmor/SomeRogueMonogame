using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace NamelessRogue.Engine.Utility
{
	public class LayeredSprite
	{
		private Texture2D texture;
		private Vector2 position;
		private float depth;
		private float moveScale;
		private float defaultSpeed;

		public Rectangle SpritePositon { get; }

		public Texture2D Texture => texture;

		public Vector2 Position { get => position; set => position = value; }

		public float Depth => depth;

		public float MoveScale => moveScale;

		public float DefaultSpeed => defaultSpeed;

		public LayeredSprite(Texture2D texture, Rectangle spritePositon, float depth, Vector2 initialPosition, float defaultSpeed = 0)
		{
			this.texture = texture;
			this.SpritePositon = spritePositon;
			this.depth = depth;
			this.position = initialPosition;
			this.defaultSpeed = defaultSpeed;
		}

		public void Update(GameTime time)
		{
			position.X += (DefaultSpeed) * time.ElapsedGameTime.Milliseconds;
		}
	}
}
