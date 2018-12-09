using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Factories;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class ChunkManagementSystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            //not elegant TODO: think of better way
            IEntity worldEntity = namelessGame.GetEntityByComponentClass<ChunkData>();
            IChunkProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<ChunkData>();
            }

            IEntity playerentity = namelessGame.GetEntityByComponentClass<Player>();
            if (playerentity != null)
            {
                Chunk currentChunk = null;
                Point? currentChunkKey = null;
                Position playerPosition = playerentity.GetComponentOfType<Position>();
                //look for current chunk
                foreach (Point key in worldProvider.GetRealityBubbleChunks().Keys)
                {
                    Chunk ch = worldProvider.GetRealityBubbleChunks()[key];
                    if (ch.IsPointInside(playerPosition.p))
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
                        if (ch.IsPointInside(playerPosition.p))
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
                                    worldProvider.GetRealityBubbleChunks().Add(p, worldProvider.GetChunks()[p]);
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
                        if (worldProvider.GetRealityBubbleChunks()[key].IsActive())
                        {
                            worldProvider.GetRealityBubbleChunks()[key].Deactivate();

                            worldProvider.GetRealityBubbleChunks().Remove(key);
                        }
                    }

                }
            }

            var justcreated = worldProvider.GetRealityBubbleChunks().Where(x => x.Value.JustCreated);
            foreach (var realityBubbleChunk in justcreated)
            {
                foreach (var tile in realityBubbleChunk.Value.GetChunkTiles())
                {
                    var entity = TerrainFurnitureFactory.GetExteriorEntities(namelessGame, tile);
                    if (entity != null)
                    {
                        if (tile.getEntitiesOnTile().Count == 0)
                        {
                            tile.getEntitiesOnTile().Add(entity);
                        }
                    }
                }
                realityBubbleChunk.Value.JustCreated = false;
            }
        }
    }
}
