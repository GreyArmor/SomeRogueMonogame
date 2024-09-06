using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Components.Interaction;
using NamelessRogue.Engine.Components.Physical;
using NamelessRogue.Engine.Factories;
using NamelessRogue.Engine.Generation.World;
using NamelessRogue.Engine.Infrastructure;
using NamelessRogue.Engine.Serialization;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Systems.Ingame
{
    public class ChunkManagementSystem : BaseSystem
    {
        private bool once = true;

        public override HashSet<Type> Signature { get; } = new HashSet<Type>();
        public override void Update(GameTime gameTime, NamelessGame namelessGame)
        {

            IEntity worldEntity = namelessGame.TimelineEntity;
            IWorldProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
            }


            IEntity playerentity = namelessGame.PlayerEntity;
            if (playerentity != null)
            {
                    Chunk currentChunk = null;
                Point? currentChunkKey = null;
                Position playerPosition = namelessGame.TestMapPosition;
				//look for current chunk
				var playerChunkPositon = new Point(playerPosition.Point.X / Constants.ChunkSize, playerPosition.Point.Y / Constants.ChunkSize);
                if (worldProvider.GetRealityBubbleChunks().TryGetValue(playerChunkPositon, out var ch))
				{                   
                    currentChunk = ch;
                    currentChunkKey = playerChunkPositon;
                }
                //if there is none, that means we just loaded the namelessGame, look for current in all chunks
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

            var justcreated = worldProvider.GetRealityBubbleChunks().Where(x => x.Value.JustCreated);
            foreach (var realityBubbleChunk in justcreated)
            {
                foreach (var tileArray1 in realityBubbleChunk.Value.GetChunkTiles())
                {
                    foreach (var tileArray2 in tileArray1)
                    {
                        foreach (var tile in tileArray2)
                        {
                            if (tile != null)
                            {
                                var entity = TerrainFurnitureFactory.GetExteriorEntities(namelessGame, tile);
                                if (entity != null)
                                {
                                    realityBubbleChunk.Value.IsAnyEntities = true;

                                    if (tile.GetEntities().Count == 0)
                                    {
                                        tile.AddEntity(entity);
                                        //   tile.Terrain = TerrainTypes.Lava;
                                        namelessGame.AddEntity(entity);
                                    }
                                }
                            }
                        }
                    }
                }
                realityBubbleChunk.Value.JustCreated = false;
            }

            if (once)
            {
                BuildingFactory.CreateBuilding(200 * Constants.ChunkSize, 200 * Constants.ChunkSize, 0, namelessGame);
                BuildingFactory.CreateBuilding(200 * Constants.ChunkSize, 200 * Constants.ChunkSize, 1, namelessGame);
                BuildingFactory.CreateBuilding(200 * Constants.ChunkSize, 200 * Constants.ChunkSize, 2, namelessGame);
                BuildingFactory.CreateBuilding(200 * Constants.ChunkSize, 200 * Constants.ChunkSize, 3, namelessGame);
                once = false;
            }

        }
    }
}
