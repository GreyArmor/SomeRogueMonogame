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
using NamelessRogue.Engine.Components.ChunksAndTiles;

namespace NamelessRogue.Engine.Components._3D
{

	public enum AnimationType { 
		Idle,
		Attack,
		Walk,
		Cast,
		Shoot,
	}

	internal class SpriteModel3D : Component
	{
		public bool IdleOnly { get; set; }
		public AnimatedSprite Sprite { get; set; }

		public string SpriteId { get; set; }
		public SpriteModel3D(NamelessGame game, string spritePath)
		{
			var spriteSheet = game.Content.Load<SpriteSheet>(spritePath, new JsonContentLoader());
			Sprite = new AnimatedSprite(spriteSheet);
			IdleOnly = false;
		}

		public SpriteModel3D(string spriteLibraryId)
		{
			IdleOnly = true;
			SpriteId = spriteLibraryId;
		}	
		public AnimationType AnimationType { get; set; }	
	}
}
