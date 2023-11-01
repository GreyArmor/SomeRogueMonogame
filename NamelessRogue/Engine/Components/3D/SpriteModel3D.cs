using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Serialization;
using NamelessRogue.shell;
using NamelessRogue.Engine.Context;

namespace NamelessRogue.Engine.Components._3D
{
	internal class SpriteModel3D : Component
	{
		public bool IdleOnly { get; set; }
		public AnimatedSprite Sprite { get; set; }
		public SpriteModel3D(NamelessGame game, string spritePath, bool idleOnly = false)
		{
			var spriteSheet = game.Content.Load<SpriteSheet>(spritePath, new JsonContentLoader());
			Sprite = new AnimatedSprite(spriteSheet);
			IdleOnly = idleOnly;
		}
		
	}
}
