using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.AI.Pathfinder;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Content;
using MonoGame.Extended;

namespace NamelessRogue.Engine.Systems
{
	internal class SpriteRenderingSystem : BaseSystem
	{
		public override HashSet<Type> Signature => new HashSet<Type>() {
			typeof(SpriteModel3D)
		};

		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			foreach (var entity in RegisteredEntities)
			{
				var spriteModel = entity.GetComponentOfType<SpriteModel3D>();
				spriteModel.Sprite.Play("attackFront");
				spriteModel.Sprite.Update(gameTime);

				namelessGame.Batch.Begin(samplerState: SamplerState.PointClamp);
				namelessGame.Batch.Draw(spriteModel.Sprite, new Vector2(100,100), 0, new Vector2(8,8));
				namelessGame.Batch.End();
			}
		}
	}
}
