using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
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
                foreach (Point key in worldProvider.getRealityBubbleChunks().Keys) {
                    Chunk ch = worldProvider.getRealityBubbleChunks()[key];
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
                    foreach (Point key in worldProvider.getChunks().Keys)
                    {
                        Chunk ch = worldProvider.getChunks()[key];
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
                            if (!worldProvider.getRealityBubbleChunks().ContainsKey(p))
                            {
                                if (worldProvider.getChunks().ContainsKey(p))
                                {
                                    worldProvider.getRealityBubbleChunks().Add(p, worldProvider.getChunks()[p]);
                                }
                            }
                        }
                    }

                    List<Point> keysToRemove = new List<Point>();
                    foreach (Point key in
                    worldProvider.getRealityBubbleChunks().Keys) {
                        double dist = Math.Abs(key.Y - currentChunkKey.Value.Y) + Math.Abs(key.X - currentChunkKey.Value.X);
                        if (dist > Constants.RealityBubbleRangeInChunks)
                        {
                            keysToRemove.Add(key);
                        }
                    }
                    foreach (Point key in
                    keysToRemove)
                    {
                        if (worldProvider.getRealityBubbleChunks()[key].IsActive())
                        {
                            worldProvider.getRealityBubbleChunks()[key].Deactivate();
                            worldProvider.getRealityBubbleChunks().Remove(key);
                        }
                    }

                }
            }




        }
    }
}
