using Veldrid;
using NamelessRogue.Engine._3DUtility;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components;
using NamelessRogue.Engine.Components._3D;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Utility;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NamelessRogue.Engine.Infrastructure;
namespace NamelessRogue.Engine.Systems._3DView
{
	public class Chunk3DManagementSystem : BaseSystem
	{
		TileAtlasConfig config;
		public override HashSet<Type> Signature { get; } = new HashSet<Type>();
		public Chunk3DManagementSystem()
		{
			config = new TileAtlasConfig();
		}
		public override void Update(GameTime gameTime, NamelessGame game)
		{
		    //return;
			IEntity worldEntity = game.TimelineEntity;
			ChunkData chunks = null;
			if (worldEntity != null)
			{
				chunks = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
			}
			while (game.Commander.DequeueCommand(out UpdateChunkCommand command))
			{
				var geometry = ChunkGeometryGeneratorWeb.GenerateChunkModelTilesOld(game, command.ChunkToUpdate, chunks);
				var chunkGeometries = game.ChunkGeometryEntiry.GetComponentOfType<Chunk3dGeometryHolder>();
				if (chunkGeometries.ChunkGeometries.TryGetValue(command.ChunkToUpdate, out var chunkToRemove))
				{
					chunkGeometries.ChunkGeometries.Remove(command.ChunkToUpdate);
					chunkToRemove.Item1.Dispose();
					chunkToRemove.Item2.Dispose();
				}
				chunkGeometries.ChunkGeometries.Add(command.ChunkToUpdate, new Tuple<Geometry3D, TerrainGeometry3D>(geometry, null));
				//break;
			}
		}
	}
}
