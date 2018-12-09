using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class InitializationSystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {

            IEntity worldEntity = namelessGame.GetEntityByComponentClass<ChunkData>();
            IChunkProvider worldProvider = null;
            if (worldEntity != null)
            {
                worldProvider = worldEntity.GetComponentOfType<ChunkData>();
            }

            foreach (IEntity entity in namelessGame.GetEntities()) {
                JustCreated jc = entity.GetComponentOfType<JustCreated>();
                if (jc != null)
                {
                    Position position = entity.GetComponentOfType<Position>();
                    OccupiesTile occupiesTile = entity.GetComponentOfType<OccupiesTile>();
                    if (occupiesTile != null && position != null)
                    {
                        Tile tile = worldProvider.GetTile(position.p.X, position.p.Y);
                        tile.getEntitiesOnTile().Add((Entity) entity);
                    }

                    entity.RemoveComponentOfType<JustCreated>();
                }
            }
        }
    }
}
