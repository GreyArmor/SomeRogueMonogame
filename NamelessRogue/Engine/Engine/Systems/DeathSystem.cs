using Microsoft.Xna.Framework;
using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.Status;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;

namespace NamelessRogue.Engine.Engine.Systems
{
    public class DeathSystem : ISystem
    {

        public void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in namelessGame.GetEntities()) {
                DeathCommand dc = entity.GetComponentOfType<DeathCommand>();
                if (dc != null)
                {
                    IEntity entityToKill = dc.getToKill();
                    entityToKill.AddComponent(new Dead());

                    Drawable drawable = entityToKill.GetComponentOfType<Drawable>();
                    if (drawable != null)
                    {
                        drawable.setRepresentation('%');
                        entityToKill.RemoveComponentOfType<DeathCommand>();
                    }

                    IEntity worldEntity = namelessGame.GetEntityByComponentClass<ChunkData>();
                    IChunkProvider worldProvider = null;
                    if (worldEntity != null)
                    {
                        worldProvider = worldEntity.GetComponentOfType<ChunkData>();
                    }

                    Position position = entityToKill.GetComponentOfType<Position>();
                    OccupiesTile occupiesTile = entityToKill.GetComponentOfType<OccupiesTile>();
                    if (occupiesTile != null && position != null)
                    {
                        Tile tile = worldProvider.getTile(position.p.Y, position.p.X);
                        tile.getEntitiesOnTile().Remove((Entity) entityToKill);
                    }

                    entityToKill.RemoveComponentOfType<OccupiesTile>();

                    Description d = entityToKill.GetComponentOfType<Description>();

                    if (d != null)
                    {
                       // namelessGame.WriteLineToConsole(d.Name + " is dead!");
                    }
                }
            }
        }
    }
}