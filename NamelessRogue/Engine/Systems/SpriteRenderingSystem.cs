using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Point = Microsoft.Xna.Framework.Point;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace NamelessRogue.Engine.Systems
{
	internal class SpriteRenderingSystem : BaseSystem
	{

		public SpriteRenderingSystem(NamelessGame game)
		{
			spriteBatch = new SpriteBatch(game.GraphicsDevice);
			//	spriteFont = Content.Load<SpriteFont>("font");

			basicEffect = new BasicEffect(game.GraphicsDevice)
			{
				TextureEnabled = true,
				VertexColorEnabled = true,
			};
		}

		public override HashSet<Type> Signature => new HashSet<Type>() {
			typeof(SpriteModel3D), typeof(Position3D)
		};


		const float scalingMagicNumber = 20f;
		private SpriteBatch spriteBatch;
		private BasicEffect basicEffect;

		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			Camera3D camera = namelessGame.PlayerEntity.GetComponentOfType<Camera3D>();
			var offset = Constants.ChunkSize * (300 - Constants.RealityBubbleRangeInChunks);
			var player = namelessGame.PlayerEntity;
			var p = player.GetComponentOfType<Position>().Point;
			var position = new Point(p.X - offset, p.Y - offset);
			var tileToDraw = namelessGame.WorldProvider.GetTile(p.X, p.Y);

			var world = Constants.ScaleDownMatrix * Matrix.CreateTranslation(position.X * Constants.ScaleDownCoeficient, position.Y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient);
			var frustrum = new Microsoft.Xna.Framework.BoundingFrustum(camera.View * camera.Projection);


			Matrix invertY = Matrix.CreateScale(1, -1, 1);
			basicEffect.World = invertY;
			basicEffect.View = Matrix.Identity;
			basicEffect.Projection = camera.Projection;

			foreach (var entity in RegisteredEntities)
			{
				var worldPos = Vector3.Transform(Vector3.One, world);

				if (frustrum.Contains(worldPos) == Microsoft.Xna.Framework.ContainmentType.Contains)
				{
					var spriteModel = entity.GetComponentOfType<SpriteModel3D>();

					var angle = AngleBetween(new Vector2(camera.Look.X, camera.Look.Y), Vector2.UnitX);

					var animationSuffix = "Front";

					if (angle > -45 && angle < 45)
					{
						animationSuffix = "Front";
					}
					else if (angle > 45 && angle < 135)
					{
						animationSuffix = "Right";
					}
					else if (angle > 135 || angle < -135)
					{
						animationSuffix = "Back";
					}
					else if (angle > -135 && angle < -45f)
					{
						animationSuffix = "Left";
					}

					var viewport = namelessGame.GraphicsDevice.Viewport;


					spriteModel.Sprite.Play("attack" + animationSuffix);
					spriteModel.Sprite.Update(gameTime);
					Vector3 textPosition = worldPos;

					Vector3 viewSpacePosition = Vector3.Transform(textPosition, camera.View * invertY);

					spriteBatch.Begin(0, null, Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, basicEffect);
					spriteModel.Sprite.Depth = viewSpacePosition.Z;
					spriteBatch.Draw(spriteModel.Sprite, new Vector2(viewSpacePosition.X, viewSpacePosition.Y), 0, new Vector2(0.0001f, 0.0001f));
					spriteBatch.End();
				}
			}
		}

		//TODO useful utility, move somewhere appropriate
		private double AngleBetween(Vector2 vector1, Vector2 vector2)
		{
			double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
			double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

			return Math.Atan2(sin, cos) * (180 / Math.PI);
		}




		//private void RenderTestPlayer(NamelessGame game, Camera3D camera)
		//{


		//	var device = game.GraphicsDevice;
		//	var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
		//	effect.CurrentTechnique = effect.Techniques["ColorTechShadowMap"];
		//	//foreach (var geometry in chunkGeometries.ChunkGeometries.Values)
		//	//{

		//	Geometry3D geometry = ModelsLibrary.Models["smallTree"];
		//	var offset = Constants.ChunkSize * (300 - Constants.RealityBubbleRangeInChunks);
		//	var player = game.PlayerEntity;
		//	var p = player.GetComponentOfType<Position>().Point;
		//	var position = new Point(p.X - offset, p.Y - offset);
		//	var tileToDraw = game.WorldProvider.GetTile(p.X, p.Y);
		//	//Matrix.CreateTranslation(Constants.ScaleDownCoeficient * position.X, Constants.ScaleDownCoeficient * position.Y, (float)tileToDraw.Elevation, out Matrix pos);
		//	var world = Constants.ScaleDownMatrix * Matrix.CreateTranslation(position.X * Constants.ScaleDownCoeficient, position.Y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient);
		//	effect.Parameters["xWorldViewProjection"].SetValue(world * camera.View * camera.Projection);

		//	EffectPass pass = effect.CurrentTechnique.Passes[1];
		//	pass.Apply();

		//	device.SetVertexBuffer(geometry.Buffer);
		//	device.Indices = geometry.IndexBuffer;

		//	device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TriangleCount);
		//	//}
		//}

	}
}
