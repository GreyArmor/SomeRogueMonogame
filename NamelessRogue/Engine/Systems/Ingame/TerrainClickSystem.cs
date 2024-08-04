using Microsoft.VisualBasic;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Numerics;


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


		//public Ray CalculateRay(Vector2 mouseLocation, Matrix4x4 view, Matrix4x4 projection, Viewport viewport)
		//{
		//	Vector3 nearPoint = ViewportUtil.Unproject(viewport,
		//		new Vector3(mouseLocation.X,
		//			mouseLocation.Y, 0.0f),
		//			projection,
		//			view,
		//			Matrix4x4.Identity);

		//	Vector3 farPoint = ViewportUtil.Unproject(viewport,

  //              new Vector3(mouseLocation.X,
		//			mouseLocation.Y, 1.0f),
		//			projection,
		//			view,
		//			Matrix4x4.Identity);

		//	Vector3 direction = farPoint - nearPoint;
  //          direction = Vector3.Normalize(direction);

		//	return new Ray(nearPoint, direction);
		//}
		public override void Update(GameTime gameTime, NamelessGame game)
		{
			//Camera3D camera = game.PlayerEntity.GetComponentOfType<Camera3D>();
			////TODO leaving mouse capture here for now, even if its not correct
			//MouseState currentMouseState = new MouseState(game.Input);
			//if (!currentMouseState.RightPressed)
			//{
			//	wasPressed = false;
			//}
			//else if (!wasPressed)
			//{
				//wasPressed = true;

				//var clickPosition = currentMouseState.Position.ToVector2();

				//var vp = new Viewport(0,0, game.GetActualWidth(), game.GetActualHeight(), camera.NearPlane, camera.FarPlane);
				//Ray r = CalculateRay(currentMouseState.Position.ToVector2(), camera.View, camera.Projection, vp);
				//List <Intersection> intersections = new List<Intersection>();
				//var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
				//{
				//	foreach (var geometryPointPair in chunkGeometries.ChunkGeometries)
				//	{
				//		var geometry = geometryPointPair.Value.Item1;
				//		if (geometry.TriangleCount > 0)
				//		{
    //                        if (RayExtensions.Intersects(r, geometry.Bounds, out float distance))
    //                        {
				//				if (CollisionMath.Intersect(geometry, r, out var triangle))
				//				{
				//					intersections.Add(new Intersection() { distance = distance, chunkId = geometryPointPair.Key, geometry = geometry, triangle = triangle });
				//				}
				//			}
				//		}
				//	}

				//	if (intersections.Any())
				//	{
				//		var closestIntersection = intersections.OrderBy(x => x.distance).FirstOrDefault();


				//		if (game.WorldProvider.GetRealityBubbleChunks().TryGetValue(closestIntersection.chunkId, out Components.ChunksAndTiles.Chunk chunk))
				//		{
				//			/*	//set chunk to water for test
				//			if (chunk != null)
				//			{
				//				for (int i = 0; i < Infrastructure.Constants.ChunkSize; i++)
				//				{
				//					for (int j = 0; j < Infrastructure.Constants.ChunkSize; j++)
				//					{
				//						var tile = chunk.GetTileLocal(i, j);
				//						tile.Biome = Biomes.Sea;
				//						tile.Terrain = TerrainTypes.Water;
				//					}
				//				}
				//			}
				//			//*/
				//			var clickedTile = closestIntersection.geometry.TriangleTerrainAssociation[closestIntersection.triangle[2]];
				//			//var tile = chunk.GetTileLocal(clicledTile.X, clicledTile.Y);
				//			//tile.Biome = Biomes.Sea;
				//			//tile.Terrain = TerrainTypes.Water;
				//			//	UpdateChunkCommand updateChunkCommand = new UpdateChunkCommand(closestIntersection.chunkId);
				//			//	game.Commander.EnqueueCommand(updateChunkCommand);

				//			var worldPos = new Point(clickedTile.X + chunk.WorldPositionBottomLeftCorner.X, clickedTile.Y + chunk.WorldPositionBottomLeftCorner.Y);

				//			var selectedGroups = game.PlayerEntity.GetComponentOfType<SelectedUnitsData>();



				//			var selectedGroupId = selectedGroups.SelectedGroups.FirstOrDefault();
				//			if (selectedGroupId != null)
				//			{
				//				var groups = game.GetEntitiesByComponentClass<Group>().Select(gr => gr.GetComponentOfType<Group>());
				//				var group = groups.FirstOrDefault(x => x.TextId == selectedGroupId);


				//				var flagbearer = group.FlagbearerId;
				//				var position = flagbearer.GetComponentOfType<Position>();

				//				FlowFieldMoveCommand moveCommand = new FlowFieldMoveCommand(position.Point, worldPos, flagbearer);
				//				game.Commander.EnqueueCommand(moveCommand);

				//			}
				//		}
				//	}
				//}
			//}
		}
	}
}
