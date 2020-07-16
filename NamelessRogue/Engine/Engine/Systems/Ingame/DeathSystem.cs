using NamelessRogue.Engine.Abstraction;
using NamelessRogue.Engine.Engine.Components.ChunksAndTiles;
using NamelessRogue.Engine.Engine.Components.Interaction;
using NamelessRogue.Engine.Engine.Components.Physical;
using NamelessRogue.Engine.Engine.Components.Rendering;
using NamelessRogue.Engine.Engine.Components.Status;
using NamelessRogue.Engine.Engine.Components.UI;
using NamelessRogue.Engine.Engine.Generation.World;
using NamelessRogue.Engine.Engine.Infrastructure;
using NamelessRogue.shell;
using System;
using System.Collections.Generic;

namespace NamelessRogue.Engine.Engine.Systems.Ingame
{
    public class DeathSystem : BaseSystem
    {
        public DeathSystem()
        {
            Signature = new HashSet<Type>();
            Signature.Add(typeof(DeathCommand));
        }

        public override HashSet<Type> Signature { get; }

        public override void Update(long gameTime, NamelessGame namelessGame)
        {
            foreach (IEntity entity in RegisteredEntities)
            {
                DeathCommand dc = entity.GetComponentOfType<DeathCommand>();
                IEntity entityToKill = dc.getToKill();
                entityToKill.AddComponent(new Dead());

                Drawable drawable = entityToKill.GetComponentOfType<Drawable>();
                if (drawable != null)
                {
                    drawable.setRepresentation('%');
                    entityToKill.RemoveComponentOfType<DeathCommand>();
                }

                IEntity worldEntity = namelessGame.TimelineEntity;
                IWorldProvider worldProvider = null;
                if (worldEntity != null)
                {
                    worldProvider = worldEntity.GetComponentOfType<TimeLine>().CurrentTimelineLayer.Chunks;
                }

                Position position = entityToKill.GetComponentOfType<Position>();
                OccupiesTile occupiesTile = entityToKill.GetComponentOfType<OccupiesTile>();
                if (occupiesTile != null && position != null)
                {
                    Tile tile = worldProvider.GetTile(position.p.Y, position.p.X);
                    tile.RemoveEntity((Entity) entityToKill);
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