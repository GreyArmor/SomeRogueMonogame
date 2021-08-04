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
        public override HashSet<Type> Signature { get; } = new HashSet<Type>();

        public override void Update(long gameTime, NamelessGame namelessGame)
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
                Position playerPosition = playerentity.GetComponentOfType<Position>();
                //look for current chunk
                foreach (Point key in worldProvider.GetRealityBubbleChunks().Keys)
                {
                    Chunk ch = worldProvider.GetRealityBubbleChunks()[key];
                    if (ch.IsPointInside(playerPosition.Point))
                    {
                        currentChunk = ch;
                        currentChunkKey = key;
                        break;
                    }
                }

                //if there is none, that means we just loaded the namelessGame, look for current in all chunks
                if (currentChunk == null)
                {
                    foreach (Point key in worldProvider.GetChunks().Keys)
                    {
                        Chunk ch = worldProvider.GetChunks()[key];
                        if (ch.IsPointInside(playerPosition.Point))
                        {
                            currentChunk = ch;
                            currentChunkKey = key;
                            break;
                        }
                    }
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
                                if (worldProvider.GetChunks().ContainsKey(p))
                                {
                                    Chunk chunk = worldProvider.GetChunks()[p];

                                    

                                    worldProvider.GetRealityBubbleChunks().Add(p, chunk);
                                    worldProvider.RealityChunks.Add(chunk);
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
                foreach (var tileArray in realityBubbleChunk.Value.GetChunkTiles())
                {
                    foreach (var tile in tileArray)
                    {
                        var entity = TerrainFurnitureFactory.GetExteriorEntities(namelessGame, tile);
                        if (entity != null)
                        {
                            if (tile.GetEntities().Count == 0)
                            {
                                tile.AddEntity(entity);

                                namelessGame.AddEntity(entity);

                            }
                        }
                    }
                }
                realityBubbleChunk.Value.JustCreated = false;
            }          

        }
    }
}
