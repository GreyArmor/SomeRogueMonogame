using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Environment;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Components.Rendering;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using static NamelessRogue.Engine.Systems.Ingame.RenderingSystem3D;
using System.Linq;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Point = Microsoft.Xna.Framework.Point;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace NamelessRogue.Engine.Systems
{
	internal class SpriteRenderingSystem : BaseSystem
	{
		class ModelInstance
		{
			public string modelId;
			public Point position;
		}
		public SpriteRenderingSystem(NamelessGame game)
		{
			spriteBatch = new SpriteBatch(game.GraphicsDevice);
			//	spriteFont = Content.Load<SpriteFont>("font");

			spriteEffect = new AlphaTestEffect(game.GraphicsDevice)
			{			
				VertexColorEnabled = true,
			};
			spriteEffect.World = invertY;
			spriteEffect.View = Matrix.Identity;
		}

		public override HashSet<Type> Signature => new HashSet<Type>() {
			typeof(SpriteModel3D), typeof(Position3D)
		};

		private SpriteBatch spriteBatch;
		private AlphaTestEffect spriteEffect;
		List<ModelInstance> objectsToDraw = new List<ModelInstance>();
		bool once = true;
		private List<ModelInstance> GetWorldObjectsToDraw(Point positon, IWorldProvider world)
		{
			var postionOffsetX = positon.X * Constants.ChunkSize;
			var postionOffsetY = positon.Y * Constants.ChunkSize;
			var objects = new List<ModelInstance>();

			for (int x = 0; x < 100 * Constants.ChunkSize; x++)
			{
				for (int y = 0; y < 100 * Constants.ChunkSize; y++)
				{
					Tile tileToDraw = world.GetTile(x + postionOffsetX, y + postionOffsetY);
					foreach (var entity in tileToDraw.GetEntities())
					{
						var furniture = entity.GetComponentOfType<Furniture>();
						var drawable = entity.GetComponentOfType<Drawable>();
						if (furniture != null && drawable != null)
						{
							if (drawable.Representation == 'T')
							{
								//var modelShift = Matrix.CreateTranslation(0.5f, 0.5f, 2f);
								//Matrix.CreateTranslation(Constants.ScaleDownCoeficient * x, Constants.ScaleDownCoeficient * y, (float)tileToDraw.Elevation, out Matrix pos);
								objects.Add(new ModelInstance() { modelId = "smallTree", position = new Point(x+ postionOffsetX, y+ postionOffsetY) });
								//return objects;s
							}
							if (drawable.Representation == 't')
							{

								//Matrix.CreateTranslation(Constants.ScaleDownCoeficient * x, Constants.ScaleDownCoeficient * y, (float)tileToDraw.Elevation, out Matrix pos);
								objects.Add(new ModelInstance() { modelId = "smallTree", position = new Point(x+ postionOffsetX, y+ postionOffsetY) });
								//return objects;
							}
						}
					}
				}
			}

			return objects;
		}
		int offset = Constants.ChunkSize * (300 - Constants.RealityBubbleRangeInChunks);
		Matrix invertY = Matrix.CreateScale(1, -1, 1);
		const float spriteScaleDownCoef = 0.0001f;
		public override void Update(GameTime gameTime, NamelessGame namelessGame)
		{
			if (once)
			{
				IEntity worldEntity = namelessGame.TimelineEntity;
				IWorldProvider worldProvider = null;
				if (worldEntity != null)
				{
					worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
				}
				once = false;
				objectsToDraw = GetWorldObjectsToDraw(new Point(300, 300), worldProvider);

				var objectGroups = objectsToDraw.GroupBy(x => x.modelId);

				foreach (var objectGroup in objectGroups)
				{
					foreach (var obj in objectGroup)
					{
						Entity doodadEntity = new Entity();
						doodadEntity.AddComponent(new SpriteModel3D("palmTree"));
						doodadEntity.AddComponent(new Position3D(new Vector3(obj.position.X, obj.position.Y, 0), Vector2.UnitX));
					}
				}
			}

			Camera3D camera = namelessGame.PlayerEntity.GetComponentOfType<Camera3D>();
			
			var player = namelessGame.PlayerEntity; 
			var frustrum = new BoundingFrustum(camera.View * camera.Projection);
			spriteEffect.Projection = camera.Projection;	
			spriteBatch.Begin(0, BlendState.AlphaBlend, Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, spriteEffect);
			foreach (var entity in RegisteredEntities)
			{
				var pos3d = entity.GetComponentOfType<Position3D>();
				var p = pos3d.Position;

				if (pos3d.WorldPosition == null)
				{
					if (pos3d.Tile == null)
					{
						var tile = namelessGame.WorldProvider.GetTile((int)p.X, (int)p.Y);
						pos3d.Tile = tile;
					}

					var tileToDraw = pos3d.Tile;
					var position = new Point((int)(p.X - offset), (int)(p.Y - offset));
					var world = Constants.ScaleDownMatrix * Matrix.CreateTranslation(position.X * Constants.ScaleDownCoeficient, position.Y * Constants.ScaleDownCoeficient, tileToDraw.ElevationVisual * Constants.ScaleDownCoeficient);
					pos3d.WorldPosition = Vector3.Transform(Vector3.One, world); ; 
				}
				var worldPos = pos3d.WorldPosition;
				if (frustrum.Contains(worldPos.Value) == Microsoft.Xna.Framework.ContainmentType.Contains)
				{
					var spriteModel = entity.GetComponentOfType<SpriteModel3D>();
					var viewport = namelessGame.GraphicsDevice.Viewport;

					if (!spriteModel.IdleOnly)
					{
						var angle = AngleBetween(new Vector2(camera.Look.X, camera.Look.Y), pos3d.Normal);

						//default
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

						spriteModel.Sprite.Play("walk" + animationSuffix);
						spriteModel.Sprite.Update(gameTime);
					}

					Vector3 spritePosition = worldPos.Value;

					Vector3 viewSpacePosition = Vector3.Transform(spritePosition, camera.View * invertY);

					if (spriteModel.IdleOnly)
					{
						SpriteLibrary.SpritesIdle[spriteModel.SpriteId].Depth = viewSpacePosition.Z;
						spriteBatch.Draw(SpriteLibrary.SpritesIdle[spriteModel.SpriteId], new Vector2(viewSpacePosition.X, viewSpacePosition.Y), 0, new Vector2(spriteScaleDownCoef, spriteScaleDownCoef));
					}
					else
					{
						spriteModel.Sprite.Depth = viewSpacePosition.Z;
						spriteBatch.Draw(spriteModel.Sprite, new Vector2(viewSpacePosition.X, viewSpacePosition.Y), 0, new Vector2(spriteScaleDownCoef, spriteScaleDownCoef));
					}
				}
			}
			spriteBatch.End();
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
