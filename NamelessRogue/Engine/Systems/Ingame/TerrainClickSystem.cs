using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Systems._3DView;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace NamelessRogue.Engine.Systems.Ingame
{
	internal class TerrainClickSystem : BaseSystem
	{
		public override HashSet<Type> Signature { get; } = new HashSet<Type>();

		private class Intersection
		{
			public float distance;
			public Point chunkId;
			public List<int> triangle;
			public Geometry3D geometry;
		}
		bool wasPressed;


		public Ray CalculateRay(Vector2 mouseLocation, Matrix view,	Matrix projection, Viewport viewport)
		{
			Vector3 nearPoint = viewport.Unproject(new Vector3(mouseLocation.X,
					mouseLocation.Y, 0.0f),
					projection,
					view,
					Matrix.Identity);

			Vector3 farPoint = viewport.Unproject(new Vector3(mouseLocation.X,
					mouseLocation.Y, 1.0f),
					projection,
					view,
					Matrix.Identity);

			Vector3 direction = farPoint - nearPoint;
			direction.Normalize();

			return new Ray(nearPoint, direction);
		}
		public override void Update(GameTime gameTime, NamelessGame game)
		{
			Camera3D camera = game.PlayerEntity.GetComponentOfType<Camera3D>();
			//TODO leaving mouse capture here for now, even if its not correct
			MouseState currentMouseState = Mouse.GetState();
			if (currentMouseState.RightButton != ButtonState.Pressed)
			{
				wasPressed = false;
			}
			else if (!wasPressed)
			{
				wasPressed = true;

				var clickPosition = currentMouseState.Position.ToVector2();

				Ray r = CalculateRay(currentMouseState.Position.ToVector2(), camera.View, camera.Projection, game.GraphicsDevice.Viewport);
				List <Intersection> intersections = new List<Intersection>();
				var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
				{
					foreach (var geometryPointPair in chunkGeometries.ChunkGeometries)
					{
						var geometry = geometryPointPair.Value.Item1;
						if (geometry.TriangleCount > 0)
						{
							var distance = r.Intersects(geometry.Bounds);
							if (distance != null)
							{
								if (CollisionMath.Intersect(geometry, r, out var triangle))
								{
									intersections.Add(new Intersection() { distance = distance.Value, chunkId = geometryPointPair.Key, geometry = geometry, triangle = triangle });
								}
							}
						}
					}

					if (intersections.Any())
					{
						var closestIntersection = intersections.OrderBy(x => x.distance).FirstOrDefault();


						if (game.WorldProvider.GetRealityBubbleChunks().TryGetValue(closestIntersection.chunkId, out Components.ChunksAndTiles.Chunk chunk))
						{
							/*	//set chunk to water for test
							if (chunk != null)
							{
								for (int i = 0; i < Infrastructure.Constants.ChunkSize; i++)
								{
									for (int j = 0; j < Infrastructure.Constants.ChunkSize; j++)
									{
										var tile = chunk.GetTileLocal(i, j);
										tile.Biome = Biomes.Sea;
										tile.Terrain = TerrainTypes.Water;
									}
								}
							}
							//*/
							var clickedTile = closestIntersection.geometry.TriangleTerrainAssociation[closestIntersection.triangle[2]];
							//var tile = chunk.GetTileLocal(clicledTile.X, clicledTile.Y);
							//tile.Biome = Biomes.Sea;
							//tile.Terrain = TerrainTypes.Water;
							//	UpdateChunkCommand updateChunkCommand = new UpdateChunkCommand(closestIntersection.chunkId);
							//	game.Commander.EnqueueCommand(updateChunkCommand);

							var worldPos = new Point(clickedTile.X + chunk.WorldPositionBottomLeftCorner.X, clickedTile.Y + chunk.WorldPositionBottomLeftCorner.Y);

							var selectedGroups = game.PlayerEntity.GetComponentOfType<SelectedUnitsData>();



							var selectedGroupId = selectedGroups.SelectedGroups.FirstOrDefault();
							if (selectedGroupId != null)
							{
								var groups = game.GetEntitiesByComponentClass<Group>().Select(gr => gr.GetComponentOfType<Group>());
								var group = groups.FirstOrDefault(x => x.TextId == selectedGroupId);


								var flagbearer = group.FlagbearerId;
								var position = flagbearer.GetComponentOfType<Position>();

								FlowFieldMoveCommand moveCommand = new FlowFieldMoveCommand(position.Point, worldPos, flagbearer);
								game.Commander.EnqueueCommand(moveCommand);

							}
						}
					}
				}
			}
		}
	}
}
