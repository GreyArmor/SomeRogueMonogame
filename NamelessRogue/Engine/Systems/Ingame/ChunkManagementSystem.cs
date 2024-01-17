using System;
using System.Collections.Generic;
using System.Linq;
using Veldrid;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.Engine.Systems._3DView;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class ChunkManagementSystem : BaseSystem
    {
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
        public override void Update(GameTime gameTime, NamelessGame game)
        {

            IEntity worldEntity = game.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }


            IEntity playerentity = game.PlayerEntity;
            if (playerentity != null)
            {
                    Chunk currentChunk = null;
                Point? currentChunkKey = null;
                Position playerPosition = game.TestMapPosition;
				//look for current chunk
				var playerChunkPositon = new Point(playerPosition.Point.X / Constants.ChunkSize, playerPosition.Point.Y / Constants.ChunkSize);
                if (worldProvider.GetRealityBubbleChunks().TryGetValue(playerChunkPositon, out var ch))
				{                   
                    currentChunk = ch;
                    currentChunkKey = playerChunkPositon;
                }
                //if there is none, that means we just loaded the game, look for current in all chunks
                else
                {
					worldProvider.GetChunks().TryGetValue(playerChunkPositon, out var worldChunk);
					currentChunk = worldChunk;
					currentChunkKey = playerChunkPositon;
				}

                if (currentChunk != null)
                {
                    for (int x = -Constants.RealityBubbleRangeInChunks + currentChunkKey.Value.X;
                        x <= Constants.RealityBubbleRangeInChunks + currentChunkKey.Value.X;
                        x++)
                    {
                        for (int y = -Constants.RealityBubbleRangeInChunks + currentChunkKey.Value.Y;
                            y <= Constants.RealityBubbleRangeInChunks + currentChunkKey.Value.Y;
                            y++)
                        {
                            Point p = new Point(x, y);
                            if (!worldProvider.GetRealityBubbleChunks().ContainsKey(p))
                            {
                                if (worldProvider.GetChunks().TryGetValue(p,out var chunk))
                                {
                                    worldProvider.GetRealityBubbleChunks().Add(p, chunk);
                                    worldProvider.RealityChunks.Add(chunk);
                                    chunk.Activate();
                                }
                            }
                        }
                    }

                    List<Point> keysToRemove = new List<Point>();
                    foreach (Point key in
                        worldProvider.GetRealityBubbleChunks().Keys)
                    {
                        double distX = Math.Abs(key.X - currentChunkKey.Value.X);
                        double distY = Math.Abs(key.Y - currentChunkKey.Value.Y);
                        if (distX > Constants.RealityBubbleRangeInChunks || distY > Constants.RealityBubbleRangeInChunks)
                        {
                            keysToRemove.Add(key);
                        }
                    }

                    foreach (Point key in
                        keysToRemove)
                    {
                        if (worldProvider.GetRealityBubbleChunks()[key].IsActive)
                        {
                            worldProvider.GetRealityBubbleChunks()[key].Deactivate();
                            worldProvider.RealityChunks.Remove(worldProvider.GetRealityBubbleChunks()[key]);
                            worldProvider.GetRealityBubbleChunks().Remove(key);

                        }
                    }

                }
            }

   //         var justcreated = worldProvider.GetRealityBubbleChunks().Where(x => x.Value.JustCreated);
   //         foreach (var realityBubbleChunk in justcreated)
   //         {
   //             foreach (var tileArray in realityBubbleChunk.Value.GetChunkTiles())
   //             {
   //                 foreach (var tile in tileArray)
   //                 {
   //                     var entity = TerrainFurnitureFactory.GetExteriorEntities(game, tile);
   //                     if (entity != null)
   //                     {
   //                         realityBubbleChunk.Value.IsAnyEntities = true;

			//				if (tile.GetEntities().Count == 0)
   //                         {
   //                             tile.AddEntity(entity);
   //                          //   tile.Terrain = TerrainTypes.Lava;
   //                             game.AddEntity(entity);

   //                         }
   //                     }
   //                 }
   //             }
   //             realityBubbleChunk.Value.JustCreated = false;
			//}          

        }
    }
}
